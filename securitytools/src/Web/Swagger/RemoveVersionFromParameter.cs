using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GGolbik.SecurityTools.Web.Swagger;

public class RemoveVersionFromParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var versionQueryParameter = operation.Parameters.First(p => p.Name == "api-version");
        operation.Parameters.Remove(versionQueryParameter);
        //versionQueryParameter.Required = false;

        //var versionHeaderParameter = operation.Parameters.Last(p => p.Name == "api-version");
        //operation.Parameters.Remove(versionHeaderParameter);
    }
}