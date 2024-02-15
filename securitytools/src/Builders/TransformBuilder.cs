using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using GGolbik.SecurityTools.Core;
using GGolbik.SecurityTools.Models;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using Serilog;

namespace GGolbik.SecurityTools.Builders;

public class TransformBuilder : SecurityBuilder<CsrBuilder>
{
    public ConfigTransform Build(ConfigTransform config)
    {
        Logger.Information("Transform {0}", config.Mode);
        var result = JsonSerializer.Deserialize<ConfigTransform>(JsonSerializer.Serialize(config))!;

        switch (result.Mode)
        {
            case TransformMode.Store:
                this.TransformStore(result, pem: false);
                break;
            case TransformMode.SinglePem:
                this.TransformStore(result, pem: true);
                break;
            case TransformMode.Pem:
                this.TransForm(result, pem: true);
                break;
            case TransformMode.Der:
                this.TransForm(result, pem: false);
                break;
            case TransformMode.Print:
                this.Print(result);
                break;
            case TransformMode.Config:
                throw Errors.CreateInvalidArgumentError("Transform mode", "Not implemented.");
            default:
                throw Errors.CreateInvalidArgumentError("Transform mode", "Invalid transform mode.");
        }

        Logger.Information("Transformed");
        return result;
    }

    /// <summary>
    /// Load all input files
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    private List<object> Load(ConfigTransform config)
    {
        Logger.Debug("Load input files.");
        var list = new List<object>();
        if (config.Input != null)
        {
            foreach (var file in config.Input)
            {
                Logger.Debug("Load input file.");
                var ex = new List<Exception>();
                try
                {
                    Logger.Debug("Try to load input file as certificate store.");
                    var store = this.LoadStore(file);
                    if (store.Count() > 0)
                    {
                        Logger.Debug("Found {0} certificates.", store.Count());
                        list.Add(store);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Logger.Verbose(e, "Failed to load file as store.");
                    ex.Add(e);
                }
                try
                {
                    Logger.Debug("Try to load input file as certificate.");
                    var entryCert = this.LoadCertificate(file);
                    if (entryCert != null)
                    {
                        Logger.Debug("Found certificate.");
                        list.Add(entryCert);
                        if (entryCert.HasPrivateKey)
                        {
                            Logger.Warning("Load private key from cert not supported");
                        }
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Logger.Verbose(e, "Failed to load file as certificate.");
                    ex.Add(e);
                }

                try
                {
                    Logger.Debug("Try to load input file as CRL.");
                    var crl = new X509CrlParser().ReadCrl(file.Data);
                    if (crl != null)
                    {
                        Logger.Debug("Found CRL.");
                        list.Add(crl);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Logger.Verbose(e, "Failed to load file as CRL.");
                    ex.Add(e);
                }

                try
                {
                    Logger.Debug("Try to load input file as CSR.");
                    var csr = this.LoadCsr(new ConfigCsr() { Csr = file }, CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions | CertificateRequestLoadOptions.SkipSignatureValidation);
                    if (csr != null)
                    {
                        Logger.Debug("Found CSR.");
                        list.Add(csr);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Logger.Verbose(e, "Failed to load file as CSR.");
                    ex.Add(e);
                }

                try
                {
                    var key = LoadPrivateKey(file, out var alg);
                    Logger.Debug("Found private key.");
                    list.Add(key);
                    continue;
                }
                catch (Exception e)
                {
                    Logger.Verbose(e, "Failed to load file as private key.");
                    ex.Add(e);
                }

                throw Errors.CreateInvalidArgumentError(new AggregateException(ex), "input", "Failed to load input file.");
            }
        }
        return list;
    }

    private void Print(ConfigTransform config)
    {
        var list = this.Load(config);
        config.Output ??= new List<X509File>();
        var mem = new MemoryStream();
        using (var stream = new StreamWriter(mem))
        {
            foreach (var item in list)
            {
                if (item is X509Certificate2Collection store)
                {
                    Logger.Debug("Print certificate store.");
                    foreach (var cert in store)
                    {
                        this.PrintCert(cert, stream);
                    }
                }
                else if (item is X509Certificate2 cert)
                {
                    Logger.Debug("Print Certificate.");
                    this.PrintCert(cert, stream);
                }
                else if (item is AsymmetricAlgorithm key)
                {
                    Logger.Debug("Print Key.");
                    this.PrintKey(key, stream);
                }
                else if (item is X509Crl crl)
                {
                    Logger.Debug("Print CRL.");
                    this.PrintCrl(crl, stream);
                }
                else if (item is CertificateRequest csr)
                {
                    Logger.Debug("Print CSR.");
                    this.PrintCsr(csr, stream);
                }
                else
                {
                    Logger.Debug("Cannot print file.");
                }
            }
        }
        config.Output.Add(new X509File()
        {
            Data = mem.ToArray()
        });
    }
    private void PrintCsr(CertificateRequest csr, StreamWriter stream)
    {
        stream.WriteLine(string.Format("Subject Name: \t{0}", csr.SubjectName.Name));
        foreach (var ext in csr.CertificateExtensions)
        {
            stream.WriteLine(string.Format("Extension: \t{0} ({1})", ext.Oid?.FriendlyName, ext.Oid?.Value));
            stream.WriteLine(string.Format("\tCritical: \t{0}", ext.Critical ? "yes" : "no"));
            stream.WriteLine("\tValue:");
            stream.WriteLine(string.Format("\t\t{0}", string.Join(Environment.NewLine + "\t\t", ext.Format(true).Split(Environment.NewLine))));
        }
    }

    private void PrintCrl(X509Crl crl, StreamWriter stream)
    {
        stream.WriteLine(string.Format("Version: \t{0}", crl.Version));
        stream.WriteLine(string.Format("Signature Algorithm: \t{0}", crl.SigAlgName));
        stream.WriteLine(string.Format("Issuer: \t{0}", crl.IssuerDN.ToString()));
        stream.WriteLine(string.Format("This Update: \t{0}", crl.ThisUpdate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss Z")));
        stream.WriteLine(string.Format("Next Update: \t{0}", crl.NextUpdate?.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss Z")));

        var asn1Octet = crl.GetExtensionValue(X509Extensions.CrlNumber);
        var asn1 = X509ExtensionUtilities.FromExtensionValue(asn1Octet);
        var crlNum = DerInteger.GetInstance(asn1).PositiveValue;
        stream.WriteLine(string.Format("CrlNumber: \t{0} (0x{1})", crlNum.ToString(10), crlNum.ToString(16)));

        stream.WriteLine("Revoked Certificates:");
        try
        {
            foreach (var entry in crl.GetRevokedCertificates())
            {
                stream.WriteLine(string.Format("\tSerial Number: \t{0} (0x{1})", entry.SerialNumber.ToString(10), entry.SerialNumber.ToString(16)));
                stream.WriteLine(string.Format("\t\tRevocation Date: \t{0}", entry.RevocationDate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss Z")));
            }
        }
        catch (Exception e)
        {
            Logger.Verbose(e, "Failed to print CRL.");
        }
    }

    private void PrintKey(AsymmetricAlgorithm key, StreamWriter stream)
    {
        stream.WriteLine(string.Format("SignatureAlgorithm: \t{0}", key.SignatureAlgorithm));
        stream.WriteLine(string.Format("KeySize: \t{0}", key.KeySize));
        stream.WriteLine(key.ExportPkcs8PrivateKeyPem());
    }

    private void PrintCert(X509Certificate2 cert, StreamWriter stream)
    {
        stream.WriteLine(string.Format("Version: \t{0}", cert.Version));
        stream.WriteLine(string.Format("Serial Number: \t{0} (0x{1})", new Org.BouncyCastle.Math.BigInteger(cert.SerialNumber, 16).ToString(10), cert.SerialNumber));
        stream.WriteLine(string.Format("Signature Algorithm: \t{0} (OID: {1})", cert.SignatureAlgorithm.FriendlyName, cert.SignatureAlgorithm.Value));
        stream.WriteLine(string.Format("Issuer: \t{0}", cert.Issuer));
        stream.WriteLine(string.Format("Validity Not Before: \t{0}", cert.NotBefore.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss Z")));
        stream.WriteLine(string.Format("Validity Not After: \t{0}", cert.NotAfter.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss Z")));
        stream.WriteLine(string.Format("Subject: \t{0}", cert.Subject));
        if (cert.PublicKey.GetRSAPublicKey() != null)
        {
            stream.WriteLine(string.Format("Public Key Algorithm: \t{0} ({1})", "RSA", cert.PublicKey.GetRSAPublicKey()!.KeySize));
        }
        if (cert.PublicKey.GetECDsaPublicKey() != null)
        {
            stream.WriteLine(string.Format("Public Key Algorithm: \t{0} ({1})", "ECDSA", cert.PublicKey.GetECDsaPublicKey()!.SignatureAlgorithm));
        }
        foreach (var ext in cert.Extensions)
        {
            stream.WriteLine(string.Format("Extension: \t{0} ({1})", ext.Oid?.FriendlyName, ext.Oid?.Value));
            stream.WriteLine(string.Format("\tCritical: \t{0}", ext.Critical ? "yes" : "no"));
            stream.WriteLine("\tValue:");
            stream.WriteLine(string.Format("\t\t{0}", string.Join(Environment.NewLine + "\t\t", ext.Format(true).Split(Environment.NewLine))));
        }
        stream.WriteLine(cert.ExportCertificatePem());
    }

    private void AddCert(IList<X509File> list, X509Certificate2 cert, bool pem, string? password)
    {
        var certFile = new X509File()
        {
            FileFormat = new X509FileFormat(pem ? X509Encoding.Pem : X509Encoding.Der)
        };
        this.SaveCert(certFile, cert);
        list.Add(certFile);

        if (this.TryLoadPrivateKey(cert, out var privateKey))
        {
            var keyFile = new X509File()
            {
                Password = password,
                FileFormat = new X509FileFormat(pem ? X509Encoding.Pem : X509Encoding.Der)
            };
            this.SaveKeyPair(keyFile, new X509File(), privateKey);
            list.Add(keyFile);
        }
    }

    private void TransForm(ConfigTransform config, bool pem)
    {
        Logger.Debug("Transform files to {0}.", pem ? "PEM" : "DER");
        if (config.Input == null)
        {
            return;
        }
        config.Output ??= new List<X509File>();
        Logger.Debug("Load input files.");
        foreach (var file in config.Input)
        {
            var ex = new List<Exception>();
            try
            {
                Logger.Debug("Try to load input file as store.");
                var entryStore = this.LoadStore(file);
                if (entryStore.Count() > 0)
                {
                    Logger.Debug("Fount {0} certificates.", entryStore.Count());
                    foreach (var cert in entryStore)
                    {
                        this.AddCert(config.Output, cert, pem, file.Password);
                    }
                    continue;
                }
            }
            catch (Exception e)
            {
                Logger.Verbose(e, "Failed to load file as store.");
                ex.Add(e);
            }

            try
            {
                Logger.Debug("Try to load input file as certificate.");
                var entryCert = this.LoadCertificate(file);
                if (entryCert != null)
                {
                    Logger.Debug("Found certificate.");
                    this.AddCert(config.Output, entryCert, pem, file.Password);
                    continue;
                }
            }
            catch (Exception e)
            {
                Logger.Verbose(e, "Failed to load input as certificate.");
                ex.Add(e);
            }

            try
            {
                Logger.Debug("Try to load input file as private key.");
                var privateKey = this.LoadPrivateKey(file, out var alg);
                var keyFile = new X509File()
                {
                    Password = file.Password,
                    FileFormat = new X509FileFormat(pem ? X509Encoding.Pem : X509Encoding.Der)
                };
                this.SaveKeyPair(keyFile, new X509File(), privateKey);
                config.Output.Add(keyFile);
            }
            catch (Exception e)
            {
                Logger.Verbose("Failed to load file as private key.");
                ex.Add(e);
            }
            throw Errors.CreateInvalidArgumentError(new AggregateException(ex), "input", "Failed to load input file.");
        }
    }

    private void TransformStore(ConfigTransform config, bool pem)
    {
        Logger.Debug("Transform all files into a single file in {0} format.", pem ? "PEM" : "DER");
        var store = new X509Certificate2Collection();
        var keys = new List<AsymmetricAlgorithm>();
        if (config.Input != null)
        {
            Logger.Debug("Load input files.");
            foreach (var file in config.Input)
            {
                var ex = new List<Exception>();
                try
                {
                    Logger.Debug("Try to load input file as store.");
                    var entryStore = this.LoadStore(file);
                    if (entryStore.Count() > 0)
                    {
                        Logger.Debug("Fount {0} certificates.", entryStore.Count());
                        store.AddRange(entryStore);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Logger.Verbose(e, "Failed to load file as store.");
                    ex.Add(e);
                }
                try
                {
                    Logger.Debug("Try to load input file as certificate.");
                    var entryCert = this.LoadCertificate(file);
                    if (entryCert != null)
                    {
                        Logger.Debug("Found certificate.");
                        store.Add(entryCert);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Logger.Verbose(e, "Failed to load file as certificate.");
                    ex.Add(e);
                }

                try
                {
                    Logger.Debug("Try to load input file as private key.");
                    var key = this.LoadPrivateKey(file, out var alg);
                    keys.Add(key);
                    continue;
                }
                catch (Exception e)
                {
                    Logger.Verbose(e, "Failed to load certificate as private key.");
                    ex.Add(e);
                }
                throw Errors.CreateInvalidArgumentError(new AggregateException(ex), "input", "Failed to load input file.");
            }
        }

        Logger.Debug("Find matching certificates and keys.");
        var removeCerts = new X509Certificate2Collection();
        var newCerts = new X509Certificate2Collection();
        var removeKeys = new List<AsymmetricAlgorithm>();
        foreach (var key in keys)
        {
            foreach (var cert in store)
            {
                var signatureGenerator = this.CreateX509SignatureGenerator(key);
                if (cert.PublicKey.Equals(signatureGenerator.PublicKey))
                {
                    if (cert.HasPrivateKey)
                    {
                        removeKeys.Remove(key);
                    }
                    else if (key is RSA rsa)
                    {
                        removeKeys.Remove(key);
                        newCerts.Add(cert.CopyWithPrivateKey(rsa));
                        removeCerts.Add(cert);
                    }
                    else if (key is ECDsa eCDsa)
                    {
                        removeKeys.Remove(key);
                        newCerts.Add(cert.CopyWithPrivateKey(eCDsa));
                        removeCerts.Add(cert);
                    }
                    else
                    {
                        Logger.Warning("Unsupported key type");
                    }
                }
            }
        }
        store.RemoveRange(removeCerts);
        store.AddRange(newCerts);
        foreach (var key in removeKeys)
        {
            keys.Remove(key);
        }

        Logger.Debug("Create output.");
        config.Output ??= new List<X509File>();
        if (config.Output.Count() == 0)
        {
            config.Output.Add(new X509File());
        }
        var storeFile = config.Output.ElementAt(0);

        Logger.Debug("Save as {0}.", pem ? "PEM" : "DER");
        if (pem)
        {
            this.SaveStorePem(storeFile, store);
            foreach (var key in keys)
            {
                var file = new X509File()
                {
                    FileFormat = new X509FileFormat(X509Encoding.Pem)
                };
                this.SavePrivateKey(file, key);
                var mem = new MemoryStream();
                mem.Write(storeFile.Data);
                mem.Write(file.Data);
                if (File.Exists(storeFile.FileName))
                {
                    using (var stream = File.OpenWrite(storeFile.FileName))
                    {
                        stream.Seek(0, SeekOrigin.End);
                        stream.Write(file.Data);
                    }
                }
                storeFile.Data = mem.ToArray();
            }
        }
        else
        {
            this.SaveStore(storeFile, store);
            if (keys.Count() > 0)
            {
                Logger.Warning("{0} unrelated keys could not be stored.");
            }
        }
    }
}
