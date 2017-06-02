using Microsoft.Owin;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DeusCloud.Helpers;
using DeusCloud.Serialization;

namespace WispCloud.Exceptions.Handlers
{
    public class AllExceptionsMiddleware : OwinMiddleware
    {
        bool _detailedErrors;

        public AllExceptionsMiddleware(OwinMiddleware next)
            : base(next)
        {
            this._detailedErrors = AppSettings.Is("detailedErrors");
        }

        public async override Task Invoke(IOwinContext context)
        {
            try
            {
                if (Next != null)
                    await Next.Invoke(context);
            }
            catch (Exception exception)
            {
                WriteException(context, exception);
            }
        }

        public void WriteException(IOwinContext context, Exception exception)
        {
            SetStatusCode(context, exception);
            if (!string.IsNullOrEmpty(exception.Message))
            {
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.Write(_detailedErrors ? GetDetailedErrorJsonBody(exception) : GetShortErrorJsonBody(exception));
            }
        }

        void SetStatusCode(IOwinContext context, Exception exception)
        {
            if (exception is DeusException)
            {
                var wispException = (exception as DeusException);
                if (exception is DeusHttpException)
                {
                    context.Response.StatusCode = (int)(wispException as DeusHttpException).Status;
                }
                else
                {
                    context.Response.StatusCode = (int)(wispException.Expected ?
                        HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        string GetDetailedErrorJsonBody(Exception exception)
        {
            var json = new StringWriter();
            WispJsonSerializer.DefaultSerializer.Serialize(json, exception);

            return json.ToString();
        }

        string GetShortErrorJsonBody(Exception exception)
        {
            var messageBuilder = new StringBuilder();
            var innerException = exception;
            while (innerException != null)
            {
                if (!string.IsNullOrEmpty(innerException.Message))
                {
                    if (messageBuilder.Length > 0)
                        messageBuilder.Append(' ');
                    messageBuilder.Append(innerException.Message);
                }

                innerException = innerException.InnerException;
            }

            return $"{{\"Message\":\"{messageBuilder}\"}}";
        }
    }

}