using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GGolbik.SecurityTools.Crypto;

namespace GGolbik.SecurityTools.Credentials;

[JsonConverter(typeof(KeyCredentialsJsonConverter))]
public class KeyCredentials : ICloneable
{
    /// <summary>
    /// The description of the credentials.
    /// </summary>
    public KeyCredentialsDescription Description { get; }

    /// <summary>
    /// The credentials. The enrypted or non-encrypted JSON string of the KeyCredentials.
    /// </summary>
    public byte[] Value { get; }

    public KeyCredentials(KeyCredentialsDescription description, byte[] value)
    {
        this.Description = description;
        this.Value = value;
    }

    public KeyCredentials(KeyCredentialsValue? value, byte[]? password = null) : this(new(), value, password)
    {

    }
    public KeyCredentials(KeyCredentialsDescription description, KeyCredentialsValue? value, byte[]? password = null)
    {
        this.Description = description;
        this.Description.Label ??= value?.ToString();
        this.Value = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
        if (Description.Encrypted)
        {
            this.Value = Crypt.EncryptAes(this.Value, password ?? []);
        }
        if(value != null)
        {
            this.Description.Kind = value.Kind;
        }
    }

    public KeyCredentialsValue? ToKeyCredentialsValue(byte[]? password = null)
    {
        if (Value.Length == 0)
        {
            return null;
        }
        try
        {
            if (Description?.Encrypted ?? false)
            {
                var decrypted = Crypt.DecryptAes(Value, password ?? []);
                return JsonSerializer.Deserialize<KeyCredentialsValue>(Encoding.UTF8.GetString(decrypted));
            }
            else
            {
                return JsonSerializer.Deserialize<KeyCredentialsValue>(Encoding.UTF8.GetString(Value));
            }
        }
        catch
        {
            return null;
        }
    }

    public object Clone()
    {
        byte[] copy = new byte[Value.Length];
        Array.Copy(Value, copy, Value.Length);
        return new KeyCredentials((KeyCredentialsDescription)Description.Clone(), copy);
    }
}