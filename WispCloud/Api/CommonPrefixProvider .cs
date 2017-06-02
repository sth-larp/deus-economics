using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace DeusCloud.Api
{
    public class CommonPrefixProvider : DefaultDirectRouteProvider
    {
        private readonly string _commonPrefix;

        public CommonPrefixProvider(string commonPrefix)
        {
            this._commonPrefix = commonPrefix;
        }

        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            var existingPrefix = base.GetRoutePrefix(controllerDescriptor);
            if (existingPrefix == null)
                return _commonPrefix;

            return $"{_commonPrefix}/{existingPrefix}";
        }

    }

}