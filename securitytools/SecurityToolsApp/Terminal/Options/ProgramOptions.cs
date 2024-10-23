namespace GGolbik.SecurityToolsApp.Terminal.Options;

using System.Text.Json;
using CommandLine;
using GGolbik.SecurityTools.Io;
using GGolbik.SecurityToolsApp.Web.Converter;

public abstract class ProgramOptions
{
    [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    [Option('l', "loglevel", Required = false, Default = LogLevel.Information, HelpText = "Set the log level.")]
    public LogLevel Level { get; set; }

    [Option("JsonNamingPolicy", Required = false, Default = "CamelCase", HelpText = "The property naming policy of the JSON serializer.")]
    public string? NamingPolicy { get; set; }

    public virtual JsonSerializerOptions GetJsonSerializerOptions()
    {
        List<KeyValuePair<string, JsonNamingPolicy>> policies = [
            new(nameof(JsonNamingPolicy.CamelCase), JsonNamingPolicy.CamelCase),
            new(nameof(PascalCaseJsonNamingPolicy.PascalCase), PascalCaseJsonNamingPolicy.PascalCase),
            new(nameof(JsonNamingPolicy.KebabCaseLower), JsonNamingPolicy.KebabCaseLower),
            new(nameof(JsonNamingPolicy.KebabCaseUpper), JsonNamingPolicy.KebabCaseUpper),
            new(nameof(JsonNamingPolicy.SnakeCaseLower), JsonNamingPolicy.SnakeCaseLower),
            new(nameof(JsonNamingPolicy.SnakeCaseUpper), JsonNamingPolicy.SnakeCaseUpper),
        ];
        int index = policies.FindIndex(item =>
        {
            return string.Equals(item.Key, this.NamingPolicy, StringComparison.InvariantCultureIgnoreCase);
        });
        JsonSerializerOptions options = new();
        if (index < 0)
        {
            options.PropertyNameCaseInsensitive = true;
            options.PropertyNamingPolicy = null;
        }
        else
        {
            options.PropertyNameCaseInsensitive = true;
            options.PropertyNamingPolicy = policies[index].Value;
        }
        options.Converters.Add(new WorkEventArgsJsonConverter());
        options.Converters.Add(new WorkRequestJsonConverter());
        options.Converters.Add(new WorkStatusJsonConverter());
        return options;
    }
}