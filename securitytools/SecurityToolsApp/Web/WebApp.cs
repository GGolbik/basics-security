using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using GGolbik.SecurityToolsApp.Web.Swagger;
using Serilog;
using System.Security.Cryptography.X509Certificates;
using GGolbik.SecurityToolsApp.Terminal.Options;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using GGolbik.SecurityToolsApp.Diagnostic;
using GGolbik.SecurityTools.X509;
using GGolbik.SecurityTools.X509.Models;
using ElectronNET.API;
using GGolbik.SecurityToolsApp.Tools;
using GGolbik.SecurityToolsApp.Work;
using GGolbik.SecurityToolsApp.Credentials;

namespace GGolbik.SecurityToolsApp.Web;

internal class WebApp : IDisposable
{

    private Serilog.ILogger Logger = Log.ForContext<WebApp>();
    private readonly ILoggingService _loggingService;
    private readonly IWorkerService _workerService;
    private readonly ISecurityToolsService _securityToolsService;
    private readonly ICredentialsService _credentialsService;

    private X509Certificate2? _webCert;

    private WebApplication? _app;

    private WebOptions _options;

    public WebApp(
        WebOptions options,
        ILoggingService loggingService,
        IWorkerService workerService,
        ISecurityToolsService securityToolsService,
        ICredentialsService credentialsService
    )
    {
        _options = options;
        _loggingService = loggingService;
        _workerService = workerService;
        _securityToolsService = securityToolsService;
        _credentialsService = credentialsService;
        this.CreateWebCert();
    }

    public void Dispose()
    {
        _app?.DisposeAsync().AsTask().Wait();
    }

    /// <summary>
    /// Configures the services for the web app.
    /// </summary>
    /// <param name="builder">The web builder</param>
    private void AddServices(WebApplicationBuilder builder)
    {
        // configure hsts
        this.AddHsts(builder);

        // lowercase routing
        builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        // add controllers
        this.AddControllers(builder);

        // add Versioning
        this.AddApiVersioningService(builder);

        // add swagger
        this.AddSwaggerService(builder);

        // Add web services
        this.AddAppServices(builder);
    }

    /// <summary>
    /// Configures HSTS if enabled by config.
    /// </summary>
    /// <param name="builder">The web builder</param>
    private void AddHsts(WebApplicationBuilder builder)
    {
        if (_options.Hsts == null)
        {
            // disabled by default
            return;
        }
        if (_options.Hsts ?? false)
        {
            builder.Services.AddHsts((options) =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(30);
            });
        }
    }

    /// <summary>
    /// Adds additional json convertes for the REST API
    /// </summary>
    /// <param name="builder">The web builder</param>
    private void AddControllers(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ErrorActionFilter>();
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = _options.GetJsonSerializerOptions().PropertyNameCaseInsensitive;
            options.JsonSerializerOptions.PropertyNamingPolicy = _options.GetJsonSerializerOptions().PropertyNamingPolicy;
            foreach (var c in _options.GetJsonSerializerOptions().Converters)
            {
                options.JsonSerializerOptions.Converters.Add(c);
            }
        });
    }

    /// <summary>
    /// Adds versioning to the API URLs
    /// </summary>
    /// <param name="builder">The web builder</param>
    private void AddApiVersioningService(WebApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
        {
            // Advertise the API versions supported for the particular endpoint.
            // It will add both api-supported-versions and api-deprecated-versions headers to our response.
            options.ReportApiVersions = true;
            // Specify the default API Version
            options.DefaultApiVersion = new ApiVersion(1, 0);
            // If the client hasn't specified the API version in the request, use the default API version number 
            options.AssumeDefaultVersionWhenUnspecified = true;
            // Finally, because we are going to support different versioning schemes, with the ApiVersionReader property, we combine different ways of reading the API version (from a query string, request header, and media type).
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("api-version")
            );
        }).AddApiExplorer(options =>
        {
            // see https://github.com/dotnet/aspnet-api-versioning/wiki/Version-Format#custom-api-version-format-strings for more info about the format.
            // this option is only necessary when versioning by url segment.
            //setup.GroupNameFormat = "'v'VVVV"; // the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVVV";
        });
    }

    /// <summary>
    /// Adds swagger service
    /// </summary>
    /// <param name="builder">The web builder</param>
    private void AddSwaggerService(WebApplicationBuilder builder)
    {
        // customize swagger
        builder.Services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>((provider) =>
        {
            var apiVersionDescriptionProvider = provider.GetRequiredService<IApiVersionDescriptionProvider>();
            var programInfo = new ProgramInfo();
            var info = new OpenApiInfo()
            {
                Title = programInfo.Title + " API",
                Description = programInfo.Description,
                Contact = new OpenApiContact()
                {
                    Name = "GGolbik",
                    Email = "support@ggolbik.de"
                }
            };
            return new ConfigureSwaggerOptions(apiVersionDescriptionProvider, info);
        });

        // add service to generate swagger doc
        builder.Services.AddSwaggerGen(options =>
        {
            // customize type name of Schemas
            options.CustomSchemaIds(type => type.ToString());
            options.SupportNonNullableReferenceTypes();

            // https://stackoverflow.com/a/54572112
            //options.ResolveConflictingActions (api => api.First());

            options.OperationFilter<SwaggerDefaultValues>();
            // This call remove version from parameter, without it we will have version as parameter 
            // for all endpoints in swagger UI
            options.OperationFilter<RemoveVersionFromParameter>();
        });
    }

    /// <summary>
    /// Adds all services of the app.
    /// </summary>
    /// <param name="builder">The web builder</param>
    private void AddAppServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton((provider) =>
        {
            return _loggingService;
        });
        builder.Services.AddSingleton((provider) =>
        {
            return _workerService;
        });
        builder.Services.AddSingleton((provider) =>
        {
            return _securityToolsService;
        });
        builder.Services.AddSingleton((provider) =>
        {
            return _credentialsService;
        });
    }

    /// <summary>
    /// Configures the middleware pipeline.
    /// WebApplicationBuilder configures a middleware pipeline that wraps middleware with UseRouting and UseEndpoints.
    /// However, apps can change the order in which UseRouting and UseEndpoints run by calling these methods explicitly.
    /// <see href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0#middleware-order">Middleware order</see>
    /// </summary>
    private void ConfigurePipeline(WebApplication app)
    {
        // enable HSTS and HTTPS redirect only in production and not in development
        if (!app.Environment.IsDevelopment())
        {
            // Use HTST if enabled
            if (_options.Hsts ?? false)
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // UseHsts excludes the following loopback hosts:
                // - localhost : The IPv4 loopback address.
                // - 127.0.0.1 : The IPv4 loopback address.
                // - [::1] : The IPv6 loopback address.
                app.UseHsts();
            }

            // perform HTTPS redirection if enabled
            if (_options.HttpsRedirection ?? false)
            {
                app.UseHttpsRedirection();
            }
        }

        if (_options.UseSerilogRequestLogging)
        {
            app.UseSerilogRequestLogging();
        }

        // configure Localization
        WebApp.UseLocalizationMiddleware(app);

        // use index files as default if no file is specified e.g. index.html
        app.UseDefaultFiles();

        // provide static files from the wwwroot folder
        app.UseStaticFiles();

        // add swagger doc
        WebApp.UseSwaggerMiddleware(app, true //app.Environment.IsDevelopment()
        );

        // enable routing
        app.UseRouting();

        // map all controllers
        app.MapControllerRoute(
            name: "default",
            pattern: "api/{controller}/{action=Index}/{id?}");

        // fallback to index.html for all others. The rest is handled by Angular
        app.MapFallbackToFile("index.html");

        // explicit call of UseEndpoints is optional.
        //app.UseEndpoints();
    }

    /// <summary>
    /// Removes all loggers and uses serilog instead.
    /// </summary>
    /// <param name="builder">The web builder</param>
    private void SetupLogging(WebApplicationBuilder builder)
    {
        // remove all default loggers and use Serilog
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog();
    }

    /// <summary>
    /// Adds Swagger doc middleware
    /// </summary>
    /// <param name="app"></param>
    /// <param name="submitButtonsEnabled"></param>
    private static void UseSwaggerMiddleware(WebApplication app, bool submitButtonsEnabled)
    {
        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "api/doc/{documentName}/doc.json";
        });
        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
        app.UseSwaggerUI(options =>
        {
            // Access swagger at path
            options.RoutePrefix = "api/doc";

            options.InjectStylesheet("/css/swagger-dark.css");

            // see https://swagger.io/docs/open-source-tools/swagger-ui/usage/configuration/ for more info about the UI configuration.
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            // iterate in reverse order to show the latest version in the swagger UI by default.
            var programInfo = new ProgramInfo();
            foreach (var desc in provider.ApiVersionDescriptions.Reverse())
            {
                // specifying the Swagger JSON endpoint.
                // define the endpoints for the different API routes.
                options.SwaggerEndpoint($"/api/doc/{desc.GroupName}/doc.json", desc.ApiVersion.ToString("'v'VVVV"));
                // Define whether the schemas of the API models should be shown. The value of -1 will hide the schemas.
                options.DefaultModelsExpandDepth(0);
                // Define whether the API groups should be expanded by default
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                // Define tab header
                options.DocumentTitle = String.Format("{0} API Documentation", programInfo.Title);
            }

            if (!submitButtonsEnabled)
            {
                options.SupportedSubmitMethods(new Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod[] {
                    //Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Get
                });
            }
        });
    }

    /// <summary>
    /// UseRequestLocalization must appear before any middleware that might check the request culture.
    /// </summary>
    private static void UseLocalizationMiddleware(WebApplication app)
    {
        var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(localizationOptions.Value);
    }

    private void CreateWebCert()
    {
        using (var service = new SecurityToolsService())
        {
            var rootCaConfig = new ConfigCert()
            {
                Csr = new ConfigCsr()
                {
                    SubjectName = new X50xSubjectName("Web"),
                },
                Extensions = new X50xExtensions()
                {
                    BasicConstraints = new X50xBasicConstraintsExtension()
                    {
                        CertificateAuthority = false,
                        Critical = true,
                        HasPathLengthConstraint = false,
                    },
                    KeyUsage = new X50xKeyUsageExtension()
                    {
                        Critical = true,
                        KeyUsages = X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment
                    },
                    ExtendedKeyUsage = new X50xExtendedKeyUsageExtension()
                    {
                        Critical = true,
                        ExtendedKeyUsages = ExtendedKeyUsageFlags.ServerAuth
                    }
                },
            };
            var result = service.Build(rootCaConfig);
            _webCert = new X509Certificate2(result.Cert!.Data!);
        }
    }

    /// <summary>
    /// Setup the web app.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private ExitCode Setup(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            Args = args,
            ContentRootPath = AppContext.BaseDirectory
        });
        builder.Host.UseWindowsService();
        builder.Host.UseSystemd();

        builder.WebHost.UseElectron(args);

        // log
        this.SetupLogging(builder);

        // Change the defaults in code
        builder.WebHost.ConfigureKestrel((options) =>
        {
            options.ConfigureHttpsDefaults((httpsOptions) =>
            {
                httpsOptions.ServerCertificate = _webCert;
            });
        });

        // Add services to the container.
        this.AddServices(builder);

        // Build the WebApplication.
        this._app = builder.Build();

        // Configure the HTTP request pipeline.
        this.ConfigurePipeline(_app);
        if (HybridSupport.IsElectronActive)
        {
            Electron.Menu.SetApplicationMenu([]);
            var window = Electron.WindowManager.CreateWindowAsync();
            window.Wait();
            window.Result.OnClosed += () => Electron.App.Quit();
        }
        return ExitCode.Success;
    }

    /// <summary>
    /// Runs the application and block the calling thread until host shutdown.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public ExitCode Run(string[] args)
    {
        if (this._app == null)
        {
            this.Setup(args);
        }
        // Run the WebApplication.
        if (this._app == null)
        {
            return ExitCode.UnknownError;
        }
        this._app.Run();
        return ExitCode.Success;
    }

    /// <summary>
    /// Starts the host synchronously.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public ExitCode Start(string[] args)
    {
        if (this._app == null)
        {
            this.Setup(args);
        }
        if (this._app == null)
        {
            return ExitCode.UnknownError;
        }
        // Run the WebApplication.
        this._app.Start();
        // print info
        foreach (var address in this.GetAddresses())
        {
            Logger.Debug("Address: {0}", address);
        }

        return ExitCode.Success;
    }

    /// <summary>
    /// returns a task which waits until shutdown is triggered via Ctrl+C or SIGTERM
    /// </summary>
    /// <returns></returns>
    public Task? WaitForShutdownAsync()
    {
        if (_app != null)
        {
            return _app.WaitForShutdownAsync();
        }
        return null;
    }

    /// <summary>
    /// Stops the app asynchronously
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_app != null)
        {
            return _app.StopAsync(cancellationToken);
        }
        return Task.CompletedTask;
    }

    public ICollection<string> GetAddresses()
    {
        if (_app == null)
        {
            return new List<string>();
        }
        var server = _app.Services.GetService<IServer>();
        var addressFeature = server?.Features.Get<IServerAddressesFeature>();
        var result = addressFeature?.Addresses ?? new List<string>();
#if DEBUG
        result = new List<string>(result);
        // see securitytools/src/ClientApp/package.json
        result.Add("http://localhost:44407");
#endif
        return result;
    }
}
