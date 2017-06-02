using System.Net.Http.Formatting;
using Newtonsoft.Json;

namespace DeusCloud.Serialization
{
    public class WispJsonMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public override JsonSerializer CreateJsonSerializer()
        {
            return WispJsonSerializer.DefaultSerializer;
        }

    }

}