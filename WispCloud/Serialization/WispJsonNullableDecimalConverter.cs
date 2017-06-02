using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeusCloud.Serialization
{
    public class WispJsonNullableDecimalConverter : JsonConverter
    {
        WispJsonDecimalConverter _decimanConverter;

        public WispJsonNullableDecimalConverter()
        {
            _decimanConverter = new WispJsonDecimalConverter();
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal?));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var exactValue = (value as decimal?);

            if (value == null && !exactValue.HasValue)
                writer.WriteValue((string)null);

            _decimanConverter.WriteJson(writer, exactValue.Value, serializer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (!token.HasValues)
                return null;

            if (string.IsNullOrEmpty(token.ToObject<string>()))
                return null;

            return _decimanConverter.Parse(token, reader, objectType, existingValue, serializer);
        }

    }

}