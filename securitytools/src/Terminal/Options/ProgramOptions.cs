namespace GGolbik.SecurityTools.Terminal.Options;

using CommandLine;

public abstract class ProgramOptions
{
    [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    [Option('l', "loglevel", Required = false, Default = LogLevel.Information, HelpText = "Set the log level.")]
    public LogLevel Level { get; set; }
}
