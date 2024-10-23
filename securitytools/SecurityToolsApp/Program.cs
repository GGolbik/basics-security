namespace GGolbik.SecurityToolsApp;

using GGolbik.SecurityToolsApp.Web;
using GGolbik.SecurityToolsApp.Terminal;
using CommandLine;
using GGolbik.SecurityToolsApp.Terminal.Options;
using CommandLine.Text;
using GGolbik.SecurityToolsApp.Diagnostic;
using GGolbik.SecurityToolsApp.Work;
using GGolbik.SecurityToolsApp.Tools;
using GGolbik.SecurityToolsApp.Credentials;

public class Program
{
    public static int Main(string[] args)
    {
        using (var loggingService = new LoggingService())
        {
            var logger = loggingService.ForContext<Program>();
            logger.Information("{0}", new ProgramInfo());
            var parser = new Parser(with =>
            {
                with.IgnoreUnknownArguments = true;
            });
            var result = parser.ParseArguments<PrintOptions, WebOptions, CertOptions, CrlOptions, CsrOptions, KeyPairOptions, TransformOptions>(args);

            if (result.Tag == ParserResultType.NotParsed)
            {
                foreach (var error in result.Errors)
                {
                    if(error is HelpVerbRequestedError)
                    {
                        var text = HelpText.AutoBuild(result);
                        Console.WriteLine(text);
                    }
                    else if(error is HelpRequestedError)
                    {
                        var text = HelpText.AutoBuild(result);
                        Console.WriteLine(text);
                    }
                    else
                    {
                        logger.Error("{0}", error.ToString());
                    }
                }
                return (int)ExitCode.InvalidArguments;
            }

            if (result.Value is ProgramOptions programOptions)
            {
                loggingService.Level = programOptions.Level;
                if (programOptions.Verbose)
                {
                    loggingService.Level = LogLevel.Trace;
                }
            }

            using (var workerService = new WorkerService())
            using (var securityToolsService = new SecurityToolsService())
            using (var credentialsService = new CredentialsService())
            {
                if (result.Value is WebOptions webOptions)
                {
                    return Program.RunWebApp(webOptions, args, loggingService, workerService, securityToolsService, credentialsService);
                }
                else if(result.Value is ConfigOptions terminalOptions)
                {
                    return Program.RunTerminalApp(terminalOptions, securityToolsService);
                }
                else
                {
                    logger.Error("Unknown verb");
                    return (int)ExitCode.InvalidArguments;
                }
            }

        }
    }

    private static int RunTerminalApp(ConfigOptions terminalOptions, ISecurityToolsService securityToolsService)
    {
        using (var terminal = new TerminalApp(terminalOptions, securityToolsService))
        {
            return (int)terminal.Start();
        }
    }

    private static int RunWebApp(WebOptions options, string[] args, ILoggingService loggingService, IWorkerService workerService, ISecurityToolsService securityToolsService, ICredentialsService credentialsService)
    {
        using (var webApp = new WebApp(options, loggingService, workerService, securityToolsService, credentialsService))
        {
            // create and start webapp
            Task? waitForShutdownTask; ;
            webApp.Start(args);

            waitForShutdownTask = webApp.WaitForShutdownAsync();

            // wait until app is stopped which might be done by a restart request.
            waitForShutdownTask?.Wait();

            return (int)ExitCode.Success;
        }
    }
}