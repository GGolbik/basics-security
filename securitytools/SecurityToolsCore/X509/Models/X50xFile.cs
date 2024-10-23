using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace GGolbik.SecurityTools.X509.Models;
public class X50xFile : ICloneable
{
    /// <summary>
    /// the file content
    /// 
    /// Has priority over <see cref="FileName"/>.
    /// </summary>
    public byte[]? Data { get; set; }

    /// <summary>
    /// The file name.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// A hint for the file format.
    /// </summary>
    public X50xFileFormat? FileFormat { get; set; }

    /// <summary>
    /// An optional password for the file.
    /// </summary>
    public string? Password { get; set; }

    public string? Alias { get; set; }

    /// <summary>
    /// If the alias type is undefined, the alias will be interpreted as an index starting at 0.
    /// <see cref="X509FindType.FindByThumbprint"/>, <see cref="X509FindType.FindBySubjectName"/>, <see cref="X509FindType.FindBySubjectDistinguishedName"/>
    /// </summary>
    public X509FindType? AliasType { get; set; }

    public X50xFile()
    {

    }

    public X50xFile(string? filename)
    {
        this.FileName = filename;
    }

    /// <summary>
    /// Returns whether there is data or a file.
    /// </summary>
    public bool Exists()
    {
        return this.Data != null | File.Exists(this.FileName);
    }

    public Stream ToStream()
    {
        return this.Data != null ? new MemoryStream(this.Data) : new FileStream(this.FileName!, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public object Clone()
    {
        return JsonSerializer.Deserialize<X50xFile>(JsonSerializer.Serialize(this))!;
    }
}
