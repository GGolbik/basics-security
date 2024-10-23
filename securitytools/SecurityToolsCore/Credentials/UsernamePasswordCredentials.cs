
namespace GGolbik.SecurityTools.Credentials;

/// <summary>
/// Related to chapter 7.36 UserIdentityToken parameters in OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.36.4
/// 
/// Related to chapter 7.37 UserTokenPolicy in OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.37
/// </summary>
public class UsernamePasswordCredentials : KeyCredentialsValue
{
    public override KeyCredentialsKind Kind => KeyCredentialsKind.UsernamePassword;

    /// <summary>
    /// A string that identifies the user.
    /// </summary>
    public string? Username { get; set; }
    /// <summary>
    /// The password for the user. The password can be an empty string.
    /// </summary>
    public string? Password { get; set; }

    public UsernamePasswordCredentials()
    {

    }

    public UsernamePasswordCredentials(string username, string? password = null)
    {
        this.Username = username;
        this.Password = password;
    }

    protected override string? GetHint()
    {
        return this.Username;
    }
}