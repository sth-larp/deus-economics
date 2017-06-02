using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace DeusCloud.Swashbuckle
{
    public class AllPropertiesWritableFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            foreach (var definition in swaggerDoc.definitions)
                foreach (var property in definition.Value.properties)
                    property.Value.readOnly = null;
        }

    }

}