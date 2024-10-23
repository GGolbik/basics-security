using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Requests;

public class GetInfoRequest : WorkRequest
{
    public const string Name = nameof(GetInfoRequest);
    public GetInfoRequest() : base(Name)
    {
        this.Data = this;
    }
}