using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Requests;

public class GetCredentialsByIdRequest : WorkRequest
{
    public const string Name = nameof(GetCredentialsByIdRequest);
    public string Id { get; }
    public GetCredentialsByIdRequest(string id) : base(Name)
    {
        this.Id = id;
        this.Data = this;
    }
}