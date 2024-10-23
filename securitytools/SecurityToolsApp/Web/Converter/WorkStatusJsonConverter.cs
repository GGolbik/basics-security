
using System.Text.Json;
using System.Text.Json.Serialization;
using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsApp.Web.Converter;
public class WorkStatusJsonConverter : JsonConverter<WorkStatus>
{
    private readonly JsonExceptionConverter _exceptionConverter = new();

    public override WorkStatus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, WorkStatus value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(WorkStatus.State)) ?? nameof(WorkStatus.State));
        writer.WriteRawValue(JsonSerializer.Serialize(value.State, options));
        writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(WorkStatus.Enqueued)) ?? nameof(WorkStatus.Enqueued));
        writer.WriteRawValue(JsonSerializer.Serialize(value.Enqueued, options));
        writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(WorkStatus.ExecutionStart)) ?? nameof(WorkStatus.ExecutionStart));
        writer.WriteRawValue(JsonSerializer.Serialize(value.ExecutionStart, options));
        writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(WorkStatus.ExecutionEnd)) ?? nameof(WorkStatus.ExecutionEnd));
        writer.WriteRawValue(JsonSerializer.Serialize(value.ExecutionEnd, options));
        if(value.ExecutionDuration == null)
        {
            writer.WriteNull(options.PropertyNamingPolicy?.ConvertName(nameof(WorkStatus.ExecutionDuration)) ?? nameof(WorkStatus.ExecutionDuration));
        }
        else
        {
            writer.WriteNumber(options.PropertyNamingPolicy?.ConvertName(nameof(WorkStatus.ExecutionDuration)) ?? nameof(WorkStatus.ExecutionDuration), value.ExecutionDuration?.TotalMilliseconds ?? 0);
        }
        writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(WorkStatus.Error)) ?? nameof(WorkStatus.Error));
        if(value.Error == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            _exceptionConverter.Write(writer, value.Error, options);
        }
        writer.WriteEndObject();
    }
}