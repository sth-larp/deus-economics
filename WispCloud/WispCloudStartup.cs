using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Owin;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using DeusCloud.Api;
using DeusCloud.Exceptions.Handlers;
using DeusCloud.Helpers;
using DeusCloud.Serialization;
using DeusCloud.Swashbuckle;
using log4net;
using WispCloud;
using WispCloud.Api;
using WispCloud.Exceptions.Handlers;
using WispCloud.Users;

[assembly: OwinStartup(typeof(WispCloudStartup))]

namespace WispCloud
{
    public sealed class WispCloudStartup
    {
        string _rootUrl;
        bool _enableSwagger;
        bool _enableSwaggerUI;
        string _oauth2DefaultScope;
        string _oauth2AuthorizeEndpoint;
        string _oauth2TokenEndpoint;

        public WispCloudStartup()
        {
            this._rootUrl = AppSettings.Url("rootUrl");
            this._enableSwagger = AppSettings.Is("enableSwagger");
            this._enableSwaggerUI = AppSettings.Is("enableSwaggerUI");
            this._oauth2DefaultScope = "User";
            this._oauth2AuthorizeEndpoint = "/oauth2/authorize";
            this._oauth2TokenEndpoint = "/oauth2/token";
        }

        public void Configuration(IAppBuilder app)
        {
            log4net.Config.XmlConfigurator.Configure();
            LogManager.GetLogger("").Info("Server started");
            app.Use<AllExceptionsMiddleware>();
            ConfigureOAuth(app);
            ConfigureSignalR(app);
            ConfigureApi(app);
            LogManager.GetLogger("").Info("Server configured");
        }

        void ConfigureOAuth(IAppBuilder app)
        {
            var authorizationOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                AuthorizeEndpointPath = new PathString(_oauth2AuthorizeEndpoint),
                TokenEndpointPath = new PathString(_oauth2TokenEndpoint),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                Provider = new WispAuthorizationProvider(),
            };
            app.UseOAuthAuthorizationServer(authorizationOptions);

            var authenticationOptions = new OAuthBearerAuthenticationOptions()
            {
                Provider = new WispOAuthBearerAuthenticationProvider(),
            };
            app.UseOAuthBearerAuthentication(authenticationOptions);
        }

        void ConfigureSignalR(IAppBuilder app)
        {
            GlobalHost.DependencyResolver = new DefaultDependencyResolver();
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => WispJsonSerializer.DefaultSerializer);

            var configuration = new HubConfiguration()
            {
                EnableJavaScriptProxies = false,
                EnableJSONP = true,
            };
            app.MapSignalR("/signalr", configuration);

            GlobalHost.HubPipeline.RequireAuthentication();
        }

        void ConfigureApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.Formatters.Clear();
            config.Formatters.Add(new WispJsonMediaTypeFormatter());

            config.Services.Replace(typeof(IHttpControllerSelector), new DeusControllerSelector(config));
            config.Services.Replace(typeof(IHttpActionSelector), new DeusActionSelector());
            config.Services.Replace(typeof(IExceptionHandler), new PassthroughHandler());

            config.Filters.Add(new DeusValidateModelAttribute());

            config.MapHttpAttributeRoutes(new CommonPrefixProvider("api"));

            ConfigureSwashbuckle(app, config);

            app.UseWebApi(config);
        }

        void ConfigureSwashbuckle(IAppBuilder app, HttpConfiguration config)
        {
            if (!_enableSwagger)
                return;

            var swaggerConfig = config.EnableSwagger("swagger/docs/{apiVersion}", x =>
                {
                    x.CustomProvider(provider => new CachingSwaggerProvider(provider));

                    x.IncludeXmlComments($@"{AppDomain.CurrentDomain.BaseDirectory}\bin\WispCloud.XML");
                    x.MapType<decimal>(() => new Schema { type = "string", format = "\\d+" });
                    x.DescribeAllEnumsAsStrings();
                    x.DocumentFilter<AllPropertiesWritableFilter>();

                    x.RootUrl(request => $"{_rootUrl}");
                    x.Schemes(new[] { "http" });

                    x.OperationFilter(() => new AuthenticationFilter(_oauth2DefaultScope));
                    x.OAuth2("oauth2")
                        .Description("OAuth2 Implicit Grant")
                        .Flow("implicit")
                        .Scopes(scopes =>
                         {
                             scopes.Add(_oauth2DefaultScope, "Get acces to user data");
                         })
                        .AuthorizationUrl($"{_rootUrl}{_oauth2AuthorizeEndpoint}")
                        .TokenUrl($"{_rootUrl}{_oauth2TokenEndpoint}");

                    x.SingleApiVersion("v1", "Wisp cloud API")
                        .Description("Api for control and synchronize all Wisp devices and apps");
                });

            if (_enableSwaggerUI)
            {
                swaggerConfig.EnableSwaggerUi("swagger/sandbox/{*assetPath}", x =>
                    {
                        x.EnableOAuth2Support("swagger-ui-client-id", "swagger-ui-client-secret", "swagger-ui-realm", "Swagger UI");
                    });
            }
        }

    }

}