namespace GGolbik.SecurityToolsApp.Terminal.Options;

using System.Text.Json;
using CommandLine;
using GGolbik.SecurityTools.Credentials;

[Verb("web", isDefault:true, HelpText = "Runs a web application.")]
public class WebOptions : ProgramOptions
{
    [Option("hsts", Required = false, Default = null, HelpText = "Whether the app shall only run in terminal.")]
    public bool? Hsts { get; set; }

    [Option("httpsRedirection", Required = false, Default = null, HelpText = "Whether the app shall perform HTTPS redirection.")]
    public bool? HttpsRedirection { get; set; }

    [Option("useSerilogRequestLogging", Required = false, Default = false, HelpText = "Request logging by Serilog. Serilog middleware condenses these into a single event that carries method, path, status code, and timing information.")]
    public bool UseSerilogRequestLogging { get; set; }

    [Option("urls", Required = false, Default = null, HelpText = "Vists https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel for more info.")]
    public string? Urls { get; set; }

    [Option("exportCredentials", Required = false, Default = true, HelpText = "Whether credentials shall be exported to the web.")]
    public bool ExportCredentials { get; set; }

    public override JsonSerializerOptions GetJsonSerializerOptions()
    {
        var options = base.GetJsonSerializerOptions();
        if(!ExportCredentials)
        {
            options.Converters.Add(new KeyCredentialsJsonConverter(false));
        }
        return options;
    }
}
