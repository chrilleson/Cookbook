using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Cookbook.Api.Extensions;

public static class SerilogExtensions
{
    public static void UseSerilog(this IHostBuilder hostBuilder) =>
        hostBuilder.UseSerilog((hostingContext, _, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithClientIp()
            .WriteTo.Console()
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = hostingContext.Configuration["Seq:ServerUrl"];
                options.Protocol = OtlpProtocol.HttpProtobuf;
                options.IncludedData = IncludedData.MessageTemplateTextAttribute | IncludedData.TraceIdField | IncludedData.SpanIdField;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = "Cookbook.Api",
                    ["environment"] = hostingContext.HostingEnvironment.EnvironmentName,
                };
            })
        );
}