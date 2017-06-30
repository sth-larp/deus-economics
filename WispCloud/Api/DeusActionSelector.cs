using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using DeusCloud.Exceptions;

namespace DeusCloud.Api
{
    public class DeusActionSelector : ApiControllerActionSelector
    {
        public override HttpActionDescriptor SelectAction(HttpControllerContext context)
        {
            //Some stupid workaround
            //This should normally work the other way
            if (context.Request.Method.Method.Equals("OPTIONS"))
            {
                var method = context.ControllerDescriptor.ControllerType
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(m => m.Name == "Options");
                return new ReflectedHttpActionDescriptor(context.ControllerDescriptor, method);
            }
#if DEBUG
            return base.SelectAction(context);
#else
            try
            {
                return base.SelectAction(context);
            }
            catch
            {
                throw new DeusHttpException(System.Net.HttpStatusCode.NotFound);
            }
#endif
        }

    }

}