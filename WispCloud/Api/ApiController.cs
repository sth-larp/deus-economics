using System.Web.Http.Results;
using DeusCloud.BasicAuth;
using DeusCloud.Logic;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Api
{
    public class ApiController : System.Web.Http.ApiController, IContextHolder
    {
        public UserContext UserContext { get { return Request.GetUserContext(); } }
        
        protected override OkResult Ok()
        {
            return new OkJsonPatchResult(this);
        }
    }

}