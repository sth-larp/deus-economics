using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;
using DeusCloud.Exceptions;

namespace DeusCloud.Api
{
    public class DeusAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            throw new DeusHttpException(HttpStatusCode.Unauthorized);
        }

    }

}