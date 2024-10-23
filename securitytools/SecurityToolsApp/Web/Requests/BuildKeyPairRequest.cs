using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Requests;

public class BuildKeyPairRequest : WorkRequest
{
    public const string Name = nameof(BuildKeyPairRequest);
    public IFormFile Config { get; }
    public List<IFormFile> Files { get; }
    public BuildKeyPairRequest(IFormFile config, List<IFormFile> files) : base(Name)
    {
        this.Config = config;
        this.Files = files;
        this.Data = this;
    }
}