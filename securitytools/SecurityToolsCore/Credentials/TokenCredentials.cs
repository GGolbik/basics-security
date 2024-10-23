
using System.Text;

namespace GGolbik.SecurityTools.Credentials;

/// <summary>
/// Related to chapter 7.36.6 IssuedIdentityToken OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.36.6
/// 
/// Related to chapter 7.37 UserTokenPolicy in OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.37
/// </summary>
public class TokenCredentials : KeyCredentialsValue
{
    public const string TokenTypeJwt = "http://opcfoundation.org/UA/UserToken#JWT";
    public const string TokenTypeKerberos = "http://docs.oasis-open.org/wss/oasis-wss-kerberos-token-profile-1.1";

    public override KeyCredentialsKind Kind => KeyCredentialsKind.Token;

    /// <summary>
    /// The token value.
    /// 
    /// Related to chapter 6.5 Issued User Identity Tokens in OPC 10000-6: UA Part 6: Mappings
    /// https://reference.opcfoundation.org/Core/Part6/v105/docs/6.5
    /// 
    /// For Kerberos this is an XML element that contains the WS-Security token as defined in the Kerberos Token Profile (Kerberos) specification.
    /// 
    /// For JWT this is a string that contains the JWT as defined in RFC 8259.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// A URI for the type of token.
    /// 
    /// Related to chapter 6.5 Issued User Identity Tokens in OPC 10000-6: UA Part 6: Mappings
    /// https://reference.opcfoundation.org/Core/Part6/v105/docs/6.5
    /// 
    /// For Kerberos this is http://docs.oasis-open.org/wss/oasis-wss-kerberos-token-profile-1.1
    /// Deprecated in Version 1.05.
    /// 
    /// For JWTs this is http://opcfoundation.org/UA/UserToken#JWT
    /// </summary>
    public string? TokenType { get; set; }

    /// <summary>
    /// An optional string which depends on the Authorization Service.
    /// The meaning of this value depends on the issuedTokenType.
    /// Further details for the different token types are defined in OPC 10000-6.
    /// 
    /// For Kerberos this string is the name of the Service Principal Name (SPN).
    /// 
    /// For JWTs this is a JSON object with fields defined in OPC 10000-6.
    /// 
    /// Related to chapter 6.5 Issued User Identity Tokens in OPC 10000-6: UA Part 6: Mappings
    /// https://reference.opcfoundation.org/Core/Part6/v105/docs/6.5
    /// </summary>
    public string? IssuerEndpointUrl { get; set; }

    public TokenCredentials()
    {

    }

    public TokenCredentials(string token, string tokenType)
    {
        this.Token = token;
        this.TokenType = tokenType;
    }

    public TokenCredentials(string token, string tokenType, string issuerEndpointUrl)
    {
        this.Token = token;
        this.TokenType = tokenType;
        this.IssuerEndpointUrl = issuerEndpointUrl;
    }

    protected override string? GetHint()
    {
        List<string> items = new();
        if(!string.IsNullOrWhiteSpace(TokenType))
        {
            items.Add(TokenType);
        }
        if(!string.IsNullOrWhiteSpace(IssuerEndpointUrl))
        {
            items.Add(IssuerEndpointUrl);
        }
        return string.Join(" - ", items);
    }
}