using Microsoft.AspNetCore.HttpOverrides;
using Users.API.Configurations;
using Users.API.Endpoints;
using Users.API.Profiles;
using Users.Infra.Data.Contexts;
using Users.Infra.CrossCutting.IoC;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddCors();

builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddJwtSecurity(builder.Configuration);
builder.Services.AddPolicies();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<Mapper>();

#region MassTransit (RabbitMQ)

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<MySqlContext>(o =>
    {
        o.UseMySql();
        o.UseBusOutbox();
    });

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

#region Executar Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<MySqlContext>();

    context.Database.EnsureCreated();
}
#endregion

app.UseMiddleware<ExceptionMiddleware>();
app.UseForwardedHeaders();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.MapUserEndpoints();

app.Run();

