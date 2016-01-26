using System;
using Newtonsoft.Json;

namespace GateScheduler.Bootstrap
{
    /// <summary>
    /// Formats TimeSpan objects as "h:mm" when serializing to/from JSON.
    /// </summary>
    internal class HoursMinutesNullableTimeSpanConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is TimeSpan)
            {
                writer.WriteValue(((TimeSpan)value).ToString(@"%h\:mm"));
                return;
            }

            var time = (TimeSpan?) value;
            if (time.HasValue)
            {
                writer.WriteValue(time.Value.ToString(@"%h\:mm"));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            string timeString = reader.Value.ToString();
            if (string.IsNullOrEmpty(timeString))
            {
                return null;
            }

            TimeSpan time;
            if (!TimeSpan.TryParse(timeString, out time))
            {
                throw new Exception(string.Format("Failed to deserialize TimeSpan from {0}", timeString));
            }
            return time;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan) || objectType == typeof(TimeSpan?);
        }
    }
}