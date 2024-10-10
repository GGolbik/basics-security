namespace GGolbik.SecurityTools.X509.Infos;

/// <summary>
/// </summary>
public class CrlValidity
{
    /// <summary>
    /// This field indicates the issue date of this CRL.
    /// </summary>
    public DateTime? ThisUpdate { get; set; }

    /// <summary>
    /// This field indicates the date by which the next CRL will be issued.
    /// The next CRL could be issued before the indicated date, but it will not be issued any later than the indicated date.
    /// CRL issuers SHOULD issue CRLs with a nextUpdate time equal to or later than all previous CRLs. 
    /// </summary>
    public DateTime? NextUpdate { get; set; }
}