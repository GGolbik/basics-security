using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Requests;

public class GetCredentialsRequest : WorkRequest
{
    public const string Name = nameof(GetCredentialsRequest);
    public GetCredentialsRequest() : base(Name)
    {
        this.Data = this;
    }
}