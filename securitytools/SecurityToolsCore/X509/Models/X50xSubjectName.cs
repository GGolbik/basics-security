
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.X509.Models;

/// <summary>
/// Provides the name of the computer, user, network device, or service that the CA issues the certificate to.
/// 
/// The subject name is commonly represented by using an X.500 or Lightweight Directory Access Protocol (LDAP) format.
/// 
/// The subject field must contain a non-empty distinguished name (DN).
/// 
/// However, [RFC 5280](https://datatracker.ietf.org/doc/html/rfc5280) does not make any requirement on which RDN(s) should be present.
/// https://datatracker.ietf.org/doc/html/rfc3280#section-4.1
/// </summary>
public class X50xSubjectName
{
    #region MUST be prepared standard attributes

    /// <summary>(C) The two-letter country code where your company is legally located. In ISO 3166 format.</summary>
    [OidAttribute("2.5.4.6")]
    public string? CountryName { get; set; }

    /// <summary>(O) Your company's legally registered name (e.g., YourCompany, Inc.).</summary>
    [OidAttribute("2.5.4.10")]
    public string? OrganizationName { get; set; }

    /// <summary>(OU) The name of your department within the organization.</summary>
    [OidAttribute("2.5.4.11")]
    public string? OrganizationalUnitName { get; set; }

    // (dnQualifier) distinguished name qualifier,

    /// <summary>(S) The state/province where your company is legally located.</summary>
    [OidAttribute("2.5.4.8")]
    public string? StateOrProvinceName { get; set; }

    /// <summary>(CN) The fully-qualified domain name (FQDN) (e.g., "www.example.com") or a name (e.g., "Susan Housley").</summary>
    [OidAttribute("2.5.4.3")]
    public string? CommonName { get; set; }

    // serialNumber. Serial number of the certificate.

    /// <summary>(DC) Holding one component, a label, of a DNS domain name. Examples: Valid values include `example` and `com` but not `example.com`. The latter is invalid as it contains multiple domains.</summary>
    [OidAttribute("0.9.2342.19200300.100.1.25")]
    public IList<string>? DomainComponents { get; set; }

    #endregion MUST be prepared standard attributes

    #region SHOULD be prepared standard attributes

    /// <summary>(L) The city where your company is legally located.</summary>
    [OidAttribute("2.5.4.7")]
    public string? LocalityName { get; set; }

    /// <summary>e.g. Dr, Prof, CEO. OID: 2.5.4.12</summary>
    [OidAttribute("2.5.4.12")]
    public string? Title { get; set; }

    /// <summary>(SN): Surname. OID: 2.5.4.4</summary>
    [OidAttribute("2.5.4.4")]
    public string? Surname { get; set; }

    /// <summary>(GN) Given name. OID: 2.5.4.42</summary>
    [OidAttribute("2.5.4.42")]
    public string? GivenName { get; set; }

    /// <summary>
    /// Initials are the capital letters which begin each word of a name.
    /// For example, if your full name is Michael Dennis Stocks, your initials will be M.D.S or MDS.
    /// </summary>
    [OidAttribute("2.5.4.43")]
    public string? Initials { get; set; }

    /// <summary>A pseudonym is a name which someone, usually a writer, uses instead of his or her real name. OID: 2.5.4.65</summary>
    [OidAttribute("2.5.4.65")]
    public string? Pseudonym { get; set; }

    /// <summary>Generation Qualifier e.g. "Jr.", "3.", "IV". OID: 2.5.4.44</summary>
    [OidAttribute("2.5.4.44")]
    public string? GenerationQualifier { get; set; }

    #endregion SHOULD be prepared standard attributes

    /// <summary>(MAIL) emailAddress</summary>
    [OidAttribute("1.2.840.113549.1.9.1")]
    public string? EmailAddress { get; set; }

    /// <summary>
    /// A list of additional entries not present in this configuration.
    /// The key of an entry is the OID.
    /// </summary>
    public IList<KeyValuePair<string, string>>? Oids { get; set; }


    public X50xSubjectName()
    {

    }

    public X50xSubjectName(string commonName)
    {
        this.CommonName = commonName;
    }

    public X500DistinguishedName ToX500DistinguishedName()
    {
        var subjectNameBuilder = new X500DistinguishedNameBuilder();

        this.AddMust(subjectNameBuilder);
        this.AddShould(subjectNameBuilder);
        this.Add(subjectNameBuilder);

        return subjectNameBuilder.Build();
    }

    private void AddMust(X500DistinguishedNameBuilder subjectNameBuilder)
    {
        if (!String.IsNullOrWhiteSpace(this.CountryName))
        {
            subjectNameBuilder.AddCountryOrRegion(this.CountryName.Trim());
        }
        if (!String.IsNullOrWhiteSpace(this.OrganizationName))
        {
            subjectNameBuilder.AddOrganizationName(this.OrganizationName.Trim());
        }
        if (!String.IsNullOrWhiteSpace(this.OrganizationalUnitName))
        {
            subjectNameBuilder.AddOrganizationalUnitName(this.OrganizationalUnitName.Trim());
        }
        if (!String.IsNullOrWhiteSpace(this.StateOrProvinceName))
        {
            subjectNameBuilder.AddStateOrProvinceName(this.StateOrProvinceName.Trim());
        }
        if (!String.IsNullOrWhiteSpace(this.CommonName))
        {
            subjectNameBuilder.AddCommonName(this.CommonName.Trim());
        }
        if (this.DomainComponents != null)
        {
            foreach (var domainComponent in this.DomainComponents)
            {
                if (!String.IsNullOrWhiteSpace(domainComponent))
                {
                    subjectNameBuilder.AddDomainComponent(domainComponent.Trim());
                }
            }
        }
    }

    private void AddShould(X500DistinguishedNameBuilder subjectNameBuilder)
    {
        if (!String.IsNullOrWhiteSpace(this.LocalityName))
        {
            subjectNameBuilder.AddLocalityName(this.LocalityName.Trim());
        }
        if (!String.IsNullOrWhiteSpace(this.Title))
        {
            subjectNameBuilder.Add("2.5.4.12", this.Title.Trim());
        }
        if (!String.IsNullOrWhiteSpace(this.Surname))
        {
            subjectNameBuilder.Add("2.5.4.4", this.Surname.Trim());
        }
        if (!String.IsNullOrWhiteSpace(this.GivenName))
        {
            subjectNameBuilder.Add("2.5.4.42", this.GivenName.Trim());
        }
        if (!String.IsNullOrWhiteSpace(this.Initials))
        {
            subjectNameBuilder.Add("2.5.4.43", this.Initials.Trim());
        }
        if (!String.IsNullOrWhiteSpace(this.Pseudonym))
        {
            subjectNameBuilder.Add("2.5.4.65", this.Pseudonym.Trim());
        }
        if (!String.IsNullOrWhiteSpace(this.GenerationQualifier))
        {
            subjectNameBuilder.Add("2.5.4.44", this.GenerationQualifier.Trim());
        }
    }

    private void Add(X500DistinguishedNameBuilder subjectNameBuilder)
    {
        if (!String.IsNullOrWhiteSpace(this.EmailAddress))
        {
            subjectNameBuilder.AddEmailAddress(this.EmailAddress.Trim());
        }

        if (this.Oids != null)
        {
            foreach (var oid in this.Oids)
            {
                if (!String.IsNullOrWhiteSpace(oid.Key) && !String.IsNullOrWhiteSpace(oid.Value))
                {
                    subjectNameBuilder.Add(oid.Key, oid.Value);
                }
            }
        }
    }

    public static implicit operator X500DistinguishedName?(X50xSubjectName? name) => name?.ToX500DistinguishedName();
}