using System.Text;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Security;
namespace GGolbik.SecurityTools.Crypto;

public class Crypt
{
    #region Encrypt AES

    /// <summary>
    /// The data is encrypted with AES 256 CBC and stored as Enveloped-Data in a CMS using the PasswordRecipientInfo with PBKDF2.
    /// 
    /// Cryptographic Message Syntax (CMS)
    /// https://datatracker.ietf.org/doc/html/rfc5652
    /// 
    /// Enveloped-Data Content Type
    /// https://datatracker.ietf.org/doc/html/rfc5652#section-6
    /// 
    /// RecipientInfo Type
    /// https://datatracker.ietf.org/doc/html/rfc5652#section-6.2
    /// 
    /// PasswordRecipientInfo Type
    /// https://datatracker.ietf.org/doc/html/rfc5652#section-6.2.4
    /// 
    /// Use of the Advanced Encryption Standard (AES) Encryption Algorithm in Cryptographic Message Syntax (CMS)
    /// https://datatracker.ietf.org/doc/html/rfc3565
    /// 
    /// AlgorithmIdentifier
    /// https://datatracker.ietf.org/doc/html/rfc5280#section-4.1.1.2
    /// 
    /// PKCS #5: Password-Based Cryptography Specification Version 2.0
    /// https://datatracker.ietf.org/doc/html/rfc2898
    /// 
    /// OWASP Password Storage Cheat Sheet PBKDF2
    /// https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#pbkdf2
    /// </summary>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="password">The password to use for encryption.</param>
    /// <returns>DER encoded CMS.</returns>
    // public static byte[] EncryptAes(Stream data, byte[] password)
    // {
    //     // Generate a random salt
    //     byte[] salt = new SecureRandom().GenerateSeed(256);
    //     int iterationCount = 1_000_000;
    //     // key encryption algorithm to use.
    //     string kekAlgorithmOid = CmsEnvelopedDataGenerator.Aes256Cbc;
    //     string encryptionOid = CmsEnvelopedDataGenerator.Aes256Cbc;

    //     // Create CMS Enveloped Data
    //     CmsEnvelopedDataGenerator generator = new CmsEnvelopedDataGenerator();
    //     generator.AddPasswordRecipient(new Pkcs5Scheme2Utf8PbeKey(Encoding.UTF8.GetChars(password), salt, iterationCount), kekAlgorithmOid);

    //     CmsProcessable cmsData = new CmsProcessableInputStream(data); // data: The plaintext. not encrypted.
    //     CmsEnvelopedData envelopedData = generator.Generate(cmsData, encryptionOid);

    //     // Output the CMS Enveloped Data
    //     byte[] envelopedDataBytes = envelopedData.GetEncoded();
    //     return envelopedDataBytes;
    // }

    public static void EncryptAes(string encryptionOid, Stream data, Stream encrypted, byte[] password, CancellationToken? cancellationToken = null)
    {
        // Generate a random salt
        byte[] salt = new SecureRandom().GenerateSeed(256);
        int iterationCount = 1_000_000;
        // key encryption algorithm to use.
        // need not to be equal to encryptionOid
        string kekAlgorithmOid = CmsEnvelopedDataGenerator.Aes256Cbc;

        // Create CMS Enveloped Data
        CmsEnvelopedDataStreamGenerator generator = new CmsEnvelopedDataStreamGenerator();

        generator.AddPasswordRecipient(new Pkcs5Scheme2Utf8PbeKey(Encoding.UTF8.GetChars(password), salt, iterationCount), kekAlgorithmOid);
        using (var stream = generator.Open(encrypted, encryptionOid))
        {
            if(cancellationToken == null)
            {
                data.CopyTo(stream);
            }
            else
            {
                data.CopyToAsync(stream, (CancellationToken)cancellationToken).Wait();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data">Data to be encrypted.</param>
    /// <param name="password">The password for encryption.</param>
    /// <returns>The enctypte data.</returns>
    public static byte[] EncryptAes(byte[] data, byte[] password)
    {
        MemoryStream encrypted = new();
        EncryptAes256Gcm(new MemoryStream(data), encrypted, password);
        return encrypted.ToArray();
    }

    public static void EncryptAes128Cbc(Stream data, Stream encrypted, byte[] password, CancellationToken? cancellationToken = null)
    {
        EncryptAes(CmsEnvelopedDataGenerator.Aes128Cbc, data, encrypted, password, cancellationToken);
    }

    public static void EncryptAes256Cbc(Stream data, Stream encrypted, byte[] password, CancellationToken? cancellationToken = null)
    {
        EncryptAes(CmsEnvelopedDataGenerator.Aes256Cbc, data, encrypted, password, cancellationToken);
    }

    public static void EncryptAes256Gcm(Stream data, Stream encrypted, byte[] password, CancellationToken? cancellationToken = null)
    {
        EncryptAes(CmsEnvelopedDataGenerator.Aes256Gcm, data, encrypted, password, cancellationToken);
    }

    #endregion

    #region Decrypt AES

    public static byte[] DecryptAes(byte[] envelopedDataBytes, byte[] password)
    {
        // Create CmsEnvelopedData object
        CmsEnvelopedData envelopedData = new CmsEnvelopedData(envelopedDataBytes);

        // Decrypt the data
        var recipient = envelopedData.GetRecipientInfos().GetRecipients().FirstOrDefault();
        if (recipient is PasswordRecipientInformation passwordRecipient)
        {
            Pkcs5Scheme2Utf8PbeKey pbeKey = new Pkcs5Scheme2Utf8PbeKey(Encoding.UTF8.GetChars(password), passwordRecipient.KeyDerivationAlgorithm);
            byte[] decryptedData = recipient.GetContent(pbeKey);

            // Output the decrypted data
            string decryptedString = Encoding.UTF8.GetString(decryptedData);
            Console.WriteLine("Decrypted Data: " + decryptedString);

            return decryptedData;
        }
        return envelopedDataBytes;
    }

    public static void DecryptAes(Stream envelopedDataBytes, byte[] password, Stream data, CancellationToken cancellationToken)
    {
        // Note: that because we are in a streaming mode only one recipient can be tried and it is important
        // that the methods on the parser are called in the appropriate order.
        CmsEnvelopedDataParser envelopedData = new CmsEnvelopedDataParser(envelopedDataBytes);

        // Decrypt the data
        var recipient = envelopedData.GetRecipientInfos().GetRecipients().FirstOrDefault();
        if (recipient is PasswordRecipientInformation passwordRecipient)
        {
            Pkcs5Scheme2Utf8PbeKey pbeKey = new Pkcs5Scheme2Utf8PbeKey(Encoding.UTF8.GetChars(password), passwordRecipient.KeyDerivationAlgorithm);
            using (var stream = recipient.GetContentStream(pbeKey).ContentStream)
            {
                stream.CopyToAsync(data, cancellationToken).Wait();
            }
        }
    }

    #endregion
}