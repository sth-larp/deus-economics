using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.IO;

namespace WispCloudClient
{
    public sealed class ApiSerializer : IDeserializer, ISerializer
    {
        public Newtonsoft.Json.JsonSerializer Serializer { get; private set; }

        public ApiSerializer()
        {
            this.Serializer = new Newtonsoft.Json.JsonSerializer();
            this.Serializer.Formatting = Formatting.Indented;
            this.Serializer.Converters.Add(new StringEnumConverter());
        }

        public string ContentType
        {
            get { return "application/json"; }
            set { }
        }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    Serializer.Serialize(jsonTextWriter, obj);
                    return stringWriter.ToString();
                }
            }
        }

        public T Deserialize<T>(IRestResponse response)
        {
            var content = response.Content;
            using (var stringReader = new StringReader(content))
            using (var jsonTextReader = new JsonTextReader(stringReader))
                return Serializer.Deserialize<T>(jsonTextReader);
        }

        public object Deserialize(IRestResponse response, Type contentType)
        {
            var content = response.Content;
            using (var stringReader = new StringReader(content))
            using (var jsonTextReader = new JsonTextReader(stringReader))
                return Serializer.Deserialize(jsonTextReader, contentType);
        }

    }

}
