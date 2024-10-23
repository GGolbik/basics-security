using GGolbik.SecurityTools.Credentials;
using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Requests;

public class UpdateCredentialsRequest : WorkRequest
{
    public const string Name = nameof(UpdateCredentialsRequest);
    public string Id { get; }
    public KeyCredentials Credentials { get; }
    public string? Password;
    public UpdateCredentialsRequest(string id, KeyCredentials credentials, string? password = null) : base(Name)
    {
        this.Id = id;
        this.Credentials = credentials;
        this.Password = password;
        this.Data = this;
    }
}