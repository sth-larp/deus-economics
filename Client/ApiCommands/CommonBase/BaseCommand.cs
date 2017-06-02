using System;
using System.Threading.Tasks;
using DeusClient.ApiCommands.Accounts.Client;
using DeusClient.Client;
using RestSharp;

namespace DeusClient.ApiCommands.CommonBase
{
    public abstract class BaseCommand
    {
        public abstract string Name { get; }
        public abstract string Resource { get; }
        public abstract Method Method { get; }
        public abstract AccountRoles Roles { get; }
        public virtual int SortIndex { get { return 100; } }

        public virtual Type InputType { get { return null; } }
        public virtual Type OutputType { get { return null; } }

        public override string ToString()
        {
            return $"{Name} ({Resource})";
        }

        protected virtual object GetRequestBodyTemplate()
        {
            return null;
        }
        public string GetRequestBodyTemplateStr()
        {
            var obj = GetRequestBodyTemplate();
            if (obj == null)
                return string.Empty;

            return CloudClient.ApiSerializer.Serialize(obj);
        }
        protected virtual object GenerateBodyRequest()
        {
            return GetRequestBodyTemplate();
        }
        public string GenerateBodyRequestStr()
        {
            var obj = GenerateBodyRequest();
            if (obj == null)
                return string.Empty;

            return CloudClient.ApiSerializer.Serialize(obj);
        }

        public RestRequest CreateRequest(CloudClient client)
        {
            return client.CreateRequest(Resource, Method);
        }

        public async Task<CommandResponse> ExecuteRequestAsync(CloudClient client, RestRequest request)
        {
            var requestTask = client.ExecuteAsync(request);
            await requestTask;

            if (!requestTask.IsCompleted)
                throw requestTask.Exception;

            return new CommandResponse(requestTask.Result, OutputType);
        }

    }

}
