using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Requests;

public class TransformRequest : WorkRequest
{
    public const string Name = nameof(TransformRequest);
    public IFormFile Config { get; }
    public List<IFormFile> Files { get; }
    public TransformRequest(IFormFile config, List<IFormFile> files) : base(Name)
    {
        this.Config = config;
        this.Files = files;
        this.Data = this;
    }
}