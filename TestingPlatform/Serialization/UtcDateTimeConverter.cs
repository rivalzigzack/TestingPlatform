using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace TestingPlatform.Serialization;
public sealed class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String || !DateTimeOffset.TryParse(reader.GetString(),
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var value))
            throw new JsonException("Ожидается дата в формате ISO 8601.");
        return value.UtcDateTime;
    }
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value.ToUniversalTime());
}
