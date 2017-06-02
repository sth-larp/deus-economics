using System.Collections.Concurrent;
using Swashbuckle.Swagger;

namespace DeusCloud.Swashbuckle
{
    public class CachingSwaggerProvider : ISwaggerProvider
    {
        static ConcurrentDictionary<string, SwaggerDocument> _cache;

        static CachingSwaggerProvider()
        {
            _cache = new ConcurrentDictionary<string, SwaggerDocument>();
        }

        ISwaggerProvider _swaggerProvider;

        public CachingSwaggerProvider(ISwaggerProvider swaggerProvider)
        {
            this._swaggerProvider = swaggerProvider;
        }

        public SwaggerDocument GetSwagger(string rootUrl, string apiVersion)
        {
            var cacheKey = $"{rootUrl} {apiVersion}";
            return _cache.GetOrAdd(cacheKey, (key) => _swaggerProvider.GetSwagger(rootUrl, apiVersion));
        }
    }
}