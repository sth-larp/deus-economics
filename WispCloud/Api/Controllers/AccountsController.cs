using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.Data.Entities;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Logic.Client;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    public sealed class AccountsController : ApiController
    {
        /// <summary>Get current user profile</summary>
        /// <returns>Person profile</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpGet]
        [Route("accounts/current")]
        [ResponseType(typeof(Account))]
        public IHttpActionResult Get()
        {
            return Ok(UserContext.Accounts.Get(UserContext.CurrentUser.Login));
        }

        /// <summary>Registers new user</summary>
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

        /// <summary>Change password</summary>
        /// <param name="clientData">Change passsword data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("accounts/changepassword")]
        public IHttpActionResult ChangePassword(ChangePasswordClientData clientData)
        {
            UserContext.Accounts.ChangePassword(clientData);
            return Ok();
        }


        /// <summary>Set properties for account</summary>
        /// <param name="clientData">Login and roles</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("accounts/properties")]
        [ResponseType(typeof(Account))]
        public IHttpActionResult SetAccountProperties(AccPropertyClientData clientData)
        {
            return Ok(UserContext.Rights.SetAccountProperties(clientData));
        }

        /// <summary>Get all accounts</summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpGet]
        [Route("accounts/list")]
        [ResponseType(typeof(List<Account>))]
        public IHttpActionResult GetAccountList()
        {
            return Ok(UserContext.Accounts.GetAccountList());
        }
    }
}
