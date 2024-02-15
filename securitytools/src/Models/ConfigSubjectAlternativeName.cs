
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.Models;

public class ConfigSubjectAlternativeName : ConfigExtension
{
    public IList<string>? EmailAddresses { get; set; }
    public IList<string>? DnsNames { get; set; }
    public IList<string>? IPAddresses { get; set; }
    public IList<string>? Uris { get; set; }
    public IList<string>? UserPrincipalNames { get; set; }

    public override X509Extension ToX509Extension()
    {
        var builder = new SubjectAlternativeNameBuilder();
        foreach(var emailAddress in this.EmailAddresses ?? new List<string>())
        {
            builder.AddEmailAddress(emailAddress);
        }
        foreach(var dnsName in this.DnsNames ?? new List<string>())
        {
            builder.AddDnsName(dnsName);
        }
        foreach(var ipAddress in this.IPAddresses ?? new List<string>())
        {
            builder.AddIpAddress(IPAddress.Parse(ipAddress));
        }
        foreach(var uri in this.Uris ?? new List<string>())
        {
            builder.AddUri(new Uri(uri));
        }
        foreach(var userPrincipalName in this.UserPrincipalNames ?? new List<string>())
        {
            builder.AddUserPrincipalName(userPrincipalName);
        }
        return builder.Build();
    }
}
