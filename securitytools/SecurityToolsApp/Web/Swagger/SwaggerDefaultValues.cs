using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GGolbik.SecurityTools.Web.Swagger;

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        if (operation.Parameters == null)
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions
                                            .First(p => p.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null &&
                description.DefaultValue != null &&
                description.DefaultValue is not DBNull &&
                description.ModelMetadata is ModelMetadata modelMetadata)
            {
                // Hint: DefaultValue might be just "1" for version "1.0" if there is at least one [ApiVersion("1")] version attribute instead of [ApiVersion("1.0")].
                var json = JsonSerializer.Serialize(
                    description.DefaultValue,
                    modelMetadata.ModelType);

                // this will set the API version, while also making it read-only
                parameter.Schema.Enum = new[] { OpenApiAnyFactory.CreateFromJson(json) };
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }
            parameter.Required = true;//|= description.IsRequired;
        }
    }
}