using RestSharp;
using System;
using System.Net;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public class CommandResponse
    {
        public IRestResponse RawResponse { get; }
        public ShortError Error { get; }
        public object Content { get; }

        public ResponseStatus Status { get { return RawResponse.ResponseStatus; } }
        public HttpStatusCode StatusCode { get { return RawResponse.StatusCode; } }

        public bool IsSuccessful { get { return (Status == ResponseStatus.Completed && StatusCode == HttpStatusCode.OK); } }

        public CommandResponse(IRestResponse rawResponse, Type contentType)
        {
            this.RawResponse = rawResponse;
            this.Error = null;
            this.Content = null;

            if (this.Status != ResponseStatus.Completed)
                return;

            if (this.StatusCode == HttpStatusCode.OK)
            {
                if (contentType != null)
                    this.Content = CloudClient.ApiSerializer.Deserialize(this.RawResponse, contentType);
            }
            else
            {
                this.Error = (CloudClient.ApiSerializer.Deserialize(this.RawResponse, typeof(ShortError)) as ShortError);
            }
        }

    }

}
