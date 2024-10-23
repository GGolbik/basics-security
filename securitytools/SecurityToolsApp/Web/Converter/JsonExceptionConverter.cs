
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GGolbik.SecurityToolsApp.Web.Converter;

public class JsonExceptionConverter : JsonConverter<Exception>
{
    public override Exception? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(this.GetMessage(value));
        }
    }

    private string GetMessage(Exception? value)
    {
        StringBuilder builder = new();
        if (value == null)
        {
            return builder.ToString();
        }
        builder.Append(value.Message);
        if (value is AggregateException e)
        {
            foreach (var item in e.InnerExceptions)
            {
                builder.Append(" # ");
                builder.Append(this.GetMessage(item));
            }
        }
        else if (value.InnerException != null)
        {
            builder.Append(" # ");
            builder.Append(this.GetMessage(value.InnerException));
        }
        return builder.ToString();
    }
}
