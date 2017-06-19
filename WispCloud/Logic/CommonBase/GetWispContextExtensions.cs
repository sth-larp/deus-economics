using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.Provider;

namespace DeusCloud.Logic.CommonBase
{
    /*public static class GetWispContextExtensions
    {
        public static UserContext GetContext(this HttpRequestMessage request)
        {
            return request.GetOwinContext().GetContext();
        }

        public static UserContext GetContext<TOptions>(this BaseContext<TOptions> baseContext)
        {
            return baseContext.OwinContext.GetContext();
        }

        public static UserContext GetContext(this IOwinContext owinContext)
        {
            const string wispContextKey = "wisp.Context";

            var wispContext = owinContext.Get<UserContext>(wispContextKey);
            if (wispContext == null)
            {
                wispContext = new UserContext();
                owinContext.Set(wispContextKey, wispContext);
            }

            return wispContext;
        }
    }*/

}