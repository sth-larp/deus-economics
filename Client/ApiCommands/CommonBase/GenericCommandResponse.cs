using RestSharp;

namespace DeusClient.ApiCommands.CommonBase
{
    public class CommandResponse<ContentType> : CommandResponse
    {
        public new ContentType Content { get { return (ContentType)base.Content; } }

        public CommandResponse(IRestResponse rawResponse)
            : base(rawResponse, typeof(ContentType))
        {
        }

    }

}
