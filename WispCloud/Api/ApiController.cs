using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using DeusCloud.BasicAuth;
using DeusCloud.Logic;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Api
{
    public class ApiController : System.Web.Http.ApiController, IContextHolder
    {
        public UserContext UserContext { get { return Request.GetUserContext(); } }

        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override OkResult Ok()
        {
            return new OkJsonPatchResult(this);
        }
    }

}