using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Requests;

public class BuildCertRequest : WorkRequest
{
    public const string Name = nameof(BuildCertRequest);
    public IFormFile Config { get; }
    public List<IFormFile> Files { get; }
    public BuildCertRequest(IFormFile config, List<IFormFile> files) : base(Name)
    {
        this.Config = config;
        this.Files = files;
        this.Data = this;
    }
}