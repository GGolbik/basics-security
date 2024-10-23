using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Requests;

public class DeleteCredentialsRequest : WorkRequest
{
    public const string Name = nameof(DeleteCredentialsRequest);
    public string Id { get; }
    public DeleteCredentialsRequest(string id) : base(Name)
    {
        this.Id = id;
        this.Data = this;
    }
}