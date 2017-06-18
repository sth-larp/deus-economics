using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Logic.Client;
using DeusCloud.Logic.Server;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    public sealed class LoyaltyController : ApiController
    {
        /// <summary>Список всех компаний, работающих по страховкам</summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpGet]
        [Route("insurance/all")]
        [ResponseType(typeof(List<Loyalty>))]
        public IHttpActionResult GetLoyalties()
        {
            return Ok(UserContext.Loyalties.GetLoyalties());
        }

        /// <summary>Список страховок, которые обслуживает компания</summary>
        /// <param name="login">Company's ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpGet]
        [Route("insurance/list")]
        [ResponseType(typeof(List<Loyalty>))]
        public IHttpActionResult GetCompanyLoyalties(string login)
        {
            return Ok(UserContext.Loyalties.GetCompanyLoyalties(login));
        }

        /// <summary>Список персон, имеющих страховку</summary>
        /// <param name="company">Issuing company's ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpGet]
        [Route("insurance/holders")]
        [ResponseType(typeof(List<InsuranceHolderServerData>))]
        public IHttpActionResult GetLoyaltyHolders(string company)
        {
            return Ok(UserContext.Loyalties.GetLoyaltyHolders(company));
        }

        /// <summary>Отменить страховку у персоны</summary>
        /// <param name="company">Issuing company's ID</param>
        /// <param name="user">User with Insurance</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("insurance/removeholder")]
        public IHttpActionResult RemoveLoyaltyHolder(string company, string user)
        {
            UserContext.Loyalties.RemoveLoyaltyHolder(company, user);
            return Ok();
        }

        /// <summary>Добавить обслуживание страховки</summary>
        /// <param name="data">Insurance data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("insurance/newrelation")]
        [ResponseType(typeof(Loyalty))]
        public IHttpActionResult AddNewLoyalty(Loyalty data)
        {
            return Ok(UserContext.Loyalties.NewLoyalty(data));
        }

        /// <summary>Удалить обслуживание страховки</summary>
        /// <param name="id">Loyalty id</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpDelete]
        [Route("insurance/deleterelation")]
        public IHttpActionResult DeleteLoyalty(int id)
        {
            UserContext.Loyalties.DeleteLoyalty(id);
            return Ok();
        }
    }
}