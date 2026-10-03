using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Fcg.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using System.Text.Json;
using Users.API.Configurations;
using Users.API.Endpoints;
using Users.API.Extensions;
using Users.API.Profiles;
using Users.Infra.CrossCutting.IoC;
using Users.Infra.Data.Contexts;


var builder = WebApplication.CreateBuilder(args);

var awsAccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") ?? string.Empty;
var awsSecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY") ?? string.Empty;

var credentials = new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey);
using var client = new AmazonSecretsManagerClient(RegionEndpoint.USEast1);

var request = new GetSecretValueRequest
{
    SecretId = "fcg-secrets"
};

var response = await client.GetSecretValueAsync(request);

if (!string.IsNullOrEmpty(response.SecretString))
{
    var secretData = JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString);
    if (secretData != null)
    {
        builder.Configuration.AddInMemoryCollection(secretData!);
    }
}

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddCors();
builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddJwtSecurity(builder.Configuration);
builder.Services.AddPolicies();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<Mapper>();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddDbContextCheck<MySqlContext>(
        name: "mysql",
        tags: ["ready"]);

#region MassTransit (Azure Service Bus)

builder.Services.AddMassTransit(x =>
{
    var topicName = Environment.GetEnvironmentVariable("USERS_TOPIC") ?? string.Empty;

    x.UsingAzureServiceBus((context, cfg) =>
    {
        var connectionString = builder.Configuration["ServiceBusConnectionString"] ?? string.Empty;

        cfg.Host(connectionString);

        cfg.Message<UserCreatedEvent>(e => e.SetEntityName(topicName));

        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
    });
});

#endregion

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

#region Health Checks
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                error = entry.Value.Exception?.Message
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }
});
#endregion

app.UseHttpMetrics();
app.UseMiddleware<ExceptionMiddleware>();
app.UseForwardedHeaders();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.MapUserEndpoints();
app.MapMetrics();
app.ApplyMigrations();

app.Run();