using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.Data.Entities.Taxes;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Logic.Client;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    public sealed class TaxController : ApiController
    {
        /// <summary>Obtain tax list</summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("tax/list")]
        [ResponseType(typeof(List<Tax>))]
        public IHttpActionResult GetTaxes()
        {
            return Ok(UserContext.Taxation.GetTaxes());
        }

        /// <summary>Create a new tax</summary>
        /// <param name="text">Tax description</param>
        /// <param name="type">Tax type</param>
        /// <param name="value">Tax value</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("tax/new")]
        [ResponseType(typeof(Tax))]
        public IHttpActionResult CreateNewTax(string text, TaxType type, float value)
        {
            return Ok(UserContext.Taxation.NewTax(text, type, value));
        }

        /// <summary>Edit existing tax</summary>
        /// <param name="text">Tax description</param>
        /// <param name="type">Tax type</param>
        /// <param name="value">Tax value</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("tax/edit")]
        [ResponseType(typeof(Tax))]
        public IHttpActionResult EditTax(string text, TaxType type, float value)
        {
            return Ok(UserContext.Taxation.EditTax(text, type, value));
        }

        /// <summary>Delete existing tax</summary>
        /// <param name="type">Tax type</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("tax/delete")]
        public IHttpActionResult DeleteTax(TaxType type)
        {
            UserContext.Taxation.DeleteTax(type);
            return Ok();
        }
    }
}
