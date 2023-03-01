using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ESS.Controllers.Api
{
    public class LeaveReportController : ApiController
    {
        private ApplicationDbContext _context;

        public LeaveReportController()
        {
            _context = new ApplicationDbContext();
        }


        public IHttpActionResult GetLeaves()
        {
            var pendingLeaves = _context.LeaveApplications
                .Include(l => l.LeaveApplicationDetails)
                .Include(l => l.Departments)
                .Include(l => l.Stations)
                .Include(l => l.Categories)
                .Include(l => l.Employee)
                .Where(l => (l.ReleaseStatusCode == ReleaseStatus.PartiallyReleased ||
                             l.ReleaseStatusCode == ReleaseStatus.InRelease))
                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>)
                .ToList();


            if (pendingLeaves.Count == 0)
                return Ok("No records found.");


            var leaveAppId = pendingLeaves.Select(l => l.LeaveAppId).ToArray();

            var appReleaseStatuses = _context.ApplReleaseStatus
                .Where(a => a.ReleaseGroupCode == ReleaseGroups.LeaveApplication &&
                            leaveAppId.Contains(a.ApplicationId))
                .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>)
                .ToList();


            foreach (LeaveApplicationDto leave in pendingLeaves)
            {
                var appRelease = appReleaseStatuses.Where(a => a.ApplicationId == leave.LeaveAppId);
                foreach (ApplReleaseStatusDto app in appRelease)
                {
                    app.ReleaserName = _context.Employees
                        .FirstOrDefault(e => e.EmpUnqId == app.ReleaseAuth)?.EmpName;
                    leave.ApplReleaseStatus.Add(app);
                }
            }

            return Ok(pendingLeaves);
        }

        public IHttpActionResult GetLeaves(string empUnqId, DateTime fromDt, DateTime toDt)
        {
            var releaseCode = _context.ReleaseAuth
                .Where(r => r.EmpUnqId == empUnqId)
                .Select(r => r.ReleaseCode)
                .ToList();


            var releaseStr = _context.ReleaseStrategyLevels
                .Where(l => releaseCode.Contains(l.ReleaseCode) &&
                            (l.ReleaseGroupCode == ReleaseGroups.LeaveApplication ||
                             l.ReleaseGroupCode == ReleaseGroups.OutStationDuty ||
                             l.ReleaseGroupCode == ReleaseGroups.CompOff
                            )
                )
                .Select(l => l.ReleaseStrategy)
                .ToList();

            var leaves = _context.LeaveApplications
                .Include(l => l.LeaveApplicationDetails)
                .Include(l => l.Departments)
                .Include(l => l.Stations)
                .Include(l => l.Categories)
                .Include(l => l.Employee)
                .Where(l =>
                    releaseStr.Contains(l.ReleaseStrategy) &&
                    l.ReleaseStatusCode != ReleaseStatus.ReleaseRejected &&
                    l.LeaveApplicationDetails.Any(
                        (d =>
                            (d.FromDt <= toDt && d.ToDt >= fromDt) ||
                            (d.FromDt >= toDt && d.ToDt <= fromDt)
                        )
                    )
                )
                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>)
                .ToList();


            foreach (var lApp in leaves)
            {
                var app = _context.ApplReleaseStatus
                    .Where(l =>
                        l.YearMonth == lApp.YearMonth &&
                        l.ReleaseGroupCode == lApp.ReleaseGroupCode &&
                        l.ApplicationId == lApp.LeaveAppId &&
                        l.IsFinalRelease
                    )
                    .ToList()
                    .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>);

                foreach (var applReleaseStatusDto in app)
                {
                    applReleaseStatusDto.ReleaserName = _context.Employees
                        .FirstOrDefault(e => e.EmpUnqId == applReleaseStatusDto.ReleaseAuth)?.EmpName;
                    lApp.ApplReleaseStatus.Add(applReleaseStatusDto);
                }
            }

            return Ok(leaves);
        }

        public IHttpActionResult GetLeaves(string empUnqId, bool employee)
        {
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult GetLeaves(DateTime fromDt, DateTime toDt, [FromBody] object requestData)
        {
            DateTime startDt = fromDt;
            DateTime endDt = toDt;

            //Create data table to return rows
            DataTable dt = new DataTable();

            if (startDt.CompareTo(endDt) > 0)
                return BadRequest("Start date is greater than end date.");

            dt.Columns.Add("EmpCode", typeof(string));
            dt.Columns.Add("EmpName", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Station", typeof(string));


            List<string> deptCodes;

            try
            {
                deptCodes = JsonConvert.DeserializeObject<List<string>>(requestData.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest("Dept codes not found!");
            }

            if (deptCodes.Count == 0)
                return BadRequest("Dept codes not found!");


            // Loop for each date
            for (DateTime tDate = startDt; tDate < endDt;)
            {
                // add this date as new column to datatable
                dt.Columns.Add(tDate.ToString("MMM-dd"), typeof(string));

                // Find all leaves on this day that are not cancelled and fully released
                var leaveOnDate = _context.LeaveApplications
                    .Include(l => l.LeaveApplicationDetails)
                    .Include(e => e.Employee)
                    .Include(d => d.Departments)
                    .Include(s => s.Stations)
                    .Where(l =>
                        l.Cancelled == false &&
                        l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                        l.LeaveApplicationDetails.Any(d => d.FromDt <= tDate && d.ToDt >= tDate) &&
                        deptCodes.Contains(l.DeptCode)
                    )
                    .ToList();


                // for each leave, 
                foreach (var l in leaveOnDate)
                {
                    var leaveApplicationDetails = l.LeaveApplicationDetails.FirstOrDefault();

                    string leaveType = leaveApplicationDetails != null ? leaveApplicationDetails.LeaveTypeCode : "";

                    var emp = dt.Select("Empcode=" + l.EmpUnqId).FirstOrDefault();

                    // If there's a record of this employee in datatable
                    if (emp != null)
                    {
                        // set leave type to this day for this employee
                        emp[tDate.ToString("MMM-dd")] = leaveType;
                    }
                    else
                    {
                        //add new row for this employee and set this day
                        DataRow dr = dt.Rows.Add();
                        dr["EmpCode"] = l.EmpUnqId;
                        dr["EmpName"] = l.Employee.EmpName;
                        dr["Department"] = l.Departments.DeptName;
                        dr["Station"] = l.Stations.StatName;
                        dr[tDate.ToString("MMM-dd")] = leaveType;
                    }
                }

                // set next day as this day
                tDate = tDate.AddDays(1);
            }

            // conver to json
            var json = JToken.FromObject(dt);
            return Ok(json);
        }

        public IHttpActionResult GetLeaves(DateTime fromDt, DateTime toDt, string deptCode = "", string statCode = "",
            string empUnqId = "")
        {
            DateTime startDt = fromDt;
            DateTime endDt = toDt;

            //Create data table to return rows
            DataTable dt = new DataTable();

            if (startDt.CompareTo(endDt) > 0)
                return BadRequest("Start date is greater than end date.");

            dt.Columns.Add("EmpCode", typeof(string));
            dt.Columns.Add("EmpName", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Station", typeof(string));


            // Loop for each date
            for (DateTime tDate = startDt; tDate < endDt;)
            {
                // add this date as new column to datatable
                dt.Columns.Add(tDate.ToString("MMM-dd"), typeof(string));

                // Find all leaves on this day that are not cancelled and fully released
                var leaveOnDate = _context.LeaveApplications
                    .Include(l => l.LeaveApplicationDetails)
                    .Include(e => e.Employee)
                    .Include(d => d.Departments)
                    .Include(s => s.Stations)
                    .Where(l =>
                        l.Cancelled == false &&
                        l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                        l.LeaveApplicationDetails.Any(d => d.FromDt <= tDate && d.ToDt >= tDate)
                    )
                    .ToList();


                // for each leave, 
                foreach (var l in leaveOnDate)
                {
                    var leaveApplicationDetails = l.LeaveApplicationDetails.FirstOrDefault();

                    string leaveType = leaveApplicationDetails != null ? leaveApplicationDetails.LeaveTypeCode : "";

                    var emp = dt.Select("Empcode=" + l.EmpUnqId).FirstOrDefault();

                    // If there's a record of this employee in datatable
                    if (emp != null)
                    {
                        // set leave type to this day for this employee
                        emp[tDate.ToString("MMM-dd")] = leaveType;
                    }
                    else
                    {
                        //add new row for this employee and set this day
                        DataRow dr = dt.Rows.Add();
                        dr["EmpCode"] = l.EmpUnqId;
                        dr["EmpName"] = l.Employee.EmpName;
                        dr["Department"] = l.Departments.DeptName;
                        dr["Station"] = l.Stations.StatName;
                        dr[tDate.ToString("MMM-dd")] = leaveType;
                    }
                }

                // set next day as this day
                tDate = tDate.AddDays(1);
            }

            // conver to json
            var json = JToken.FromObject(dt);
            return Ok(json);
        }
    }
}