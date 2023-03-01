using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using Antlr.Runtime.Misc;
using AutoMapper;
using ESS.Dto;
using ESS.Models;

namespace ESS.Controllers.Api
{
    public class ShiftScheduleController : ApiController
    {
        public enum ReportModes
        {
            ExcelDownload,
            CurrentMonthAll,
            CurrentMonthReleased,
            PreviousMonthAll,
            PreviousMonthReleased
        }

        private static ApplicationDbContext _context;

        public ShiftScheduleController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetOpenMonth()
        {
            var openMonth = _context.SsOpenMonth.FirstOrDefault()?.YearMonth;
            if (openMonth == null)
                return BadRequest("Open month is null. Pl check.");

            //var dateTime = new DateTime(
            //    Convert.ToInt32(openMonth.ToString().Substring(0, 4)),
            //    Convert.ToInt32(openMonth.ToString().Substring(4, 2)), 1);

            string year = openMonth.ToString().Substring(0, 4);
            string month = openMonth.ToString().Substring(4, 2);

            DateTime result;
            try
            {
                result = DateTime.ParseExact($"{year}-{month}-01", "yyyy-MM-dd", null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            return Ok(result);
        }

        public IHttpActionResult GetSchedule(int openMonth)
        {
            SsOpenMonth currentMonth = _context.SsOpenMonth.FirstOrDefault();

            var schedule = _context.ShiftSchedules
                .Include(s => s.ShiftScheduleDetails)
                .Include(s => s.Company)
                .Include(s => s.Departments)
                .Include(s => s.Stations)
                .Include(s => s.Employee)
                .Include(s => s.ReleaseStatus)
                .Where(s =>
                    s.YearMonth == currentMonth.YearMonth &&
                    s.ReleaseStatusCode != ReleaseStatus.ReleaseRejected)
                .AsEnumerable()
                .Select(Mapper.Map<ShiftSchedules, ShiftScheduleDto>)
                .ToList();

            if (schedule.Count == 0)
                return BadRequest("No records found.");

            return Ok(schedule);
        }

        public IHttpActionResult GetSchedule(string empUnqId, ReportModes mode)
        {
            // LIST OF MODES:
            // 1: EXCEL DOWNLOAD FORMAT
            // 2: CURRENT OPENMONTH - ALL SCHEDULES (RELEASED/UNRELEASED ALL)
            // 3: CURRENT OPENMONTH - ONLY RELEASED ONE
            // 4: PREVIOUS MONTH RELEASED

            // Check if employee passed is a releaser...

            try
            {
                var relCode = _context.ReleaseAuth
                    .Where(e => e.EmpUnqId == empUnqId)
                    .Select(e => e.ReleaseCode)
                    .ToList();

                relCode.RemoveAll(r => r.StartsWith("GP_"));

                if (relCode.Count == 0) return BadRequest("Employee is not a releaser.");

                var relStrLvl = _context.ReleaseStrategyLevels
                    .Where(r =>
                        relCode.Contains(r.ReleaseCode) &&
                        r.ReleaseGroupCode == ReleaseGroups.ShiftSchedule
                    )
                    .Select(r => r.ReleaseStrategy).ToArray();

                var vRelStr = _context.ReleaseStrategy
                    .Where(r => relStrLvl.Contains(r.ReleaseStrategy) &&
                                r.Active &&
                                r.ReleaseGroupCode == ReleaseGroups.ShiftSchedule)
                    .ToList();

                var allRelStr = vRelStr.Select(v => v.ReleaseStrategy).ToArray();

                var openMonth = _context.SsOpenMonth.FirstOrDefault()?.YearMonth;
                if (openMonth == null) return BadRequest("Open month not configured.");

                //carry on the good work...

                //DateTime fromDt = DateTime.Parse("01/" + openMonth.ToString().Substring(4, 2) + "/" +
                //                                 openMonth.ToString().Substring(0, 4));

                var fromDt = new DateTime(
                    Convert.ToInt32(openMonth.ToString().Substring(0, 4)),
                    Convert.ToInt32(openMonth.ToString().Substring(4, 2)), 1);

                DateTime toDt = fromDt.AddMonths(1).AddDays(-1);

                // If data required for previous month,
                if (mode == ReportModes.PreviousMonthAll || mode == ReportModes.PreviousMonthReleased)
                {
                    // then change from date, to date and openMonth to
                    fromDt = fromDt.AddMonths(-1);
                    toDt = fromDt.AddMonths(1).AddDays(-1);
                    openMonth = Convert.ToInt32(fromDt.Year.ToString("0000") + fromDt.Month.ToString("00"));
                }

                // Get all schedules from master whether released or not except rejected...

                var shedules = _context.ShiftSchedules
                    .Where(s => s.YearMonth == openMonth &&
                                allRelStr.Contains(s.ReleaseStrategy) &&
                                s.ReleaseStatusCode != ReleaseStatus.ReleaseRejected
                    ).ToList();


                //REMOVED ON 03/10/2020:
                //NOW WE'LL NOT RETURN BAD REQUEST, BUT INSTEAD RETURN SS TEMPLATE
                //FOR REMAINING EMPLOYEES WHOSE SS IS NOT UPLOADED...

                //return bad request if we've asked for blank format and any employee is found already uploaded
                //if (mode == ReportModes.ExcelDownload && shedules.Count > 0)
                //    return Content(HttpStatusCode.BadRequest, shedules);
                //REMOVED ON 03/10/2020

                if (mode != ReportModes.ExcelDownload && shedules.Count == 0)
                    return Content(HttpStatusCode.BadRequest, "No records found.");

                var outputTable = new DataTable("ShiftSchedule");
                outputTable.Columns.Add("EmpUnqId");

                outputTable.Columns.Add("EmpName");
                outputTable.Columns.Add("DeptName");
                outputTable.Columns.Add("StatName");
                outputTable.Columns.Add("Designation");
                outputTable.Columns.Add("CatName");

                //for (int dt = fromDt.Day; dt <= toDt.Day; dt++)
                DateTime loopDate = fromDt;
                for (int dt = 1; dt <= toDt.Day; dt++)
                {
                    string dayStr = dt.ToString("00") + "_" + loopDate.DayOfWeek.ToString().Substring(0, 2);
                    outputTable.Columns.Add(dayStr);
                    loopDate = loopDate.AddDays(1);
                }
                //outputTable.Columns["D" + dt.Day.ToString("00")].SetOrdinal(dt.Day);


                // Loop for each employee under this releaser
                foreach (ReleaseStrategies relStr in vRelStr)
                {
                    if (mode == ReportModes.ExcelDownload)
                        if (shedules.Any(s => s.ReleaseStrategy == relStr.ReleaseStrategy))
                            continue;

                    // loop for each day of month
                    DataRow dr = outputTable.NewRow();
                    dr["EmpUnqId"] = relStr.ReleaseStrategy;

                    EmployeeDto employeeDto = _context.Employees
                        .Where(e => e.EmpUnqId == relStr.ReleaseStrategy)
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
                        .Single(e => e.EmpUnqId == relStr.ReleaseStrategy);

                    dr["EmpName"] = employeeDto.EmpName;
                    dr["DeptName"] = employeeDto.DeptName;
                    dr["StatName"] = employeeDto.StatName;
                    dr["Designation"] = employeeDto.DesgName;
                    dr["CatName"] = employeeDto.CatName;

                    if (mode == ReportModes.ExcelDownload)
                    {
                        outputTable.Rows.Add(dr);
                        continue;
                    }

                    // If there are multiple schedules uploaded for this employee, 
                    // show only last one

                    List<ShiftSchedules> schedule;

                    if (mode == ReportModes.CurrentMonthReleased || mode == ReportModes.PreviousMonthReleased)
                        schedule = _context.ShiftSchedules
                            .Where(s =>
                                s.ReleaseStrategy == relStr.ReleaseStrategy &&
                                s.YearMonth == openMonth &&
                                s.ReleaseStatusCode == ReleaseStatus.FullyReleased)
                            .OrderByDescending(s => s.ScheduleId)
                            .ToList();
                    else
                        schedule = _context.ShiftSchedules
                            .Where(s =>
                                s.ReleaseStrategy == relStr.ReleaseStrategy &&
                                s.YearMonth == openMonth)
                            .OrderByDescending(s => s.ScheduleId)
                            .ToList();

                    if (!schedule.Any())
                        continue;

                    ShiftSchedules sch = schedule.First();

                    var schDtl = _context.ShiftScheduleDetails
                        .Where(s =>
                            s.YearMonth == sch.YearMonth &&
                            s.ScheduleId == sch.ScheduleId &&
                            s.EmpUnqId == sch.EmpUnqId).ToList();

                    for (DateTime dt = fromDt; dt <= toDt;)
                    {
                        string dayStr = dt.Day.ToString("00") + "_" + dt.DayOfWeek.ToString().Substring(0, 2);

                        //dr["D" + dt.Day.ToString("00")] = schDtl.First(s => s.ShiftDay == dt.Day).ShiftCode ?? "";
                        dr[dayStr] = schDtl.First(s => s.ShiftDay == dt.Day).ShiftCode ?? "";
                        dt = dt.AddDays(1);
                    }

                    outputTable.Rows.Add(dr);
                }

                return Ok(outputTable);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        public IHttpActionResult GetSchedule(string empUnqId)
        {
            // set output table
            var outputTable = new DataTable("ShiftSchedule");
            outputTable.Columns.Add("EmpUnqId");

            outputTable.Columns.Add("EmpName");
            outputTable.Columns.Add("DeptName");
            outputTable.Columns.Add("StatName");
            outputTable.Columns.Add("Designation");
            outputTable.Columns.Add("CatName");


            var openMonth = _context.SsOpenMonth.FirstOrDefault()?.YearMonth;
            if (openMonth == null) return BadRequest("Open month not configured.");

            var fromDt = new DateTime(
                Convert.ToInt32(openMonth.ToString().Substring(0, 4)),
                Convert.ToInt32(openMonth.ToString().Substring(4, 2)), 1);

            DateTime toDt = fromDt.AddMonths(1).AddDays(-1);

            // fill output table with columns = days of months
            DateTime loopDate = fromDt;
            for (int dt = 1; dt <= toDt.Day; dt++)
            {
                string dayStr = dt.ToString("00");
                outputTable.Columns.Add(dayStr);
                loopDate = loopDate.AddDays(1);
            }


            EmployeeDto employeeDto = _context.Employees
                .Where(e => e.EmpUnqId == empUnqId)
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
                .Single(e => e.EmpUnqId == empUnqId);

            var schedules = _context.ShiftSchedules
                .Where(s => s.YearMonth == openMonth &&
                            s.EmpUnqId == empUnqId &&
                            s.ReleaseStatusCode != ReleaseStatus.ReleaseRejected
                ).ToList();
            if (schedules.Count == 0)
                return BadRequest("No records found.");

            foreach (var schDtl in schedules.Select(sch => _context.ShiftScheduleDetails
                .Where(s =>
                    s.YearMonth == sch.YearMonth &&
                    s.ScheduleId == sch.ScheduleId &&
                    s.EmpUnqId == sch.EmpUnqId).ToList()))
            {
                DataRow dr = outputTable.NewRow();
                dr["EmpUnqId"] = empUnqId;
                dr["EmpName"] = employeeDto.EmpName;
                dr["DeptName"] = employeeDto.DeptName;
                dr["StatName"] = employeeDto.StatName;
                dr["Designation"] = employeeDto.DesgName;
                dr["CatName"] = employeeDto.CatName;

                for (DateTime dt = fromDt; dt <= toDt;)
                {
                    string dayStr = dt.Day.ToString("00"); // + "_" + dt.DayOfWeek.ToString().Substring(0, 2);

                    //dr["D" + dt.Day.ToString("00")] = schDtl.First(s => s.ShiftDay == dt.Day).ShiftCode ?? "";
                    dr[dayStr] = schDtl.First(s => s.ShiftDay == dt.Day).ShiftCode ?? "";
                    dt = dt.AddDays(1);
                }

                outputTable.Rows.Add(dr);
            }

            return Ok(outputTable);
        }

        public IHttpActionResult GetSchedule(DateTime fromDate, DateTime toDate, int openMonth)
        {
            try
            {
                //var openMonth = _context.SsOpenMonth.FirstOrDefault()?.YearMonth;
                //if (openMonth == null) return BadRequest("Open month not configured.");

                //carry on the good work...

                //DateTime fromDt = DateTime.Parse("01/" + openMonth.ToString().Substring(4, 2) + "/" +
                //                                 openMonth.ToString().Substring(0, 4));

                DateTime fromDt = new DateTime(
                    Convert.ToInt32(openMonth.ToString().Substring(0, 4)),
                    Convert.ToInt32(openMonth.ToString().Substring(4, 2)),
                    1
                );

                //var fromDt = new DateTime(fromDate.Year,
                //    fromDate.Month, 1);
                DateTime toDt = fromDt.AddMonths(1).AddDays(-1);

                //var openMonth = Convert.ToInt32(fromDate.Year.ToString() + fromDate.Month.ToString("00"));

                var shedules = _context.ShiftSchedules
                    .Where(s => s.ReleaseDt >= fromDate && s.AddDt <= toDate &&
                                s.YearMonth == openMonth &&
                                s.ReleaseStatusCode == ReleaseStatus.FullyReleased
                    ).ToList();

                var outputTable = new DataTable("ShiftSchedule");
                outputTable.Columns.Add("EmpUnqId");
                outputTable.Columns.Add("EmpName");
                outputTable.Columns.Add("DeptName");
                outputTable.Columns.Add("StatName");
                outputTable.Columns.Add("Designation");
                outputTable.Columns.Add("CatName");

                for (DateTime dt = fromDt; dt <= toDt;)
                {
                    outputTable.Columns.Add("D" + dt.Day.ToString("00"));
                    dt = dt.AddDays(1);
                }

                outputTable.Columns.Add("FinalReleaseDate", typeof(DateTime));
                outputTable.Columns.Add("ReleaseUser");
                outputTable.Columns.Add("AddDate", typeof(DateTime));
                outputTable.Columns.Add("AddUser");

                // Loop for each employee under this releaser
                foreach (ShiftSchedules sch in shedules)
                {
                    // loop for each day of month
                    DataRow dr = outputTable.NewRow();
                    dr["EmpUnqId"] = sch.ReleaseStrategy;

                    EmployeeDto employeeDto = _context.Employees
                        .Where(e => e.EmpUnqId == sch.ReleaseStrategy)
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
                        .Single(e => e.EmpUnqId == sch.ReleaseStrategy);

                    dr["EmpName"] = employeeDto.EmpName;
                    dr["DeptName"] = employeeDto.DeptName;
                    dr["StatName"] = employeeDto.StatName;
                    dr["Designation"] = employeeDto.DesgName;
                    dr["CatName"] = employeeDto.CatName;

                    var schDtl = _context.ShiftScheduleDetails
                        .Where(s =>
                            s.YearMonth == sch.YearMonth &&
                            s.ScheduleId == sch.ScheduleId &&
                            s.EmpUnqId == sch.EmpUnqId).ToList();

                    for (DateTime dt = fromDt; dt <= toDt;)
                    {
                        Debug.Print(sch.EmpUnqId);
                        dr["D" + dt.Day.ToString("00")] = schDtl.First(s => s.ShiftDay == dt.Day).ShiftCode ?? "";
                        dt = dt.AddDays(1);
                    }

                    dr["FinalReleaseDate"] = sch.ReleaseDt;
                    dr["ReleaseUser"] = sch.ReleaseUser;
                    dr["AddDate"] = sch.AddDt;
                    dr["AddUser"] = sch.AddUser;

                    outputTable.Rows.Add(dr);
                }

                return Ok(outputTable);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        public IHttpActionResult UploadSchedule(string empUnqId)
        {
            HttpContext httpContext = HttpContext.Current;

            // Check for any uploaded file  
            if (httpContext.Request.Files.Count <= 0) return BadRequest("NO FILES???");

            // Create new folder if does not exist.

            try
            {
                string folder = HostingEnvironment.MapPath(@"~/App_Data/tmp/");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder ?? throw new InvalidOperationException("Folder not found"));

                //Loop through uploaded files 
                for (int i = 0; i < httpContext.Request.Files.Count;)
                {
                    HttpPostedFile postedFile = httpContext.Request.Files[i];

                    if (postedFile.ContentLength <= 0) return BadRequest("Passed file is null!");

                    try
                    {
                        string fileExt = Path.GetExtension(postedFile.FileName);
                        if (fileExt != ".csv") return BadRequest("Invalid file extension.");

                        postedFile.SaveAs(
                            HostingEnvironment.MapPath(@"~/App_Data/tmp/") +
                            empUnqId + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv");

                        SsOpenMonth ssOpenMonth = _context.SsOpenMonth.FirstOrDefault();
                        int openMonth = 0;

                        if (ssOpenMonth != null)
                            openMonth = ssOpenMonth.YearMonth;
                        else
                            return BadRequest("SS Open month not configured.");


                        using (var reader = new StreamReader(postedFile.InputStream))
                        {
                            // This will just rip off the first Header line off the excel file
                            var header = reader.ReadLine()?.Split(',');
                            if (header == null)
                                return BadRequest("Header is null!!");

                            //validate columns:
                            //first check number of columns. 
                            //first 5 are fixed, and rest are number of days of month

                            var fromDt = new DateTime(
                                Convert.ToInt32(openMonth.ToString().Substring(0, 4)),
                                Convert.ToInt32(openMonth.ToString().Substring(4, 2)), 1);

                            for (int j = 6; j < header.Length; j++)
                            {
                                if (header[j].Length < 2)
                                    return BadRequest("Invalid template format. Pl don't change column order");

                                bool result = int.TryParse(header[j].Substring(0, 2), out int val);
                                if (result && j - 5 == val) continue;

                                return BadRequest("Invalid template format. Pl don't change column order");
                            }

                            //column validation over


                            var schedules = new List<ShiftScheduleDto>();

                            while (!reader.EndOfStream)
                            {
                                var row = reader.ReadLine()?.Split(',');
                                if (row == null) continue;

                                var sch = new ShiftScheduleDto
                                {
                                    YearMonth = openMonth,
                                    EmpUnqId = row[0],
                                    ReleaseGroupCode = ReleaseGroups.ShiftSchedule,
                                    ReleaseStrategy = row[0],
                                    ReleaseStatusCode = ReleaseStatus.PartiallyReleased,
                                    AddDt = DateTime.Now,
                                    AddUser = empUnqId,
                                    ShiftScheduleDetails = new List<ShiftScheduleDetailDto>()
                                };

                                for (int rowIndex = 6; rowIndex < row.Length; rowIndex++)
                                {
                                    var schLine = new ShiftScheduleDetailDto
                                    {
                                        YearMonth = sch.YearMonth,
                                        ShiftDay = rowIndex - 5,
                                        ShiftCode = row[rowIndex]
                                    };

                                    sch.ShiftScheduleDetails.Add(schLine);
                                }

                                schedules.Add(sch);
                            }

                            var errors = SchValidate(schedules, empUnqId);

                            // Check if any unreleased schedule exist for any of the employee...

                            var schEmps = schedules.Select(s => s.EmpUnqId).ToArray();
                            var existingSch = _context.ShiftSchedules
                                .Where(s => s.YearMonth == openMonth &&
                                            schEmps.Contains(s.EmpUnqId) &&
                                            s.ReleaseStatusCode != ReleaseStatus.FullyReleased &&
                                            s.ReleaseStatusCode != ReleaseStatus.ReleaseRejected)
                                .Select(e => e.EmpUnqId)
                                .ToArray();

                            if (existingSch.Length > 0)
                                errors.Add("A shift schedule already uploaded for " + existingSch.Length +
                                           " employees, still pending for release.");

                            if (errors.Count > 0)
                                return Content(HttpStatusCode.BadRequest, errors);


                            // OUR SHIFT SCHEDULE IS OK TO UPLOAD...

                            int maxId = _context.ShiftSchedules
                                .Select(s => (int?) s.ScheduleId)
                                .Max() ?? 0;

                            maxId++;

                            using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                            {
                                foreach (ShiftScheduleDto sch in schedules)
                                {
                                    Employees emp = _context.Employees.FirstOrDefault(e => e.EmpUnqId == sch.EmpUnqId);

                                    if (emp == null) return BadRequest("Employee is null!!!");

                                    sch.ScheduleId = maxId;
                                    sch.CompCode = emp.CompCode;
                                    sch.WrkGrp = emp.WrkGrp;
                                    sch.UnitCode = emp.UnitCode;
                                    sch.DeptCode = emp.DeptCode;
                                    sch.StatCode = emp.StatCode;

                                    foreach (ShiftScheduleDetailDto detail in sch.ShiftScheduleDetails)
                                        detail.ScheduleId = maxId;

                                    // get the release strategy
                                    ReleaseStrategies relStrat = _context.ReleaseStrategy
                                        .FirstOrDefault(
                                            r => r.ReleaseStrategy == sch.EmpUnqId &&
                                                 r.ReleaseGroupCode == ReleaseGroups.ShiftSchedule &&
                                                 r.Active
                                        );

                                    if (relStrat == null)
                                    {
                                        errors.Add("Release strategy not found for emp: " + sch.EmpUnqId);
                                        continue;
                                    }

                                    relStrat.ReleaseStrategyLevels = new ListStack<ReleaseStrategyLevels>();
                                    var relStratLvl = _context.ReleaseStrategyLevels
                                        .Where(r => r.ReleaseGroupCode == ReleaseGroups.ShiftSchedule &&
                                                    r.ReleaseStrategy == relStrat.ReleaseStrategy)
                                        .ToList();

                                    relStrat.ReleaseStrategyLevels.AddRange(relStratLvl);

                                    var apps = new List<ApplReleaseStatusDto>();

                                    foreach (ReleaseStrategyLevels relStratReleaseStrategyLevel in relStrat
                                        .ReleaseStrategyLevels)
                                    {
                                        var appRelStat = new ApplReleaseStatus
                                        {
                                            YearMonth = sch.YearMonth,
                                            ReleaseGroupCode = sch.ReleaseGroupCode,
                                            ApplicationId = sch.ScheduleId,
                                            ReleaseStrategy = relStratReleaseStrategyLevel.ReleaseStrategy,
                                            ReleaseStrategyLevel = relStratReleaseStrategyLevel.ReleaseStrategyLevel,
                                            ReleaseCode = relStratReleaseStrategyLevel.ReleaseCode,
                                            ReleaseStatusCode = ReleaseStatus.NotReleased,
                                            ReleaseDate = null,
                                            ReleaseAuth = empUnqId,
                                            IsFinalRelease = relStratReleaseStrategyLevel.IsFinalRelease
                                        };


                                        if (relStratReleaseStrategyLevel.ReleaseStrategyLevel <= 1)
                                        {
                                            appRelStat.ReleaseStatusCode = ReleaseStatus.FullyReleased;
                                            appRelStat.ReleaseDate = DateTime.Now;
                                        }
                                        else if (relStratReleaseStrategyLevel.ReleaseStrategyLevel == 2)
                                        {
                                            appRelStat.ReleaseStatusCode = ReleaseStatus.InRelease;
                                            appRelStat.ReleaseDate = null;
                                        }
                                        else
                                        {
                                            appRelStat.ReleaseStatusCode = ReleaseStatus.NotReleased;
                                            appRelStat.ReleaseDate = null;
                                        }

                                        //add to collection
                                        apps.Add(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>(appRelStat));

                                        _context.ApplReleaseStatus.Add(appRelStat);
                                    }

                                    _context.ShiftSchedules.Add(Mapper.Map<ShiftScheduleDto, ShiftSchedules>(sch));

                                    _context.SaveChanges();

                                    //transaction.Commit();
                                    sch.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
                                    sch.ApplReleaseStatus.AddRange(apps);
                                }


                                if (errors.Count > 0)
                                {
                                    transaction.Rollback();
                                    return Content(HttpStatusCode.BadRequest, errors);
                                }

                                transaction.Commit();
                            }

                            return Ok(schedules);
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Error:" + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            return Ok();
        }

        //will be used from update schedule controller also...
        public static List<string> SchValidate(List<ShiftScheduleDto> schedule, string empUnqId)
        {
            //get all shifts in a list
            if (_context == null) _context = new ApplicationDbContext();

            List<string> shifts = _context.Shifts.Select(s => s.ShiftCode).ToList();

            //REMEMBER TO ADD WO manually AFTER SHIFT SYNC SO THAT IT APPEARS ABOVE...

            var errors = new List<string>();

            // check for duplicate rows..
            {
                var duplicateEntries = schedule.GroupBy(e => e.EmpUnqId).Where(g => g.Count() > 1).ToList();
                if (duplicateEntries.Count > 0)
                    errors.Add("Duplicated records found in file.");
            }

            var empCodes = schedule.Select(e => e.EmpUnqId).Distinct().ToArray();

            var emps = _context.Employees.Where(
                    e => empCodes.Contains(e.EmpUnqId))
                .ToList();


            foreach (ShiftScheduleDto dto in schedule)
            {
                // Check if employee exist
                Employees emp = emps.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp == null)
                {
                    errors.Add("Emp: " + dto.EmpUnqId + " does not exist.");
                    continue;
                }

                // see if employee is under his release...
                if (!CheckReleaser(dto.EmpUnqId, empUnqId))
                    errors.Add("Emp:" + dto.EmpUnqId + ": Not under release of " + empUnqId);

                // Now check that each Shift of this is not blank and is valid

                //    foreach (ShiftScheduleDetails detail in dto.ShiftScheduleDetails)
                //    if (!shifts.Contains(detail.ShiftCode))
                //        errors.Add("Emp:" + dto.EmpUnqId + ": Day " + detail.ShiftDay + " shift codes is invalid (" +
                //                   detail.ShiftCode + ")");
                //   ABOVE CODE CONVERTED TO LINQ BELOW :-
                //
                //    errors.AddRange(
                //    from detail in dto.ShiftScheduleDetails
                //    where !shifts.Contains(detail.ShiftCode)
                //    select "Emp:" + dto.EmpUnqId + ": Day " + detail.ShiftDay + " shift codes is invalid (" +
                //           detail.ShiftCode + ")");
                //
                //  WHICH IS THEN CONVERTED TO BELOW LAMBDA EXPRESSION:

                errors.AddRange(
                    dto.ShiftScheduleDetails.Where(d => !shifts.Contains(d.ShiftCode))
                        .Select(detail => "Emp:" + dto.EmpUnqId + ": Day " + detail.ShiftDay +
                                          " shift codes is invalid (" + detail.ShiftCode + ")")
                );

                // Count number of week offs -- should not be more than 5

                int countWo = dto.ShiftScheduleDetails.Count(e => e.ShiftCode == "WO");
                if (countWo > 5)
                    errors.Add("Emp:" + dto.EmpUnqId + ": Week off more than 5");


                // GET previous month week off

                var prevYear = int.Parse(dto.YearMonth.ToString().Substring(0, 4)); // previous year
                var prevMonth = int.Parse(dto.YearMonth.ToString().Substring(4, 2)); // previous month
                var dt = new DateTime(prevYear, prevMonth, 1).AddDays(-1); // construct date
                var prevYearMonth = int.Parse(dt.Year.ToString("0000") + dt.Month.ToString("00")); // previous yearmonth

                //var prevWeekOff = _context.ShiftScheduleDetails
                //    .FirstOrDefault(s => s.YearMonth == prevYearMonth &&
                //                         s.EmpUnqId == dto.EmpUnqId &&
                //                         s.ShiftCode == "WO");


                //if (prevWeekOff != null)
                //{
                //    var prevDate = new DateTime(dt.Year, dt.Month, prevWeekOff.ShiftDay);
                //    prevWoDay = prevDate.DayOfWeek;
                //}

                var prevWoDay = DayOfWeek.Sunday;

                // Get schedule from ATTD
                AttdShiftScheduleDto attdSchedule = Helpers.CustomHelper
                    .GetattdShiftSchedule(prevYearMonth, dto.EmpUnqId, emp.Location);

                if (attdSchedule.EmpUnqId != null)
                {
                    foreach (PropertyInfo field in attdSchedule.GetType().GetProperties())
                    {
                        if (field.Name.ToUpper() == "ITEM") break;

                        var value = field.GetValue(attdSchedule).ToString();
                        if (value != "WO") continue;

                        int day = int.Parse(field.Name.Substring(1, 2));
                        var prevDate = new DateTime(dt.Year, dt.Month, day);
                        prevWoDay = prevDate.DayOfWeek;
                    }
                }

                // Check for week off gaps
                List<ShiftScheduleDetailDto> weekOffs =
                    dto.ShiftScheduleDetails.Where(s => s.ShiftCode == "WO").ToList();

                var prevDay = 0;

                var thisYear = int.Parse(dto.YearMonth.ToString().Substring(0, 4));
                var thisMonth = int.Parse(dto.YearMonth.ToString().Substring(4, 2));


                foreach (ShiftScheduleDetailDto wo in weekOffs)
                {
                    if (prevDay != 0 && wo.ShiftDay - prevDay != 7)
                    {
                        errors.Add("WO must be on same week days. Check week off on " + wo.ShiftDay);
                        break;
                    }

                    var thisWoDay = new DateTime(thisYear, thisMonth, wo.ShiftDay).DayOfWeek;

                    if (attdSchedule.EmpUnqId != null)
                    {
                        if (thisWoDay != prevWoDay)
                        {
                            errors.Add("WO must be on same week day as previous schedule. Check week off of " +
                                       dto.EmpUnqId + " on " + wo.ShiftDay);
                            break;
                        }
                    }

                    prevDay = wo.ShiftDay;
                }
            }

            return errors;
        }

        private static bool CheckReleaser(string empUnqId, string releaser)
        {
            //Get release strategy of the employee

            ReleaseStrategies releaseStr = _context.ReleaseStrategy.FirstOrDefault(e =>
                e.ReleaseStrategy == empUnqId && e.Active && e.ReleaseGroupCode == ReleaseGroups.ShiftSchedule);

            if (releaseStr == null)
                return false;

            //get release codes of release strategy levels of above 
            var relCodes = _context.ReleaseStrategyLevels.Where(
                    r => r.ReleaseGroupCode == ReleaseGroups.ShiftSchedule &&
                         r.ReleaseStrategy == releaseStr.ReleaseStrategy)
                .Select(c => c.ReleaseCode)
                .ToArray();

            // Get release auth and compare it to the releaser...
            ReleaseAuth relCode =
                _context.ReleaseAuth.FirstOrDefault(e => e.EmpUnqId == releaser && relCodes.Contains(e.ReleaseCode));

            // if release code is null, return false, else return tru
            return relCode != null;
        }

        [HttpPut]
        public IHttpActionResult ChangeOpenMonth(int yearMonth)
        {
            SsOpenMonth currentMonth = _context.SsOpenMonth.First() ?? new SsOpenMonth();

            currentMonth.YearMonth = yearMonth;

            string sql = "UPDATE SsOpenMonths " +
                         "Set YearMonth = " + currentMonth.YearMonth + ", " +
                         "PostingEnabled = 1 ";

            _context.Database.ExecuteSqlCommand(sql);

            return Ok();
        }
    }
}