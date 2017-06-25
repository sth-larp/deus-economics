using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.BasicAuth;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Logic.Client;
using DeusCloud.Logic.Server;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    public sealed class AccountsController : ApiController
    {
        /// <summary>Получить профиль аккаунта</summary>
        /// <param name="login">User login</param>
        /// <returns>Person profile</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [BasicAuth]
        [HttpGet]
        [Route("accounts/profile")]
        [ResponseType(typeof(Account))]
        public IHttpActionResult Get(string login)
        {
            return Ok(UserContext.Accounts.GetProfile(login));
        }

        /// <summary>Получить профиль аккаунта с историей событий</summary>
        /// <param name="login">User login</param>
        /// <returns>Person profile with history</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [BasicAuth]
        [HttpGet]
        [Route("accounts/fullprofile")]
        [ResponseType(typeof(FullAccountServerData))]
        public IHttpActionResult GetFull(string login)
        {
            return Ok(UserContext.Accounts.GetFullProfile(login));
        }

        /// <summary>Зарегистрировать аккаунт</summary>
        /// <param name="clientData">Registration data</param>
        /// <returns>New user profile</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("accounts/register")]
        [ResponseType(typeof(Account))]
        public IHttpActionResult Registration(RegistrationClientData clientData)
        {
            return Ok(UserContext.Accounts.Registration(clientData));
        }

        /// <summary>Сменить пароль</summary>
        /// <param name="clientData">Change passsword data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [BasicAuth]
        [HttpPost]
        [Route("accounts/changepassword")]
        public IHttpActionResult ChangePassword(ChangePasswordClientData clientData)
        {
            UserContext.Accounts.ChangePassword(clientData);
            return Ok();
        }


        /// <summary>Установить свойства аккаунта</summary>
        /// <param name="clientData">Login and roles</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [BasicAuth]
        [HttpPost]
        [Route("accounts/properties")]
        [ResponseType(typeof(Account))]
        public IHttpActionResult SetAccountProperties(AccPropertyClientData clientData)
        {
            return Ok(UserContext.Rights.SetAccountProperties(clientData));
        }

        /// <summary>Установить индекс аккаунта</summary>
        /// <param name="data">Index data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [BasicAuth]
        [HttpPost]
        [Route("accounts/setindex")]
        [ResponseType(typeof(Account))]
        public IHttpActionResult SetIndex(AccIndexClientData data)
        {
            return Ok(UserContext.Rights.SetAccountIndex(data));
        }

        /// <summary>Get all accounts</summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [BasicAuth]
        [HttpGet]
        [Route("accounts/list")]
        [ResponseType(typeof(List<Account>))]
        public IHttpActionResult GetAccountList()
        {
            return Ok(UserContext.Accounts.GetAccountList());
        }
    }
}
