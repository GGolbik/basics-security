using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using GGolbik.SecurityTools.X509.Infos;
using GGolbik.SecurityTools.X509.Models;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;

namespace GGolbik.SecurityTools.X509;

public static class UtilsExtensions
{
    public static CertificateInfo ToCertificateInfo(this X509Certificate2 certificate)
    {
        return new()
        {
            Thumbprint = certificate.Thumbprint,
            PublicKeyThumbprint = certificate.PublicKey.ToThumbprint(),
            Version = certificate.Version,
            SerialNumber = certificate.SerialNumber,
            SignatureAlgorithm = certificate.SignatureAlgorithm.Value ?? certificate.SignatureAlgorithm.FriendlyName,
            Issuer = certificate.Issuer,
            Validity = new()
            {
                NotBefore = certificate.NotBefore,
                NotAfter = certificate.NotAfter
            },
            Subject = certificate.Subject,
            SubjectPublicKeyInfo = certificate.PublicKey.GetPublicKey()?.ToKeyPairInfo(),
            IssuerUniqueIdentifier = null,
            SubjectUniqueIdentifier = null,
            Extensions = certificate.Extensions.ToX509ExtensionsInfo()
        };
    }

    public static X509ExtensionsInfo ToX509ExtensionsInfo(this X509ExtensionCollection extensions)
    {
        X509ExtensionsInfo result = new();
        foreach (var extension in extensions)
        {
            X509ExtensionInfo extensionInfo = new();
            if (extension is X509KeyUsageExtension keyUsage)
            {
                extensionInfo = result.KeyUsage = new()
                {
                    KeyUsages = keyUsage.KeyUsages
                };
            }
            else if (extension is X509EnhancedKeyUsageExtension exKeyUsage)
            {
                Dictionary<string, string?> oids = new();
                foreach (var oid in exKeyUsage.EnhancedKeyUsages)
                {
                    if (!string.IsNullOrWhiteSpace(oid.Value ?? oid.FriendlyName))
                    {
                        oids.Add(oid.Value ?? oid.FriendlyName!, oid.FriendlyName);
                    }
                }
                extensionInfo = result.ExtendedKeyUsage = new()
                {
                    Oids = oids
                };
            }
            else if (extension is X509SubjectAlternativeNameExtension subjectAlt)
            {
                var lines = subjectAlt.Format(true).Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
                var names = lines.Where((item) => !string.IsNullOrWhiteSpace(item)).ToList();
                extensionInfo = result.SubjectAlternativeName = new()
                {
                    SubjectAlternativeNames = names
                };
            }
            else if (extension is X509BasicConstraintsExtension basic)
            {
                extensionInfo = result.BasicConstraints = new()
                {
                    CertificateAuthority = basic.CertificateAuthority,
                    HasPathLengthConstraint = basic.HasPathLengthConstraint,
                    PathLengthConstraint = basic.PathLengthConstraint
                };
            }
            else if (extension is X509SubjectKeyIdentifierExtension subId)
            {
                extensionInfo = result.SubjectKeyIdentifier = new()
                {
                    SubjectKeyIdentifier = subId.SubjectKeyIdentifierBytes.ToArray()
                };
            }
            else if (extension is X509AuthorityKeyIdentifierExtension id)
            {
                extensionInfo = result.AuthorityKeyIdentifier = new()
                {
                    AuthorityKeyIdentifier = id.KeyIdentifier?.ToArray()
                };
            }
            else if (string.Equals(extension.Oid?.Value, "2.5.29.18"))
            {
                // id-ce-issuerAltName: 2.5.29.18
                extensionInfo = result.IssuerAlternativeName = new()
                {

                };
            }
            else if (string.Equals(extension.Oid?.Value, "2.5.29.20"))
            {
                // id-ce-cRLNumber: 2.5.29.20
                extensionInfo = result.CrlNumber = new()
                {

                };
            }
            else if (string.Equals(extension.Oid?.Value, "2.5.29.21"))
            {
                // id-ce-cRLReasons: 2.5.29.21
                extensionInfo = result.CrlReasonCode = new()
                {

                };
            }
            else
            {
                result.Extensions ??= [];
                result.Extensions.Add(extensionInfo);
            }
            extensionInfo.Critical = extension.Critical;
            extensionInfo.Oid = extension.Oid?.Value ?? extension.Oid?.FriendlyName;
            extensionInfo.Value = extension.RawData;
        }
        return result;
    }

    public static KeyPairInfo ToKeyPairInfo(this AsymmetricAlgorithm keyPair)
    {
        KeyPairInfo result = new()
        {
            Thumbprint = keyPair.ToThumbprint()
        };
        if (keyPair is RSA rsa)
        {
            result.SignatureAlgorithm = SignatureAlgorithmName.Rsa;
            result.KeySize = rsa.KeySize;
        }
        else if (keyPair is DSA dsa)
        {
            result.SignatureAlgorithm = SignatureAlgorithmName.Dsa;
            result.KeySize = dsa.KeySize;
        }
        else if (keyPair is ECDsa ecdsa)
        {
            result.SignatureAlgorithm = SignatureAlgorithmName.Ecdsa;
            result.KeySize = ecdsa.KeySize;
            result.Eccurve = ecdsa.ExportParameters(false).Curve.Oid.Value ?? ecdsa.ExportParameters(true).Curve.Oid.FriendlyName;
            //ECDsa.Create("brainpoolP160r1".ToEccurve());
        }
        else if (keyPair is ECDiffieHellman ecdh)
        {
            result.SignatureAlgorithm = SignatureAlgorithmName.Ecdh;
            result.KeySize = ecdh.KeySize;
            result.Eccurve = ecdh.ExportParameters(false).Curve.Oid.Value ?? ecdh.ExportParameters(true).Curve.Oid.FriendlyName;
        }
        else
        {
            throw new ArgumentException("Unknown asymmetric algorithm.");
        }
        return result;
    }

    public static CrlInfo ToCrlInfo(this X509Crl crl)
    {
        string thumbprint;
        using (var sha1 = SHA1.Create())
        {
            thumbprint = Convert.ToHexString(sha1.ComputeHash(crl.GetEncoded()));
        }
        CrlInfo result = new()
        {
            Thumbprint = thumbprint,
            SignatureAlgorithm = crl.SignatureAlgorithm.ToString(),
            Version = null,
            Issuer = crl.IssuerDN.ToString(),
            Validity = new()
            {
                ThisUpdate = crl.ThisUpdate,
                NextUpdate = crl.NextUpdate
            },
            RevokedCertificates = []
        };
        foreach (var item in crl.GetRevokedCertificates())
        {
            result.RevokedCertificates.Add(new()
            {
                SerialNumber = Convert.ToHexString(item.SerialNumber.ToByteArray()),
                RevocationDate = item.RevocationDate,
            });
        }
        return result;
    }

    public static X509Certificate2 CopyWithPrivateKeyAuto(this X509Certificate2 cert, AsymmetricAlgorithm keyPair)
    {
        if (keyPair is RSA rsa)
        {
            return cert.CopyWithPrivateKey(rsa);
        }
        if (keyPair is DSA dsa)
        {
            return cert.CopyWithPrivateKey(dsa);
        }
        if (keyPair is ECDsa ecdsa)
        {
            return cert.CopyWithPrivateKey(ecdsa);
        }
        if (keyPair is ECDiffieHellman ecdh)
        {
            return cert.CopyWithPrivateKey(ecdh);
        }
        throw new ArgumentException("Unsupported key");
    }

    public static string ToPem(this X509Crl crl)
    {
        var stream = new MemoryStream();
        using (var streamWriter = new StreamWriter(stream))
        using (var pemWriter = new PemWriter(streamWriter))
        {
            pemWriter.WriteObject(crl);
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public static byte[] ToDer(this X509Crl crl)
    {
        return crl.GetEncoded();
    }

    public static string ToPem(this X509Certificate2 cert)
    {
        return cert.ExportCertificatePem();
    }
    public static byte[] ToDer(this X509Certificate2 cert)
    {
        return cert.Export(X509ContentType.Cert);
    }

    public static byte[] ToDer(this AsymmetricAlgorithm keyPair)
    {
        return keyPair.ExportPkcs8PrivateKey();
    }

    public static AsymmetricAlgorithm Clone(this AsymmetricAlgorithm keyPair)
    {
        return X509Reader.ReadKeyPairFromDer(keyPair.ToDer(), out var read, null) ?? throw new ArgumentNullException();
    }
    public static CertificateRequest Clone(this CertificateRequest csr, CertificateRequestLoadOptions loadOptions = CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions)
    {
        return csr.Clone(HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1, loadOptions);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="csr"></param>
    /// <param name="signerHashAlgorithm">The hash algorithm to use when creating a certificate or new signing request.</param>
    /// <param name="loadOptions"></param>
    /// <returns></returns>
    public static CertificateRequest Clone(this CertificateRequest csr, HashAlgorithmName signerHashAlgorithm, RSASignaturePadding? signerSignaturePadding, CertificateRequestLoadOptions loadOptions = CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions)
    {
        return CertificateRequest.LoadSigningRequest(csr.ToDer(), signerHashAlgorithm, loadOptions, signerSignaturePadding);
    }

    public static X509Certificate2 Clone(this X509Certificate2 cert, bool includePrivateKey = true)
    {
        var key = cert.GetPrivateKey();
        if (includePrivateKey && key != null)
        {
            return X509Certificate2.CreateFromPem(cert.ToPem(), key.ToPem());
        }
        return new X509Certificate2(cert.ToDer());
    }

    public static X509Crl Clone(this X509Crl crl)
    {
        return new X509Crl(crl.ToDer());
    }

    public static byte[] ToDer(this AsymmetricAlgorithm keyPair, byte[] password, PbeParameters? pbeParameters = null)
    {
        pbeParameters ??= new PbeParameters(
            PbeEncryptionAlgorithm.Aes256Cbc,
            HashAlgorithmName.SHA512,
            100000
        );
        return keyPair.ExportEncryptedPkcs8PrivateKey(password, pbeParameters);
    }
    public static String ToPem(this AsymmetricAlgorithm keyPair)
    {
        return keyPair.ExportPkcs8PrivateKeyPem();
    }

    public static string ToPem(this AsymmetricAlgorithm keyPair, byte[] password, PbeParameters? pbeParameters = null)
    {
        pbeParameters ??= new PbeParameters(
            PbeEncryptionAlgorithm.Aes256Cbc,
            HashAlgorithmName.SHA512,
            100000
        );
        return keyPair.ExportEncryptedPkcs8PrivateKeyPem(Encoding.UTF8.GetString(password).ToCharArray(), pbeParameters);
    }

    public static byte[] ToDer(this Org.BouncyCastle.X509.X509Certificate cert)
    {
        return cert.GetEncoded();
    }

    /// <summary>
    /// Creates a PEM-encoded PKCS#10 CertificationRequest representing the current state of this object using the provided signature generator.
    /// </summary>
    /// <param name="csr"></param>
    public static string ToPem(this CertificateRequest csr)
    {
        // encode request to base64
        return csr.CreateSigningRequestPem();
    }

    /// <summary>
    /// Creates a PEM-encoded PKCS#10 CertificationRequest representing the current state of this object using the provided signature generator.
    /// </summary>
    /// <param name="csr"></param>
    public static byte[] ToDer(this CertificateRequest csr)
    {
        // encode request to base64
        return csr.CreateSigningRequest();
    }

    public static AsymmetricAlgorithm? GetPrivateKey(this X509Certificate2 cert)
    {
        AsymmetricAlgorithm? result = null;
        if (!cert.HasPrivateKey)
        {
            return result;
        }
        result ??= cert.GetRSAPrivateKey();
        result ??= cert.GetECDsaPrivateKey();
        result ??= cert.GetDSAPrivateKey();
        result ??= cert.GetECDiffieHellmanPrivateKey();
        return result;
    }

    public static AsymmetricAlgorithm? GetPublicKey(this PublicKey keyPair)
    {
        AsymmetricAlgorithm? result = null;
        result ??= keyPair.GetRSAPublicKey();
        result ??= keyPair.GetDSAPublicKey();
        result ??= keyPair.GetECDsaPublicKey();
        result ??= keyPair.GetECDiffieHellmanPublicKey();
        return result;
    }

    public static string ToThumbprint(this AsymmetricAlgorithm keyPair)
    {
        using (var sha1 = SHA1.Create())
        {
            return Convert.ToHexString(sha1.ComputeHash(keyPair.ExportSubjectPublicKeyInfo()));
        }
    }

    public static string ToThumbprint(this PublicKey key)
    {
        using (var sha1 = SHA1.Create())
        {
            return Convert.ToHexString(sha1.ComputeHash(key.ExportSubjectPublicKeyInfo()));
        }
    }

    public static string ToThumbprint(this Org.BouncyCastle.X509.X509Certificate cert)
    {
        using (var sha1 = SHA1.Create())
        {
            return Convert.ToHexString(sha1.ComputeHash(cert.ToDer()));
        }
    }

    public static string ToThumbprint(this X509Crl crl)
    {
        using (var sha1 = SHA1.Create())
        {
            return Convert.ToHexString(sha1.ComputeHash(crl.ToDer()));
        }
    }

    public static string ToThumbprint(this CertificateRequest csr)
    {
        using (var sha1 = SHA1.Create())
        {
            return Convert.ToHexString(sha1.ComputeHash(csr.ToDer()));
        }
    }

    public static readonly string OidPattern = "^([0-9]+)(\\.[0-9]+)*$";
    public static ECCurve ToEccurve(this string value)
    {
        List<Exception> ex = new();
        if (new Regex(OidPattern).IsMatch(value))
        {
            try
            {
                return ECCurve.CreateFromValue(value);
            }
            catch (Exception e)
            {
                ex.Add(e);
            }
        }
        try
        {
            return ECCurve.CreateFromFriendlyName(value);
        }
        catch (Exception e)
        {
            ex.Add(e);
            throw new AggregateException(ex);
        }
    }

    public static int CopyTo(this Stream source, int count, Stream dest, int bufferSize = 81920)
    {
        byte[] buffer = new byte[bufferSize];
        int read;
        int position = 0;
        do
        {
            position += read = source.Read(buffer, 0, Math.Min(bufferSize, count));
            count -= read;
            dest.Write(buffer, 0, read);
        } while (read != 0 && count > 0);
        return position;
    }

    /// <summary>
    /// Creates an ASN.1 DER-encoded PKCS#10 CertificationRequest value representing the state of the current object.
    /// </summary>
    /// <param name="csr"></param>
    /// <param name="path"></param>
    public static void SaveDer(this CertificateRequest csr, string path)
    {
        using (var stream = new FileStream(path, FileMode.Create))
        {
            csr.SaveDer(stream);
        }
    }

    /// <summary>
    /// Creates an ASN.1 DER-encoded PKCS#10 CertificationRequest value representing the state of the current object.
    /// </summary>
    /// <param name="csr"></param>
    /// <param name="stream"></param>
    public static void SaveDer(this CertificateRequest csr, Stream stream)
    {
        byte[] encoded = csr.CreateSigningRequest();
        stream.Write(encoded);
    }

    /// <summary>
    /// Creates a PEM-encoded PKCS#10 CertificationRequest representing the current state of this object using the provided signature generator.
    /// </summary>
    /// <param name="csr"></param>
    /// <param name="path"></param>
    public static void SavePem(this CertificateRequest csr, string path)
    {
        using (var stream = new FileStream(path, FileMode.Create))
        {
            csr.SavePem(stream);
        }
    }

    /// <summary>
    /// Creates a PEM-encoded PKCS#10 CertificationRequest representing the current state of this object using the provided signature generator.
    /// </summary>
    /// <param name="csr"></param>
    /// <param name="stream"></param>
    public static void SavePem(this CertificateRequest csr, Stream stream)
    {
        // encode request to base64
        string encoded = csr.CreateSigningRequestPem();
        // write request to a file
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            writer.Write(encoded);
        }
    }

    public static HashAlgorithmName ToHashAlgorithm(this string str)
    {
        List<HashAlgorithmName> algs = new()
        {
            HashAlgorithmName.MD5,
            HashAlgorithmName.SHA1,
            HashAlgorithmName.SHA256,
            HashAlgorithmName.SHA384,
            HashAlgorithmName.SHA512,
        };
        foreach (var alg in algs)
        {
            if (alg.Name != null && str.Contains(alg.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return alg;
            }
        }
        return HashAlgorithmName.FromOid(str);
    }

    public static void Print(this X509Certificate2 cert, Stream stream)
    {
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            writer.WriteLine(string.Format("Version: \t{0}", cert.Version));
            writer.WriteLine(string.Format("Serial Number: \t{0} (0x{1})", new Org.BouncyCastle.Math.BigInteger(cert.SerialNumber, 16).ToString(10), cert.SerialNumber));
            writer.WriteLine(string.Format("Signature Algorithm: \t{0} (OID: {1})", cert.SignatureAlgorithm.FriendlyName, cert.SignatureAlgorithm.Value));
            writer.WriteLine(string.Format("Issuer: \t{0}", cert.Issuer));
            writer.WriteLine(string.Format("Validity Not Before: \t{0}", cert.NotBefore.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss Z")));
            writer.WriteLine(string.Format("Validity Not After: \t{0}", cert.NotAfter.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss Z")));
            writer.WriteLine(string.Format("Subject: \t{0}", cert.Subject));
            if (cert.PublicKey.GetRSAPublicKey() != null)
            {
                writer.WriteLine(string.Format("Public Key Algorithm: \t{0} ({1})", "RSA", cert.PublicKey.GetRSAPublicKey()!.KeySize));
            }
            if (cert.PublicKey.GetECDsaPublicKey() != null)
            {
                writer.WriteLine(string.Format("Public Key Algorithm: \t{0} ({1})", "ECDSA", cert.PublicKey.GetECDsaPublicKey()!.SignatureAlgorithm));
            }
            foreach (var ext in cert.Extensions)
            {
                writer.WriteLine(string.Format("Extension: \t{0} ({1})", ext.Oid?.FriendlyName, ext.Oid?.Value));
                writer.WriteLine(string.Format("\tCritical: \t{0}", ext.Critical ? "yes" : "no"));
                writer.WriteLine("\tValue:");
                writer.WriteLine(string.Format("\t\t{0}", string.Join(Environment.NewLine + "\t\t", ext.Format(true).Split(Environment.NewLine))));
            }
            writer.WriteLine(cert.ExportCertificatePem());
        }
    }

    public static void Print(this CertificateRequest csr, Stream stream)
    {
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            writer.WriteLine(string.Format("Subject Name: \t{0}", csr.SubjectName.Name));
            foreach (var ext in csr.CertificateExtensions)
            {
                writer.WriteLine(string.Format("Extension: \t{0} ({1})", ext.Oid?.FriendlyName, ext.Oid?.Value));
                writer.WriteLine(string.Format("\tCritical: \t{0}", ext.Critical ? "yes" : "no"));
                writer.WriteLine("\tValue:");
                writer.WriteLine(string.Format("\t\t{0}", string.Join(Environment.NewLine + "\t\t", ext.Format(true).Split(Environment.NewLine))));
            }
        }
    }

    public static void Print(this AsymmetricAlgorithm key, Stream stream)
    {
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            writer.WriteLine(string.Format("SignatureAlgorithm: \t{0}", key.SignatureAlgorithm));
            writer.WriteLine(string.Format("KeySize: \t{0}", key.KeySize));
            writer.WriteLine(key.ToPem());
        }
    }

    public static void Print(this X509Crl crl, Stream stream)
    {
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            writer.WriteLine(string.Format("Version: \t{0}", crl.Version));
            writer.WriteLine(string.Format("Signature Algorithm: \t{0}", crl.SigAlgName));
            writer.WriteLine(string.Format("Issuer: \t{0}", crl.IssuerDN.ToString()));
            writer.WriteLine(string.Format("This Update: \t{0}", crl.ThisUpdate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss Z")));
            writer.WriteLine(string.Format("Next Update: \t{0}", crl.NextUpdate?.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss Z")));

            var asn1Octet = crl.GetExtensionValue(X509Extensions.CrlNumber);
            var asn1 = X509ExtensionUtilities.FromExtensionValue(asn1Octet);
            var crlNum = DerInteger.GetInstance(asn1).PositiveValue;
            writer.WriteLine(string.Format("CrlNumber: \t{0} (0x{1})", crlNum.ToString(10), crlNum.ToString(16)));

            writer.WriteLine("Revoked Certificates:");
            try
            {
                foreach (var entry in crl.GetRevokedCertificates())
                {
                    writer.WriteLine(string.Format("\tSerial Number: \t{0} (0x{1})", entry.SerialNumber.ToString(10), entry.SerialNumber.ToString(16)));
                    writer.WriteLine(string.Format("\t\tRevocation Date: \t{0}", entry.RevocationDate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss Z")));
                }
            }
            catch
            {
            }
        }
    }

    public static ConfigCsr ToConfigCsr(this CertificateRequest csr)
    {
        ConfigCsr config = new();
        config.SubjectName = csr.SubjectName.ToConfigSubjectName();
        config.KeyPair = csr.PublicKey.GetPublicKey()?.ToConfigKeyPair();
        var extensions = new X509ExtensionCollection();
        foreach (var extension in extensions)
        {
            extensions.Add(extension);
        }
        config.Extensions = extensions.ToConfigExtensions();
        return config;
    }

    public static ConfigCsr ToConfigCsr(this X509Certificate2 cert)
    {
        ConfigCsr config = new();
        config.SubjectName = cert.SubjectName.ToConfigSubjectName();
        config.KeyPair = cert.PublicKey.GetPublicKey()?.ToConfigKeyPair();
        try
        {
            config.HashAlgorithm = cert.SignatureAlgorithm.FriendlyName?.ToHashAlgorithm().Name;
        }
        catch { }
        config.Extensions = cert.Extensions.ToConfigExtensions();
        return config;
    }

    public static ConfigExtensions ToConfigExtensions(this X509ExtensionCollection extensions)
    {
        ConfigExtensions config = new();
        config.Extensions ??= [];
        foreach (var extension in extensions)
        {
            if (extension is X509BasicConstraintsExtension basicConstraintsExtension)
            {
                config.BasicConstraints = new ConfigBasicConstraintsExtension()
                {
                    Critical = basicConstraintsExtension.Critical,
                    CertificateAuthority = basicConstraintsExtension.CertificateAuthority,
                    HasPathLengthConstraint = basicConstraintsExtension.HasPathLengthConstraint,
                    PathLengthConstraint = basicConstraintsExtension.PathLengthConstraint
                };
            }
            else if (extension is X509AuthorityKeyIdentifierExtension x509AuthorityKeyIdentifierExtension)
            {
                config.AuthorityKeyIdentifier = new ConfigAuthorityKeyIdentifierExtension()
                {
                    Critical = x509AuthorityKeyIdentifierExtension.Critical,
                    IncludeIssuerAndSerial = x509AuthorityKeyIdentifierExtension.SerialNumber != null || x509AuthorityKeyIdentifierExtension.NamedIssuer != null,
                    IncludeKeyIdentifier = x509AuthorityKeyIdentifierExtension.KeyIdentifier != null
                };
            }
            else if (extension is X509SubjectKeyIdentifierExtension x509SubjectKeyIdentifierExtension)
            {
                config.SubjectKeyIdentifier = new ConfigSubjectKeyIdentifierExtension()
                {
                    Critical = x509SubjectKeyIdentifierExtension.Critical
                };
            }
            else if (extension is X509EnhancedKeyUsageExtension x509EnhancedKeyUsageExtension)
            {
                config.ExtendedKeyUsage = new ConfigExtendedKeyUsageExtension()
                {
                    Critical = x509EnhancedKeyUsageExtension.Critical,
                    Oids = new HashSet<string>()
                };
                foreach (var oid in x509EnhancedKeyUsageExtension.EnhancedKeyUsages)
                {
                    var name = oid.FriendlyName ?? oid.Value;
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }
                    config.ExtendedKeyUsage.Oids.Add(name);
                }
            }
            else if (extension is X509KeyUsageExtension x509KeyUsageExtension)
            {
                config.KeyUsage = new ConfigKeyUsageExtension()
                {
                    Critical = x509KeyUsageExtension.Critical,
                    KeyUsages = x509KeyUsageExtension.KeyUsages
                };
            }
            else if (extension is X509SubjectAlternativeNameExtension x509SubjectAlternativeNameExtension)
            {
                // https://www.codeproject.com/Questions/5336056/How-to-add-subject-alternative-names-for-serianumb
                config.SubjectAlternativeName = new ConfigSubjectAlternativeName()
                {
                    Critical = x509SubjectAlternativeNameExtension.Critical
                };
                var subjectAltNames = GeneralNames.GetInstance(X509ExtensionUtilities.FromExtensionValue(new DerOctetString(extension.RawData)));
                foreach (var subname in subjectAltNames.GetNames())
                {
                    switch (subname.TagNo)
                    {
                        case GeneralName.OtherName:
                            var seq = (Asn1Sequence)subname.Name;
                            var anotherName = new ConfigOtherName()
                            {
                                TypeOid = ((DerObjectIdentifier)seq.First()).Id,
                                Value = seq.Last().GetEncoded()
                            };
                            config.SubjectAlternativeName.OtherNames ??= [];
                            config.SubjectAlternativeName.OtherNames.Add(anotherName);
                            break;
                        case GeneralName.Rfc822Name:
                            config.SubjectAlternativeName.EmailAddresses ??= [];
                            config.SubjectAlternativeName.EmailAddresses.Add(subname.Name.ToAsn1Object().ToString() ?? "");
                            break;
                        case GeneralName.DnsName:
                            config.SubjectAlternativeName.DnsNames ??= [];
                            config.SubjectAlternativeName.DnsNames.Add(subname.Name.ToAsn1Object().ToString() ?? "");
                            break;
                        case GeneralName.X400Address:
                            config.SubjectAlternativeName.X400Addresses ??= [];
                            config.SubjectAlternativeName.X400Addresses.Add(subname.Name.GetDerEncoded());
                            break;
                        case GeneralName.DirectoryName:
                            config.SubjectAlternativeName.DirectoryNames ??= [];
                            config.SubjectAlternativeName.DirectoryNames.Add(subname.Name.ToAsn1Object().ToString() ?? "");
                            break;
                        case GeneralName.EdiPartyName:
                            break;
                        case GeneralName.UniformResourceIdentifier:
                            config.SubjectAlternativeName.Uris ??= [];
                            config.SubjectAlternativeName.Uris.Add(subname.Name.ToAsn1Object().ToString() ?? "");
                            break;
                        case GeneralName.IPAddress:
                            var octets = ((Asn1OctetString)subname.Name).GetOctets();
                            config.SubjectAlternativeName.IPAddresses ??= [];
                            try
                            {
                                config.SubjectAlternativeName.IPAddresses.Add(new IPAddress(octets).ToString());
                            }
                            catch
                            {
                                config.SubjectAlternativeName.IPAddresses.Add(Convert.ToHexString(octets));
                            }
                            break;
                        case GeneralName.RegisteredID:
                            config.SubjectAlternativeName.RegisteredIds ??= [];
                            config.SubjectAlternativeName.RegisteredIds.Add(subname.Name.ToAsn1Object().ToString() ?? "");
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                var name = extension.Oid?.FriendlyName ?? extension.Oid?.Value;
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                var e = new ConfigExtensionDefault()
                {
                    Critical = extension.Critical,
                    Value = extension.RawData,
                    Oid = name
                };
                config.Extensions.Add(e);
            }
        }
        return config;
    }

    public static ConfigKeyPair ToConfigKeyPair(this AsymmetricAlgorithm key)
    {
        ConfigKeyPair config = new();
        if (key is RSA rsa)
        {
            config.SignatureAlgorithm = SignatureAlgorithmName.Rsa;
            config.KeySize = rsa.KeySize;
        }
        if (key is DSA dsa)
        {
            config.SignatureAlgorithm = SignatureAlgorithmName.Dsa;
        }
        if (key is ECDsa ecdsa)
        {
            config.SignatureAlgorithm = SignatureAlgorithmName.Ecdsa;
            var curve = ecdsa.ExportParameters(false).Curve.Oid;
            config.Eccurve = curve.FriendlyName ?? curve.Value;
        }
        if (key is ECDiffieHellman ecdh)
        {
            config.SignatureAlgorithm = SignatureAlgorithmName.Ecdh;
            var curve = ecdh.ExportParameters(false).Curve.Oid;
            config.Eccurve = curve.FriendlyName ?? curve.Value;
        }
        return config;
    }

    public static ConfigSubjectName ToConfigSubjectName(this X500DistinguishedName x500DistinguishedName)
    {
        var x500Name = X509Name.GetInstance(x500DistinguishedName.RawData);
        ConfigSubjectName config = new();
        // get all values with OIDs
        List<KeyValuePair<string, string>> oids = new();
        var valueList = x500Name.GetValueList();
        var oidList = x500Name.GetOidList();
        for (int i = 0; i < valueList.Count() && i < oidList.Count(); i++)
        {
            oids.Add(new KeyValuePair<string, string>(oidList[i].Id, valueList[i]));
        }
        config.Oids = oids;
        // map oids to fields
        foreach (var prop in typeof(ConfigSubjectName).GetProperties())
        {
            var attr = prop.GetCustomAttribute<OidAttribute>();
            if (attr == null)
            {
                continue;
            }
            // find values matching this prop OID
            var list = oids.Where((item) =>
            {
                return string.Equals(item.Key, attr.Id);
            }).ToList();
            if (list.Count() == 0)
            {
                // no match
                continue;
            }
            // set value to prop
            bool mapped = false;
            if (prop.PropertyType == typeof(string))
            {
                mapped = true;
                prop.SetValue(config, list.First().Value);
            }
            else if (prop.PropertyType.IsAssignableTo(typeof(ICollection<string>)))
            {
                mapped = true;
                prop.SetValue(config, list.Select((item) =>
                {
                    return item.Value;
                }).ToList());
            }
            if (mapped)
            {
                // remove mapped values from list
                oids.RemoveAll((item) =>
                {
                    return list.Contains(item);
                });
            }
        }
        return config;
    }
}
