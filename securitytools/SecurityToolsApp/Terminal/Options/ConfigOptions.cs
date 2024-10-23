namespace GGolbik.SecurityToolsApp.Terminal.Options;

using CommandLine;
using GGolbik.SecurityToolsApp.Web.Models;

public abstract class ConfigOptions : ProgramOptions
{
    [Option('c', "config", Required = true, Default = null, HelpText = "The config to use.")]
    public string? ConfigFile { get; set; }

    [Option("outputFile", Required = false, Default = null, HelpText = "The path to the output file.")]
    public string? OutputFile { get; set; }
    
    [Option("outputType", Required = false, Default = MediaType.Zip, HelpText = "The format of the output file. [Zip, Tar, TarGz, Json]")]
    public MediaType OutputType { get; set; }
}
