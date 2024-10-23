
namespace GGolbik.SecurityTools.Credentials;

/// <summary>
/// Related to chapter 7.36.3 AnonymousIdentityToken in OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.36.3
/// 
/// Related to chapter 7.37 UserTokenPolicy in OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.37
/// </summary>
public class AnonymousCredentials : KeyCredentialsValue
{
    public override KeyCredentialsKind Kind => KeyCredentialsKind.Anonymous;

    protected override string? GetHint()
    {
        return "Anonymous";
    }
}