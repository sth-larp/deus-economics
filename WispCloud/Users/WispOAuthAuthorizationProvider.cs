using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WispCloud.Data;
using WispCloud.Logic;
using WispCloud.Templates;

namespace WispCloud.Users
{
    public sealed class WispAuthorizationProvider : OAuthAuthorizationServerProvider
    {
        ClaimsIdentity GetBearerIdentity(Account user)
        {
            var identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Login));
            return identity;
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            Uri uri;
            Try.Condition(Uri.TryCreate(context.RedirectUri, UriKind.Absolute, out uri), "Bad redirect uri;");
            context.Validated();
            return Task.FromResult(0);
        }

        public async override Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            var formData = await context.Request.ReadFormAsync();

            if (!formData.Any())
            {
                context.Response.RenderEmbeddedResource("WispCloud.Templates.Files.OAuth2AuthorizationForm.html");
                context.RequestCompleted();
                return;
            }

            var login = formData["login"];
            var password = formData["password"];

            var user = context.GetWispContext().Data.Accounts.FirstOrDefault(x => x.Login == login && x.Active);
            Try.Condition(user != null && user.ComparePasword(password), "The user name or password is incorrect;");

            var authentication = context.OwinContext.Authentication;
            authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            authentication.SignIn(GetBearerIdentity(user));

            context.RequestCompleted();
            return;
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult(0);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Try.Condition(!string.IsNullOrEmpty(context.UserName) && !string.IsNullOrEmpty(context.Password), "The user name or password is incorrect;");

            var user = context.GetWispContext().Data.Accounts.FirstOrDefault(x => x.Login == context.UserName && x.Active);
            Try.Condition(user != null && user.ComparePasword(context.Password), "The user name or password is incorrect;");

            context.Validated(GetBearerIdentity(user));

            return Task.FromResult(0);
        }

    }

}