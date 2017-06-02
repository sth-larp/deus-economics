using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;
using WispCloud.Data;
using WispCloud.Logic;
using WispCloud.Templates;

namespace WispCloud.Users
{
    public sealed class WispAuthorizationProvider : OAuthAuthorizationServerProvider
    {
        static string LoginOrPasswordIncorrectMessage { get; }
        static string UserInactiveMessage { get; }

        static WispAuthorizationProvider()
        {
            LoginOrPasswordIncorrectMessage = "The login or password is incorrect;";
            UserInactiveMessage = "User inactive;";
        }

        ClaimsIdentity GetBearerIdentity(Account user)
        {
            var identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Login));
            identity.AddClaim(new Claim(ClaimTypes.UserData, user.TokenSalt.ToString()));

            return identity;
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            Uri uri;
            Try.Condition(Uri.TryCreate(context.RedirectUri, UriKind.Absolute, out uri), "Bad redirect uri;");

            context.Validated();

            return Task.CompletedTask;
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

            Try.Condition(!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password),
                LoginOrPasswordIncorrectMessage);

            var user = context.GetContext().Accounts.Get(login, password);
            Try.NotNull(user, LoginOrPasswordIncorrectMessage);
            Try.Condition(user.Active, UserInactiveMessage);

            var authentication = context.OwinContext.Authentication;
            authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            authentication.SignIn(GetBearerIdentity(user));

            context.RequestCompleted();

            return;
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();

            return Task.CompletedTask;
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Try.Condition(!string.IsNullOrEmpty(context.UserName) && !string.IsNullOrEmpty(context.Password),
                LoginOrPasswordIncorrectMessage);

            var user = context.GetContext().Accounts.Get(context.UserName, context.Password);
            Try.NotNull(user, LoginOrPasswordIncorrectMessage);
            Try.Condition(user.Active, UserInactiveMessage);

            context.Validated(GetBearerIdentity(user));

            return Task.CompletedTask;
        }

    }

}