namespace GGolbik.SecurityTools.X509.Models;

/// <summary>
/// https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.6
/// 
///    EDIPartyName ::= SEQUENCE {
///         nameAssigner            [0]     DirectoryString OPTIONAL,
///         partyName               [1]     DirectoryString }
/// </summary>
public class ConfigEdiPartyName
{
    public string? NameAssigner { get; set; }
    public string? PartyName { get; set; }
}