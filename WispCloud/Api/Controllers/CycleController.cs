using System.Web.Http;
using DeusCloud.BasicAuth;
using DeusCloud.Logic.Client;

namespace DeusCloud.Api.Controllers
{
    [BasicAuth]
    [DeusValidateModel]
    [DeusTraceLogging]
    public sealed class CycleController : ApiController
    {
        /// <summary>Сменить цикл, задав корпорациям новые значения индекса</summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("cycle/new")]
        public IHttpActionResult NewCycle(SwitchCycleClientData data)
        {
            UserContext.Constants.NewCycle();
            UserContext.Insurances.SwitchCycle(data);
            UserContext.Payments.SwitchCycle();
            return Ok();
        }
    }
}