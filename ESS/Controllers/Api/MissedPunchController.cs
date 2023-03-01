using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class MissedPunchController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public MissedPunchController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        [ActionName("getmissedpunch")]
        public IHttpActionResult GetMissedPunch(DateTime fromDt, DateTime toDt, bool isPostedOnly, string empUnqId = "",
            string punchType = "")
        {
            toDt = toDt.AddDays(1).AddMinutes(-1);

            IQueryable<MissedPunch> missedPunchList = _context.MissedPunches
                .Include(e => e.Employee)
                .Include(a => a.MissedPunchReleaseStatus)
                .Where(m => m.AddDate >= fromDt && m.AddDate <= toDt);

            if (empUnqId != "")
                missedPunchList = missedPunchList.Where(e => e.EmpUnqId == empUnqId);

            if (isPostedOnly)
                missedPunchList = missedPunchList.Where(p => p.PostingFlag);

            if(punchType != "")
                missedPunchList = missedPunchList.Where(e => e.PunchType == punchType);

            List<MissedPunchDto> missedPunch = missedPunchList.AsEnumerable()
                .Select(Mapper.Map<MissedPunch, MissedPunchDto>).ToList();

            if (!missedPunch.Any())
                return BadRequest("No record found.");

            List<string> empList = missedPunch.Select(e => e.EmpUnqId).ToList();

            List<EmployeeDto> employeeDto = _context.Employees
                .Where(e => empList.Contains(e.EmpUnqId))
                .Select(e => new EmployeeDto
                {
                    EmpUnqId = e.EmpUnqId,
                    EmpName = e.EmpName,
                    FatherName = e.FatherName,
                    Active = e.Active,
                    Pass = e.Pass,

                    CompCode = e.CatCode,
                    WrkGrp = e.WrkGrp,
                    UnitCode = e.UnitCode,
                    DeptCode = e.DeptCode,
                    StatCode = e.StatCode,
                    CatCode = e.CatCode,
                    EmpTypeCode = e.EmpTypeCode,
                    GradeCode = e.GradeCode,
                    DesgCode = e.DesgCode,
                    IsHod = e.IsHod,

                    CompName = e.Company.CompName,
                    WrkGrpDesc = e.WorkGroup.WrkGrpDesc,
                    UnitName = e.Units.UnitName,
                    DeptName = e.Departments.DeptName,
                    StatName = e.Stations.StatName,
                    CatName = e.Categories.CatName,
                    EmpTypeName = e.EmpTypes.EmpTypeName,
                    GradeName = e.Grades.GradeName,
                    DesgName = e.Designations.DesgName,

                    Location = e.Location
                }).ToList();

            foreach (EmployeeDto dto in employeeDto)
            {
                dto.YearlyCount = _context.MissedPunches.Count(e => e.EmpUnqId == dto.EmpUnqId &&
                                                                    e.ReleaseStatusCode ==
                                                                    ReleaseStatus.FullyReleased &&
                                                                    e.AddDate.Year == DateTime.Today.Year);
            }

            foreach (MissedPunchDto missedPunchDto in missedPunch)
            {
                missedPunchDto.Employee = employeeDto.FirstOrDefault(e => e.EmpUnqId == missedPunchDto.EmpUnqId);
                IEnumerable<MissedPunchReleaseStatusDto> appl = _context.MissedPunchReleaseStatus
                    .Where(l => l.ApplicationId == missedPunchDto.Id)
                    .ToList()
                    .Select(Mapper.Map<MissedPunchReleaseStatus, MissedPunchReleaseStatusDto>);

                foreach (MissedPunchReleaseStatusDto releaseStatusDto in appl)
                {
                    List<ReleaseAuth> relCode = _context.ReleaseAuth
                        .Where(r => r.ReleaseCode == releaseStatusDto.ReleaseCode)
                        .ToList();

                    foreach (ReleaseAuth unused in relCode.Where(auth => auth.EmpUnqId == empUnqId))
                        releaseStatusDto.ReleaseAuth = empUnqId;

                    missedPunchDto.MissedPunchReleaseStatus.Add(releaseStatusDto);
                }
            }

            return Ok(missedPunch);
        }

        [HttpGet]
        [ActionName("getmissedpunchreleaser")]
        public IHttpActionResult GetMissedPunch(DateTime fromDt, DateTime toDt, string empUnqId)
        {
            toDt = toDt.AddDays(1).AddMinutes(-1);

            var releaseCode = _context.ReleaseAuth
                .Where(r => r.EmpUnqId == empUnqId)
                .Select(r => r.ReleaseCode)
                .ToList();

            var releaseStr = _context.ReleaseStrategyLevels
                .Where(l => releaseCode.Contains(l.ReleaseCode) &&
                            l.ReleaseGroupCode == ReleaseGroups.LeaveApplication)
                .Select(l => l.ReleaseStrategy)
                .ToList();

            IQueryable<MissedPunch> missedPunchList = _context.MissedPunches
                .Include(e => e.Employee)
                .Where(m => m.AddDate >= fromDt && m.AddDate <= toDt &&
                            releaseStr.Contains(m.ReleaseStrategy)
                );

            List<MissedPunchDto> missedPunch = missedPunchList.AsEnumerable()
                .Select(Mapper.Map<MissedPunch, MissedPunchDto>).ToList();

            if (!missedPunch.Any())
                return BadRequest("No record found.");

            List<string> empList = missedPunch.Select(e => e.EmpUnqId).ToList();

            List<EmployeeDto> employeeDto = _context.Employees
                .Where(e => empList.Contains(e.EmpUnqId))
                .Select(e => new EmployeeDto
                {
                    EmpUnqId = e.EmpUnqId,
                    EmpName = e.EmpName,
                    FatherName = e.FatherName,
                    Active = e.Active,
                    Pass = e.Pass,

                    CompCode = e.CatCode,
                    WrkGrp = e.WrkGrp,
                    UnitCode = e.UnitCode,
                    DeptCode = e.DeptCode,
                    StatCode = e.StatCode,
                    CatCode = e.CatCode,
                    EmpTypeCode = e.EmpTypeCode,
                    GradeCode = e.GradeCode,
                    DesgCode = e.DesgCode,
                    IsHod = e.IsHod,

                    CompName = e.Company.CompName,
                    WrkGrpDesc = e.WorkGroup.WrkGrpDesc,
                    UnitName = e.Units.UnitName,
                    DeptName = e.Departments.DeptName,
                    StatName = e.Stations.StatName,
                    CatName = e.Categories.CatName,
                    EmpTypeName = e.EmpTypes.EmpTypeName,
                    GradeName = e.Grades.GradeName,
                    DesgName = e.Designations.DesgName,

                    Location = e.Location
                }).ToList();

            foreach (MissedPunchDto missedPunchDto in missedPunch)
            {
                missedPunchDto.Employee = employeeDto.FirstOrDefault(e => e.EmpUnqId == missedPunchDto.EmpUnqId);
            }

            return Ok(missedPunch);
        }

        [HttpPost]
        [ActionName("createmissedpunch")]
        public IHttpActionResult CreateMissedPunch([FromBody] object requestData)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model state invalid.");

            var dto = JsonConvert.DeserializeObject<MissedPunchDto>(requestData.ToString());

            if (dto.Id != 0) return BadRequest("Id must be zero!");


            // if this is attendance punch correction, then get details from location table


            //first get release strategy details based on comp, wrkgrp, unit, dept, stat and cat code
            ReleaseStrategies relStrat = _context.ReleaseStrategy
                .FirstOrDefault(
                    r =>
                        r.ReleaseGroupCode == ReleaseGroups.LeaveApplication &&
                        r.ReleaseStrategy == dto.EmpUnqId &&
                        r.Active == true
                );

            if (relStrat == null)
                return BadRequest("Release strategy not configured.");

            dto.ReleaseStrategy = relStrat.ReleaseStrategy;
            dto.ReleaseStatusCode = ReleaseStatus.NotReleased;

            MissedPunch missPunch = Mapper.Map<MissedPunchDto, MissedPunch>(dto);
            //if (!string.IsNullOrEmpty(missPunch.InTimeUser))
            //    missPunch.InTime = DateTime.Now;
            //else if (!string.IsNullOrEmpty(missPunch.OutTimeUser))
            //    missPunch.OutTime = DateTime.Now;

            var loc = _context.Employees.FirstOrDefault(e => e.EmpUnqId == missPunch.EmpUnqId)?.Location;
            if (loc != null && loc == Locations.Ipu)
            {
                if (!string.IsNullOrEmpty(missPunch.InTimeUser))
                    missPunch.InTime = DateTime.Now;
                else if (!string.IsNullOrEmpty(missPunch.OutTimeUser))
                    missPunch.OutTime = DateTime.Now;
            }
            else
            {
                if (!string.IsNullOrEmpty(missPunch.InTimeUser))
                    missPunch.InTime = dto.InTime;
                else if (!string.IsNullOrEmpty(missPunch.OutTimeUser))
                    missPunch.OutTime = dto.OutTime;

            }


            if(dto.PunchType == MissedPunch.AttdPunch && loc !=null)
            {
                Locations location = _context.Location.FirstOrDefault(l => l.Location == loc);
                if(location != null)
                {
                    int attdPunchLimitDays = location.AttdPunchLimitDays;
                    int attdPunchMonthLimit = location.AttdPunchMonthLimit;

                    if(dto.AttdPunchTime != null && 
                       (DateTime.Today - dto.AttdPunchTime.Value.Date).Days > attdPunchLimitDays)
                        return BadRequest("Limit for applying for missed punch is " + attdPunchLimitDays + " days.");


                    if(attdPunchMonthLimit != 0)
                    {
                        var count = _context.MissedPunches.Count(e=>
                            e.EmpUnqId == dto.EmpUnqId &&
                            e.PunchType == MissedPunch.AttdPunch &&
                            e.AttdPunchTime.Value.Month == DateTime.Now.Month &&
                            e.AttdPunchTime.Value.Year == DateTime.Now.Year);
                        if(count > attdPunchMonthLimit)
                            return BadRequest("Monthly limit for missed punch is " + attdPunchMonthLimit + " per month.");
                    }
                }
            }

            
            _context.MissedPunches.Add(missPunch);
            _context.SaveChanges();

            //get release strategy levels
            List<ReleaseStrategyLevels> relStratLevels = _context.ReleaseStrategyLevels
                .Where(
                    rl =>
                        rl.ReleaseGroupCode == ReleaseGroups.LeaveApplication &&
                        rl.ReleaseStrategy == relStrat.ReleaseStrategy
                ).ToList();


            relStrat.ReleaseStrategyLevels = relStratLevels;


            //create a temp collection to be added to leaveapplicationdto later on
            var apps = new List<MissedPunchReleaseStatusDto>();

            foreach (ReleaseStrategyLevels relStratReleaseStrategyLevel in relStrat.ReleaseStrategyLevels)
            {
                //get releaser ID from ReleaseAuth model
                ReleaseAuth relAuth = _context.ReleaseAuth
                    .FirstOrDefault(ra => ra.ReleaseCode == relStratReleaseStrategyLevel.ReleaseCode);

                MissedPunchReleaseStatus mpRelStat = new MissedPunchReleaseStatus
                {
                    ApplicationId = missPunch.Id,
                    ReleaseStrategy = relStratReleaseStrategyLevel.ReleaseStrategy,
                    ReleaseStrategyLevel = relStratReleaseStrategyLevel.ReleaseStrategyLevel,
                    ReleaseCode = relStratReleaseStrategyLevel.ReleaseCode,
                    ReleaseStatusCode =
                        relStratReleaseStrategyLevel.ReleaseStrategyLevel == 1
                            ? ReleaseStatus.InRelease
                            : ReleaseStatus.NotReleased,
                    ReleaseDate = null,
                    ReleaseAuth = relAuth.EmpUnqId,
                    IsFinalRelease = relStratReleaseStrategyLevel.IsFinalRelease
                };

                //add to collection
                apps.Add(Mapper.Map<MissedPunchReleaseStatus, MissedPunchReleaseStatusDto>(mpRelStat));

                _context.MissedPunchReleaseStatus.Add(mpRelStat);
            }

            _context.SaveChanges();

            dto.Id = missPunch.Id;
            dto.MissedPunchReleaseStatus = new List<MissedPunchReleaseStatusDto>();
            dto.MissedPunchReleaseStatus.AddRange(apps);

            return Ok(dto);
        }

        [HttpPut]
        [ActionName("UpdateTime")]
        public IHttpActionResult UpdateTime(int id, string inOut, string empUnqId, DateTime punchTime)
        {
            try
            {
                MissedPunch missedpunch = _context.MissedPunches.FirstOrDefault(p => p.Id == id);
                if (missedpunch == null)
                    return BadRequest("Missed punch not found with id " + id);

                string loc = _context.Employees.FirstOrDefault(e => e.EmpUnqId == missedpunch.EmpUnqId)?.Location;

                switch (inOut.ToUpper())
                {
                    case "I":
                        missedpunch.InTime = DateTime.Now;
                        missedpunch.InTimeUser = empUnqId;
                        if (loc != null && loc != Locations.Ipu)
                            missedpunch.InTime = punchTime;

                        break;
                    case "O":
                        missedpunch.OutTime = DateTime.Now;
                        missedpunch.OutTimeUser = empUnqId;

                        if (loc != null && loc != Locations.Ipu)
                            missedpunch.InTime = punchTime;

                        break;
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            return Ok();
        }

        [HttpGet]
        [ActionName("getapplreleasestatus")]
        public IHttpActionResult GetApplReleaseStatus(string empUnqId)
        {
            string[] relAuth = _context.ReleaseAuth
                .Where(r => r.EmpUnqId == empUnqId)
                .Select(r => r.ReleaseCode)
                .ToArray();


            List<MissedPunchReleaseStatus> app = _context.MissedPunchReleaseStatus
                .Where(l =>
                    relAuth.Contains(l.ReleaseCode) &&
                    l.ReleaseStatusCode == ReleaseStatus.InRelease)
                .ToList();

            int[] appIds = app.Select(a => a.ApplicationId).ToArray();

            List<MissedPunchDto> missedPunches = _context.MissedPunches
                .Include(e => e.Employee)
                .Include(a => a.MissedPunchReleaseStatus)
                .Where(l => appIds.Contains(l.Id)).ToList()
                .Select(Mapper.Map<MissedPunch, MissedPunchDto>)
                .ToList();


            foreach (MissedPunchDto dto in missedPunches)
            {
                IEnumerable<MissedPunchReleaseStatusDto> appl = _context.MissedPunchReleaseStatus
                    .Where(l => l.ApplicationId == dto.Id)
                    .ToList()
                    .Select(Mapper.Map<MissedPunchReleaseStatus, MissedPunchReleaseStatusDto>);

                foreach (MissedPunchReleaseStatusDto releaseStatusDto in appl)
                {
                    List<ReleaseAuth> relCode = _context.ReleaseAuth
                        .Where(r => r.ReleaseCode == releaseStatusDto.ReleaseCode)
                        .ToList();

                    foreach (ReleaseAuth unused in relCode.Where(auth => auth.EmpUnqId == empUnqId))
                        releaseStatusDto.ReleaseAuth = empUnqId;

                    dto.MissedPunchReleaseStatus.Add(releaseStatusDto);
                }


                EmployeeDto employeeDto = _context.Employees
                    .Select(e => new EmployeeDto
                    {
                        EmpUnqId = e.EmpUnqId,
                        EmpName = e.EmpName,
                        FatherName = e.FatherName,
                        Active = e.Active,
                        Pass = e.Pass,

                        CompCode = e.CatCode,
                        WrkGrp = e.WrkGrp,
                        UnitCode = e.UnitCode,
                        DeptCode = e.DeptCode,
                        StatCode = e.StatCode,
                        CatCode = e.CatCode,
                        EmpTypeCode = e.EmpTypeCode,
                        GradeCode = e.GradeCode,
                        DesgCode = e.DesgCode,
                        IsHod = e.IsHod,

                        CompName = e.Company.CompName,
                        WrkGrpDesc = e.WorkGroup.WrkGrpDesc,
                        UnitName = e.Units.UnitName,
                        DeptName = e.Departments.DeptName,
                        StatName = e.Stations.StatName,
                        CatName = e.Categories.CatName,
                        EmpTypeName = e.EmpTypes.EmpTypeName,
                        GradeName = e.Grades.GradeName,
                        DesgName = e.Designations.DesgName,

                        Location = e.Location
                    })
                    .Single(e => e.EmpUnqId == dto.EmpUnqId);

                // get yearly missed punch application count for this employee
                employeeDto.YearlyCount = _context.MissedPunches.Count(e => e.EmpUnqId == employeeDto.EmpUnqId &&
                                                                            e.ReleaseStatusCode ==
                                                                            ReleaseStatus.FullyReleased &&
                                                                            e.AddDate.Year == DateTime.Today.Year);
                dto.Employee = employeeDto;
            }

            return Ok(missedPunches);
        }

        [HttpPost]
        [ActionName("apprelease")]
        public IHttpActionResult AppRelease([FromBody] object requestData, string empUnqId, string releaseStatusCode)
        {
            var dto = JsonConvert.DeserializeObject<MissedPunchReleaseStatusDto>(requestData.ToString());

            MissedPunchReleaseStatus appRelease = _context.MissedPunchReleaseStatus
                .SingleOrDefault(a => a.ApplicationId == dto.ApplicationId &&
                                      a.ReleaseStrategyLevel == dto.ReleaseStrategyLevel);
            if (appRelease == null)
                return BadRequest("Invalid app release status details...");

            if (appRelease.ReleaseStatusCode != ReleaseStatus.InRelease)
                return BadRequest("Application is not in release state...");

            appRelease.Remarks = dto.Remarks;

            //get the release auth
            ReleaseAuth relAuth = _context.ReleaseAuth
                .SingleOrDefault(r => r.ReleaseCode == appRelease.ReleaseCode &&
                                      r.EmpUnqId == empUnqId &&
                                      r.Active);
            if (relAuth == null)
                return BadRequest("Invalid release code. Check if active");

            //get the release strategy levels
            string vRelStr = appRelease.ReleaseStrategy;
            ReleaseStrategyLevels relStrLevel = _context.ReleaseStrategyLevels
                .SingleOrDefault(r => r.ReleaseGroupCode == ReleaseGroups.LeaveApplication &&
                                      r.ReleaseStrategy == vRelStr &&
                                      r.ReleaseStrategyLevel == appRelease.ReleaseStrategyLevel &&
                                      r.ReleaseCode == appRelease.ReleaseCode);
            if (relStrLevel == null)
                return BadRequest("Release strategy details not found...");


            try
            {
                MissedPunch missedPunch = _context.MissedPunches.SingleOrDefault(
                    m => m.Id == appRelease.ApplicationId);

                if (missedPunch == null)
                    return BadRequest("Missed punch not found!");

                //call transaction to update multiple tables
                using (DbContextTransaction trnsaction = _context.Database.BeginTransaction())
                {
                    if (releaseStatusCode == ReleaseStatus.ReleaseRejected)
                    {
                        appRelease.ReleaseStatusCode = ReleaseStatus.ReleaseRejected;
                        missedPunch.ReleaseStatusCode = ReleaseStatus.ReleaseRejected;
                    }
                    else if (releaseStatusCode == ReleaseStatus.FullyReleased)
                    {
                        //If this level is not the final level, set next level to "I"
                        if (!appRelease.IsFinalRelease)
                        {
                            //get next level releaser from app release
                            MissedPunchReleaseStatus nextLevel = _context.MissedPunchReleaseStatus.SingleOrDefault(
                                a => a.ApplicationId == appRelease.ApplicationId &&
                                     a.ReleaseStrategy == appRelease.ReleaseStrategy &&
                                     a.ReleaseStrategyLevel == appRelease.ReleaseStrategyLevel + 1
                            );
                            if (nextLevel == null)
                                return BadRequest(
                                    "This is not final release, and next level not found! Chk app release");

                            nextLevel.ReleaseStatusCode = ReleaseStatus.InRelease;
                            missedPunch.ReleaseStatusCode = ReleaseStatus.PartiallyReleased;
                        }
                        else
                        {
                            //this is final level...
                            missedPunch.ReleaseStatusCode = ReleaseStatus.FullyReleased;
                        }

                        //Finally set this application details status to "F"
                        appRelease.ReleaseStatusCode = ReleaseStatus.FullyReleased;
                        appRelease.ReleaseDate = DateTime.Now.Date;
                        appRelease.ReleaseAuth = empUnqId;
                    }

                    //update database here...
                    _context.SaveChanges();

                    //you'll need to update releasestrategy table manually because keyfield involved.
                    //basically this is why i've to use transaction.
                    //string strSql = "update MissedPunchReleaseStatus set ReleaseStrategy = '" + vRelStr + "' " +
                    //                "where ReleaseGroupCode = 'LA' " +
                    //                "and ApplicationId = " + appRelease.ApplicationId + "";

                    //_context.Database.ExecuteSqlCommand(strSql);

                    //commit changes....
                    trnsaction.Commit();
                }


                return Ok(Mapper.Map<MissedPunch, MissedPunchDto>(missedPunch));
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }
        }


        //private class AutoPostObject
        //{
        //    public DateTime SanctionDt { get; set; }
        //    public string EmpUnqID { get; set; }
        //    public DateTime SanTime { get; set; }
        //    public string SanFlg { get; set; } // I for in, O for out, T for TPA hours
        //    public int TPAHours { get; set; }
        //    public string Reason { get; set; }
        //    public bool PostedFlg { get; set; }
        //    public string ERROR { get; set; }
        //}

        //private AutoPostObject AutoPost(AutoPostObject punchData, string location, out bool output)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(Helpers.CustomHelper.GetAttendanceServerApi(location));

        //        StringContent content = new StringContent(JsonConvert.SerializeObject(punchData),
        //            Encoding.UTF8, "application/json");

        //        var responseTask = client.PostAsync("/api/sanction/postsanction", content);
        //        responseTask.Wait();

        //        HttpResponseMessage result = responseTask.Result;
        //        output = result.IsSuccessStatusCode;

        //        AutoPostObject autoPost = new AutoPostObject();

        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = result.Content.ReadAsAsync<AutoPostObject>();
        //            readTask.Wait();

        //            autoPost = readTask.Result;

        //            return autoPost;
        //        }
        //        else
        //        {
        //            var readTask = result.Content.ReadAsAsync<AutoPostObject>();
        //            readTask.Wait();

        //            autoPost = readTask.Result;
        //            // Some error was there, return it without changing posting flags

        //            return autoPost;
        //        }
        //    }
        //}
    }
}