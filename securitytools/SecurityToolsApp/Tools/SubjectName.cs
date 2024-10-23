namespace GGolbik.SecurityToolsApp.Tools;

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
public class SubjectName
{

    #region MUST be prepared standard attributes

    /// <summary>(C) The two-letter country code where your company is legally located. In ISO 3166 format.</summary>
    public string? CountryName { get; set; }

    /// <summary>(O) Your company's legally registered name (e.g., YourCompany, Inc.).</summary>
    public string? OrganizationName { get; set; }

    /// <summary>(OU) The name of your department within the organization.</summary>
    public string? OrganizationalUnitName { get; set; }

    // (dnQualifier) distinguished name qualifier,

    /// <summary>(S) The state/province where your company is legally located.</summary>
    public string? StateOrProvinceName { get; set; }

    /// <summary>(CN) The fully-qualified domain name (FQDN) (e.g., "www.example.com") or a name (e.g., "Susan Housley").</summary>
    public string? CommonName { get; set; }

    // serialNumber. Serial number of the certificate.

    /// <summary>(DC) Holding one component, a label, of a DNS domain name. Examples: Valid values include `example` and `com` but not `example.com`. The latter is invalid as it contains multiple domains.</summary>
    public IList<string>? DomainComponents { get; set; }

    #endregion MUST be prepared standard attributes

    #region SHOULD be prepared standard attributes

    /// <summary>(L) The city where your company is legally located.</summary>
    public string? LocalityName { get; set; }

    /// <summary>e.g. Dr, Prof, CEO. OID: 2.5.4.12</summary>
    public string? Title { get; set; }
    
    /// <summary>(SN): Surname. OID: 2.5.4.4</summary>
    public string? Surname { get; set; }

    /// <summary>(GN) Given name. OID: 2.5.4.42</summary>
    public string? GivenName { get; set; }

    /// <summary>
    /// Initials are the capital letters which begin each word of a name.
    /// For example, if your full name is Michael Dennis Stocks, your initials will be M.D.S or MDS.
    /// OID: 2.5.4.43
    /// </summary>
    public string? Initials { get; set; }

    /// <summary>A pseudonym is a name which someone, usually a writer, uses instead of his or her real name. OID: 2.5.4.65</summary>
    public string? Pseudonym { get; set; }
    

    /// <summary>Generation Qualifier e.g. "Jr.", "3.", "IV". OID: 2.5.4.44</summary>
    public string? GenerationQualifier { get; set; }

    #endregion SHOULD be prepared standard attributes

    /// <summary>(MAIL) emailAddress</summary>
    public string? EmailAddress { get; set; }

    /// <summary>
    /// A list of additional entries not present in this configuration.
    /// The key of an entry is the OID.
    /// </summary>
    public IList<KeyValuePair<string, string>>? Oids { get; set; }
}