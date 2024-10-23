using System.Text.Json;
using System.Text.Json.Serialization;

namespace GGolbik.SecurityTools.Credentials;

public class KeyCredentialsValueJsonConverter : JsonConverter<KeyCredentialsValue>
{
    /// <summary>
    /// 
    /// </summary>
    public KeyCredentialsValueJsonConverter()
    {
    }

    public override KeyCredentialsValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var json = JsonDocument.ParseValue(ref reader);
        return Read(json, typeToConvert, options);
    }

    public KeyCredentialsValue? Read(JsonDocument json, Type typeToConvert, JsonSerializerOptions options)
    {
        string propertyKind = nameof(KeyCredentialsValue.Kind);
        if (options.PropertyNamingPolicy != null)
        {
            propertyKind = options.PropertyNamingPolicy.ConvertName(propertyKind);
        }
        var stringComparison = options.PropertyNameCaseInsensitive ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

        var properties = json.RootElement.EnumerateObject().Where((item) =>
        {
            return string.Equals(item.Name, propertyKind, stringComparison);
        });
        if (properties.Count() == 0)
        {
            throw new JsonException();
        }
        var kind = properties.First().Value.Deserialize<KeyCredentialsKind>();
        switch (kind)
        {
            case KeyCredentialsKind.Anonymous:
                return JsonSerializer.Deserialize<AnonymousCredentials?>(json, options);
            case KeyCredentialsKind.UsernamePassword:
                return JsonSerializer.Deserialize<UsernamePasswordCredentials?>(json, options);
            case KeyCredentialsKind.Certificate:
                return JsonSerializer.Deserialize<CertificateCredentials?>(json, options);
            case KeyCredentialsKind.Token:
                return JsonSerializer.Deserialize<TokenCredentials?>(json, options);
            default:
                throw new JsonException();
        }
    }

    public override void Write(Utf8JsonWriter writer, KeyCredentialsValue value, JsonSerializerOptions options)
    {
        if (value is AnonymousCredentials anonymous)
        {
            JsonSerializer.Serialize(writer, anonymous, options);
        }
        else if (value is UsernamePasswordCredentials usernamePassword)
        {
            JsonSerializer.Serialize(writer, usernamePassword, options);
        }
        else if (value is CertificateCredentials certificate)
        {
            JsonSerializer.Serialize(writer, certificate, options);
        }
        else if (value is TokenCredentials token)
        {
            JsonSerializer.Serialize(writer, token, options);
        }
        else
        {
            throw new JsonException();
        }
    }
}