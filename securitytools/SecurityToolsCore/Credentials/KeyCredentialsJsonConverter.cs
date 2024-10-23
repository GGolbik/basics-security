using System.Text.Json;
using System.Text.Json.Serialization;

namespace GGolbik.SecurityTools.Credentials;

public class KeyCredentialsJsonConverter : JsonConverter<KeyCredentials>
{
    /// <summary>
    /// Whether <see cref="KeyCredentials.Value"/> may be serialized.
    /// </summary>
    private bool _includeCredentials;

    /// <summary>
    /// 
    /// </summary>
    public KeyCredentialsJsonConverter() : this(true)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="includeCredentials">Whether <see cref="KeyCredentials.Value"/> may be serialized.</param>
    public KeyCredentialsJsonConverter(bool includeCredentials)
    {
        _includeCredentials = includeCredentials;
    }

    public override KeyCredentials? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        string propertyDescription = nameof(KeyCredentials.Description);
        string propertyValue = nameof(KeyCredentials.Value);
        if (options.PropertyNamingPolicy != null)
        {
            propertyDescription = options.PropertyNamingPolicy.ConvertName(propertyDescription);
            propertyValue = options.PropertyNamingPolicy.ConvertName(propertyValue);
        }
        var stringComparison = options.PropertyNameCaseInsensitive ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

        KeyCredentialsDescription? description = null;
        byte[]? credentials = null;
        KeyCredentialsValue? value = null;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (description == null)
                {
                    throw new JsonException();
                }
                if (value != null)
                {
                    return new KeyCredentials(description, value);
                }
                else
                {
                    return new KeyCredentials(description, credentials ?? []);
                }
            }
            // Get the key.
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            string? propertyName = reader.GetString();

            if (string.Equals(propertyName, propertyDescription, stringComparison))
            {
                // Get the value.
                reader.Read();
                description = JsonSerializer.Deserialize<KeyCredentialsDescription>(ref reader, options);
            }
            else if (string.Equals(propertyName, propertyValue, stringComparison))
            {
                // Get the value.
                reader.Read();
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var json = JsonDocument.ParseValue(ref reader);
                    value = JsonSerializer.Deserialize<KeyCredentialsValue>(json, options);
                }
                else
                {
                    credentials = reader.GetBytesFromBase64();
                }
            }
            // else ignore
        }
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, KeyCredentials value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(KeyCredentials.Description)) ?? nameof(KeyCredentials.Description));
        writer.WriteRawValue(JsonSerializer.Serialize(value.Description, options));
        if (_includeCredentials)
        {
            KeyCredentialsValue? v = null;
            try
            {
                v = value.ToKeyCredentialsValue(null);
            }
            catch { }
            if (v != null)
            {
                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(KeyCredentials.Value)) ?? nameof(KeyCredentials.Value));
                writer.WriteRawValue(JsonSerializer.Serialize(v, options));
            }
            else
            {
                writer.WriteBase64String(nameof(KeyCredentials.Value), value.Value);
            }
        }
        writer.WriteEndObject();
    }
}