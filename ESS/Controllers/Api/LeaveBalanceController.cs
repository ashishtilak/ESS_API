using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Microsoft.Ajax.Utilities;

namespace ESS.Controllers.Api
{
    public class LeaveBalanceController : ApiController
    {
        private ApplicationDbContext _context;

        public LeaveBalanceController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetLeaveBalance(int yearMonth)
        {
            var employees = _context.Employees
                .Where(e => e.Active == true && (e.WrkGrp == "COMP" || e.WrkGrp == "OUTSOURCE"))
                .Select(e => e.EmpUnqId)
                .ToList();

            List<LeaveBalanceDto> result = new List<LeaveBalanceDto>();

            foreach (var empUnqId in employees)
            {
                //get leave balance from attendance server
                //note this will not have leaves that are not posted.

                var leaveBalDto = ESS.Helpers.CustomHelper.GetLeaveBalance(empUnqId, yearMonth);

                //now get applied and released leaves that are not posted

                var leaveAppDtl = _context.LeaveApplications
                    .Where(l =>
                        l.EmpUnqId == empUnqId &&
                        l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                        l.LeaveApplicationDetails.Any(d =>
                            (
                                (d.IsPosted == LeaveApplicationDetails.NotPosted ||
                                 d.IsPosted == LeaveApplicationDetails.PartiallyPosted)
                                || d.Cancelled == true) &&
                            d.IsCancellationPosted == false
                        )
                    )
                    .Include(l => l.LeaveApplicationDetails)
                    .ToList();

                foreach (var apps in leaveAppDtl)
                {
                    foreach (var details in apps.LeaveApplicationDetails)
                    {
                        if (details.IsPosted != LeaveApplicationDetails.NotPosted)
                        {
                            if (details.IsPosted != LeaveApplicationDetails.PartiallyPosted)
                                continue;
                        }

                        try
                        {
                            var l = leaveBalDto.Single(x => x.LeaveTypeCode == details.LeaveTypeCode);

                            if (l != null)
                            {
                                if (details.Cancelled == true &&
                                    (details.IsCancellationPosted == null || details.IsCancellationPosted == false))
                                {
                                    //only reduce if full leave is not cancelled
                                    if (details.ParentId != 0)
                                        l.Availed -= details.TotalDays;
                                }
                                else
                                {
                                    if (details.LeaveTypeCode == LeaveTypes.SickLeave)
                                    {
                                        if (details.TotalDays > 3)
                                        {
                                            l.Availed = l.Availed - 3 + details.TotalDays;
                                        }
                                        else
                                            l.Availed += details.TotalDays;
                                    }

                                    //else
                                    //    l.Availed += details.TotalDays;
                                }
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }


                result.AddRange(leaveBalDto);

                //var mahi = leaveBalDto.Where(l => l.EmpUnqId == "20010581").ToList();
                //foreach (var m in mahi)
                //{
                //    m.Balance = 0;
                //}
            }

            return Ok(result);
        }

        public IHttpActionResult GetLeaveBalance(string empUnqId, int yearMonth)
        {
            //get leave balance from attendance server
            //note this will not have leaves that are not posted.

            var leaveBalDto = ESS.Helpers.CustomHelper.GetLeaveBalance(empUnqId, yearMonth);

            //now get applied and released leaves that are not posted
            var allLeaveAppIds = _context.LeaveApplications
                .Where(l => l.EmpUnqId == empUnqId &&
                            l.ReleaseStatusCode == ReleaseStatus.FullyReleased)
                .Select(l => l.LeaveAppId)
                .ToArray();


            var leavAppIds = _context.LeaveApplicationDetails
                .Where(d =>
                    (
                        d.Cancelled == true
                        || (d.IsPosted == LeaveApplicationDetails.NotPosted ||
                            d.IsPosted == LeaveApplicationDetails.PartiallyPosted)) &&
                    allLeaveAppIds.Contains(d.LeaveAppId) &&
                    d.IsCancellationPosted == false
                )
                .Select(l => l.LeaveAppId)
                .ToArray();


            var leaveAppDtl = _context.LeaveApplications
                .Where(l =>
                        l.EmpUnqId == empUnqId &&
                        l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                        leavAppIds.Contains(l.LeaveAppId)
//                    l.LeaveApplicationDetails.Any(d =>
//                        (
//                            (d.IsPosted == LeaveApplicationDetails.NotPosted ||
//                             d.IsPosted == LeaveApplicationDetails.PartiallyPosted)
//                            || d.Cancelled == true) &&
//                        d.IsCancellationPosted == false
//                    )
                )
                .Include(l => l.LeaveApplicationDetails)
                .ToList();


            string currentYear = yearMonth.ToString().Substring(0, 4);

            foreach (LeaveApplications app in
                from app in leaveAppDtl.ToList()
                let tYear = app.YearMonth.ToString().Substring(0, 4)
                where tYear != currentYear
                select app)
            {
                leaveAppDtl.Remove(app);
            }

            foreach (var apps in leaveAppDtl)
            {
                foreach (var details in apps.LeaveApplicationDetails)
                {
                    if (details.IsPosted != LeaveApplicationDetails.NotPosted)
                    {
                        if (details.IsPosted != LeaveApplicationDetails.PartiallyPosted)
                            continue;
                    }

                    try
                    {
                        var l = leaveBalDto.Single(x => x.LeaveTypeCode == details.LeaveTypeCode);

                        if (l != null)
                        {
                            if (details.Cancelled == true &&
                                (details.IsCancellationPosted == null || details.IsCancellationPosted == false))
                            {
                                //only reduce if full leave is not cancelled
                                if (details.ParentId != 0)
                                    l.Availed -= details.TotalDays;
                            }
                            else
                            {
                                if (details.LeaveTypeCode == LeaveTypes.SickLeave)
                                {
                                    if (details.TotalDays > 3)
                                    {
                                        l.Availed = l.Availed - 3 + details.TotalDays;
                                    }
                                    else
                                        l.Availed += details.TotalDays;
                                }
                                else
                                    l.Availed += details.TotalDays;
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            //var mahi = leaveBalDto.Where(l => l.EmpUnqId == "20010581").ToList();
            //foreach (var m in mahi)
            //{
            //    m.Balance = 0;
            //}

            return Ok(leaveBalDto);
        }

        public IHttpActionResult GetLeaveBalance(string empUnqId, int yearMonth, bool flag)
        {
            //get leave balance from attendance server
            //note this will not have leaves that are not posted.

            var leaveBalDto = ESS.Helpers.CustomHelper.GetLeaveBalance(empUnqId, yearMonth);
            return Ok(leaveBalDto);
        }
    }
}