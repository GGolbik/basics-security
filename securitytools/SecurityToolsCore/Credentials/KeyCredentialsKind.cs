/// <summary>
/// 
/// Related to chapter 7.37 UserTokenPolicy in OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.37
/// </summary>
public enum KeyCredentialsKind
{
    None = 0,
    /// <summary>
    /// No token is required.
    /// </summary>
    Anonymous = 1,
    /// <summary>
    /// A username/password token.
    /// </summary>
    UsernamePassword = 2,
    /// <summary>
    /// An X.509 v3 Certificate token.
    /// </summary>
    Certificate = 3,
    /// <summary>
    /// Any token issued by an Authorization Service.
    /// </summary>
    Token = 4,
}