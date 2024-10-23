using System.Security.Cryptography.X509Certificates;
using BCX509 = Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X500;

namespace GGolbik.SecurityTools.X509.Models;

/// <summary>
/// The subject alternative name extension allows identities to be bound
/// to the subject of the certificate.  These identities may be included
/// in addition to or in place of the identity in the subject field of
/// the certificate.
/// 
/// https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.6
/// 
/// SubjectAltName ::= GeneralNames
/// 
///    GeneralNames ::= SEQUENCE SIZE (1..MAX) OF GeneralName
/// 
///    GeneralName ::= CHOICE {
///         otherName                       [0]     OtherName,
///         rfc822Name                      [1]     IA5String,
///         dNSName                         [2]     IA5String,
///         x400Address                     [3]     ORAddress,
///         directoryName                   [4]     Name,
///         ediPartyName                    [5]     EDIPartyName,
///         uniformResourceIdentifier       [6]     IA5String,
///         iPAddress                       [7]     OCTET STRING,
///         registeredID                    [8]     OBJECT IDENTIFIER }
/// 
///    OtherName ::= SEQUENCE {
///         type-id    OBJECT IDENTIFIER,
///         value      [0] EXPLICIT ANY DEFINED BY type-id }
/// 
///    EDIPartyName ::= SEQUENCE {
///         nameAssigner            [0]     DirectoryString OPTIONAL,
///         partyName               [1]     DirectoryString }
/// </summary>
public class X50xSubjectAlternativeName : X50xExtension
{
    /// <summary>
    /// otherName                       [0]     OtherName,
    /// e.g.: OID: 1.3.6.1.4.1.311.20.2.3
    /// </summary>
    public IList<X50xOtherName>? OtherNames { get; set; }
    /// <summary>
    /// rfc822Name                      [1]     IA5String,
    /// It contains an email address of the user.
    /// </summary>
    public IList<string>? EmailAddresses { get; set; }
    /// <summary>
    /// dNSName                         [2]     IA5String,
    /// </summary>
    public IList<string>? DnsNames { get; set; }
    /// <summary>
    /// x400Address                     [3]     ORAddress,
    /// </summary>
    public IList<byte[]>? X400Addresses { get; set; }
    /// <summary>
    /// directoryName (DN)                  [4]     Name,
    /// </summary>
    public IList<string>? DirectoryNames { get; set; }
    /// <summary>
    /// ediPartyName                    [5]     EDIPartyName,
    /// </summary>
    public IList<X50xEdiPartyName>? EdiPartyNames { get; set; }
    /// <summary>
    /// uniformResourceIdentifier       [6]     IA5String,
    /// </summary>
    public IList<string>? Uris { get; set; }
    /// <summary>
    /// iPAddress                       [7]     OCTET STRING,
    /// </summary>
    public IList<string>? IPAddresses { get; set; }
    /// <summary>
    /// registeredID                    [8]     OBJECT IDENTIFIER
    /// </summary>
    public IList<string>? RegisteredIds { get; set; }

    // https://www.codeproject.com/Questions/5336056/How-to-add-subject-alternative-names-for-serianumb
    public override X509Extension ToX509Extension()
    {
        List<BCX509.GeneralName> names = new();
        BCX509.GeneralName altName = new BCX509.GeneralName(BCX509.GeneralName.DnsName, "example.com");

        foreach (var otherName in this.OtherNames ?? new List<X50xOtherName>())
        {
            if (string.IsNullOrWhiteSpace(otherName.TypeOid))
            {
                continue;
            }
            var vector = new Asn1EncodableVector();
            vector.Add(new DerObjectIdentifier(otherName.TypeOid));
            vector.Add(new DerTaggedObject(0, Asn1Object.FromByteArray(otherName.Value)));
            var sequence = new DerSequence(vector);
            names.Add(new(BCX509.GeneralName.EdiPartyName, sequence));
        }
        foreach (var emailAddress in this.EmailAddresses ?? new List<string>())
        {
            names.Add(new(BCX509.GeneralName.Rfc822Name, emailAddress));
        }
        foreach (var dnsName in this.DnsNames ?? new List<string>())
        {
            names.Add(new(BCX509.GeneralName.DnsName, dnsName));
        }
        foreach (var x400 in this.X400Addresses ?? new List<byte[]>())
        {
            names.Add(new(BCX509.GeneralName.X400Address, Asn1Object.FromByteArray(x400)));
        }
        foreach (var directoryName in this.DirectoryNames ?? new List<string>())
        {
            names.Add(new(BCX509.GeneralName.DirectoryName, directoryName));
        }
        foreach (var ediPartyName in this.EdiPartyNames ?? new List<X50xEdiPartyName>())
        {
            var vector = new Asn1EncodableVector();
            Asn1Encodable taggedValue = ediPartyName.NameAssigner == null ? DerNull.Instance : new DirectoryString(ediPartyName.NameAssigner);
            Asn1TaggedObject taggedOptionalElement = new DerTaggedObject(0, taggedValue);
            vector.Add(taggedOptionalElement);
            vector.Add(new DerTaggedObject(1, new DirectoryString(ediPartyName.PartyName)));
            var sequence = new DerSequence(vector);
            names.Add(new(BCX509.GeneralName.EdiPartyName, sequence));
        }
        foreach (var uri in this.Uris ?? new List<string>())
        {
            names.Add(new(BCX509.GeneralName.UniformResourceIdentifier, uri));
        }
        foreach (var ipAddress in this.IPAddresses ?? new List<string>())
        {
            names.Add(new(BCX509.GeneralName.IPAddress, ipAddress));
        }
        foreach (var registeredId in this.RegisteredIds ?? new List<string>())
        {
            names.Add(new(BCX509.GeneralName.RegisteredID, registeredId));
        }
        BCX509.GeneralNames subjectAltName = new(names.ToArray());
        BCX509.X509ExtensionsGenerator gen = new();
        gen.AddExtension(BCX509.X509Extensions.SubjectAlternativeName, this.Critical ?? false, subjectAltName);
        return new X509Extension(BCX509.X509Extensions.SubjectAlternativeName.Id, gen.Generate().GetExtension(BCX509.X509Extensions.SubjectAlternativeName).Value.GetOctets(), this.Critical ?? false);
    }
}
