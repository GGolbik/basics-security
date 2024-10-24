using System.IO.Compression;
using System.Text;
using System.Text.Json;
using GGolbik.SecurityTools.X509.Models;
using GGolbik.SecurityToolsApp.Web.Models;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.AspNetCore.Mvc;
using GGolbik.SecurityTools.X509;
using GGolbik.SecurityToolsApp.Work;
using GGolbik.SecurityToolsApp.Tools;
using GGolbik.SecurityTools.Work;
using GGolbik.SecurityToolsApp.Web.Requests;
using Microsoft.Extensions.Options;

namespace GGolbik.SecurityToolsApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SecurityToolsController : Controller
{
    private readonly ILogger<SecurityToolsController> _logger;

    private readonly ISecurityToolsService _service;

    private readonly IWorkerService _workerService;

    private readonly IOptions<JsonOptions> _options;

    private ProgramInfo _info = new();


    public SecurityToolsController(ILogger<SecurityToolsController> logger, ISecurityToolsService service, IWorkerService workerService, IOptions<JsonOptions> options)
    {
        _logger = logger;
        _service = service;
        _workerService = workerService;
        _options = options;
        _workerService.SetRequestHandler(GetInfoRequest.Name, this.HandleGetInfo);
        _workerService.SetRequestHandler(BuildCertRequest.Name, this.HandleBuildCert);
        _workerService.SetRequestHandler(BuildCsrRequest.Name, this.HandleBuildCsr);
        _workerService.SetRequestHandler(BuildCrlRequest.Name, this.HandleBuildCrl);
        _workerService.SetRequestHandler(BuildKeyPairRequest.Name, this.HandleBuildKeyPair);
        _workerService.SetRequestHandler(TransformRequest.Name, this.HandleBuildTransform);
    }

    /// <summary>
    /// Returns info about the application.
    /// </summary>
    /// <returns>The application info.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload sent in a 200 response depends on the request method.</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpGet("info")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ProgramInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult GetInfo()
    {
        return _workerService.GetResponse(new GetInfoRequest());
    }

    private object? HandleGetInfo(WorkRequest request, CancellationToken token)
    {
        return Ok(_info);
    }

    /// <summary>
    /// Creates a certificate from a certificate signing request (CSR). If no issuer is provided, a self-signed certificate will be created.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="files"></param>
    /// <returns>The created archive or the request.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload sent in a 200 response depends on the request method.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpPost("build/certificate")]
    [Consumes("multipart/form-data")]
    [Produces("application/zip", "application/x-tar", "application/gzip", "application/octet-stream")]
    [ProducesResponseType(typeof(ConfigCert), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult BuildCert(IFormFile config, List<IFormFile> files)
    {
        return _workerService.GetResponse(new BuildCertRequest(config, files));
    }

    private object? HandleBuildCert(WorkRequest request, CancellationToken token)
    {
        if (request.Data is not BuildCertRequest buildCertRequest)
        {
            throw new ArgumentException();
        }
        var config = buildCertRequest.Config;
        var files = buildCertRequest.Files;

        var x_config = this.GetConfig<ConfigCert>(config);
        var configFiles = new HashSet<X50xFile>();
        FindAllFiles(configFiles, x_config);
        ReplaceFilename(configFiles, files);

        // build
        var result = _service.Build(x_config);

        // create files
        var resultFiles = new Dictionary<string, byte[]>
        {
            { "input.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x_config, _options.Value.JsonSerializerOptions) ?? "") },
            { "output.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result, _options.Value.JsonSerializerOptions) ?? "") },
            { "result.csr", result.Csr!.Csr!.Data! },
            { "issuer.key", result.KeyPair!.PrivateKey!.Data! },
            { "issuer.key.pub", result.KeyPair!.PublicKey!.Data! },
            { "result.crt", result.Cert!.Data! }
        };
        var filename = result.Csr?.SubjectName?.ToX500DistinguishedName().Name;

        var accept = this.GetMediaType();
        switch (accept)
        {
            case MediaType.Tar:
                return this.CreateTar(resultFiles, filename);
            case MediaType.TarGz:
                return this.CreateTarGz(resultFiles, filename);
            default:
                return this.CreateZip(resultFiles, filename);
        }
    }

    /// <summary>
    /// Creates a certificate signing request (CSR).
    /// </summary>
    /// <param name="config"></param>
    /// <param name="files"></param>
    /// <returns>The created archive or the request.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload sent in a 200 response depends on the request method.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpPost("build/csr")]
    [Consumes("multipart/form-data")]
    [Produces("application/zip", "application/x-tar", "application/gzip", "application/octet-stream")]
    [ProducesResponseType(typeof(ConfigCsr), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult BuildCsr(IFormFile config, List<IFormFile> files)
    {
        return _workerService.GetResponse(new BuildCsrRequest(config, files));
    }

    private object? HandleBuildCsr(WorkRequest request, CancellationToken token)
    {
        if (request.Data is not BuildCsrRequest buildCsrRequest)
        {
            throw new ArgumentException();
        }
        var config = buildCsrRequest.Config;
        var files = buildCsrRequest.Files;

        var x_config = this.GetConfig<ConfigCsr>(config);
        var configFiles = new HashSet<X50xFile>();
        FindAllFiles(configFiles, x_config);
        ReplaceFilename(configFiles, files);

        // build
        var result = _service.Build(x_config);

        // create files
        var resultFiles = new Dictionary<string, byte[]>
        {
            { "input.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x_config, _options.Value.JsonSerializerOptions) ?? "") },
            { "output.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result, _options.Value.JsonSerializerOptions) ?? "") },
            { "result.csr", result.Csr!.Data! },
            { "issuer.key", result.KeyPair!.PrivateKey!.Data! },
            { "issuer.key.pub", result.KeyPair!.PublicKey!.Data! },
        };

        var filename = result.SubjectName?.ToX500DistinguishedName().Name;

        var accept = this.GetMediaType();
        switch (accept)
        {
            case MediaType.Tar:
                return this.CreateTar(resultFiles, filename);
            case MediaType.TarGz:
                return this.CreateTarGz(resultFiles, filename);
            default:
                return this.CreateZip(resultFiles, filename);
        }
    }


    /// <summary>
    /// Creates a certificate revocation list (CRL).
    /// </summary>
    /// <param name="config"></param>
    /// <param name="files"></param>
    /// <returns>The created archive or the request.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload sent in a 200 response depends on the request method.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpPost("build/crl")]
    [Consumes("multipart/form-data")]
    [Produces("application/zip", "application/x-tar", "application/gzip", "application/octet-stream")]
    [ProducesResponseType(typeof(ConfigCrl), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult BuildCrl(IFormFile config, List<IFormFile> files)
    {
        return _workerService.GetResponse(new BuildCrlRequest(config, files));
    }

    private object? HandleBuildCrl(WorkRequest request, CancellationToken token)
    {
        if (request.Data is not BuildCrlRequest buildCrlRequest)
        {
            throw new ArgumentException();
        }
        var config = buildCrlRequest.Config;
        var files = buildCrlRequest.Files;

        var x_config = this.GetConfig<ConfigCrl>(config);
        var configFiles = new HashSet<X50xFile>();
        FindAllFiles(configFiles, x_config);
        ReplaceFilename(configFiles, files);

        // build
        var result = _service.Build(x_config);

        // create files
        var resultFiles = new Dictionary<string, byte[]>
        {
            { "input.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x_config, _options.Value.JsonSerializerOptions) ?? "") },
            { "output.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result, _options.Value.JsonSerializerOptions) ?? "") },
            { "issuer.key", result.KeyPair!.PrivateKey!.Data! },
            { "issuer.crt", result.Issuer!.Data! },
            { "result.crl", result.Crl!.Data! },
        };

        var accept = this.GetMediaType();
        switch (accept)
        {
            case MediaType.Tar:
                return this.CreateTar(resultFiles);
            case MediaType.TarGz:
                return this.CreateTarGz(resultFiles);
            default:
                return this.CreateZip(resultFiles);
        }
    }

    /// <summary>
    /// Creates a key pair.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="files"></param>
    /// <returns>The created archive or the request.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload sent in a 200 response depends on the request method.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpPost("build/keypair")]
    [Consumes("multipart/form-data")]
    [Produces("application/zip", "application/x-tar", "application/gzip", "application/octet-stream")]
    [ProducesResponseType(typeof(ConfigKeyPair), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult BuildKeyPair(IFormFile config, List<IFormFile> files)
    {
        return _workerService.GetResponse(new BuildKeyPairRequest(config, files));
    }

    private object? HandleBuildKeyPair(WorkRequest request, CancellationToken token)
    {
        if (request.Data is not BuildKeyPairRequest buildKeyPairRequest)
        {
            throw new ArgumentException();
        }
        var config = buildKeyPairRequest.Config;
        var files = buildKeyPairRequest.Files;

        var x_config = this.GetConfig<ConfigKeyPair>(config);
        var configFiles = new HashSet<X50xFile>();
        FindAllFiles(configFiles, x_config);
        ReplaceFilename(configFiles, files);

        // build
        var result = _service.Build(x_config);

        // create files
        var resultFiles = new Dictionary<string, byte[]>
        {
            { "input.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x_config, _options.Value.JsonSerializerOptions) ?? "") },
            { "output.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result, _options.Value.JsonSerializerOptions) ?? "") },
            { "result.key", result.PrivateKey!.Data! },
            { "result.key.pub", result.PublicKey!.Data! },
        };

        var accept = this.GetMediaType();
        switch (accept)
        {
            case MediaType.Tar:
                return this.CreateTar(resultFiles);
            case MediaType.TarGz:
                return this.CreateTarGz(resultFiles);
            default:
                return this.CreateZip(resultFiles);
        }
    }

    /// <summary>
    /// Transforms a list of input files into the requested output format.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="files"></param>
    /// <returns>The created archive or the request.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload sent in a 200 response depends on the request method.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpPost("build/transform")]
    [Consumes("multipart/form-data")]
    [Produces("application/zip", "application/x-tar", "application/gzip", "application/octet-stream")]
    [ProducesResponseType(typeof(ConfigTransform), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult Transform(IFormFile config, List<IFormFile> files)
    {
        return _workerService.GetResponse(new TransformRequest(config, files));
    }

    private object? HandleBuildTransform(WorkRequest request, CancellationToken token)
    {
        if (request.Data is not TransformRequest transformRequest)
        {
            throw new ArgumentException();
        }
        var config = transformRequest.Config;
        var files = transformRequest.Files;

        var x_config = this.GetConfig<ConfigTransform>(config);
        var configFiles = new HashSet<X50xFile>();
        FindAllFiles(configFiles, x_config);
        ReplaceFilename(configFiles, files);

        // build
        var result = _service.Build(x_config);

        // create files
        var resultFiles = new Dictionary<string, byte[]>
        {
            { "input.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x_config, _options.Value.JsonSerializerOptions) ?? "") },
            { "output.conf.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result, _options.Value.JsonSerializerOptions) ?? "") },
        };

        if (result.Output != null)
        {
            for (int i = 0; i < result.Output.Count(); i++)
            {
                if (result.Output.ElementAt(i).Data != null)
                {
                    string name = i + "_" + (result.Output.ElementAt(i).Alias ?? "");
                    name = name.ReplaceAll(Path.GetInvalidFileNameChars(), '_');
                    resultFiles.Add(name, result.Output.ElementAt(i).Data!);
                }
            }
        }

        var accept = this.GetMediaType();
        switch (accept)
        {
            case MediaType.Tar:
                return this.CreateTar(resultFiles);
            case MediaType.TarGz:
                return this.CreateTarGz(resultFiles);
            default:
                return this.CreateZip(resultFiles);
        }
    }

    /// <summary>
    /// Returns a string representation of the provided file.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>The created archive or the request.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload sent in a 200 response depends on the request method.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpPost("print")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult Print([FromBody] X50xFile request)
    {
        var reqConfig = new ConfigTransform
        {
            Mode = TransformMode.Print,
            Input = new List<X50xFile>() {
                request
            }
        };

        // build
        var config = _service.Build(reqConfig);

        var result = new List<string>();
        if (config.Output != null)
        {
            for (int i = 0; i < config.Output.Count(); i++)
            {
                if (config.Output.ElementAt(i).Data != null)
                {
                    result.Add(Encoding.UTF8.GetString(config.Output.ElementAt(i).Data ?? new byte[0]));
                }
            }
        }

        return Ok(result);
    }

    /// <summary>
    /// Transform a list of input files into the requested output format.
    /// </summary>
    /// <param name="files"></param>
    /// <returns>The created archive or the request.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload sent in a 200 response depends on the request method.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpPost("print/files")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [DisableRequestSizeLimit]
    [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult Print(List<IFormFile> files)
    {
        var reqConfig = new ConfigTransform
        {
            Mode = TransformMode.Print,
            Input = new List<X50xFile>()
        };
        foreach (var formFile in files)
        {
            using (var mem = new MemoryStream())
            {
                using (var stream = formFile.OpenReadStream())
                {
                    stream.CopyTo(mem);
                }
                reqConfig.Input.Add(new X50xFile()
                {
                    Data = mem.ToArray()
                });
            }
        }

        // build
        var config = _service.Build(reqConfig);

        var result = new List<string>();
        if (config.Output != null)
        {
            for (int i = 0; i < config.Output.Count(); i++)
            {
                if (config.Output.ElementAt(i).Data != null)
                {
                    result.Add(Encoding.UTF8.GetString(config.Output.ElementAt(i).Data ?? new byte[0]));
                }
            }
        }

        return Ok(result);
    }

    private T GetConfig<T>(IFormFile configFile)
    {
        if (!configFile.ContentType.ToLower().Contains("application/json"))
        {
            throw new ArgumentException("config must be of type application/json");
        }
        using (var stream = configFile.OpenReadStream())
        {
            var config = JsonSerializer.Deserialize<T>(stream, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            if (config == null)
            {
                throw new ArgumentException("config is missing.");
            }
            return config;
        }
    }

    private MediaType GetMediaType()
    {
        var accepts = Request.Headers.Accept.Where((item) =>
        {
            if (item == null)
            {
                return false;
            }
            var types = new List<string>() {
                "application/zip",
                "application/x-tar",
                "application/gzip"
            };
            foreach (var type in types)
            {
                if (item.Contains(type))
                {
                    return true;
                }
            }
            return false;
        }).Select((item) =>
        {
            if (item!.Contains("application/zip"))
            {
                return MediaType.Zip;
            }
            if (item!.Contains("application/x-tar"))
            {
                return MediaType.Tar;
            }
            if (item!.Contains("application/gzip"))
            {
                return MediaType.TarGz;
            }
            return MediaType.None;
        });
        return accepts.FirstOrDefault(MediaType.Zip);
    }


    private void FindAllFiles(HashSet<X50xFile> files, object? obj)
    {
        if (obj == null)
        {
            return;
        }

        var objType = obj.GetType();
        Array? array = null;
        if(objType.IsArray)
        {
            array = obj as Array;
        }
        var list = obj as IEnumerable<object>;
        if (array != null)
        {
            foreach (var item in array)
            {
                FindAllFiles(files, item);
            }
        }
        else if (objType.IsPrimitive)
        {
            return;
        }
        else if (list != null)
        {
            foreach (var item in list)
            {
                FindAllFiles(files, item);
            }
        }
        else if(objType.Namespace?.StartsWith("GGolbik") ?? false)
        {
            foreach (var propertyInfo in objType.GetProperties())
            {
                if (propertyInfo.PropertyType.IsPrimitive)
                {
                    continue;
                }
                var value = propertyInfo.GetValue(obj);
                if (value != null && value is X50xFile file)
                {
                    files.Add(file);
                }
                else
                {
                    FindAllFiles(files, value);
                }
            }
        }
    }

    private void ReplaceFilename(HashSet<X50xFile> configFiles, List<IFormFile> formFiles)
    {
        foreach (var file in configFiles)
        {
            if (file.Data == null && !string.IsNullOrEmpty(file.FileName))
            {
                var formFile = formFiles.Find((IFormFile formFile) =>
                {
                    return file.FileName.Equals(formFile.FileName);
                });
                if (formFile == null)
                {
                    _logger.LogWarning("No matching file found for {0}", file.FileName);
                }
                else
                {
                    using (var mem = new MemoryStream())
                    {
                        using (var stream = formFile.OpenReadStream())
                        {
                            stream.CopyTo(mem);
                        }
                        file.Data = mem.ToArray();
                    }
                }
            }
            file.FileName = null;
        }
    }

    private FileResult CreateZip(IDictionary<string, byte[]> files, string? name = null)
    {
        var stream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose);
        try
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
            stream.Flush();
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            return File(stream, "application/zip", ToFilename(name, "result", "zip"));
        }
        catch
        {
            stream.Close();
            throw;
        }
    }

    private FileResult CreateTar(IDictionary<string, byte[]> files, string? name = null)
    {
        var stream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose);
        try
        {
            this.CreateTar(stream, files);
            stream.Flush();
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            return File(stream, "application/x-tar", ToFilename(name, "result", "tar"));
        }
        catch
        {
            stream.Close();
            throw;
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


    private FileResult CreateTarGz(IDictionary<string, byte[]> files, string? name = null)
    {
        var stream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose);
        try
        {
            using (var wrapper = new StreamWrapper(stream, true))
            using (var gzip = new GZipOutputStream(wrapper))
            {
                this.CreateTar(gzip, files);
            }
            stream.Flush();
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            return File(stream, "application/gzip", ToFilename(name, "result", "tar.gz"));
        }
        catch
        {
            stream.Close();
            throw;
        }
    }

    private static string ToFilename(string? name, string defaultName, string extension)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = defaultName;
        }
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        name = name.Replace(' ', '_');
        name = name.Replace('\\', '_');
        name = name.Replace('/', '_');
        name = name.Replace(',', '_');
        name = name.Replace(';', '_');
        name = name.Replace('=', '_');
        return name + "." + extension;
    }

}
