using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class EmpSeparationController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public EmpSeparationController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetResignation(string empUnqId)
        {
            var resignations = _context.EmpSeparations
                .Include(e => e.Employee)
                .Where(e => e.EmpUnqId == empUnqId
                    ).AsEnumerable()
                .Select(Mapper.Map<EmpSeparation, EmpSeparationDto>)
                .ToList();

            if (!resignations.Any())
                return BadRequest("No entries found.");

            foreach (EmpSeparationDto dto in resignations)
            {
                Employees emp = _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.WorkGroup)
                    .Include(e => e.Units)
                    .Include(e => e.Departments)
                    .Include(e => e.Stations)
                    .Include(e => e.Categories)
                    .Include(e => e.Grades)
                    .Include(e => e.Designations)
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp == null)
                    continue;

                dto.Employee.UnitName = emp.Units.UnitName;
                dto.Employee.CompName = emp.Company.CompName;
                dto.Employee.WrkGrpDesc = emp.WorkGroup.WrkGrpDesc;
                dto.Employee.UnitName = emp.Units.UnitName;
                dto.Employee.DeptName = emp.Departments.DeptName;
                dto.Employee.StatName = emp.Stations.StatName;
                dto.Employee.CatName = emp.Categories.CatName;
                dto.Employee.GradeName = emp.Grades.GradeName;
                dto.Employee.DesgName = emp.Designations.DesgName;

                List<ApplReleaseStatusDto> appRelease = _context.ApplReleaseStatus
                    .Where(a => a.ReleaseGroupCode == ReleaseGroups.NoDues && a.ApplicationId == dto.Id).AsEnumerable()
                    .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>)
                    .ToList();

                foreach (ApplReleaseStatusDto relDto in appRelease)
                {
                    relDto.ReleaserName = _context.Employees.FirstOrDefault(e=>e.EmpUnqId == relDto.ReleaseAuth)?.EmpName;
                }
                dto.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
                dto.ApplReleaseStatus = appRelease;
                dto.FurtherReleaserName =
                    _context.Employees.FirstOrDefault(e => e.EmpUnqId == dto.FurtherReleaser)?.EmpName;
                dto.HrUserName =
                    _context.Employees.FirstOrDefault(e => e.EmpUnqId == dto.HrUser)?.EmpName;
            }

            return Ok(resignations);
        }

        public IHttpActionResult GetResignations(DateTime fromDt, DateTime toDt)
        {
            var resignations = _context.EmpSeparations
                .Include(e => e.Employee)
                .Where(e => 
                    e.ApplicationDate >= fromDt && e.ApplicationDate <= toDt &&
                    e.ReleaseStatusCode != ReleaseStatus.ReleaseRejected
                    ).AsEnumerable()
                .Select(Mapper.Map<EmpSeparation, EmpSeparationDto>)
                .ToList();

            if (!resignations.Any())
                return BadRequest("No entries found.");

            foreach (EmpSeparationDto dto in resignations)
            {
                Employees emp = _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.WorkGroup)
                    .Include(e => e.Units)
                    .Include(e => e.Departments)
                    .Include(e => e.Stations)
                    .Include(e => e.Categories)
                    .Include(e => e.Grades)
                    .Include(e => e.Designations)
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp == null)
                    continue;

                dto.Employee.UnitName = emp.Units.UnitName;
                dto.Employee.CompName = emp.Company.CompName;
                dto.Employee.WrkGrpDesc = emp.WorkGroup.WrkGrpDesc;
                dto.Employee.UnitName = emp.Units.UnitName;
                dto.Employee.DeptName = emp.Departments.DeptName;
                dto.Employee.StatName = emp.Stations.StatName;
                dto.Employee.CatName = emp.Categories.CatName;
                dto.Employee.GradeName = emp.Grades.GradeName;
                dto.Employee.DesgName = emp.Designations.DesgName;

                List<ApplReleaseStatusDto> appRelease = _context.ApplReleaseStatus
                    .Where(a => a.ReleaseGroupCode == ReleaseGroups.NoDues && a.ApplicationId == dto.Id).AsEnumerable()
                    .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>)
                    .ToList();

                foreach (ApplReleaseStatusDto relDto in appRelease)
                {
                    relDto.ReleaserName = _context.Employees.FirstOrDefault(e=>e.EmpUnqId == relDto.ReleaseAuth)?.EmpName;
                }
                dto.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
                dto.ApplReleaseStatus = appRelease;

                dto.FurtherReleaserName =
                    _context.Employees.FirstOrDefault(e => e.EmpUnqId == dto.FurtherReleaser)?.EmpName;
            }

            return Ok(resignations);
        }

        public IHttpActionResult GetResignationHr()
        {
            var resignations = _context.EmpSeparations
                .Include(e => e.Employee)
                .Where(e => e.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                            e.StatusHr == false).AsEnumerable()
                .Select(Mapper.Map<EmpSeparation, EmpSeparationDto>)
                .ToList();

            if (!resignations.Any())
                return BadRequest("No entries found.");

            foreach (EmpSeparationDto dto in resignations.ToList())
            {
                Employees emp = _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.WorkGroup)
                    .Include(e => e.Units)
                    .Include(e => e.Departments)
                    .Include(e => e.Stations)
                    .Include(e => e.Categories)
                    .Include(e => e.Grades)
                    .Include(e => e.Designations)
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp == null)
                    continue;

                dto.Employee.UnitName = emp.Units.UnitName;
                dto.Employee.CompName = emp.Company.CompName;
                dto.Employee.WrkGrpDesc = emp.WorkGroup.WrkGrpDesc;
                dto.Employee.UnitName = emp.Units.UnitName;
                dto.Employee.DeptName = emp.Departments.DeptName;
                dto.Employee.StatName = emp.Stations.StatName;
                dto.Employee.CatName = emp.Categories.CatName;
                dto.Employee.GradeName = emp.Grades.GradeName;
                dto.Employee.DesgName = emp.Designations.DesgName;
                dto.FurtherReleaserName =
                    _context.Employees.FirstOrDefault(e => e.EmpUnqId == dto.FurtherReleaser)?.EmpName;
            }

            return Ok(resignations);
        }

        public IHttpActionResult GetFurtherRelease(string empUnqId, bool flag)
        {
            var resignations = _context.EmpSeparations
                .Include(e => e.Employee)
                .Where(e => e.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                            e.FurtherReleaseRequired == true &&
                            e.FurtherReleaser == empUnqId &&
                            e.FurtherReleaseStatusCode == ReleaseStatus.InRelease &&
                            e.StatusHr == false).AsEnumerable()
                .Select(Mapper.Map<EmpSeparation, EmpSeparationDto>)
                .ToList();

            if (!resignations.Any())
                return BadRequest("No entries found.");

            foreach (EmpSeparationDto dto in resignations.ToList())
            {
                Employees emp = _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.WorkGroup)
                    .Include(e => e.Units)
                    .Include(e => e.Departments)
                    .Include(e => e.Stations)
                    .Include(e => e.Categories)
                    .Include(e => e.Grades)
                    .Include(e => e.Designations)
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp == null)
                    continue;

                dto.Employee.UnitName = emp.Units.UnitName;
                dto.Employee.CompName = emp.Company.CompName;
                dto.Employee.WrkGrpDesc = emp.WorkGroup.WrkGrpDesc;
                dto.Employee.UnitName = emp.Units.UnitName;
                dto.Employee.DeptName = emp.Departments.DeptName;
                dto.Employee.StatName = emp.Stations.StatName;
                dto.Employee.CatName = emp.Categories.CatName;
                dto.Employee.GradeName = emp.Grades.GradeName;
                dto.Employee.DesgName = emp.Designations.DesgName;

                dto.FurtherReleaserName =
                    _context.Employees.FirstOrDefault(e => e.EmpUnqId == dto.FurtherReleaser)?.EmpName;
            }

            return Ok(resignations);
        }

        [HttpPost]
        public IHttpActionResult Resign([FromBody] object requestData)
        {
            try
            {
                var req = JsonConvert.DeserializeObject<EmpSeparation>(requestData.ToString());

                if (!ModelState.IsValid)
                    return BadRequest("Invalid state.");

                var emp = _context.Employees.FirstOrDefault(e => e.EmpUnqId == req.EmpUnqId);
                if (emp == null || emp.Active == false)
                    return BadRequest("Invalid employee or employee not active.");

                // check if already in resigned
                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    EmpSeparation resignation = _context.EmpSeparations
                        .FirstOrDefault(e => e.EmpUnqId == req.EmpUnqId &&
                                             (e.ReleaseStatusCode == ReleaseStatus.FullyReleased ||
                                              e.ReleaseStatusCode == ReleaseStatus.PartiallyReleased ||
                                              e.ReleaseStatusCode == ReleaseStatus.InRelease));
                    if (resignation != null)
                        return BadRequest("Already resigned!");

                    resignation = new EmpSeparation
                    {
                        Id = 0,
                        EmpUnqId = req.EmpUnqId,
                        ApplicationDate = req.ApplicationDate,
                        Mode = req.Mode,
                        Reason = req.Reason,
                        ReasonOther = req.ReasonOther,
                        RelieveDate = req.RelieveDate,
                        ResignText = req.ResignText,
                        ReleaseStatusCode = ReleaseStatus.InRelease,
                        FurtherReleaseRequired = false
                    };

                    _context.EmpSeparations.Add(resignation);
                    _context.SaveChanges();

                    ReleaseStrategies relStr = _context.ReleaseStrategy
                        .FirstOrDefault(
                            r =>
                                r.ReleaseGroupCode == ReleaseGroups.NoDues &&
                                r.ReleaseStrategy == req.EmpUnqId &&
                                r.Active == true
                        );

                    if (relStr == null)
                        return BadRequest("Release strategy not configured.");

                    var relStratLevels = _context.ReleaseStrategyLevels
                        .Where(
                            rl =>
                                rl.ReleaseGroupCode == ReleaseGroups.NoDues &&
                                rl.ReleaseStrategy == relStr.ReleaseStrategy
                        ).ToList();


                    List<ApplReleaseStatusDto> apps = new List<ApplReleaseStatusDto>();

                    foreach (ReleaseStrategyLevels level in relStratLevels)
                    {
                        //get releaser ID from ReleaseAuth model
                        var relAuth = _context.ReleaseAuth
                            .FirstOrDefault(ra => ra.ReleaseCode == level.ReleaseCode);

                        if (relAuth == null)
                            return BadRequest("Releaser not configured.");

                        var appRelStat = new ApplReleaseStatus
                        {
                            YearMonth = int.Parse(DateTime.Now.ToString("yyyyMM")),
                            ReleaseGroupCode = level.ReleaseGroupCode,
                            ApplicationId = resignation.Id,
                            ReleaseStrategy = level.ReleaseStrategy,
                            ReleaseStrategyLevel = level.ReleaseStrategyLevel,
                            ReleaseCode = level.ReleaseCode,
                            ReleaseStatusCode =
                                level.ReleaseStrategyLevel == 1
                                    ? ReleaseStatus.InRelease
                                    : ReleaseStatus.NotReleased,
                            ReleaseDate = null,
                            ReleaseAuth = relAuth.EmpUnqId,
                            IsFinalRelease = level.IsFinalRelease
                        };

                        //add to collection
                        apps.Add(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>(appRelStat));

                        _context.ApplReleaseStatus.Add(appRelStat);
                    }

                    var resignationDto = Mapper.Map<EmpSeparation, EmpSeparationDto>(resignation);
                    resignationDto.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
                    resignationDto.ApplReleaseStatus.AddRange(apps);

                    _context.SaveChanges();
                    transaction.Commit();

                    return Ok(resignationDto);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }
        }

        [HttpPut]
        public IHttpActionResult FurtherRelease(int id, string empUnqId, string releaseStatusCode)
        {
            var resignation = _context.EmpSeparations.FirstOrDefault(r => r.Id == id);
            if (resignation == null)
                return BadRequest("Invalid resignation id");

            var emp = _context.Employees.FirstOrDefault(e => e.EmpUnqId == empUnqId);
            if (emp == null)
                return BadRequest("Invalid employee code");

            if (releaseStatusCode == ReleaseStatus.InRelease)
            {
                resignation.FurtherReleaseRequired = true;
                resignation.FurtherReleaser = emp.EmpUnqId;
                resignation.FurtherReleaseStatusCode = ReleaseStatus.InRelease;
            }
            else if (releaseStatusCode == ReleaseStatus.FullyReleased)
            {
                if (resignation.FurtherReleaser != empUnqId)
                    return BadRequest("Invalid further releaser.");

                resignation.FurtherReleaseRequired = true;
                resignation.FurtherReleaser = emp.EmpUnqId;
                resignation.FurtherReleaseStatusCode = ReleaseStatus.FullyReleased;
                resignation.FurtherReleaseDate = DateTime.Now;
            }

            _context.SaveChanges();

            return Ok();
        }

        [HttpPut]
        public IHttpActionResult HrRelease(int id, string empUnqId)
        {
            var resignation = _context.EmpSeparations.FirstOrDefault(e => e.Id == id);
            if (resignation == null)
                return BadRequest("Resignation not found.");


            if (resignation.FurtherReleaseRequired &&
                resignation.FurtherReleaseStatusCode != ReleaseStatus.FullyReleased)
                return BadRequest("Resignation not released by further releaser.");


            resignation.StatusHr = true;
            resignation.HrStatusDate = DateTime.Now;
            resignation.HrUser = empUnqId;
            _context.SaveChanges();

            // TODO: Send these details to No-dues table.

            return Ok();
        }
    }
}