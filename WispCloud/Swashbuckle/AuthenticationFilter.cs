using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace DeusCloud.Swashbuckle
{
    public class AuthenticationFilter : IOperationFilter
    {
        string _defaultScope;

        public AuthenticationFilter(string defaultScope)
        {
            this._defaultScope = defaultScope;
        }

        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var needAuthorization = apiDescription.ActionDescriptor.GetFilterPipeline()
                .Select(filterInfo => filterInfo.Instance)
                .OfType<System.Web.Http.AuthorizeAttribute>()
                .Any();

            if (needAuthorization)
            {
                if (operation.security == null)
                    operation.security = new List<IDictionary<string, IEnumerable<string>>>();

                var oAuthRequirements = new Dictionary<string, IEnumerable<string>>
                {
                    { "oauth2", new [] { _defaultScope } }
                };

                operation.security.Add(oAuthRequirements);
            }
        }

    }

}