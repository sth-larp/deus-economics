using Microsoft.Owin;
using RazorEngine;
using RazorEngine.Templating;
using System.IO;
using System.Net;
using System.Reflection;

namespace WispCloud.Templates
{
    public static class TemplateRenderer
    {
        public static string GetEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        public static void RenderEmbeddedResource(this IOwinResponse response, string resourceName)
        {
            response.Write(GetEmbeddedResource(resourceName));
            response.ContentType = MimeTypes.GetMimeType(resourceName);
            response.StatusCode = (int)HttpStatusCode.OK;
        }

        public static string RenderTemplate<ModelType>(string templateResourceName, ModelType model)
        {
            var modelType = typeof(ModelType);
            var key = $"{templateResourceName} {modelType.FullName}";

            if (!Engine.Razor.IsTemplateCached(key, modelType))
                Engine.Razor.Compile(GetEmbeddedResource(templateResourceName), key, modelType);

            return Engine.Razor.Run(key, modelType, model);
        }

        public static void RenderEmbeddedTemplate<ModelType>(this IOwinResponse response, string resourceName, ModelType model)
        {
            response.Write(RenderTemplate(resourceName, model));
            response.ContentType = MimeTypes.GetMimeType(resourceName);
            response.StatusCode = (int)HttpStatusCode.OK;
        }

    }

}