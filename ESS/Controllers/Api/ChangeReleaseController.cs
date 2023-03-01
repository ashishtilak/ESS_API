using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Antlr.Runtime.Misc;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class ChangeReleaseController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public ChangeReleaseController()
        {
            _context = new ApplicationDbContext();
        }

        public class ReleaseAuthEmp
        {
            public string ReleaseCode { get; set; }
            public string EmpUnqId { get; set; }
            public string EmpName { get; set; }
        }

        public IHttpActionResult GetReleaseAuth(string empUnqId)
        {
            //first get the release auth for this employee
            var relAuth = _context.ReleaseAuth
                .Where(r => r.EmpUnqId == empUnqId)
                .ToList();

            //list of result to be returned
            var result = new List<ReleaseAuthEmp>();


            //for each release auth of this employee,
            //get the release codes
            foreach (ReleaseAuth auth in relAuth)
            {
                var employees = _context.ReleaseAuth
                    .Where(r => r.ReleaseCode == auth.ReleaseCode)
                    .Select(e => e.EmpUnqId)
                    .ToList();

                // now we have multiple employees against release code (if exist)
                foreach (string emp in employees)
                {
                    var res = new ReleaseAuthEmp
                    {
                        ReleaseCode = auth.ReleaseCode,
                        EmpUnqId = emp
                    };

                    Employees empname = _context.Employees.SingleOrDefault(e => e.EmpUnqId == emp);
                    if (empname != null)
                        res.EmpName = empname.EmpName;

                    result.Add(res);
                }
            }

            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult ChangeReleaseStrategy([FromBody] object requestData)
        {
            ReleaseStrategyDto dto = JsonConvert.DeserializeObject<ReleaseStrategyDto>(requestData.ToString());

            //check if release strategy exist. If exist, do nothing,
            //if not, create new release strategy object and add to context

            ReleaseStrategies releaseStrategy = _context.ReleaseStrategy
                .SingleOrDefault(r =>
                    r.ReleaseGroupCode == dto.ReleaseGroupCode &&
                    r.ReleaseStrategy == dto.ReleaseStrategy
                );

            if (releaseStrategy == null)
            {
                releaseStrategy = new ReleaseStrategies
                {
                    ReleaseGroupCode = dto.ReleaseGroupCode,
                    ReleaseStrategy = dto.ReleaseStrategy,
                    ReleaseStrategyName = dto.ReleaseStrategyName,
                    IsHod = false,
                    Active = true,
                    UpdDt = dto.UpdDt,
                    UpdUser = dto.UpdUser
                };

                _context.ReleaseStrategy.Add(releaseStrategy);
            }
            else
            {
                releaseStrategy.UpdDt = dto.UpdDt;
                releaseStrategy.UpdUser = dto.UpdUser;
            }

            var releaseStrategyLevels = _context.ReleaseStrategyLevels
                .Where(rl =>
                    rl.ReleaseGroupCode == dto.ReleaseGroupCode &&
                    rl.ReleaseStrategy == dto.ReleaseStrategy
                ).ToList();


            _context.ReleaseStrategyLevels.RemoveRange(releaseStrategyLevels);


            ReleaseStrategyLevelDto last = dto.ReleaseStrategyLevels.LastOrDefault();

            foreach (ReleaseStrategyLevels newRelStrLevel in dto.ReleaseStrategyLevels.Select(level =>
                new ReleaseStrategyLevels
                {
                    ReleaseGroupCode = dto.ReleaseGroupCode,
                    ReleaseStrategy = dto.ReleaseStrategy,
                    ReleaseStrategyLevel = level.ReleaseStrategyLevel,
                    ReleaseCode = level.ReleaseCode,
                    //check if this is last line, in that case 
                    IsFinalRelease = (last != null && last.Equals(level))
                }))
            {
                _context.ReleaseStrategyLevels.Add(newRelStrLevel);
            }

            _context.SaveChanges();

            return Ok();
        }


        // release strategy inactive releasestrategy, gpreleasestrategy
        // releaseauth in active

        [HttpGet]
        public IHttpActionResult Cleanup()
        {
            try
            {
                //TODO: Remove hard coded "COMP" 
                var emps = _context.Employees
                    .Where(e => (e.WrkGrp == "COMP" || e.WrkGrp == "OUTSOURCE") && e.Active == false)
                    .Select(e => e.EmpUnqId).ToArray();
                var releaseStrategies = _context.ReleaseStrategy
                    .Where(r => emps.Contains(r.ReleaseStrategy) && r.Active)
                    .ToList();

                foreach (var rel in releaseStrategies)
                {
                    rel.Active = false;
                }


                var gpReleaseStrategies = _context.GpReleaseStrategy
                    .Where(r => emps.Contains(r.GpReleaseStrategy) && r.Active)
                    .ToList();

                foreach (var rel in gpReleaseStrategies)
                {
                    rel.Active = false;
                }

                _context.SaveChanges();


                var releaseAuths = _context.ReleaseAuth
                    .Where(r => emps.Contains(r.EmpUnqId) && r.Active)
                    .ToList();

                foreach (var rel in releaseAuths)
                {
                    rel.Active = false;
                }

                _context.SaveChanges();


                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}