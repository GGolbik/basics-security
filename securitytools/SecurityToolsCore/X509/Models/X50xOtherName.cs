
namespace GGolbik.SecurityTools.X509.Models;

/// <summary>
/// https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.6
/// 
///    OtherName ::= SEQUENCE {
///         type-id    OBJECT IDENTIFIER,
///         value      [0] EXPLICIT ANY DEFINED BY type-id }
/// </summary>
public class X50xOtherName
{
    public string? TypeOid { get; set; }
    public byte[]? Value { get; set; }
}