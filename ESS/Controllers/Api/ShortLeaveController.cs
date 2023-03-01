using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;
using System.Data.Entity;

namespace ESS.Controllers.Api
{
    public class ShortLeaveController : ApiController
    {
        private ApplicationDbContext _context;
        private const int MaxDaysToConsider = 30;

        public ShortLeaveController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetShortLeaves(string empUnqId)
        {
            var leaves = new LeaveApplicationController();
            return Ok(leaves.GetLeaveApplication(empUnqId, MaxDaysToConsider));
        }


        [HttpPost]
        [ActionName("CreateShortLeave")]
        public IHttpActionResult CreateShortLeave([FromBody] object requestData)
        {
            var leaveCancelDto = JsonConvert.DeserializeObject<LeaveApplicationDto>(requestData.ToString());

            if (!ModelState.IsValid)
                return BadRequest();

            //Now check if ID is passed, return bad request if id is also passed
            if (leaveCancelDto.LeaveAppId != 0) return BadRequest("What should I do this ID???");

            //Get Max ID
            int maxId;
            try
            {
                maxId = _context.LeaveApplications.Max(i => i.LeaveAppId);
            }
            catch
            {
                maxId = 0;
            }

            leaveCancelDto.LeaveAppId = maxId + 1;


            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    //set this if not already:
                    //leaveCancelDto.Cancelled = true;

                    foreach (var leaveApplicationDetailDto in leaveCancelDto.LeaveApplicationDetails)
                    {
                        leaveApplicationDetailDto.LeaveAppId = leaveCancelDto.LeaveAppId;

                        //this is necessory to cancel the leave:
                        //leaveApplicationDetailDto.Cancelled = true;
                        leaveApplicationDetailDto.ParentId = leaveCancelDto.ParentId;

                        //posting status to "N"
                        leaveApplicationDetailDto.IsPosted = LeaveApplicationDetails.NotPosted;
                    }


                    //update cancelled leave Id in ParentId field of parent leave
                    //and set cancelled flag for details too

                    var parentleave = _context.LeaveApplications
                        .Include(l => l.LeaveApplicationDetails)
                        .Where(l => l.LeaveAppId == leaveCancelDto.ParentId)
                        .ToList();

                    foreach (var dtl in parentleave)
                    {
                        dtl.ParentId = leaveCancelDto.LeaveAppId;
                        dtl.Cancelled = true;

                        foreach (var parentLeaveDtl in dtl.LeaveApplicationDetails)
                        {
                            parentLeaveDtl.ParentId = leaveCancelDto.LeaveAppId;
                            parentLeaveDtl.Cancelled = true;
                        }
                    }


                    //add code for application release status table

                    //first get release strategy details based on comp, wrkgrp, unit, dept, stat and cat code
                    var relStrat = _context.ReleaseStrategy
                        .Single(
                            r =>
                                r.ReleaseGroupCode ==
                                leaveCancelDto.ReleaseGroupCode && //Get the same as LeaveApplication
                                r.ReleaseStrategy == leaveCancelDto.EmpUnqId &&
                                //r.CompCode == leaveCancelDto.CompCode &&
                                //r.WrkGrp == leaveCancelDto.WrkGrp &&
                                //r.UnitCode == leaveCancelDto.UnitCode &&
                                //r.DeptCode == leaveCancelDto.DeptCode &&
                                //r.StatCode == leaveCancelDto.StatCode &&
                                //r.SecCode == leaveCancelDto.SecCode &&
                                ////r.CatCode == leaveCancelDto.CatCode &&
                                //r.IsHod == leaveCancelDto.IsHod &&
                                r.Active == true
                        );

                    if (relStrat == null)
                        return BadRequest("Release strategy not configured.");

                    leaveCancelDto.ReleaseGroupCode = leaveCancelDto.ReleaseGroupCode; //It'll remain same.. i.e., LC
                    leaveCancelDto.ReleaseStrategy = relStrat.ReleaseStrategy;
                    leaveCancelDto.ReleaseStatusCode = ReleaseStatus.NotReleased;


                    //get release strategy levels
                    var relStratLevels = _context.ReleaseStrategyLevels
                        .Where(
                            rl =>
                                rl.ReleaseGroupCode ==
                                ReleaseGroups.LeaveApplication && //Release group to be LeaveApplication
                                rl.ReleaseStrategy == relStrat.ReleaseStrategy
                        ).ToList();


                    relStrat.ReleaseStrategyLevels = relStratLevels;


                    //Now for each release strategy details record create ApplReleaseStatus record

                    foreach (var relStratReleaseStrategyLevel in relStrat.ReleaseStrategyLevels)
                    {
                        //get releaser ID from ReleaseAuth model
                        var relAuth = _context.ReleaseAuth
                            .FirstOrDefault(ra => ra.ReleaseCode == relStratReleaseStrategyLevel.ReleaseCode);


                        ApplReleaseStatus appRelStat = new ApplReleaseStatus
                        {
                            YearMonth = leaveCancelDto.YearMonth,
                            ReleaseGroupCode = leaveCancelDto.ReleaseGroupCode,
                            ApplicationId = leaveCancelDto.LeaveAppId,
                            ReleaseStrategy = relStratReleaseStrategyLevel.ReleaseStrategy,
                            ReleaseStrategyLevel = relStratReleaseStrategyLevel.ReleaseStrategyLevel,
                            ReleaseCode = relStratReleaseStrategyLevel.ReleaseCode,
                            ReleaseStatusCode =
                                relStratReleaseStrategyLevel.ReleaseStrategyLevel == 1
                                    ? ReleaseStatus.InRelease
                                    : ReleaseStatus.NotReleased,
                            ReleaseDate = null,
                            ReleaseAuth = (relAuth.EmpUnqId ?? ""),
                            IsFinalRelease = relStratReleaseStrategyLevel.IsFinalRelease
                        };

                        _context.ApplReleaseStatus.Add(appRelStat);
                    }


                    _context.LeaveApplications.Add(Mapper.Map<LeaveApplicationDto, LeaveApplications>(leaveCancelDto));


                    _context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            return Created(new Uri(Request.RequestUri + "?leaveAppId=" + leaveCancelDto.LeaveAppId), leaveCancelDto);
        }


        [HttpPost]
        [ActionName("CancelLeave")]
        public IHttpActionResult CancelLeave(string releaseGroupCode, int leaveAppId)
        {
            var leaveApp = _context.LeaveApplications
                .Include(l => l.LeaveApplicationDetails)
                .SingleOrDefault(l => l.ReleaseGroupCode == releaseGroupCode && l.LeaveAppId == leaveAppId);


            if (leaveApp == null)
                return BadRequest("Invalid Leave application code.");


            // Set cancelled flag, and also reset release status to not released
            leaveApp.Cancelled = true;
            leaveApp.ReleaseStatusCode = ReleaseStatus.NotReleased;

            // Set cancelled flag for details table also
            foreach (var detail in leaveApp.LeaveApplicationDetails)
            {
                detail.Cancelled = true;
            }


            // Reset release status of application release table

            var appRel = _context.ApplReleaseStatus
                .Where(a =>
                    a.YearMonth == leaveApp.YearMonth &&
                    a.ReleaseGroupCode == leaveApp.ReleaseGroupCode &&
                    a.ApplicationId == leaveApp.LeaveAppId)
                .OrderBy(a => a.ReleaseStrategyLevel)
                .ToList();


            List<ApplReleaseStatusDto> apps = new List<ApplReleaseStatusDto>();
            foreach (var appRelDtl in appRel)
            {
                appRelDtl.ReleaseStatusCode = appRelDtl.ReleaseStrategyLevel == 1
                    ? ReleaseStatus.InRelease
                    : ReleaseStatus.NotReleased;

                apps.Add(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>(appRelDtl));
            }


            var leaveAppDto = Mapper.Map<LeaveApplications, LeaveApplicationDto>(leaveApp);
            leaveAppDto.ApplReleaseStatus = new List<ApplReleaseStatusDto>();

            leaveAppDto.ApplReleaseStatus.AddRange(apps);


            _context.SaveChanges();

            return Ok(leaveAppDto);
        }
    }
}