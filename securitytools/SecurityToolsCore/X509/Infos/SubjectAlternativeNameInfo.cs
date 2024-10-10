using GGolbik.SecurityTools.X509.Infos;

public class SubjectAlternativeNameInfo : X509ExtensionInfo
{
    /// <summary>
    /// EmailAddresses, DnsNames, IPAddresses, Uris, UserPrincipalNames
    /// </summary>
    public IList<string>? SubjectAlternativeNames { get; set; }

    public override string? ToString()
    {
        if (this.SubjectAlternativeNames == null)
        {
            return $"{this.GetType().Name}";
        }
        return $"{this.GetType().Name}:{string.Join(";", this.SubjectAlternativeNames)}";
    }
}