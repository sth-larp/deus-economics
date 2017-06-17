using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.Data.Entities.Transactions;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    public sealed class LoyaltyController : ApiController
    {
        /// <summary>Get list of all loyal services</summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpGet]
        [Route("loyalties/list")]
        [ResponseType(typeof(List<Loyalty>))]
        public IHttpActionResult GetLoyalties()
        {
            return Ok(UserContext.Loyalties.GetLoyalties());
        }

        /// <summary>Add a new insurance loyalty relation</summary>
        /// <param name="data">Insurance data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("loyalties/new")]
        [ResponseType(typeof(Loyalty))]
        public IHttpActionResult AddNewLoyalty(Loyalty data)
        {
            return Ok(UserContext.Loyalties.NewLoyalty(data));
        }

        /// <summary>Delete existing insurance loyalty relation</summary>
        /// <param name="id">Loyalty id</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpDelete]
        [Route("loyalties/delete")]
        public IHttpActionResult DeleteLoyalty(int id)
        {
            UserContext.Loyalties.DeleteLoyalty(id);
            return Ok();
        }
    }
}