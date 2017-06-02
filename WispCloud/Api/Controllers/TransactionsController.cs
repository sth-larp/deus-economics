﻿using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.Transactions;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    public sealed class TransactionsController : ApiController
    {
        /// <summary>Transfer money between accounts</summary>
        /// <param name="sender">Money sender</param>
        /// <param name="receiver">Money receiver</param>
        /// <param name="amount">Amount</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("transfer")]
        public IHttpActionResult Transfer(string sender, string receiver, float amount)
        {
            UserContext.Transactions.Transfer(sender, receiver, amount);
            return Ok();
        }

        /// <summary>Obtain transaction history</summary>
        /// <param name="login">Transaction account</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("history")]
        [ResponseType(typeof(List<Transaction>))]
        public IHttpActionResult GetTransactionHistory(string login)
        {
            return Ok(UserContext.Transactions.GetHistory(login));
        }
    }
}
