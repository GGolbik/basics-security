namespace GGolbik.SecurityTools.Terminal.Options;

using System.Text.Json;
using CommandLine;
using GGolbik.SecurityTools.Io;

public abstract class ProgramOptions
{
    [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    [Option('l', "loglevel", Required = false, Default = LogLevel.Information, HelpText = "Set the log level.")]
    public LogLevel Level { get; set; }

    [Option("JsonNamingPolicy", Required = false, Default = "CamelCase", HelpText = "The property naming policy of the JSON serializer.")]
    public string? NamingPolicy { get; set; }

    public JsonSerializerOptions GetJsonSerializerOptions()
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
        if (index < 0)
        {
            return new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null
            };
        }
        return new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = policies[index].Value
        };
    }
}