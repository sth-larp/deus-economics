using DeusClient.ApiCommands.Accounts.Client;
using Microsoft.AspNet.SignalR.Client;
using RestSharp;
using RestSharp.Authenticators;

namespace DeusClient.Client
{
    public sealed class Authenticator : IAuthenticator
    {
        public Authorization AuthorizationData { get; }
        public string HeaderName { get; }
        public string HeaderValue { get; }

        public Authenticator(Authorization authorizationData)
        {
            this.AuthorizationData = authorizationData;

            this.HeaderName = "Authorization";
            this.HeaderValue = $"{AuthorizationData.token_type} {AuthorizationData.access_token}";
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddHeader(HeaderName, HeaderValue);
        }

        public void Authenticate(HubConnection connection)
        {
            connection.Headers[HeaderName] = HeaderValue;
        }

    }

}
