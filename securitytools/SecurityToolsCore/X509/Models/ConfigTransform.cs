
using System.Text.Json;

namespace GGolbik.SecurityTools.X509.Models;


public class ConfigTransform : ICloneable
{
    /// <summary>
    /// Indicates a particular version of the <see cref="ConfigTransform"/>.
    /// </summary>
    public SchemaVersion? SchemaVersion { get; set; } = "1.0";

    public TransformMode? Mode { get; set; }

    public IList<X50xFile>? Input { get; set; }

    public IList<X50xFile>? Output { get; set; }

    public object Clone()
    {
        return JsonSerializer.Deserialize<ConfigTransform>(JsonSerializer.Serialize(this))!;
    }
}
