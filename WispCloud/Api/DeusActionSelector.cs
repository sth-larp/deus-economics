using System.Web.Http.Controllers;
using DeusCloud.Exceptions;

namespace DeusCloud.Api
{
    public class DeusActionSelector : ApiControllerActionSelector
    {
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
#if DEBUG
            return base.SelectAction(controllerContext);
#else
            try
            {
                return base.SelectAction(controllerContext);
            }
            catch
            {
                throw new DeusHttpException(System.Net.HttpStatusCode.NotFound);
            }
#endif
        }

    }

}