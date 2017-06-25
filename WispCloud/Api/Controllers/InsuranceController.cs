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
    public sealed class InsuranceController : ApiController
    {
        /// <summary>Список всех компаний, обслуживающих страховки</summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("insurance/loyals")]
        [ResponseType(typeof(List<Loyalty>))]
        public IHttpActionResult GetLoyalties()
        {
            return Ok(UserContext.Insurances.GetLoyalties());
        }

        /// <summary>Список обслуживаемых компанией страховок</summary>
        /// <param name="login">Company's ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("insurance/loyalties")]
        [ResponseType(typeof(List<Loyalty>))]
        public IHttpActionResult GetCompanyLoyalties(string login)
        {
            return Ok(UserContext.Insurances.GetCompanyLoyalties(login));
        }

        /// <summary>Список персон, имеющих страховку</summary>
        /// <param name="company">Issuing company's ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("insurance/holders")]
        [ResponseType(typeof(List<InsuranceHolderServerData>))]
        public IHttpActionResult GetLoyaltyHolders(string company)
        {
            return Ok(UserContext.Insurances.GetLoyaltyHolders(company));
        }

        /// <summary>Отменить страховку у персоны</summary>
        /// <param name="user">User with Insurance</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("insurance/removeholder")]
        public IHttpActionResult RemoveInsuranceHolder(string user)
        {
            UserContext.Insurances.RemoveInsuranceHolder(user);
            return Ok();
        }

        /// <summary>Украсть страховку</summary>
        /// <param name="data">Steal insurance data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("insurance/steal")]
        public IHttpActionResult RemoveInsuranceHolder(StealInsuranceClientData data)
        {
            UserContext.Insurances.StealInsurance(data);
            return Ok();
        }

        /// <summary>Сменить/добавить страховку персоне</summary>
        /// <param name="data">Insurance data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("insurance/changeholder")]
        public IHttpActionResult ChangeInsuranceHolder(SetInsuranceClientData data)
        {
            UserContext.Insurances.SetInsuranceHolder(data);
            return Ok();
        }

        /// <summary>Добавить обслуживание страховки</summary>
        /// <param name="data">Insurance data</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("insurance/newloyalty")]
        [ResponseType(typeof(Loyalty))]
        public IHttpActionResult AddNewLoyalty(Loyalty data)
        {
            return Ok(UserContext.Insurances.NewLoyalty(data));
        }

        /// <summary>Удалить обслуживание страховки</summary>
        /// <param name="id">Loyalty id</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("insurance/deleteloyalty")]
        public IHttpActionResult DeleteLoyalty(int id)
        {
            UserContext.Insurances.DeleteLoyalty(id);
            return Ok();
        }
    }
}