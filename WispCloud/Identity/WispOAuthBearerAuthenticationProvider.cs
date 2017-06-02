using Microsoft.Owin.Security.OAuth;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DeusCloud.Logic.CommonBase;
using WispCloud.Logic;

namespace WispCloud.Users
{
    public class WispOAuthBearerAuthenticationProvider : OAuthBearerAuthenticationProvider
    {
        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            var wispContext = context.GetContext();
            var identity = context.Ticket.Identity;

            var account = wispContext.Accounts.Get(identity.Name);
            if (account == null || !account.Active)
                throw new DeusHttpException(System.Net.HttpStatusCode.Unauthorized);

            var tokenSalt = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData);
            if (tokenSalt == null || tokenSalt.Value != account.TokenSalt.ToString())
                throw new DeusHttpException(System.Net.HttpStatusCode.Unauthorized);

            wispContext.SetCurrentUser(account, context.Request.GetAuthorization());
            context.Validated();

            return Task.CompletedTask;
        }

    }

}