using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.Models;
public class X509File
{
    /// <summary>
    /// Indicates a particular version of the <see cref="X509File"/>.
    /// </summary>
    public SchemaVersion? SchemaVersion { get; set; } = "1.0";

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
    public X509FileFormat? FileFormat { get; set; }

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

    public X509File()
    {

    }

    public X509File(string? filename)
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
}
