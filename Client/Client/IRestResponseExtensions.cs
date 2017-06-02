using RestSharp;

namespace WispCloudClient
{
    public static class IRestResponseExtensions
    {
        public static string GetClearContentType(this IRestResponse response)
        {
            if (string.IsNullOrEmpty(response.ContentType))
                return null;

            var clearContentType = response.ContentType;
            var indexOfContentTypeEnd = clearContentType.IndexOf(';');
            if (indexOfContentTypeEnd >= 0)
                clearContentType = clearContentType.Remove(indexOfContentTypeEnd);

            return clearContentType;
        }

    }

}
