using System.Web.Http;
using System.Web.Http.Description;
using WispCloud.Logic;

namespace WispCloud.Api.Controllers
{
    [WispValidateModel]
    [WispAuthorize]
    [RoutePrefix("installations/{installationID:long}/groups")]
    public class GroupsController : WispApiController
    {
        /// <summary>
        /// Get groups and power bars hierarchy
        /// </summary>
        /// <remarks>Get groups and power bars hierarchy. Hierarchy also contains all power bars(and it only one way to get it).</remarks>
        /// <param name="installationID">Installation ID</param>
        /// <returns>Groups and power bars hierarchy</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(TopGroup))]
        public IHttpActionResult Get(long installationID)
        {
            WispContext.Rights.CheckInstallationAceesible(installationID);
            return Ok(WispContext.Groups.Get(installationID));
        }

        /// <summary>
        /// Create new group
        /// </summary>
        /// <param name="installationID">Installation ID</param>
        /// <param name="clientData">New group data</param>
        /// <returns>Created group item</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(GroupItem))]
        public IHttpActionResult CreateGroup(long installationID, GroupClientData clientData)
        {
            WispContext.Rights.CheckRoleInInstallation(installationID, InInstallationRoles.Administrator);
            return Ok(WispContext.Groups.Create(installationID, clientData));
        }

        /// <summary>
        /// Edit group
        /// </summary>
        /// <param name="installationID">Installation ID</param>
        /// <param name="groupID">Group ID</param>
        /// <param name="clientData">Edit group data</param>
        /// <returns>Edited group item</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("{groupID:min(1)}")]
        [ResponseType(typeof(GroupItem))]
        public IHttpActionResult EditGroup(long installationID, int groupID, GroupClientData clientData)
        {
            WispContext.Rights.CheckInstallationAceesible(installationID);
            return Ok(WispContext.Groups.Edit(installationID, groupID, clientData));
        }

        /// <summary>
        /// Delete group
        /// </summary>
        /// <remarks>Delete group, and all relations of this group.</remarks>
        /// <param name="installationID">Installation ID</param>
        /// <param name="groupID">Group ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("{groupID:min(1)}")]
        public IHttpActionResult DeleteGroup(long installationID, int groupID)
        {
            WispContext.Rights.CheckRoleInInstallation(installationID, InInstallationRoles.Administrator);
            WispContext.Groups.Delete(installationID, groupID);
            return Ok();
        }

    }

}