using RestSharp;
using System;
using System.Threading.Tasks;

namespace WispCloudClient.ApiCommands
{
    public abstract class OutputCommand<Output> : BaseCommand, IOutputCommand<Output>
    {
        public override Type InputType { get { return null; } }
        public override Type OutputType { get { return typeof(Output); } }

    }

    public interface IOutputCommand<Output>
    {
    }

    public static class IOutputCommandExtensions
    {
        public static async Task<CommandResponse<Output>> ExecuteRequestExactAsync<Output>(this IOutputCommand<Output> iOutputCommand, CloudClient client, RestRequest request)
        {
            var requestTask = client.ExecuteAsync(request);
            await requestTask;

            if (!requestTask.IsCompleted)
                throw requestTask.Exception;

            return new CommandResponse<Output>(requestTask.Result);
        }
    }

}
