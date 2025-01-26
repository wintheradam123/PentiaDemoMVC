using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    // Dates given can be in either format
    private const string DateFormat1 = "dd-MM-yyyy HH:mm";
    private const string DateFormat2 = "yyyy-MM-dd HH:mm";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        // Try both
        if (DateTime.TryParseExact(dateString, DateFormat1, null, System.Globalization.DateTimeStyles.None, out DateTime date))
        {
            return date;
        }
        if (DateTime.TryParseExact(dateString, DateFormat2, null, System.Globalization.DateTimeStyles.None, out date))
        {
            return date;
        }
        throw new FormatException($"String '{dateString}' was not recognized as a valid DateTime.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat2));
    }
}
