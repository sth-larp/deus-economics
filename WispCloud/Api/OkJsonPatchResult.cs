using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace DeusCloud.Api
{
    /// <summary>
    /// Replaces empty content for empty Json content on Ok()
    /// See ApiController for usage
    /// </summary>
    public class OkJsonPatchResult : OkResult
    {
        readonly MediaTypeWithQualityHeaderValue acceptJson = new MediaTypeWithQualityHeaderValue("application/json");

        public OkJsonPatchResult(HttpRequestMessage request) : base(request) { }
        public OkJsonPatchResult(System.Web.Http.ApiController controller) : base(controller) { }

        public override Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var accept = Request.Headers.Accept;
            var jsonFormat = accept.Any(h => h.Equals(acceptJson));

            if (jsonFormat)
            {
                return Task.FromResult(ExecuteResult());
            }
            else
            {
                return base.ExecuteAsync(cancellationToken);
            }
        }

        public HttpResponseMessage ExecuteResult()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json"),
                RequestMessage = Request
            };
        }
    }
}