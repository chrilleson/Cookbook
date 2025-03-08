using Cookbook.Api.Extensions;
using Cookbook.Api.Middlewares;
using Cookbook.Application;
using Cookbook.Infrastructure.Persistence;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder
    .AddServiceDefaults()
    .AddPersistence()
    .AddRedisOutputCache();

// Add services to the container.
builder.Services
    .AddRepositories()
    .AddApplication()
    .AddApi();

var app = builder.Build();

// Apply migrations on startup
await app.ApplyMigrations();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseCors(builder.Configuration.GetValue<string>("AllowedOrigin")!);
app.UseSwagger();
app.UseSwaggerUI();
app.UseOutputCache();

app.UseHealthChecks("/health");

app.MapEndpoints();
app.MapGet("/", () => "Cookbook Api is alive and kicking!");

app.Run();
