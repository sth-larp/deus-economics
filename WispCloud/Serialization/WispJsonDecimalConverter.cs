using System;
using DeusCloud.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeusCloud.Serialization
{
    public class WispJsonDecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((Math.Truncate((decimal)value)).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            return Parse(token, reader, objectType, existingValue, serializer);
        }

        public object Parse(JToken token, JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (token.Type != JTokenType.String)
                    throw new DeusException("Can parse decimal only from string");

                var value = decimal.Parse(token.ToObject<string>());
                if (Math.Truncate(value) != value)
                    throw new DeusException("Decimal value not integer");

                return value;
            }
            catch (Exception exception)
            {
                var jsonTextReader = (reader as JsonTextReader);
                throw new JsonSerializationException(
                    $"Cant parse integer number from string; Path '{jsonTextReader.Path}', line {jsonTextReader.LineNumber}, position {jsonTextReader.LinePosition};",
                    exception);
            }
        }

    }

}