namespace GGolbik.SecurityTools.X509.Builders;

public class SecurityBuilderEventArgs : EventArgs
{
    public SecurityBuilderEventKind Kind { get; set; }
    public string? Message { get; private set; }

    public SecurityBuilderEventArgs() {}
    public SecurityBuilderEventArgs(SecurityBuilderEventKind kind)
    {
        this.Kind = kind;
    }
    public SecurityBuilderEventArgs(string message)
    {
        this.Message = message;
    }
    public SecurityBuilderEventArgs(string format, params object?[] args)
    {
        this.Message = string.Format(format, args);
    }
    public static implicit operator SecurityBuilderEventArgs(string arg) => new SecurityBuilderEventArgs(arg);
    public static implicit operator SecurityBuilderEventArgs(SecurityBuilderEventKind arg) => new SecurityBuilderEventArgs(arg);
}

public enum SecurityBuilderEventKind
{
    None,
    BuildingCertificate,
    BuildingSelfSignedCertificate,
    BuildingCrl,
    BuildingKeyPair,
    BuildingCsr,
    LoadingIssuer,
    LoadingCertificate,
    LoadingIssuerKeyPair,
    LoadingKeyPair,
    LoadingCrl,
    LoadingCsr,
    AddingExtensions,
    AddingCrlEntries,
    BuiltCertificate,
    BuiltCrl,
    BuiltCsr,
    BuiltKeyPair
}