using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;

namespace WispCloud.Swashbuckle
{
    public class AssignOAuth2SecurityRequirements : IOperationFilter
    {
        string _defaultScope;

        public AssignOAuth2SecurityRequirements(string defaultScope)
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