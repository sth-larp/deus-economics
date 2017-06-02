using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.Exceptions;

namespace DeusCloud.Api.Controllers
{
    [RoutePrefix("error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : System.Web.Http.ApiController
    {
        [HttpGet]
        [Route("notfound")]
        public IHttpActionResult _NotFound()
        {
            throw new DeusHttpException(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("forbidden")]
        public IHttpActionResult Forbidden()
        {
            throw new DeusHttpException(HttpStatusCode.Forbidden);
        }

        [HttpGet]
        [Route("unauthorized")]
        public IHttpActionResult Unauthorized()
        {
            throw new DeusHttpException(HttpStatusCode.Unauthorized);
        }

    }

}