using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;

namespace DeusCloud.BasicAuth
{
    public class BasicAuthAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                // Gets header parameters  
                string authenticationString = actionContext.Request.Headers.Authorization.Parameter;
                string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));

                // Gets username and password  
                string username = originalString.Split(':')[0];
                string password = originalString.Split(':')[1];

                var userContext = actionContext.Request.GetOwinContext().GetUserContext();

                var account = userContext.Accounts.Get(username, password);
                if (account == null || account.Status != AccountStatus.Active)
                    throw new DeusHttpException(HttpStatusCode.Unauthorized);
                
                userContext.SetCurrentUser(account);
            }

            base.OnAuthorization(actionContext);
        }
    }
}