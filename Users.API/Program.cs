using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using Users.API.Configurations;
using Users.API.Endpoints;
using Users.API.Profiles;
using Users.API.Extensions;
using Users.Infra.CrossCutting.IoC;
using Users.Infra.Data.Contexts;
using Users.Domain.Events;

var builder = WebApplication.CreateBuilder(args);

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

#region MassTransit (RabbitMQ)

builder.Services.AddMassTransit(x =>
{
    var exchangeName = Environment.GetEnvironmentVariable("USER_CREATED_EXCHANGE_NAME") ?? "user-created-event";

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        var user = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER") ?? "guest";
        var password = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS") ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(user);
            h.Password(password);
        });

        cfg.Message<UserCreatedEvent>(m => m.SetEntityName(exchangeName));

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
    options.StartTimeout = TimeSpan.FromSeconds(30);
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

app.UseMiddleware<ExceptionMiddleware>();
app.UseForwardedHeaders();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();

app.MapUserEndpoints();

app.ApplyMigrations();

app.Run();