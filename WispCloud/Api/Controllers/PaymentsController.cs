using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Logic.Client;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    public sealed class PaymentsController : ApiController
    {
        /// <summary>Obtain regular payments list</summary>
        /// <param name="login">Payer</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpGet]
        [Route("payments/list")]
        [ResponseType(typeof(List<Payment>))]
        public IHttpActionResult GetPayments(string login)
        {
            return Ok(UserContext.Payments.GetPayments(login));
        }

        /// <summary>Create a new payment</summary>
        /// <param name="data">Payment data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("payments/new")]
        [ResponseType(typeof(Payment))]
        public IHttpActionResult CreateNewPayment(PaymentClientData data)
        {
            return Ok(UserContext.Payments.NewPayment(data));
        }

        /// <summary>Edit existing payment</summary>
        /// <param name="id">Payment id</param>
        /// <param name="data">Payment data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("payments/edit")]
        [ResponseType(typeof(Payment))]
        public IHttpActionResult EditPayment(int id, PaymentClientData data)
        {
            return Ok(UserContext.Payments.EditPayment(id, data));
        }

        /// <summary>Delete existing payment</summary>
        /// <param name="id">Payment id</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("payments/delete")]
        public IHttpActionResult DeletePayment(int id)
        {
            UserContext.Payments.DeletePayment(id);
            return Ok();
        }

        /// <summary>Perform ALL regular payments</summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("payments/perform")]
        public IHttpActionResult PerformPayments()
        {
            UserContext.Payments.Run();
            return Ok();
        }
    }
}
