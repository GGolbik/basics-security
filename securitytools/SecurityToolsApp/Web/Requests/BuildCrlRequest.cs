using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Requests;

public class BuildCrlRequest : WorkRequest
{
    public const string Name = nameof(BuildCrlRequest);
    public IFormFile Config { get; }
    public List<IFormFile> Files { get; }
    public BuildCrlRequest(IFormFile config, List<IFormFile> files) : base(Name)
    {
        this.Config = config;
        this.Files = files;
        this.Data = this;
    }
}