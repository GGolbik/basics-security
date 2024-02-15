
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GGolbik.SecurityTools.Models;

public class SchemaVersionJsonConverter : JsonConverter<SchemaVersion?>
{
    public override SchemaVersion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String && reader.TokenType != JsonTokenType.Null)
        {
            throw new JsonException(string.Format(
                "Failed to parse input to {0}. Expected input to be of type {1} but is {2}.",
                nameof(SchemaVersion),
                JsonTokenType.String,
                reader.TokenType
            ));
        }
        string? input = null;
        try
        {
            input = reader.GetString();
            return SchemaVersion.Parse(input);
        }
        catch (Exception e)
        {
            throw new JsonException(string.Format("Failed to parse '{0}' to {1}.", input, nameof(SchemaVersion)), e);
        }
    }

    public override void Write(Utf8JsonWriter writer, SchemaVersion? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString());
    }
}

/// <summary>
/// Represents a version of a schema.
/// </summary>
[JsonConverter(typeof(SchemaVersionJsonConverter))]
public sealed class SchemaVersion
{
    /// <summary>
    /// The major version of the schema.
    /// A change of the major version indicates incompatible changes.
    /// </summary>
    public uint Major { get; }

    /// <summary>
    /// The minor version of the schema.
    /// A change of the minor version indicates additional functionality in a backward compatible manner.
    /// </summary>
    public uint Minor { get; }

    /// <summary>
    /// Creates a new instance of a <see cref="SchemaVersion"/>.
    /// </summary>
    /// <param name="major">The major version of the schema.</param>
    /// <param name="minor">The minor version of the schema.</param>
    public SchemaVersion(uint major, uint minor = 0)
    {
        this.Major = major;
        this.Minor = minor;
    }

    public override string ToString()
    {
        return String.Format("{0}.{1}", this.Major, this.Minor);
    }

    /// <summary>
    /// Parses a string to an instance of <see cref="SchemaVersion"/>.
    /// </summary>
    /// <param name="input">The string to parse.</param>
    /// <returns>The version or null.</returns>
    /// <exception cref="ArgumentException">If input is an unrecognized format.</exception>
    public static SchemaVersion? Parse(string? input)
    {
        if (input == null)
        {
            return null;
        }
        var values = input.Split('.');
        if (values.Length > 2)
        {
            throw new ArgumentException(String.Format("Unrecognized format for {0}. Expect [major].[minor] but is {1}", nameof(SchemaVersion), input), nameof(input));
        }
        var major = UInt32.Parse(values[0]);
        uint minor = 0;
        if (values.Length > 1)
        {
            minor = UInt32.Parse(values[1]);
        }
        return new SchemaVersion(major, minor);
    }

    public static implicit operator string?(SchemaVersion? version) => version?.ToString();
    public static implicit operator SchemaVersion?(string? version) => Parse(version);
}