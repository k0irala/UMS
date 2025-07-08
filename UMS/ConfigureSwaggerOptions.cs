using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UMS;

public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider apiVersion) : IConfigureNamedOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var api in apiVersion.ApiVersionDescriptions)
        {
            var openApiInfo = new OpenApiInfo()
            {
                Title = $"Api Version{api.ApiVersion}",
                Version = api.ApiVersion.ToString(),
            };
            options.SwaggerDoc(api.GroupName, openApiInfo);
        }
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
       Configure(options);
    }
}