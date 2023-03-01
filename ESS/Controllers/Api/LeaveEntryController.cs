using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ESS.Controllers.Api
{
    public class LeaveEntryController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetLeaveEntries(string empUnqId)
        {
            try
            {
                var leaves = Helpers.CustomHelper.GetLeaveEntries(empUnqId);
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.ToString());
            }
        }
    }
}