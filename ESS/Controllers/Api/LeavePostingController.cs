using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using System.Data.Entity;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class LeavePostingController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public LeavePostingController()
        {
            _context = new ApplicationDbContext();
            _context.Database.Log = s => Debug.WriteLine(s);
        }

        //this will give pending leaves for posting (ESS->Leave posting)

        public IHttpActionResult GetLeaves(DateTime fromDt, DateTime toDt, bool woFlag)
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
                .Where(
                    l => l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                         l.LeaveApplicationDetails.Any(ld =>
                             (
                                 ld.IsPosted == LeaveApplicationDetails.NotPosted ||
                                 ld.IsPosted == LeaveApplicationDetails.PartiallyPosted ||
                                 (
                                     ld.IsPosted == LeaveApplicationDetails.FullyPosted
                                     &&
                                     (
                                         ld.IsCancellationPosted == false ||
                                         ld.IsCancellationPosted == null
                                     ) &&
                                     ld.Cancelled == true
                                 )
                             ) &&
                             (
                                 (ld.FromDt <= toDt && ld.ToDt >= fromDt) ||
                                 (ld.FromDt >= toDt && ld.ToDt <= fromDt)
                             )
                         )
                ).AsEnumerable()
                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>)
                .ToList();

            leaveAppDto.RemoveAll(l => l.LeaveApplicationDetails.Any(
                    ld =>
                        ld.IsPosted == LeaveApplicationDetails.NotPosted &&
                        ld.Cancelled &&
                        ld.ParentId == 0
                )
            );


            // IF Flag = true, return only Week Off applications
            if (woFlag)
            {
                // remove all leave application except Week off as leave type
                leaveAppDto.RemoveAll(l => l.LeaveApplicationDetails.Any(
                    ld => ld.LeaveTypeCode != LeaveTypes.WeekOff
                ));
            }
            else
            {
                // remove all leave application with Week off as leave type
                leaveAppDto.RemoveAll(l => l.LeaveApplicationDetails.Any(
                    ld => ld.LeaveTypeCode == LeaveTypes.WeekOff
                ));
            }


            foreach (var lApp in leaveAppDto)
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
                    lApp.ApplReleaseStatus.Add(applReleaseStatusDto);
                }
            }

            return Ok(leaveAppDto);
        }


        //this will give pending leaves for posting
        public IHttpActionResult GetLeaves(DateTime fromDt, DateTime toDt, string postingFlg)
        {
            var leaveIds = _context.LeaveApplicationDetails
                .Where(d => ((d.FromDt <= toDt && d.ToDt >= fromDt) || (d.FromDt >= toDt && d.ToDt <= fromDt))
                            && d.IsPosted == postingFlg)
                .Select(l => l.LeaveAppId)
                .ToArray();

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
                .Where(l =>
                    (l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                     l.Cancelled == false &&
                     leaveIds.Contains(l.LeaveAppId)) ||
                    (l.ReleaseStatusCode == ReleaseStatus.ReleaseRejected &&
                     l.Cancelled == false &&
                     leaveIds.Contains(l.LeaveAppId)
                    )
                ).AsEnumerable()
                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>)
                .ToList();


            //            var leaveAppDto = _context.LeaveApplications
            //                .Include(e => e.Employee)
            //                .Include(c => c.Company)
            //                .Include(cat => cat.Categories)
            //                .Include(w => w.WorkGroup)
            //                .Include(d => d.Departments)
            //                .Include(s => s.Stations)
            //                .Include(u => u.Units)
            //                .Include(r => r.ReleaseGroup)
            //                .Include(rs => rs.RelStrategy)
            //                .Include(l => l.LeaveApplicationDetails)
            //                .Where(l => l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
            //                            l.Cancelled == false &&
            //                            l.LeaveApplicationDetails.Any(
            //                                (d =>
            //                                    (
            //                                        (d.FromDt <= toDt && d.ToDt >= fromDt) ||
            //                                        (d.FromDt >= toDt && d.ToDt <= fromDt)
            //                                    ) && d.IsPosted == postingFlg
            //                                )
            //                            )
            //                            ||
            //                            l.ReleaseStatusCode == ReleaseStatus.ReleaseRejected &&
            //                            l.Cancelled == false &&
            //                            l.LeaveApplicationDetails.Any(
            //                                (d =>
            //                                    (
            //                                        (d.FromDt <= toDt && d.ToDt >= fromDt) ||
            //                                        (d.FromDt >= toDt && d.ToDt <= fromDt)
            //                                    ) && d.IsPosted == postingFlg
            //                                )
            //                            )
            //                ).AsEnumerable()
            //                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>)
            //                .ToList();


            foreach (var lApp in leaveAppDto)
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

            return Ok(leaveAppDto);
        }


        [HttpPut]
        public IHttpActionResult GetDeptLeaves(DateTime fromDt, DateTime toDt, [FromBody] object requestData)
        {
            List<string> deptCodes;
            try
            {
                deptCodes = JsonConvert.DeserializeObject<List<string>>(requestData.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid object");
            }

            var leaveIds = _context.LeaveApplicationDetails
                .Where(d => ((d.FromDt <= toDt && d.ToDt >= fromDt) || (d.FromDt >= toDt && d.ToDt <= fromDt))
                            && d.IsPosted == LeaveApplicationDetails.FullyPosted
                )
                .Select(l => l.LeaveAppId)
                .ToArray();

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
                .Where(l =>
                    deptCodes.Contains(l.DeptCode) &&
                    (l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                     l.Cancelled == false &&
                     leaveIds.Contains(l.LeaveAppId)) ||
                    (l.ReleaseStatusCode == ReleaseStatus.ReleaseRejected &&
                     l.Cancelled == false &&
                     leaveIds.Contains(l.LeaveAppId)
                    )
                ).AsEnumerable()
                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>)
                .ToList();

            foreach (var lApp in leaveAppDto)
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

            return Ok(leaveAppDto);
        }


        //this will give all leaves for export in excel
        //create a temporary class for data in rows only

        private class Leaves
        {
            //header fields
            public int LeaveAppId;
            public string EmpUnqId;
            public string EmpName;
            public string FatherName;
            public string WrkGrpDesc;
            public string CatName;
            public string DeptName;
            public string StatName;
            public string Remarks;
            public string ReleaseStrategy;
            public string Lcoation;

            public string LeaveTypeCode;
            public DateTime FromDt;
            public DateTime ToDt;
            public bool HalfDayFlag;
            public float TotalDays;
            public string LeaveReason;

            public string IsPosted;
            public bool Cancelled;
            public int ParentId;
            public bool IsCancellationPosted;
        }

        public IHttpActionResult GetLeaves(DateTime fromDt, DateTime toDt)
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
                .Where(l => l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                            l.Cancelled == false &&
                            l.LeaveApplicationDetails.Any(
                                (d =>
                                    (
                                        (d.FromDt <= toDt && d.ToDt >= fromDt) ||
                                        (d.FromDt >= toDt && d.ToDt <= fromDt)
                                    )
                                )
                            )
                )
                .ToList()
                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>);


            var leaves = (from appHdr in leaveAppDto
                from appDtl in appHdr.LeaveApplicationDetails
                select new Leaves
                {
                    LeaveAppId = appHdr.LeaveAppId,
                    EmpUnqId = appHdr.EmpUnqId,
                    EmpName = appHdr.Employee.EmpName,
                    FatherName = appHdr.Employee.FatherName,
                    WrkGrpDesc = appHdr.WorkGroup.WrkGrpDesc,
                    CatName = appHdr.CatCode,
                    DeptName = appHdr.Departments.DeptName,
                    StatName = appHdr.Stations.StatName,
                    Remarks = appHdr.Remarks,
                    Lcoation = appHdr.Employee.Location,
                    LeaveTypeCode = appDtl.LeaveTypeCode,
                    FromDt = appDtl.FromDt,
                    ToDt = appDtl.ToDt,
                    TotalDays = appDtl.TotalDays,
                    HalfDayFlag = appDtl.HalfDayFlag,
                    LeaveReason = appDtl.Remarks,
                    IsPosted = appDtl.IsPosted,
                    Cancelled = appDtl.Cancelled,
                    ParentId = appDtl.ParentId,
                    IsCancellationPosted = appDtl.IsCancellationPosted
                }).ToList();


            // FOR EDUCATION PURPOSE ONLY:
            //============================
            // ABOVE LINQ EXPRESSION CAN BE WRITTEN IN FOR LOOP AS FOLLOWS:
            //
            // BUT ABOVE ONE IS VERY FAST COMPARED TO FOR LOOP
            //
            //foreach (var appHdr in leaveAppDto)
            //{
            //    foreach (var appDtl in appHdr.LeaveApplicationDetails)
            //    {
            //        Leaves l = new Leaves
            //        {
            //            LeaveAppId = appHdr.LeaveAppId,
            //            EmpUnqId = appHdr.EmpUnqId,
            //            EmpName = appHdr.Employee.EmpName,
            //            FatherName = appHdr.Employee.FatherName,
            //            WrkGrpDesc = appHdr.WorkGroup.WrkGrpDesc,
            //            CatName = appHdr.CatCode,
            //            DeptName = appHdr.Departments.DeptName,
            //            StatName = appHdr.Stations.StatName,
            //            Remarks = appHdr.Remarks,
            //            FromDt = appDtl.FromDt,
            //            ToDt = appDtl.ToDt,
            //            TotalDays = appDtl.TotalDays,
            //            HalfDayFlag = appDtl.HalfDayFlag,
            //            LeaveReason = appDtl.Remarks,
            //            Cancelled = appDtl.Cancelled,
            //            ParentId = appDtl.ParentId,
            //            IsCancellationPosted = appDtl.IsCancellationPosted
            //        };
            //        leaves.Add(l);
            //    }
            //}


            return Ok(leaves);
        }


        public class AttdLeavePost
        {
            public int AppID { get; set; }
            public string EmpUnqID { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string LeaveTyp { get; set; }
            public bool HalfDay { get; set; }
            public bool PostedFlg { get; set; }
            public string AttdUser { get; set; }
            public string Remarks { get; set; }
            public string ERROR { get; set; }
            public string Location { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostLeaves([FromBody] object requestData)
        {
            var leavePostingDtos = JsonConvert.DeserializeObject<List<LeavePostingDto>>(requestData.ToString());

            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    foreach (LeavePostingDto dto in leavePostingDtos)
                    {
                        LeavePostingDto dto1 = dto;
                        var leaveApplications = _context.LeaveApplications
                            .Include(l => l.LeaveApplicationDetails)
                            .Where(l => l.LeaveAppId == dto1.LeaveAppId && l.YearMonth == dto1.YearMonth)
                            .ToList();

                        foreach (LeaveApplications leaveApplication in leaveApplications)
                        {
                            Employees emp = _context.Employees.Single(e => e.EmpUnqId == leaveApplication.EmpUnqId);

                            foreach (LeaveApplicationDetails leaveApplicationDetails in leaveApplication
                                .LeaveApplicationDetails.ToList())
                            {
                                // IF does not match then skip line

                                if (leaveApplicationDetails.LeaveAppId != dto1.LeaveAppId ||
                                    leaveApplicationDetails.LeaveAppItem != dto1.LeaveAppItem ||
                                    leaveApplicationDetails.YearMonth != dto1.YearMonth) continue;

                                // CHECK IF HR USER HAS REJECTED THE LEAVE
                                // IF SO, SET STATUS TO REJECT WITH REMARKS
                                // UPDATE STATUS IN LEAVE APPLICATION AND APP RELEASE TABLES

                                if (dto1.IsPosted == LeaveApplicationDetails.PostingRejected)
                                {
                                    leaveApplication.Remarks = "HR: " + dto1.Remarks;
                                    if (leaveApplication.ReleaseStatusCode == ReleaseStatus.NotReleased)
                                    {
                                        if ((leaveApplication.Cancelled ?? true) || leaveApplication.ParentId != 0)
                                        {
                                            return BadRequest("Self Cancellation of cancelled leave not allowed.");
                                        }

                                        leaveApplication.Remarks = "Cancelled by Self.";
                                    }


                                    //Change release status to "R"

                                    leaveApplication.ReleaseStatusCode = ReleaseStatus.ReleaseRejected;
                                    //l.UpdUser = dto1.UserId;
                                    //l.UpdDt = DateTime.Now;

                                    leaveApplicationDetails.PostUser = dto1.UserId;
                                    leaveApplicationDetails.PostedDt = DateTime.Now;

                                    var appRelStat = _context.ApplReleaseStatus
                                        .Where(
                                            la =>
                                                la.YearMonth == leaveApplication.YearMonth &&
                                                la.ReleaseGroupCode == leaveApplication.ReleaseGroupCode &&
                                                la.ApplicationId == leaveApplication.LeaveAppId
                                        )
                                        .ToList();

                                    //also update app release table
                                    foreach (ApplReleaseStatus b in appRelStat)
                                    {
                                        b.ReleaseStatusCode = ReleaseStatus.ReleaseRejected;
                                        // b.ReleaseDate = DateTime.Now;
                                    }

                                    leaveApplicationDetails.IsPosted = LeaveApplicationDetails.PostingRejected;

                                    //now skip this loop
                                    continue;
                                }


                                //if partial posted, then do not post in attendance
                                //only post fully posted leaves in attendance server.
                                if (dto.IsPosted == LeaveApplicationDetails.PartiallyPosted)
                                {
                                    //UPDATE POSTED FLAGE

                                    if (leaveApplicationDetails.LeaveTypeCode == LeaveTypes.SickLeave &&
                                        leaveApplicationDetails.TotalDays > 3)
                                    {
                                        int slcount = 1;
                                        DateTime dt = leaveApplicationDetails.FromDt ?? DateTime.Now;

                                        // loop for each day
                                        while (dt <= leaveApplicationDetails.ToDt)
                                        {
                                            if (slcount > 3)
                                            {
                                                // post remaining days as lwp

                                                // this day is not holiday/week off
                                                // so post SL on this day...


                                                // if this day is holiday/week off, then skip this day
                                                var holidays =
                                                    Helpers.CustomHelper.GetHolidays(dt, dt,
                                                        leaveApplication.CompCode, leaveApplication.WrkGrp,
                                                        leaveApplication.Employee.Location);
                                                if (holidays.Count != 0)
                                                {
                                                    dt = dt.AddDays(1);
                                                    continue;
                                                }


                                                var weekOffs =
                                                    Helpers.CustomHelper.GetWeeklyOff(dt, dt,
                                                        leaveApplication.EmpUnqId);
                                                if (weekOffs.Count != 0)
                                                {
                                                    dt = dt.AddDays(1);
                                                    continue;
                                                }


                                                //create object to pass
                                                AttdLeavePost attdLeaveObj =
                                                    new AttdLeavePost
                                                    {
                                                        AppID = leaveApplicationDetails.LeaveAppId,
                                                        EmpUnqID = leaveApplication.EmpUnqId,
                                                        FromDate = dt, //will cause error if dates are null
                                                        ToDate = leaveApplicationDetails.ToDt ??
                                                                 DateTime.Now
                                                                     .AddDays(-1), //will cause error if dates are null
                                                        LeaveTyp = LeaveTypes.LeaveWithoutPay,
                                                        HalfDay = false,
                                                        PostedFlg = false,
                                                        AttdUser = dto
                                                            .UserId,
                                                        Remarks = leaveApplicationDetails.Remarks,
                                                        ERROR = "",
                                                        Location = emp.Location
                                                    };

                                                attdLeaveObj = AttdPostLeave(attdLeaveObj, emp.Location,
                                                    out var result);

                                                // if there was error in leave posting,
                                                // save partial  posting flag in ESS
                                                // and return bad request

                                                if (!result)
                                                {
                                                    leaveApplicationDetails.IsPosted = dto1.IsPosted;
                                                    leaveApplicationDetails.PostUser = dto1.UserId;
                                                    leaveApplicationDetails.PostedDt = DateTime.Now;

                                                    //l.UpdUser = dto1.UserId;
                                                    //l.UpdDt = DateTime.Now;

                                                    _context.SaveChanges();

                                                    return BadRequest("Error: " + attdLeaveObj.ERROR);
                                                }

                                                dt = leaveApplicationDetails.ToDt ?? DateTime.Now;
                                            }
                                            else
                                            {
                                                // if this day is holiday/week off, then skip this day
                                                var holidays =
                                                    Helpers.CustomHelper.GetHolidays(dt, dt,
                                                        leaveApplication.CompCode, leaveApplication.WrkGrp,
                                                        leaveApplication.Employee.Location);
                                                if (holidays.Count != 0)
                                                {
                                                    dt = dt.AddDays(1);
                                                    continue;
                                                }


                                                var weekOffs =
                                                    Helpers.CustomHelper.GetWeeklyOff(dt, dt,
                                                        leaveApplication.EmpUnqId);
                                                if (weekOffs.Count != 0)
                                                {
                                                    dt = dt.AddDays(1);
                                                    continue;
                                                }
                                                //


                                                // if this is first SL, set the date as from date
                                                if (slcount == 1)
                                                {
                                                    leaveApplicationDetails.FromDt = dt;
                                                }


                                                // if this is third SL, the post all three SL to attendance server
                                                if (slcount == 3)
                                                {
                                                    //create object to pass
                                                    AttdLeavePost attdLeaveObj =
                                                        new AttdLeavePost
                                                        {
                                                            AppID = leaveApplicationDetails.LeaveAppId,
                                                            EmpUnqID = leaveApplication.EmpUnqId,
                                                            FromDate = leaveApplicationDetails.FromDt ??
                                                                       DateTime
                                                                           .Now, //will cause error if dates are null
                                                            ToDate = dt,
                                                            LeaveTyp = leaveApplicationDetails.LeaveTypeCode,
                                                            HalfDay = leaveApplicationDetails.HalfDayFlag,
                                                            PostedFlg = false,
                                                            AttdUser = dto
                                                                .UserId,
                                                            Remarks = leaveApplicationDetails.Remarks,
                                                            ERROR = "",
                                                            Location = emp.Location
                                                        };

                                                    if (attdLeaveObj.LeaveTyp == LeaveTypes.OtCOff)
                                                        attdLeaveObj.LeaveTyp = LeaveTypes.CompOff;

                                                    attdLeaveObj = AttdPostLeave(attdLeaveObj, emp.Location,
                                                        out var result);

                                                    // if there was error in leave posting,
                                                    // save partial  posting flag in ESS
                                                    // and return bad request

                                                    if (!result)
                                                    {
                                                        leaveApplicationDetails.IsPosted = dto1.IsPosted;
                                                        leaveApplicationDetails.PostUser = dto1.UserId;
                                                        leaveApplicationDetails.PostedDt = DateTime.Now;

                                                        //l.UpdUser = dto1.UserId;
                                                        //l.UpdDt = DateTime.Now;

                                                        _context.SaveChanges();

                                                        return BadRequest("Error: " + attdLeaveObj.ERROR);
                                                    }
                                                }
                                            }

                                            // increase the sl count and set next date
                                            slcount++;
                                            dt = dt.AddDays(1);
                                        }
                                    }

                                    leaveApplicationDetails.IsPosted = dto1.IsPosted;
                                    leaveApplicationDetails.PostUser = dto1.UserId;
                                    leaveApplicationDetails.PostedDt = DateTime.Now;
                                    //l.UpdUser = dto1.UserId;
                                    //l.UpdDt = DateTime.Now;
                                }
                                //added on 14.08.2019 by Ashish
                                //for forcefully "full" post partially posted SLs
                                //Algorythm:
                                // at the time of partially posting total leave of n days,
                                // 3 SL and (n-3) LWP are posted in attendance, ESS shows total n SLs
                                // Now here, reduce the days of SL to 3 working days
                                // and create new leave app line item with (n-3) LWP
                                // This will also set "Posted flag for both lines"
                                else if (dto.IsPosted == LeaveApplicationDetails.ForcefullyPosted)
                                {
                                    //this applies to only SLs...
                                    if (leaveApplicationDetails.LeaveTypeCode != LeaveTypes.SickLeave) continue;
                                    //Count SL days...
                                    int slCount = 1;
                                    DateTime dt = leaveApplicationDetails.FromDt ?? DateTime.Now;

                                    //loop for each days:
                                    while (dt <= leaveApplicationDetails.ToDt)
                                    {
                                        if (slCount <= 3)
                                        {
                                            // If this day is a holiday or week off, skip this day from count
                                            var holidays =
                                                Helpers.CustomHelper.GetHolidays(dt, dt,
                                                    leaveApplication.CompCode, leaveApplication.WrkGrp,
                                                    leaveApplication.Employee.Location);
                                            if (holidays.Count != 0)
                                            {
                                                dt = dt.AddDays(1);
                                                continue;
                                            }

                                            var weekOffs =
                                                Helpers.CustomHelper.GetWeeklyOff(dt, dt,
                                                    leaveApplication.EmpUnqId);
                                            if (weekOffs.Count != 0)
                                            {
                                                dt = dt.AddDays(1);
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            // Post remaining days as LWP:

                                            // if this day is holiday/week off, then skip this day
                                            var holidays =
                                                Helpers.CustomHelper.GetHolidays(dt, dt,
                                                    leaveApplication.CompCode, leaveApplication.WrkGrp,
                                                    leaveApplication.Employee.Location);
                                            if (holidays.Count != 0)
                                            {
                                                dt = dt.AddDays(1);
                                                continue;
                                            }

                                            var weekOffs =
                                                Helpers.CustomHelper.GetWeeklyOff(dt, dt,
                                                    leaveApplication.EmpUnqId);
                                            if (weekOffs.Count != 0)
                                            {
                                                dt = dt.AddDays(1);
                                                continue;
                                            }
                                            // this day is not holiday/week off
                                            // so post LWP on this day...

                                            //GET TEMP LEAVE APP DETAIL OBJECT FROM CONTEXT
                                            //TO APPEND NEW ITEM TO IT....
                                            //CAN'T USE THIS OBJECT BECAUSE IT'LL BREAK THE LOOP

                                            LeaveApplications tmpLd = _context.LeaveApplications
                                                .FirstOrDefault(l => l.YearMonth == leaveApplicationDetails.YearMonth &&
                                                                     l.LeaveAppId == leaveApplicationDetails
                                                                         .LeaveAppId);

                                            int maxDetailId =
                                                tmpLd.LeaveApplicationDetails.Max(d => d.LeaveAppItem);

                                            LeaveApplicationDetails newDetail = new LeaveApplicationDetails
                                            {
                                                YearMonth = leaveApplicationDetails.YearMonth,
                                                LeaveAppId = leaveApplicationDetails.LeaveAppId,
                                                LeaveAppItem = maxDetailId + 1,
                                                CompCode = leaveApplicationDetails.CompCode,
                                                WrkGrp = leaveApplicationDetails.WrkGrp,
                                                LeaveTypeCode = LeaveTypes.LeaveWithoutPay,
                                                FromDt = dt,
                                                ToDt = leaveApplicationDetails.ToDt,
                                                HalfDayFlag = false,
                                                TotalDays = leaveApplicationDetails.TotalDays - 3,
                                                IsPosted = LeaveApplicationDetails.FullyPosted,
                                                Remarks = "Forcefully posted",
                                                Cancelled = false,
                                                ParentId = 0,
                                                IsCancellationPosted = false,
                                                PostUser = dto1.UserId,
                                                PostedDt = DateTime.Now
                                            };

                                            tmpLd.LeaveApplicationDetails.Add(newDetail);

                                            // shorten the leave application TO date of this SL 
                                            leaveApplicationDetails.ToDt = dt.AddDays(-1);
                                            leaveApplicationDetails.TotalDays = 3;
                                            leaveApplicationDetails.IsPosted = LeaveApplicationDetails.FullyPosted;

                                            dt = newDetail.ToDt ?? DateTime.Today;
                                        }

                                        slCount++;
                                        dt = dt.AddDays(1);
                                    }
                                }
                                //End of change on 14.08.2019

                                else
                                {
                                    //POST LEAVE IN ATTENDANCE SERVER.
                                    //Call AttendanceServerApi's /api/leavepost method
                                    // 

                                    //DO NOT POST If full leave is cancelled and is being posted.
                                    if (leaveApplicationDetails.Cancelled == true &&
                                        leaveApplicationDetails.ParentId == 0)
                                    {
                                        leaveApplicationDetails.IsPosted = dto1.IsPosted;
                                        leaveApplicationDetails.PostUser = dto1.UserId;
                                        leaveApplicationDetails.PostedDt = DateTime.Now;
                                        //l.UpdUser = dto1.UserId;
                                        //l.UpdDt = DateTime.Now;
                                        leaveApplicationDetails.IsCancellationPosted = true;
                                        continue;
                                    }


                                    //create object to pass
                                    AttdLeavePost attdLeaveObj =
                                        new AttdLeavePost
                                        {
                                            AppID = leaveApplicationDetails.LeaveAppId,
                                            EmpUnqID = leaveApplication.EmpUnqId,
                                            FromDate =
                                                leaveApplicationDetails.FromDt ??
                                                DateTime.Now, //will cause error if dates are null
                                            ToDate =
                                                leaveApplicationDetails.ToDt ??
                                                DateTime.Now.AddDays(-1), //will cause error if dates are null
                                            LeaveTyp = leaveApplicationDetails.LeaveTypeCode,
                                            HalfDay = leaveApplicationDetails.HalfDayFlag,
                                            PostedFlg = false,
                                            AttdUser = dto.UserId,
                                            Remarks = leaveApplicationDetails.Remarks,
                                            ERROR = "",
                                            Location = emp.Location
                                        };

                                    //Change From date to To date in case of COff

                                    if (attdLeaveObj.LeaveTyp == LeaveTypes.CompOff)
                                    {
                                        //In case of CompOff, 
                                        leaveApplicationDetails.FromDt = leaveApplicationDetails.ToDt;
                                        attdLeaveObj.FromDate = attdLeaveObj.ToDate;
                                    }

                                    if (attdLeaveObj.LeaveTyp == LeaveTypes.OtCOff)
                                        attdLeaveObj.LeaveTyp = LeaveTypes.CompOff;

                                    attdLeaveObj = AttdPostLeave(attdLeaveObj, emp.Location, out var result);

                                    if (result)
                                    {
                                        //UPDATE POSTED FLAGE

                                        leaveApplicationDetails.IsPosted = dto1.IsPosted;
                                        leaveApplicationDetails.PostUser = dto1.UserId;
                                        leaveApplicationDetails.PostedDt = DateTime.Now;
                                        //l.UpdUser = dto1.UserId;
                                        //l.UpdDt = DateTime.Now;

                                        // If this leave is a cancelled leave, which was fully posted previously,
                                        // we'll set the IsCancellationPosted flag

                                        if (dto1.IsPosted == LeaveApplicationDetails.FullyPosted &&
                                            leaveApplicationDetails.Cancelled == true)
                                        {
                                            leaveApplicationDetails.IsCancellationPosted = true;
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest("Error: " + attdLeaveObj.ERROR);
                                    }
                                }
                            } //LEAVE APPLICATION DETAIL LOOP 
                        } // LEAVE APPLICATION HEADER LOOP

                        _context.SaveChanges();
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Exception realerror = ex;
                while (realerror.InnerException != null)
                    realerror = realerror.InnerException;


                return BadRequest(realerror.ToString());
            }

            return Ok();
        }


        private AttdLeavePost AttdPostLeave(AttdLeavePost attdLeaveObj, string location, out bool output)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Helpers.CustomHelper.GetAttendanceServerApi(location));

                StringContent content = new StringContent(JsonConvert.SerializeObject(attdLeaveObj),
                    Encoding.UTF8, "application/json");

                var responseTask = client.PostAsync("/api/leavepost", content);
                responseTask.Wait();

                HttpResponseMessage result = responseTask.Result;
                output = result.IsSuccessStatusCode;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<AttdLeavePost>();
                    readTask.Wait();

                    var attdLeave = readTask.Result;

                    return attdLeave;
                }
                else
                {
                    var readTask = result.Content.ReadAsAsync<AttdLeavePost>();
                    readTask.Wait();

                    var attdLeave = readTask.Result;
                    // Some error was there, return it without changing posting flags


                    return attdLeave;
                }
            }
        }
    }
}