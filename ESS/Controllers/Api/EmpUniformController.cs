using System;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using AutoMapper;
using ESS.Models;
using ESS.Dto;
using Newtonsoft.Json;


namespace ESS.Controllers.Api
{
    public class EmpUniformController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public EmpUniformController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetEmpUniform(int year = 0, string empUnqId = "")
        {
            var empUniDto = _context.EmpUniform
                .Include(e => e.Employee)
                .Where(e => empUnqId == "" || e.EmpUnqId == empUnqId)
                .Where(e => year == 0 || e.Year == year)
                .ToList()
                .Select(Mapper.Map<EmpUniform, EmpUniformDto>);

            return Ok(empUniDto);
        }


        [HttpPost]
        public IHttpActionResult UpdateUniform([FromBody] object requestData)
        {
            var empUniDto = JsonConvert.DeserializeObject<EmpUniformDto>(requestData.ToString());

            if (!ModelState.IsValid)
                return BadRequest();

            //Now check if ID is passed, return bad request if id is also passed

            var empUniDtl = _context.EmpUniform
                .SingleOrDefault(
                    e => e.EmpUnqId == empUniDto.EmpUnqId &&
                         e.Year == empUniDto.Year
                );

            if (empUniDtl != null)
            {
                empUniDtl.ShirtSize = empUniDto.ShirtSize;
                empUniDtl.TrouserSize = empUniDto.TrouserSize;
                empUniDtl.UpdUser = empUniDto.UpdUser;
                empUniDtl.UpdTime = DateTime.Now;
            }
            else
            {
                EmpUniform empUni = new EmpUniform
                {
                    EmpUnqId = empUniDto.EmpUnqId,
                    Year = empUniDto.Year,
                    TrouserSize = empUniDto.TrouserSize,
                    ShirtSize = empUniDto.ShirtSize,
                    UpdUser = empUniDto.UpdUser,
                    UpdTime = DateTime.Now
                };

                _context.EmpUniform.Add(empUni);
            }

            _context.SaveChanges();

            return Ok();
        }
    }
}