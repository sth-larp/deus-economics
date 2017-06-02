using Microsoft.AspNet.SignalR;
using Microsoft.Owin;

namespace WispCloud.Users
{
    public static class AuthorizationExtensions
    {
        static string AuthorizationHeaderKey { get; }

        static AuthorizationExtensions()
        {
            AuthorizationHeaderKey = "Authorization";
        }

        public static string GetAuthorizationCore(string authorization)
        {
            if (string.IsNullOrEmpty(authorization))
                return null;

            var firstSpace = authorization.IndexOf(' ');
            if (firstSpace <= 0)
                return null;

            var tokenType = authorization.Substring(0, firstSpace).ToLower();
            var token = authorization.Substring(firstSpace + 1);

            return $"{tokenType} {token}";
        }

        public static string GetAuthorization(this IRequest request)
        {
            return GetAuthorizationCore(request.Headers[AuthorizationHeaderKey]);
        }

        public static string GetAuthorization(this IOwinRequest request)
        {
            return GetAuthorizationCore(request.Headers[AuthorizationHeaderKey]);
        }

    }

}