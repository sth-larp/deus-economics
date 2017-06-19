using System.Net.Http;
using DeusCloud.Logic;
using Microsoft.Owin;
using Microsoft.Owin.Security.Provider;

namespace DeusCloud.BasicAuth
{
    public static class HttpActionContextExtensions
    {
        public static UserContext GetUserContext(this HttpRequestMessage request)
        {
            return request.GetOwinContext().GetUserContext();
        }

        public static UserContext GetUserContext<TOptions>(this BaseContext<TOptions> baseContext)
        {
            return baseContext.OwinContext.GetUserContext();
        }

        public static UserContext GetUserContext(this IOwinContext owinContext)
        {
            const string wispContextKey = "basic.Context";

            var wispContext = owinContext.Get<UserContext>(wispContextKey);
            if (wispContext == null)
            {
                wispContext = new UserContext();
                owinContext.Set(wispContextKey, wispContext);
            }

            return wispContext;
        }
    }
}