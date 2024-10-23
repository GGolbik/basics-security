using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using GGolbik.SecurityTools.Store;
using GGolbik.SecurityTools.X509.Models;
using Org.BouncyCastle.X509;

namespace GGolbik.SecurityTools.X509.Builders;

public class TransformBuilder : SecurityBuilder<CsrBuilder>
{
    public ConfigTransform Build(ConfigTransform config)
    {
        var result = (ConfigTransform)config.Clone();

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
                this.TransFormConfig(result);
                break;
            default:
                throw new ArgumentException("Transform mode", "Invalid transform mode.");
        }

        return result;
    }

    /// <summary>
    /// Load all input files
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    private List<object> Load(ConfigTransform config)
    {
        var list = new List<object>();
        if (config.Input != null)
        {
            foreach (var file in config.Input)
            {
                if (!file.Exists())
                {
                    continue;
                }
                using (var stream = file.ToStream())
                {
                    X509Reader.Read(stream, file.Password, (cert) =>
                    {
                        list.Add(cert);
                    }, (keyPair) =>
                    {
                        list.Add(keyPair);
                    }, (crl) =>
                    {
                        list.Add(crl);
                    }, (csr) =>
                    {
                        list.Add(csr);
                    });
                }
            }
        }
        return list;
    }

    private void Print(ConfigTransform config)
    {
        var list = this.Load(config);
        config.Output ??= new List<X50xFile>();
        var mem = new MemoryStream();
        foreach (var item in list)
        {
            if (item is X509Certificate2 cert)
            {
                cert.Print(mem);
            }
            else if (item is AsymmetricAlgorithm key)
            {
                key.Print(mem);
            }
            else if (item is X509Crl crl)
            {
                crl.Print(mem);
            }
            else if (item is CertificateRequest csr)
            {
                csr.Print(mem);
            }
        }
        config.Output.Add(new X50xFile()
        {
            Data = mem.ToArray()
        });
    }

    private void TransForm(ConfigTransform config, bool pem)
    {
        if (config.Input == null)
        {
            return;
        }
        config.Output ??= new List<X50xFile>();
        if (config.Input != null)
        {
            for (int i = 0; i < config.Input.Count(); i++)
            {
                var file = config.Input[i];
                var fileOutput = new X50xFile();
                if (config.Output.Count() < i)
                {
                    fileOutput = config.Output[i];
                }
                fileOutput.FileFormat = new X50xFileFormat(pem ? X509Encoding.Pem : X509Encoding.Der);

                if (!file.Exists())
                {
                    continue;
                }
                InMemCertificateStore store = new();
                using (var stream = file.ToStream())
                {
                    store.Add(stream, new PasswordFinder(file.Password));
                }
                foreach (var cert in store.GetCertificates())
                {
                    this.SaveCert(fileOutput, cert);
                    config.Output.Add((X50xFile)fileOutput.Clone());
                    config.Output.Last().Alias = cert.Subject.ReplaceAll(Path.GetInvalidFileNameChars(), '_') + (pem ? ".pem" : ".crt");
                }
                foreach (var key in store.GetKeyPairs())
                {
                    this.SaveKeyPair(fileOutput, new X50xFile(), key);
                    config.Output.Add((X50xFile)fileOutput.Clone());
                    config.Output.Last().Alias = key.ToThumbprint() + (pem ? ".pem" : ".key");
                }
                foreach (var crl in store.GetCrls())
                {
                    this.SaveCrl(fileOutput, crl);
                    config.Output.Add((X50xFile)fileOutput.Clone());
                    config.Output.Last().Alias = crl.IssuerDN.ToString().ReplaceAll(Path.GetInvalidFileNameChars(), '_') + ".crl";
                }
                foreach (var csr in store.GetCsrs())
                {
                    this.SaveCsr(fileOutput, csr);
                    config.Output.Add((X50xFile)fileOutput.Clone());
                    config.Output.Last().Alias = csr.SubjectName.Name.ReplaceAll(Path.GetInvalidFileNameChars(), '_') + ".csr";
                }
            }
        };
    }

    private void TransformStore(ConfigTransform config, bool pem)
    {
        InMemCertificateStore certificateStore = new();
        if (config.Input != null)
        {
            foreach (var file in config.Input)
            {
                if (!file.Exists())
                {
                    continue;
                }
                using (var stream = file.ToStream())
                {
                    certificateStore.Add(stream, new PasswordFinder(file.Password));
                }
            }
        }

        config.Output ??= new List<X50xFile>();
        if (config.Output.Count() == 0)
        {
            config.Output.Add(new X50xFile());
        }
        var storeFile = config.Output.ElementAt(0);
        storeFile.FileFormat = new X50xFileFormat(X509ContentType.Pkcs12, pem ? X509Encoding.Pem : X509Encoding.Der);
        if (pem)
        {
            this.SaveStorePem(storeFile, certificateStore);
        }
        else
        {
            X509Certificate2Collection store = new();
            store.AddRange(certificateStore.GetCertificates(true).ToArray());
            this.SaveStore(storeFile, store);
        }
        storeFile.Alias = pem ? "store.pem" : "store.p12";
    }

    protected void SaveStorePem(X50xFile file, CertificateStore store)
    {
        MemoryStream stream = new();
        using (StreamWriter writer = new(stream))
        {
            foreach (var cert in store.GetCertificates())
            {
                writer.WriteLine(cert.ToPem());
            }
            foreach (var key in store.GetKeyPairs())
            {
                writer.WriteLine(string.IsNullOrEmpty(file.Password) ? key.ToPem() : key.ToPem(new PasswordFinder(file.Password)!));
            }
            foreach (var crl in store.GetCrls())
            {
                writer.WriteLine(crl.ToPem());
            }
            foreach (var csr in store.GetCsrs())
            {
                writer.WriteLine(csr.ToPem());
            }
        }
        file.Data = stream.ToArray();

        // save to file
        if (!string.IsNullOrEmpty(file.FileName))
        {
            var dir = Path.GetDirectoryName(file.FileName);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(file.FileName, file.Data!);
        }
    }

    private void TransFormConfig(ConfigTransform config)
    {
        var list = this.Load(config);
        config.Output ??= new List<X50xFile>();
        foreach (var obj in list)
        {
            if (obj is X509Certificate2 cert)
            {
                config.Output.Add(new X50xFile()
                {
                    Data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cert.ToConfigCsr(), new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })),
                    Alias = "config_" + cert.Subject.ReplaceAll(Path.GetInvalidFileNameChars(), '_') + ".json"
                });
            }
        }
    }
}
