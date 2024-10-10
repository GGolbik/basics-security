namespace GGolbik.SecurityTools.Terminal.Options;

using CommandLine;


[Verb("print", HelpText = "Prints the content of a file")]
public class PrintOptions : ProgramOptions
{
    [Option("inputFile", Required = true, Default = null, HelpText = "The path to the file to print.")]
    public string? InputFile { get; set; }
    
}