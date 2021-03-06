﻿using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.BasicAuth;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Logic.Client;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    [BasicAuth]
    public sealed class AccessController : ApiController
    {
        /// <summary>Установить роли доступа одного аккаунта к другому</summary>
        /// <param name="accessData">Access roles</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("accounts/access/set")]
        [ResponseType(typeof(AccountAccess))]
        public IHttpActionResult SetAccountAccesses(AccountAccessClientData accessData)
        {
            return Ok(UserContext.Rights.SetAccountAccess(accessData));
        }

        /// <summary>List of accounts having access to slave account</summary>
        /// <param name="slave">Slave account</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("accounts/access/masters")]
        [ResponseType(typeof(List<AccountAccess>))]
        public IHttpActionResult GetAccessMasters(string slave)
        {
            return Ok(UserContext.Rights.GetAccessMasters(slave));
        }

        /// <summary>List of accounts which master account can access</summary>
        /// <param name="master">Master account</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("accounts/access/slaves")]
        [ResponseType(typeof(List<AccountAccess>))]
        public IHttpActionResult GetAccessSlaves(string master)
        {
            return Ok(UserContext.Rights.GetAccessSlaves(master));
        }
    }
}
