using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.BasicAuth;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Logic.Client;
using DeusCloud.Logic.Server;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    [BasicAuth]
    public sealed class TransactionsController : ApiController
    {
        /// <summary>Перевести кредиты между счетами</summary>
        /// <param name="data">Transaction data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("transfer")]
        public IHttpActionResult Transfer(TransferClientData data)
        {
            UserContext.Transactions.Transfer(data);
            return Ok();
        }

        /// <summary>Купить имплант со списанием индекса</summary>
        /// <param name="data">Transaction data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("implant")]
        public IHttpActionResult TransferImplant(ImplantClientData data)
        {
            UserContext.Transactions.Implant(data);
            return Ok();
        }

        /// <summary>Get P2P tax value</summary>
        /// <param name="sender">Sender account</param>
        /// <param name="receiver">Receiver account</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("tax")]
        [ResponseType(typeof(float))]
        public IHttpActionResult GetTax(string sender, string receiver)
        {
            return Ok(UserContext.Transactions.GetTax(sender, receiver));
        }

        /// <summary>Obtain transaction history</summary>
        /// <param name="login">Account name</param>
        /// <param name="take">Number of transactions (optional)</param>
        /// <param name="skip">Offset (optional)</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("transactions")]
        [ResponseType(typeof(List<Transaction>))]
        public IHttpActionResult GetTransactionHistory(string login, int take = 30, int skip = 0)
        {
            return Ok(UserContext.Transactions.GetHistory(login, take, skip));
        }
    }
}
