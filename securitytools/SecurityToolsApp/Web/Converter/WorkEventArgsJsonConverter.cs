
using System.Text.Json;
using System.Text.Json.Serialization;
using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Converter;
public class WorkEventArgsJsonConverter : JsonConverter<WorkEventArgs>
{
    public override WorkEventArgs? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, WorkEventArgs value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(options.PropertyNamingPolicy?.ConvertName(nameof(WorkEventArgs.Id)) ?? nameof(WorkEventArgs.Id), value.Id);
        writer.WriteString(options.PropertyNamingPolicy?.ConvertName(nameof(WorkEventArgs.Kind)) ?? nameof(WorkEventArgs.Kind), value.Kind);
        writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(WorkEventArgs.Status)) ?? nameof(WorkEventArgs.Status));

        writer.WriteRawValue(JsonSerializer.Serialize(value.Status, options));
        writer.WriteEndObject();
    }
}