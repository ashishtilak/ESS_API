using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;

namespace ESS.Controllers.Api
{
    public class RolesController : ApiController
    {
        private ApplicationDbContext _context;

        public RolesController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetRoleAuth(int roleId)
        {
            var roleAuth = _context.RoleAuths.Where(r => r.RoleId == roleId)
                .Select(Mapper.Map<RoleAuth, RoleAuthDto>);

            return Ok(roleAuth);
        }

        public IHttpActionResult GetRoleAuth(string empUnqId)
        {
            var empRoles = _context.RoleUser.Where(r => r.EmpUnqId == empUnqId).Select(r => r.RoleId).ToArray();

            var roleAuth = _context.RoleAuths
                .Where(r => empRoles.Contains(r.RoleId))
                .Select(Mapper.Map<RoleAuth, RoleAuthDto>);

            return Ok(roleAuth);
        }
    }
}