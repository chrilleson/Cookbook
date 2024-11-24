using Cookbook.Api.Extensions;
using Cookbook.Api.Middlewares;
using Cookbook.Application;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddPersistence(builder.Configuration.GetValue<string>("ConnectionStrings:Postgres"));
builder.Services.AddRepositories();
builder.Services.AddApplication();
builder.Services.AddApi();

var app = builder.Build();

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
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/health");

app.MapControllers();
app.MapGet("/", () => "Cookbook Api is alive and kicking!");

app.Run();