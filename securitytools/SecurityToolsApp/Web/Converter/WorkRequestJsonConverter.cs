
using System.Text.Json;
using System.Text.Json.Serialization;
using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Converter;
public class WorkRequestJsonConverter : JsonConverter<WorkRequest>
{
    public override WorkRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, WorkRequest value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(options.PropertyNamingPolicy?.ConvertName(nameof(WorkRequest.Kind)) ?? nameof(WorkRequest.Kind), value.Kind);
        writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(WorkRequest.NotBefore)) ?? nameof(WorkRequest.NotBefore));
        writer.WriteRawValue(JsonSerializer.Serialize(value.NotBefore, options));
        writer.WriteNumber(options.PropertyNamingPolicy?.ConvertName(nameof(WorkRequest.Timeout)) ?? nameof(WorkRequest.Timeout), value.Timeout.TotalMilliseconds);
        writer.WriteEndObject();
    }
}