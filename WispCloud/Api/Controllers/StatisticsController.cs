using System.Web.Http;
using System.Web.Http.Description;
using DeusCloud.BasicAuth;
using DeusCloud.Logic.Server;

namespace DeusCloud.Api.Controllers
{
    [DeusValidateModel]
    [DeusTraceLogging]
    [BasicAuth]
    public sealed class StatisticsController : ApiController
    {
        /// <summary>Получить статистику из ALICE</summary>
        /// <param name="ingame">Только персонажи в игре (optional)</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("stat/alice")]
        [ResponseType(typeof(StatServerData))]
        public IHttpActionResult GetStatistics(bool ingame = true)
        {
            return Ok(UserContext.Stat.GetStatistics(ingame));
        }
    }
}