using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Rolillness
{
    public class MyJsonSerializer : JsonSerializer
    {
        public static MyJsonSerializer DefaultSerializer { get; }

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

        static MyJsonSerializer()
        {
            DefaultSerializer = new MyJsonSerializer();
        }

        public MyJsonSerializer()
        {
            this.
            MissingMemberHandling = MissingMemberHandling.Ignore;
            NullValueHandling = NullValueHandling.Include;
            TypeNameHandling = TypeNameHandling.None;
            // Converters.Add(new StringEnumConverter());
        }
    }
}
