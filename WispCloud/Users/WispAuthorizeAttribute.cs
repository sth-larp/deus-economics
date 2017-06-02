using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WispCloud
{
    public class WispAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            throw new WispHttpException(HttpStatusCode.Unauthorized);
        }

    }

}