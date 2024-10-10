
using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GGolbik.SecurityTools.Web.Swagger;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    private readonly OpenApiInfo info;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
    /// </summary>
    /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
    /// <param name="info"></param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, OpenApiInfo info)
    {
        this.provider = provider;
        this.info = info;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        // Set the comments/documentation path of your API for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = this.info.Title,
            Version = description.ApiVersion.ToString("'v'VVVV"),
            Description = this.info.Description,
            Contact = this.info.Contact,
            License = this.info.License,
            TermsOfService = this.info.TermsOfService,
            Extensions = this.info.Extensions
        };

        if (description.IsDeprecated)
        {
            info.Description = (String.IsNullOrWhiteSpace(this.info.Description) ? "" : this.info.Description + " ") + "<b>This API version is deprecated.</b>";
        }

        return info;
    }
}