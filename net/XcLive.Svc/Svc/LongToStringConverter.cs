using System.Text.Json;
using System.Text.Json.Serialization;
using SchemaBuilder.Svc.Core.Ext;

namespace SchemaBuilder.Svc.Svc;

public class LongToStringConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var val = reader.GetString();
        if (val.IsNullOrWhiteSpace())
        {
            return 0;
        }

        return long.Parse(val);
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

public class NullableLongToStringConverter : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string stringValue = reader.GetString();
        return stringValue.IsNullOrEmpty() ? (long?)null : long.Parse(stringValue);
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}

public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format;

    public JsonDateTimeConverter(string format)
    {
        _format = format;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString(), _format, null);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_format));
    }
}

public class JsonNullableDateTimeConverter : JsonConverter<DateTime?>
{
    private readonly string _format;

    public JsonNullableDateTimeConverter(string format)
    {
        _format = format;
    }

    // 反序列化：从指定格式的字符串或 null 转换为可空 DateTime
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null; // 如果是 null，则直接返回 null
        }

        var dateString = reader.GetString();
        if (DateTime.TryParseExact(dateString, _format, null, System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }

        throw new JsonException($"Unable to convert \"{dateString}\" to DateTime using format {_format}.");
    }

    // 序列化：将 DateTime? 转换为指定格式的字符串或 null
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString(_format)); // 如果有值，则按指定格式写入
        }
        else
        {
            writer.WriteNullValue(); // 如果是 null，则写入 JSON 的 null
        }
    }
}