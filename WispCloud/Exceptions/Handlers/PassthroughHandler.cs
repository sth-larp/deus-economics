using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace DeusCloud.Exceptions.Handlers
{
    public class PassthroughHandler : IExceptionHandler
    {
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var info = ExceptionDispatchInfo.Capture(context.Exception);
            info.Throw();

            return Task.FromResult(0);
        }

    }

}