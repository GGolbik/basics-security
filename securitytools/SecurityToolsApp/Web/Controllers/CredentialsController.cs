using Microsoft.AspNetCore.Mvc;
using GGolbik.SecurityToolsApp.Work;
using GGolbik.SecurityTools.Work;
using GGolbik.SecurityToolsApp.Web.Requests;
using GGolbik.SecurityToolsApp.Credentials;
using GGolbik.SecurityTools.Credentials;
using GGolbik.SecurityTools.X509;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityToolsApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CredentialsController : Controller
{
    private readonly ILogger<CredentialsController> _logger;

    private readonly ICredentialsService _service;

    private readonly IWorkerService _workerService;

    private ProgramInfo _info = new();

    private IOptions<JsonOptions> _options;

    public CredentialsController(ILogger<CredentialsController> logger, ICredentialsService service, IWorkerService workerService, IOptions<JsonOptions> options)
    {
        _logger = logger;
        _service = service;
        _workerService = workerService;
        _options = options;
        _workerService.SetRequestHandler(GetCredentialsRequest.Name, this.HandleGetCredentials);
        _workerService.SetRequestHandler(GetCredentialsByIdRequest.Name, this.HandleGetCredentialsById);
        _workerService.SetRequestHandler(AddCredentialsRequest.Name, this.HandleAddCredentials);
        _workerService.SetRequestHandler(UpdateCredentialsRequest.Name, this.HandleUpdateCredentials);
        _workerService.SetRequestHandler(DeleteCredentialsRequest.Name, this.HandleDeleteCredentials);
    }

    private T? Parse<T>(IFormFile file)
    {
        using (var stream = file.OpenReadStream())
        {
            return JsonSerializer.Deserialize<T>(stream, _options.Value.JsonSerializerOptions);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The credentials and their IDs.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload sent in a 200 response depends on the request method.</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpGet()]
    [ProducesResponseType(typeof(IDictionary<string, KeyCredentials>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult GetCredentials()
    {
        return _workerService.GetResponse(new GetCredentialsRequest());
    }

    private object? HandleGetCredentials(WorkRequest request, CancellationToken token)
    {
        if (request.Data is not GetCredentialsRequest)
        {
            throw new ArgumentException();
        }
        return Ok(_service.GetCredentials());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The credentials.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload sent in a 200 response depends on the request method.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IDictionary<string, KeyCredentials>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult GetCredentialsById([FromRoute] string id)
    {
        return _workerService.GetResponse(new GetCredentialsByIdRequest(id));
    }

    private object? HandleGetCredentialsById(WorkRequest request, CancellationToken token)
    {
        if (request.Data is not GetCredentialsByIdRequest data)
        {
            throw new ArgumentException();
        }
        return Ok(_service.GetCredentials(data.Id));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The ID.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload sent in a 200 response depends on the request method.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpPost()]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult AddCredentials([FromForm(Name = "credentials")] IFormFile credentialsJson, [FromForm(Name = "password")] string? password = null)
    {
        var credentials = this.Parse<KeyCredentials>(credentialsJson);
        if (credentials == null)
        {
            throw new ArgumentException();
        }
        return _workerService.GetResponse(new AddCredentialsRequest(credentials, password));
    }

    private object? HandleAddCredentials(WorkRequest request, CancellationToken token)
    {
        if (request.Data is not AddCredentialsRequest data)
        {
            throw new ArgumentException();
        }
        KeyCredentials credentials = data.Credentials;
        if (data.Credentials.ToKeyCredentialsValue(null) is CertificateCredentials credentialsValue)
        {
            X509Certificate2? cert = null;
            X509Reader.ReadCertificates(new MemoryStream(credentialsValue.Certificate ?? []), data.Password, (c) => {
                cert = c;
            });
            var key = X509Reader.ReadKeyPair(new MemoryStream(credentialsValue.KeyPair ?? []), data.Password == null ? null : Encoding.UTF8.GetBytes(data.Password));
            credentials = new KeyCredentials(credentials.Description, new CertificateCredentials(cert?.ToDer(), key?.ToDer()));
        }
        return Ok(_service.AddCredentials(credentials));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The credentials.</returns>
    /// <response code="204">The 204 (No Content) status code indicates that the request has succeeded.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpPost("{id}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult UpdateCredentials([FromRoute] string id, [FromForm(Name = "credentials")] IFormFile credentialsJson, [FromForm] string? password = null)
    {
        var credentials = this.Parse<KeyCredentials>(credentialsJson);
        if (credentials == null)
        {
            throw new ArgumentException();
        }
        return _workerService.GetResponse(new UpdateCredentialsRequest(id, credentials, password));
    }

    private object? HandleUpdateCredentials(WorkRequest request, CancellationToken token)
    {
        if (request.Data is not UpdateCredentialsRequest data)
        {
            throw new ArgumentException();
        }
        KeyCredentials credentials = data.Credentials;
        if (data.Credentials.ToKeyCredentialsValue(null) is CertificateCredentials cert)
        {
            if (cert.KeyPair != null && data.Password != null)
            {
                var key = X509Reader.ReadKeyPair(new MemoryStream(cert.KeyPair), Encoding.UTF8.GetBytes(data.Password));
                credentials = new KeyCredentials(credentials.Description, new CertificateCredentials(cert.Certificate, cert.KeyPair));
            }
        }
        _service.UpdateCredentials(data.Id, credentials);
        return NoContent();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The credentials.</returns>
    /// <response code="204">The 204 (No Content) status code indicates that the request has succeeded.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
    public IActionResult DeleteCredentials([FromRoute] string id)
    {
        return _workerService.GetResponse(new DeleteCredentialsRequest(id));
    }

    private object? HandleDeleteCredentials(WorkRequest request, CancellationToken token)
    {
        if (request.Data is not DeleteCredentialsRequest data)
        {
            throw new ArgumentException();
        }
        _service.DeleteCredentials(data.Id);
        return NoContent();
    }

}
