using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.BasicAuth;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Logic.Client;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    [BasicAuth]
    public sealed class PaymentsController : ApiController
    {
        /// <summary>Полный список регулярных платежей</summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("payments/all")]
        [ResponseType(typeof(List<Payment>))]
        public IHttpActionResult GetPayments()
        {
            return Ok(UserContext.Payments.GetAllPayments());
        }

        /// <summary>Список зарплат, которые платит компания</summary>
        /// <param name="login">Плательщик</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("payments/list")]
        [ResponseType(typeof(List<Payment>))]
        public IHttpActionResult GetPayments(string login)
        {
            return Ok(UserContext.Payments.GetPayments(login));
        }

        /// <summary>Список зарплат пользователя</summary>
        /// <param name="login">Получатель</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("payments/salaries")]
        [ResponseType(typeof(List<Payment>))]
        public IHttpActionResult GetSalaries(string login)
        {
            return Ok(UserContext.Payments.GetSalaries(login));
        }

        /// <summary>Создать регулярный платеж</summary>
        /// <param name="data">Payment data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("payments/new")]
        [ResponseType(typeof(Payment))]
        public IHttpActionResult CreateNewPayment(PaymentClientData data)
        {
            return Ok(UserContext.Payments.NewPayment(data));
        }

        /// <summary>Редактировать регулярный платеж</summary>
        /// <param name="id">Payment id</param>
        /// <param name="data">Payment data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("payments/edit")]
        [ResponseType(typeof(Payment))]
        public IHttpActionResult EditPayment(int id, PaymentClientData data)
        {
            return Ok(UserContext.Payments.EditPayment(id, data));
        }

        /// <summary>Удалить регулярный платеж</summary>
        /// <param name="id">Payment id</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("payments/delete")]
        public IHttpActionResult DeletePayment(int id)
        {
            UserContext.Payments.DeletePayment(id);
            return Ok();
        }
    }
}
