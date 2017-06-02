using Microsoft.Owin;
using Microsoft.Owin.Security.Provider;
using System.Net.Http;

namespace WispCloud.Logic
{
    public static class GetWispContextExtensions
    {
        public static WispContext GetWispContext(this HttpRequestMessage request)
        {
            return request.GetOwinContext().GetWispContext();
        }

        public static WispContext GetWispContext<TOptions>(this BaseContext<TOptions> baseContext)
        {
            return baseContext.OwinContext.GetWispContext();
        }

        public static WispContext GetWispContext(this IOwinContext owinContext)
        {
            const string wispContextKey = "wisp.Context";

            var wispContext = owinContext.Get<WispContext>(wispContextKey);
            if (wispContext == null)
            {
                wispContext = new WispContext();
                owinContext.Set(wispContextKey, wispContext);
            }

            return wispContext;
        }

    }

}