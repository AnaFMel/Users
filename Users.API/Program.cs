using MassTransit;
using MassTransit.Topology;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System.Text.Json;
using Users.API.Configurations;
using Users.API.Endpoints;
using Users.API.Extensions;
using Users.API.Profiles;
using Users.Infra.CrossCutting.IoC;
using Users.Infra.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddCors();
builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddJwtSecurity(builder.Configuration);
builder.Services.AddPolicies();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<Mapper>();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("UsersAPI"))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter()
    );

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddDbContextCheck<MySqlContext>(
        name: "mysql",
        tags: ["ready"]);

#region MassTransit (AWS SQS / LocalStack)
builder.Services.AddMassTransit(x =>
{
    x.UsingAmazonSqs((context, cfg) =>
    {
        var localstackHost = Environment.GetEnvironmentVariable("LOCALSTACK_HOST") ?? "localstack";

        cfg.Host(new Uri($"amazonsqs://{localstackHost}:4566"), h =>
        {
            h.AccessKey("test");
            h.SecretKey("test");

            // Aponta o SQS para o LocalStack
            h.Config(new Amazon.SQS.AmazonSQSConfig
            {
                ServiceURL = $"http://{localstackHost}:4566"
            });

            // Aponta o SNS para o LocalStack (necessário se sua app faz publish em tópicos SNS)
            h.Config(new Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = $"http://{localstackHost}:4566"
            });
        });

        cfg.ConfigureEndpoints(context);
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

app.MapPrometheusScrapingEndpoint();
app.UseMiddleware<ExceptionMiddleware>();
app.UseForwardedHeaders();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.MapUserEndpoints();
app.ApplyMigrations();

app.Run();