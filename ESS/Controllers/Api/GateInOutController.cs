using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ESS.Models;

namespace ESS.Controllers.Api
{
    public class GateInOutController : ApiController
    {
        private ApplicationDbContext _context;
        private const int TIME_LIMIT = 5;


        public GateInOutController()
        {
            _context = new ApplicationDbContext();
        }
    }
}