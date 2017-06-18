using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.Data.Entities.Constants;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    public sealed class ConstantsController : ApiController
    {
        /// <summary>Obtain constant list</summary>
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
        /// <param name="text">Constant description</param>
        /// <param name="type">Constant type</param>
        /// <param name="value">Constant value</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("constant/new")]
        [ResponseType(typeof(Constant))]
        public IHttpActionResult CreateNewConstant(string text, ConstantType type, float value)
        {
            return Ok(UserContext.Constants.NewConstant(text, type, value));
        }

        /// <summary>Edit existing constant</summary>
        /// <param name="text">Constant description</param>
        /// <param name="type">Constant type</param>
        /// <param name="value">Constant value</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("constant/edit")]
        [ResponseType(typeof(Constant))]
        public IHttpActionResult EditConstant(string text, ConstantType type, float value)
        {
            return Ok(UserContext.Constants.EditConstant(text, type, value));
        }

        /// <summary>Delete existing constant</summary>
        /// <param name="type">Constant type</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [DeusAuthorize]
        [HttpPost]
        [Route("constant/delete")]
        public IHttpActionResult DeleteConstant(ConstantType type)
        {
            UserContext.Constants.DeleteConstant(type);
            return Ok();
        }
    }
}
