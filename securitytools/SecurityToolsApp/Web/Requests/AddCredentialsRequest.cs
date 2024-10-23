using GGolbik.SecurityTools.Credentials;
using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Requests;

public class AddCredentialsRequest : WorkRequest
{
    public const string Name = nameof(AddCredentialsRequest);
    public KeyCredentials Credentials { get; }
    public string? Password { get; }
    public AddCredentialsRequest(KeyCredentials credentials, string? password = null) : base(Name)
    {
        this.Credentials = credentials;
        this.Password = password;
        this.Data = this;
    }
}