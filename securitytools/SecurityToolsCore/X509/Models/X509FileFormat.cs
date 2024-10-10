
using System.Security.Cryptography.X509Certificates;
using GGolbik.SecurityTools.X509;

namespace GGolbik.SecurityTools.X509.Models;
public class X509FileFormat
{
    /// <summary>
    /// Indicates a particular version of the <see cref="X509FileFormat"/>.
    /// </summary>
    public SchemaVersion? SchemaVersion { get; set; } = "1.0";

    /// <summary>
    /// The kind of content.
    /// </summary>
    public X509ContentType? ContentKind { get; set; }

    /// <summary>
    /// Whether to create a PEM encoded format. Otherwise DER.
    /// If Format is Pkcs12, all certifcates are encoded as PEM and keys as Pkcs8 pem into one file.
    /// </summary>
    public X509Encoding? Encoding { get; set; } // otherwise DER

    /// <summary>
    /// The code page identifier of the encoding to use, e.g. 65001 for utf-8, or 28591 for iso-8859-1 or 20127 for us-ascii.
    /// </summary>
    public int? CodePage { get; set; }

    public X509FileFormat()
    {

    }

    public X509FileFormat(X509ContentType format)
    {
        this.ContentKind = format;
    }
    public X509FileFormat(X509Encoding encoding)
    {
        this.Encoding = encoding;
    }

    public X509FileFormat(X509ContentType format, X509Encoding encoding)
    {
        this.ContentKind = format;
        this.Encoding = encoding;
    }
}