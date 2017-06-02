using System.IO;
using Newtonsoft.Json;

namespace DeusCloud.Serialization
{
    public class WispJsonSerializer : JsonSerializer
    {
        public static WispJsonSerializer DefaultSerializer { get; }

        public static string SerializeToJsonString(object data)
        {
            if (data == null)
                return null;

            var stringWriter = new StringWriter();
            DefaultSerializer.Serialize(stringWriter, data);
            return stringWriter.ToString();
        }
        public static DataType DeserializeJson<DataType>(string data) where DataType : class
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var reader = new StringReader(data);
            return (DataType)DefaultSerializer.Deserialize(reader, typeof(DataType));
        }

        static WispJsonSerializer()
        {
            DefaultSerializer = new WispJsonSerializer();
        }

        public WispJsonSerializer()
        {
            this.
            MissingMemberHandling = MissingMemberHandling.Ignore;
            NullValueHandling = NullValueHandling.Include;
            TypeNameHandling = TypeNameHandling.None;
            Converters.Add(new WispJsonDecimalConverter());
            Converters.Add(new WispJsonNullableDecimalConverter());
            Converters.Add(new JsonIntolerantEnumConverter());
           // Converters.Add(new StringEnumConverter());
        }
    }
}