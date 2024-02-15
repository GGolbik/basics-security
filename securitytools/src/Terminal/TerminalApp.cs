using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using GGolbik.SecurityTools.Models;
using GGolbik.SecurityTools.Services;
using GGolbik.SecurityTools.Terminal.Options;
using GGolbik.SecurityTools.Web.Models;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Serilog;

namespace GGolbik.SecurityTools.Terminal;

public class TerminalApp : IDisposable
{
    private Serilog.ILogger Logger = Log.ForContext<TerminalApp>();
    private readonly ProgramOptions _options;
    private readonly ISecurityToolsService _service;

    public TerminalApp(ProgramOptions options, ISecurityToolsService securityToolsService)
    {
        _options = options;
        _service = securityToolsService;
    }

    public void Dispose()
    {
    }

    public ExitCode Start()
    {
        if (_options is CertOptions certOptions)
        {
            return this.Start(certOptions);
        }
        else if (_options is CrlOptions crlOptions)
        {
            return this.Start(crlOptions);
        }
        else if (_options is CsrOptions csrOptions)
        {
            return this.Start(csrOptions);
        }
        else if (_options is TransformOptions transformOptions)
        {
            return this.Start(transformOptions);
        }
        else if (_options is KeyPairOptions keyPairOptions)
        {
            return this.Start(keyPairOptions);
        }
        else if (_options is PrintOptions printOptions)
        {
            return this.Start(printOptions);
        }
        else
        {
            Logger.Error("Unknown verb");
            return ExitCode.InvalidArguments;
        }
    }

    private ExitCode Start(CertOptions certOptions)
    {
        var readResult = this.ReadConfig(certOptions, out ConfigCert? request);
        if (readResult != ExitCode.Success)
        {
            return readResult;
        }
        // build
        ConfigCert config;
        try
        {
            config = _service.Build(request!);
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return ExitCode.UnknownError;
        }
        if (certOptions.OutputFile != null)
        {
            // create files
            var resultFiles = new Dictionary<string, byte[]>
            {
                { "input.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request, Program.JsonOptions)) },
                { "output.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(config, Program.JsonOptions)) },
            };
            if (config.Csr?.Csr?.Exists() ?? false)
            {
                resultFiles.Add("result.csr", config.Csr.Csr.Data ?? File.ReadAllBytes(config.Csr.Csr.FileName!));
            }
            if (config.KeyPair?.PrivateKey?.Exists() ?? false)
            {
                resultFiles.Add("issuer.key", config.KeyPair.PrivateKey.Data ?? File.ReadAllBytes(config.KeyPair.PrivateKey.FileName!));
            }
            if (config.KeyPair?.PublicKey?.Exists() ?? false)
            {
                resultFiles.Add("issuer.key.pub", config.KeyPair.PublicKey.Data ?? File.ReadAllBytes(config.KeyPair.PublicKey.FileName!));
            }
            if (config.Cert?.Exists() ?? false)
            {
                resultFiles.Add("result.crt", config.Cert.Data ?? File.ReadAllBytes(config.Cert.FileName!));
            }
            return this.WriteOutput(resultFiles, certOptions.OutputFile, certOptions.OutputType);
        }
        else
        {
            return ExitCode.Success;
        }
    }

    private ExitCode Start(CrlOptions crlOptions)
    {

        var readResult = this.ReadConfig(crlOptions, out ConfigCrl? request);
        if (readResult != ExitCode.Success)
        {
            return readResult;
        }
        // build
        ConfigCrl config;
        try
        {
            config = _service.Build(request!);
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return ExitCode.UnknownError;
        }

        if (crlOptions.OutputFile != null)
        {
            // create files
            var resultFiles = new Dictionary<string, byte[]>
            {
                { "input.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request, Program.JsonOptions)) },
                { "output.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(config, Program.JsonOptions)) },
            };
            if (config.KeyPair?.PrivateKey?.Exists() ?? false)
            {
                resultFiles.Add("issuer.key", config.KeyPair.PrivateKey.Data ?? File.ReadAllBytes(config.KeyPair.PrivateKey.FileName!));
            }
            if (config.Issuer?.Exists() ?? false)
            {
                resultFiles.Add("issuer.crt", config.Issuer.Data ?? File.ReadAllBytes(config.Issuer.FileName!));
            }
            if (config.Crl?.Exists() ?? false)
            {
                resultFiles.Add("result.crl", config.Crl.Data ?? File.ReadAllBytes(config.Crl.FileName!));
            }
            return this.WriteOutput(resultFiles, crlOptions.OutputFile, crlOptions.OutputType);
        }
        else
        {
            return ExitCode.Success;
        }
    }

    private ExitCode Start(CsrOptions csrOptions)
    {
        var readResult = this.ReadConfig(csrOptions, out ConfigCsr? request);
        if (readResult != ExitCode.Success)
        {
            return readResult;
        }
        // build
        ConfigCsr config;
        try
        {
            config = _service.Build(request!);
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return ExitCode.UnknownError;
        }

        if (csrOptions.OutputFile != null)
        {
            // create files
            var resultFiles = new Dictionary<string, byte[]>
            {
                { "input.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request, Program.JsonOptions)) },
                { "output.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(config, Program.JsonOptions)) },
            };
            if (config.Csr?.Exists() ?? false)
            {
                resultFiles.Add("result.csr", config.Csr.Data ?? File.ReadAllBytes(config.Csr.FileName!));
            }
            if (config.KeyPair?.PrivateKey?.Exists() ?? false)
            {
                resultFiles.Add("issuer.key", config.KeyPair.PrivateKey.Data ?? File.ReadAllBytes(config.KeyPair.PrivateKey.FileName!));
            }
            if (config.KeyPair?.PublicKey?.Exists() ?? false)
            {
                resultFiles.Add("issuer.key.pub", config.KeyPair.PublicKey.Data ?? File.ReadAllBytes(config.KeyPair.PublicKey.FileName!));
            }
            return this.WriteOutput(resultFiles, csrOptions.OutputFile, csrOptions.OutputType);
        }
        else
        {
            return ExitCode.Success;
        }
    }

    private ExitCode Start(KeyPairOptions keyPairOptions)
    {
        var readResult = this.ReadConfig(keyPairOptions, out ConfigKeyPair? request);
        if (readResult != ExitCode.Success)
        {
            return readResult;
        }
        // build
        ConfigKeyPair config;
        try
        {
            config = _service.Build(request!);
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return ExitCode.UnknownError;
        }

        if (keyPairOptions.OutputFile != null)
        {
            // create files
            var resultFiles = new Dictionary<string, byte[]>
            {
                { "input.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request, Program.JsonOptions)) },
                { "output.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(config, Program.JsonOptions)) },
            };
            if (config.PrivateKey?.Exists() ?? false)
            {
                resultFiles.Add("result.key", config.PrivateKey.Data ?? File.ReadAllBytes(config.PrivateKey.FileName!));
            }
            if (config.PublicKey?.Exists() ?? false)
            {
                resultFiles.Add("result.key.pub", config.PublicKey.Data ?? File.ReadAllBytes(config.PublicKey.FileName!));
            }
            return this.WriteOutput(resultFiles, keyPairOptions.OutputFile, keyPairOptions.OutputType);
        }
        else
        {
            return ExitCode.Success;
        }
    }

    private ExitCode Start(TransformOptions transformOptions)
    {
        var readResult = this.ReadConfig(transformOptions, out ConfigTransform? request);
        if (readResult != ExitCode.Success)
        {
            return readResult;
        }
        // build
        ConfigTransform config;
        try
        {
            config = _service.Build(request!);
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return ExitCode.UnknownError;
        }

        if (transformOptions.OutputFile != null)
        {
            // create files
            var resultFiles = new Dictionary<string, byte[]>
            {
                { "input.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request, Program.JsonOptions)) },
                { "output.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(config, Program.JsonOptions)) },
            };
            return this.WriteOutput(resultFiles, transformOptions.OutputFile, transformOptions.OutputType);
        }
        else
        {
            return ExitCode.Success;
        }
    }

    private ExitCode Start(PrintOptions printOptions)
    {
        // build
        var request = new ConfigTransform()
        {
            Mode = TransformMode.Print,
            Input = new List<X509File>()
            {
                new X509File()
                {
                    FileName = printOptions.InputFile
                }
            }
        };
        ConfigTransform config;
        try
        {
            config = _service.Build(request);
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return ExitCode.UnknownError;
        }

        if (config.Output != null)
        {
            for (int i = 0; i < config.Output.Count(); i++)
            {
                if (config.Output.ElementAt(i).Data != null)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(config.Output.ElementAt(i).Data ?? new byte[0]));
                }
            }
        }
        return ExitCode.Success;
    }

    private ExitCode ReadConfig<T>(ConfigOptions options, out T? config)
    {
        config = default(T);
        if (!File.Exists(options.ConfigFile))
        {
            Logger.Error("File {0} does not exist.", options.ConfigFile);
            return ExitCode.InvalidArguments;
        }
        config = JsonSerializer.Deserialize<T>(File.OpenRead(options.ConfigFile));
        if (config == null)
        {
            Logger.Error("Config is null.");
            return ExitCode.InvalidArguments;
        }
        return ExitCode.Success;
    }

    private ExitCode WriteOutput(Dictionary<string, byte[]> resultFiles, string? outFile, MediaType outType)
    {
        if (!File.Exists(outFile))
        {
            Logger.Error("File {0} does not exist.", outFile);
            return ExitCode.InvalidArguments;
        }
        try
        {
            using (var stream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose))
            {
                switch (outType)
                {
                    case Web.Models.MediaType.Tar:
                        this.CreateTar(stream, resultFiles);
                        break;
                    case Web.Models.MediaType.TarGz:
                        this.CreateTarGz(stream, resultFiles);
                        break;
                    case Web.Models.MediaType.Zip:
                        this.CreateZip(stream, resultFiles);
                        break;
                    default:
                        Logger.Error("Unsupported output format.");
                        return ExitCode.InvalidArguments;
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error("Failed to write to output file.", e);
            return ExitCode.UnknownError;
        }
        return ExitCode.Success;
    }

    private void CreateZip(Stream stream, IDictionary<string, byte[]> files, string? name = null)
    {
        using (var result = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var entry in files)
            {
                var zipArchiveEntry = result.CreateEntry(entry.Key);
                using (var entryStream = zipArchiveEntry.Open())
                {
                    entryStream.Write(entry.Value);
                }
            }
        }
    }

    private void CreateTar(Stream stream, IDictionary<string, byte[]> files)
    {
        var modTime = DateTime.UtcNow;
        var tarStream = new TarOutputStream(stream, null);
        TarArchive.CreateOutputTarArchive(tarStream);
        foreach (var entry in files)
        {
            var tarEntry = TarEntry.CreateTarEntry(entry.Key);
            tarEntry.Name = entry.Key;
            tarEntry.Size = entry.Value.Length;
            tarEntry.ModTime = modTime;
            tarStream.PutNextEntry(tarEntry);
            tarStream.Write(entry.Value);
            tarStream.CloseEntry();
        }
    }

    private void CreateTarGz(Stream stream, IDictionary<string, byte[]> files, string? name = null)
    {
        using (var gzip = new GZipOutputStream(stream))
        {
            this.CreateTar(gzip, files);
        }
    }

}