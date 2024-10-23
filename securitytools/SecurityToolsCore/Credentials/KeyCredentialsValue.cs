
using System.Text.Json.Serialization;

namespace GGolbik.SecurityTools.Credentials;

/// <summary>
/// Related to chapter 8 KeyCredential Management in OPC 10000-12: UA Part 12: Discovery and Global Services
/// See also https://reference.opcfoundation.org/GDS/v105/docs/8
/// 
/// The credentialSecret is a UA Binary encoded form of one of the EncryptedSecret DataTypes defined in OPC 10000-4.
/// 
/// Related to chapter 7.36.2.3 EncryptedSecret Format in OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.36.2.3
/// 
/// The secret in the EncryptedSecret is the tokenData that depends on the IssuedIdentityToken.
/// If the tokenData is a String is it encoded using UTF-8 first.
/// 
/// Related to chapter 7.36.6 IssuedIdentityToken in OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.36.6
/// 
/// The tokenData in the IssuedIdentityToken is the text or binary representation of the token.
/// The format of the data depends on the associated UserTokenPolicy.
/// 
/// Related to chapter 7.37 UserTokenPolicy in OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.37
/// </summary>
[JsonConverter(typeof(KeyCredentialsValueJsonConverter))]
public abstract class KeyCredentialsValue
{
    protected abstract string? GetHint();

    public abstract KeyCredentialsKind Kind { get; }

    public override string? ToString()
    {
        return this.GetHint();
    }
}