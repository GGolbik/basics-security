using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Requests;

public class BuildCsrRequest : WorkRequest
{
    public const string Name = nameof(BuildCsrRequest);
    public IFormFile Config { get; }
    public List<IFormFile> Files { get; }
    public BuildCsrRequest(IFormFile config, List<IFormFile> files) : base(Name)
    {
        this.Config = config;
        this.Files = files;
        this.Data = this;
    }
}