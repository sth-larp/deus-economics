using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using log4net;

namespace DeusCloud.Api
{
    public sealed class DeusTraceLoggingAttribute : ActionFilterAttribute
    {
        public ILog Logger { get; set; }

        public DeusTraceLoggingAttribute()
        {
            Logger = LogManager.GetLogger("LogFileAppender");
        }

        public override void OnActionExecuted(HttpActionExecutedContext ctx)
        {
            Log(ctx.Response);
        }

        public override void OnActionExecuting(HttpActionContext ctx)
        {
            Log(ctx.Request);
        }

        private void Log(HttpRequestMessage request)
        {
            var message = "request URL:{0} ;body: {1}";

            var url = request.RequestUri.ToString();
            var body = request.Content.ReadAsStringAsync().Result;
            Logger.DebugFormat(message, url, body);
        }

        private void Log(HttpResponseMessage response)
        {
            var message = "response body: {0}";
            string responseBody = null;
            if (response != null)
            {
                responseBody = response.Content?.ReadAsStringAsync().Result;
            }
            Logger.DebugFormat(message, responseBody ?? "(null)");
        }
    }
}