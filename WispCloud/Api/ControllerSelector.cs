using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace DeusCloud.Api
{
    public class ControllerSelector : DefaultHttpControllerSelector
    {
        public ControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
#if DEBUG
            return base.SelectController(request);
#else
            try
            {
                return base.SelectController(request);
            }
            catch
            {
                throw new DeusHttpException(System.Net.HttpStatusCode.NotFound);
            }
#endif
        }

    }

}