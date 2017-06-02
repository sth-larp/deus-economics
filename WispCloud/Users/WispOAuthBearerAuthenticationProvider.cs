using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using WispCloud.Logic;

namespace WispCloud.Users
{
    public class WispOAuthBearerAuthenticationProvider : OAuthBearerAuthenticationProvider
    {
        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            var wispContext = context.GetWispContext();
            wispContext.SetIdentity(context);
            if (wispContext.CurrentUser == null || !wispContext.CurrentUser.Active)
                throw new WispHttpException(System.Net.HttpStatusCode.Unauthorized);

            context.Validated();
            return Task.FromResult(0);
        }

    }
}