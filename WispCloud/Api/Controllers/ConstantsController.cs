using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.BasicAuth;
using DeusCloud.Data.Entities.Constants;
using DeusCloud.Logic.Client;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    [BasicAuth]
    public sealed class ConstantsController : ApiController
    {
        /// <summary>Получить список констант</summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("constant/list")]
        [ResponseType(typeof(List<Constant>))]
        public IHttpActionResult GetConstants()
        {
            return Ok(UserContext.Constants.GetConstants());
        }

        /// <summary>Create a new constant</summary>
        /// <param name="text">Описание</param>
        /// <param name="name">Имя</param>
        /// <param name="value">Значение</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("constant/new")]
        [ResponseType(typeof(Constant))]
        public IHttpActionResult CreateNewConstant(string text, string name, float value)
        {
            return Ok(UserContext.Constants.NewConstant(text, name, value));
        }

        /// <summary>Edit existing constant</summary>
        /// <param name="data">Данные</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("constant/edit")]
        [ResponseType(typeof(Constant))]
        public IHttpActionResult EditConstant(ConstantClientData data)
        {
            return Ok(UserContext.Constants.EditConstant(data));
        }

        /// <summary>Delete existing constant</summary>
        /// <param name="name">Имя</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("constant/delete")]
        public IHttpActionResult DeleteConstant(string name)
        {
            UserContext.Constants.DeleteConstant(name);
            return Ok();
        }
    }
}
