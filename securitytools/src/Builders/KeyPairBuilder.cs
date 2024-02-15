using System.Security.Cryptography;
using System.Text.Json;
using GGolbik.SecurityTools.Models;

namespace GGolbik.SecurityTools.Builders;

public class KeyPairBuilder : SecurityBuilder<CsrBuilder>
{
    public ConfigKeyPair Build(ConfigKeyPair config)
    {
        Logger.Information("Build key pair.");
        var result = JsonSerializer.Deserialize<ConfigKeyPair>(JsonSerializer.Serialize(config))!;

        Logger.Debug("Load or generate private key.");
        AsymmetricAlgorithm privateKey = this.LoadOrGeneratePrivateKey(result);

        result.PrivateKey ??= new X509File();
        result.PublicKey ??= new X509File();
        HashAlgorithmName? keyAlg = string.IsNullOrEmpty(result.HashAlgorithm) ? null : ToHashAlgorithm(result.HashAlgorithm);
        Logger.Debug("Create key pair.");
        this.SaveKeyPair(result.PrivateKey, result.PublicKey, privateKey, keyAlg);
        Logger.Debug("Built key pair.");
        return result;
    }
}
