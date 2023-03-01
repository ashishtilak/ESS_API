using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ESS.Models;

namespace ESS.Controllers.Api
{
    public class SyncController : ApiController
    {
        private ApplicationDbContext _context;

        public SyncController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        public IHttpActionResult SyncDb(string location)
        {
            try
            {
                Helpers.CustomHelper.SyncData(location);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            return Ok("Record synchronization completed successfully.");
        }
    }
}