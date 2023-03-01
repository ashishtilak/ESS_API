using System;
using System.Collections.Generic;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class LeaveApplicationController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public LeaveApplicationController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetLeaveApplication()
        {
            var leaveAppDto = _context.LeaveApplications
                .Include(e => e.Employee)
                .Include(c => c.Company)
                .Include(cat => cat.Categories)
                .Include(w => w.WorkGroup)
                .Include(d => d.Departments)
                .Include(s => s.Stations)
                .Include(u => u.Units)
                .Include(r => r.ReleaseGroup)
                .Include(rs => rs.RelStrategy)
                .Include(l => l.LeaveApplicationDetails)
                .ToList()
                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>)
                .Take(100);

            return Ok(leaveAppDto);
        }

        public IHttpActionResult GetLeaveApplication(int leaveAppId)
        {
            var leaveApp = _context.LeaveApplications
                .Include(e => e.Employee)
                .Include(c => c.Company)
                .Include(cat => cat.Categories)
                .Include(w => w.WorkGroup)
                .Include(d => d.Departments)
                .Include(s => s.Stations)
                .Include(u => u.Units)
                .Include(r => r.ReleaseGroup)
                .Include(rs => rs.RelStrategy)
                .Include(l => l.LeaveApplicationDetails)
                .Where(lv => lv.LeaveAppId == leaveAppId)
                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>)
                .ToList();


            foreach (var lApp in leaveApp)
            {
                var app = _context.ApplReleaseStatus
                    .Where(l =>
                        l.YearMonth == lApp.YearMonth &&
                        l.ReleaseGroupCode == lApp.ReleaseGroupCode &&
                        l.ApplicationId == lApp.LeaveAppId)
                    .ToList()
                    .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>);

                foreach (var applReleaseStatusDto in app)
                {
                    lApp.ApplReleaseStatus.Add(applReleaseStatusDto);
                }
            }

            return Ok(leaveApp);
        }

        public IHttpActionResult GetLeaveApplication(string empUnqId)
        {
            var leaveAppDto = _context.LeaveApplications
                .Include(e => e.Employee)
                .Include(c => c.Company)
                .Include(cat => cat.Categories)
                .Include(w => w.WorkGroup)
                .Include(d => d.Departments)
                .Include(s => s.Stations)
                .Include(u => u.Units)
                .Include(r => r.ReleaseGroup)
                .Include(rs => rs.RelStrategy)
                .Include(l => l.LeaveApplicationDetails)
                .Where(lv => lv.EmpUnqId == empUnqId)
                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>)
                .ToList();


            foreach (var lApp in leaveAppDto)
            {
                var app = _context.ApplReleaseStatus
                    .Where(l =>
                        l.YearMonth == lApp.YearMonth &&
                        l.ReleaseGroupCode == lApp.ReleaseGroupCode &&
                        l.ApplicationId == lApp.LeaveAppId)
                    .ToList()
                    .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>);

                foreach (var applReleaseStatusDto in app)
                {
                    applReleaseStatusDto.ReleaserName = _context.Employees
                        .FirstOrDefault(e => e.EmpUnqId == applReleaseStatusDto.ReleaseAuth)
                        .EmpName;
                    lApp.ApplReleaseStatus.Add(applReleaseStatusDto);
                }
            }

            return Ok(leaveAppDto);
        }

        public List<LeaveApplicationDto> GetLeaveApplication(string empUnqId, int days)
        {
            DateTime reqDate = DateTime.Now.AddDays(days * -1);

            var leaveAppDto = _context.LeaveApplications
                .Include(e => e.Employee)
                .Include(c => c.Company)
                .Include(cat => cat.Categories)
                .Include(w => w.WorkGroup)
                .Include(d => d.Departments)
                .Include(s => s.Stations)
                .Include(u => u.Units)
                .Include(r => r.ReleaseGroup)
                .Include(rs => rs.RelStrategy)
                .Include(l => l.LeaveApplicationDetails)
                .Where(lv =>
                    lv.EmpUnqId == empUnqId &&
                    lv.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                    lv.ParentId == 0 &&
                    lv.Cancelled == false &&
                    lv.LeaveApplicationDetails.Any(la => la.ToDt >= reqDate)).AsEnumerable()
                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>)
                .ToList();


            foreach (var lApp in leaveAppDto)
            {
                var app = _context.ApplReleaseStatus
                    .Where(l =>
                        l.YearMonth == lApp.YearMonth &&
                        l.ReleaseGroupCode == lApp.ReleaseGroupCode &&
                        l.ApplicationId == lApp.LeaveAppId)
                    .ToList()
                    .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>);

                foreach (var applReleaseStatusDto in app)
                {
                    lApp.ApplReleaseStatus.Add(applReleaseStatusDto);
                }
            }

            return leaveAppDto;
        }

        [HttpPost]
        public IHttpActionResult CreateLeaveApplication([FromBody] object requestData)
        {
            var leaveApplicationDto = JsonConvert.DeserializeObject<LeaveApplicationDto>(requestData.ToString());

            if (!ModelState.IsValid)
                return BadRequest();

            //Now check if ID is passed, return bad request if id is also passed
            if (leaveApplicationDto.LeaveAppId != 0) return BadRequest("What should I do this ID???");

            //get the next id from db and fill leaveapplication data
            int maxId;

            try
            {
                maxId = _context.LeaveApplications.Max(i => i.LeaveAppId);
            }
            catch
            {
                maxId = 0;
            }

            leaveApplicationDto.LeaveAppId = maxId + 1;

            //also fill id in leave application details

            //Checks for leave application are here

            if (leaveApplicationDto.LeaveApplicationDetails.Count == 0)
                return BadRequest("Leave application details not found!");

            foreach (LeaveApplicationDetailDto leaveApplicationDetailDto in leaveApplicationDto.LeaveApplicationDetails)
            {
                leaveApplicationDetailDto.LeaveAppId = leaveApplicationDto.LeaveAppId;

                //posting status to "N"
                leaveApplicationDetailDto.IsPosted = LeaveApplicationDetails.NotPosted;
            }

            //add code for application release status table

            //first get release strategy details based on comp, wrkgrp, unit, dept, stat and cat code
            ReleaseStrategies relStrat = _context.ReleaseStrategy
                .FirstOrDefault(
                    r =>
                        r.ReleaseGroupCode == leaveApplicationDto.ReleaseGroupCode &&
                        r.ReleaseStrategy == leaveApplicationDto.EmpUnqId &&
                        r.Active == true
                );

            if (relStrat == null)
                return BadRequest("Release strategy not configured.");

            leaveApplicationDto.ReleaseGroupCode =
                leaveApplicationDto.ReleaseGroupCode; //what am I doing here!!! :D hahahaha
            leaveApplicationDto.ReleaseStrategy = relStrat.ReleaseStrategy;
            leaveApplicationDto.ReleaseStatusCode = ReleaseStatus.NotReleased;

            leaveApplicationDto.AddDt = DateTime.Now;


            //get release strategy levels
            var relStratLevels = _context.ReleaseStrategyLevels
                .Where(
                    rl =>
                        rl.ReleaseGroupCode == leaveApplicationDto.ReleaseGroupCode &&
                        rl.ReleaseStrategy == relStrat.ReleaseStrategy
                ).ToList();


            relStrat.ReleaseStrategyLevels = relStratLevels;

            //Now for each release strategy details record create ApplReleaseStatus record

            //create a temp collection to be added to leaveapplicationdto later on
            var apps = new List<ApplReleaseStatusDto>();

            foreach (ReleaseStrategyLevels relStratReleaseStrategyLevel in relStrat.ReleaseStrategyLevels)
            {
                //get releaser ID from ReleaseAuth model
                ReleaseAuth relAuth = _context.ReleaseAuth
                    .FirstOrDefault(ra => ra.ReleaseCode == relStratReleaseStrategyLevel.ReleaseCode);


                ApplReleaseStatus appRelStat = new ApplReleaseStatus
                {
                    YearMonth = leaveApplicationDto.YearMonth,
                    ReleaseGroupCode = leaveApplicationDto.ReleaseGroupCode,
                    ApplicationId = leaveApplicationDto.LeaveAppId,
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
                apps.Add(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>(appRelStat));

                _context.ApplReleaseStatus.Add(appRelStat);
            }


            _context.LeaveApplications.Add(Mapper.Map<LeaveApplicationDto, LeaveApplications>(leaveApplicationDto));

            _context.SaveChanges();

            //Create app release status object and add all app release lines
            leaveApplicationDto.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
            leaveApplicationDto.ApplReleaseStatus.AddRange(apps);

            return Created(new Uri(Request.RequestUri + "?leaveAppId=" + leaveApplicationDto.LeaveAppId),
                leaveApplicationDto);
        }

        [HttpPut]
        public IHttpActionResult TogglePlFlag(string empUnqId)
        {
            using(DbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                Locations loc = _context.Location.FirstOrDefault();
            
                if(loc == null)
                    return BadRequest("Location table not found.");

                var plCheckLog = new PlCheckLog
                {
                    EmpUnqId = empUnqId, 
                    UpdateDate = DateTime.Now,
                    OldValue = loc.PlCheck,
                    NewValue =  loc.PlCheck != true
                };

                loc.PlCheck = loc.PlCheck != true;

                _context.PlCheckLogs.Add(plCheckLog);

                _context.SaveChanges();
                
                transaction.Commit();
            }
            return Ok();
        }
    }
}