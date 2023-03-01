using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Antlr.Runtime.Misc;
using AutoMapper;
using ESS.Dto;
using ESS.Models;

namespace ESS.Controllers.Api
{
    public class ShiftScheduleUpdateController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public ShiftScheduleUpdateController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetSchedule(string empUnqId)
        {
            //algorythm is simple:
            //For releaser passed, find the employees for which ss is already uploaded & released
            //From today's date to this month's end date, create a template with existing uploaded ss

            try
            {
                var relCode = _context.ReleaseAuth
                    .Where(e => e.EmpUnqId == empUnqId)
                    .Select(e => e.ReleaseCode)
                    .ToArray();

                if (relCode.Length == 0) return BadRequest("Employee is not a releaser.");

                var relStrLvl = _context.ReleaseStrategyLevels
                    .Where(r =>
                        relCode.Contains(r.ReleaseCode) &&
                        r.ReleaseGroupCode == ReleaseGroups.ShiftSchedule)
                    .Select(r => r.ReleaseStrategy).ToArray();

                var vRelStr = _context.ReleaseStrategy
                    .Where(r => relStrLvl.Contains(r.ReleaseStrategy) &&
                                r.Active &&
                                r.ReleaseGroupCode == ReleaseGroups.ShiftSchedule)
                    .ToList();

                var allRelStr = vRelStr.Select(v => v.ReleaseStrategy).ToArray();

                //don't get openmonth from db, as after 25th, the same will be changed to next month
                //for uploading of next month's ss
                int openMonth = Convert.ToInt32(DateTime.Now.Year + DateTime.Now.Month.ToString("00"));


                DateTime fromDt = DateTime.Now.Date;
                DateTime toDt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);


                // Get all schedules from master whether released or not except rejected...
                var schedules = _context.ShiftSchedules
                    .Where(s => s.YearMonth == openMonth &&
                                allRelStr.Contains(s.ReleaseStrategy) &&
                                s.ReleaseStatusCode != ReleaseStatus.ReleaseRejected
                    ).ToList();

                if (schedules.Count == 0)
                    return BadRequest("No shift schedule found uploaded for this employee.");


                var outputTable = new DataTable("ShiftSchedule");
                outputTable.Columns.Add("EmpUnqId");

                // not require these columns:

                outputTable.Columns.Add("EmpName");
                outputTable.Columns.Add("DeptName");
                outputTable.Columns.Add("StatName");
                outputTable.Columns.Add("Designation");
                outputTable.Columns.Add("CatName");
                outputTable.Columns.Add("Remarks");

                DateTime loopDate = fromDt;
                for (int dt = fromDt.Day; dt <= toDt.Day; dt++)
                {
                    string dayStr = dt.ToString("00") + "_" + loopDate.DayOfWeek.ToString().Substring(0, 2);
                    outputTable.Columns.Add(dayStr);
                    loopDate = loopDate.AddDays(1);
                }

                // Loop for each employee under this releaser
                foreach (ReleaseStrategies relStr in vRelStr)
                {
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

                    // don't need these columns:

                    dr["EmpName"] = employeeDto.EmpName;
                    dr["DeptName"] = employeeDto.DeptName;
                    dr["StatName"] = employeeDto.StatName;
                    dr["Designation"] = employeeDto.DesgName;
                    dr["CatName"] = employeeDto.CatName;
                    dr["Remarks"] = "";

                    var tmpSchedule = schedules
                        .Where(s =>
                            s.ReleaseStrategy == relStr.ReleaseStrategy &&
                            s.YearMonth == openMonth &&
                            s.ReleaseStatusCode == ReleaseStatus.FullyReleased)
                        .OrderByDescending(s => s.ScheduleId)
                        .ToList();

                    if (!tmpSchedule.Any())
                        continue;


                    ShiftSchedules sch = tmpSchedule.First();

                    //Get schedule from ESS
                    var schDtl = _context.ShiftScheduleDetails
                        .Where(s =>
                            s.YearMonth == sch.YearMonth &&
                            s.ScheduleId == sch.ScheduleId &&
                            s.EmpUnqId == sch.EmpUnqId).ToList();

                    // Get schedule from ATTD
                    AttdShiftScheduleDto attdSchedule = Helpers.CustomHelper
                        .GetattdShiftSchedule(sch.YearMonth, sch.EmpUnqId, employeeDto.Location);

                    for (DateTime dt = fromDt; dt <= toDt;)
                    {
                        string dayStr = dt.Day.ToString("00") + "_" + dt.DayOfWeek.ToString().Substring(0, 2);

                        // change following line to get current schedule from ATTD
                        if (attdSchedule.EmpUnqId == null)
                        {
                            dr[dayStr] = schDtl.First(s => s.ShiftDay == dt.Day).ShiftCode ?? "";
                        }
                        else
                        {
                            dr[dayStr] = attdSchedule["D" + dt.Day.ToString("00")].ToString();
                        }

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

        [HttpPost]
        public IHttpActionResult UpldateSchedule(string empUnqId)
        {
            // Get file from Http Context of current request
            HttpContext httpContext = HttpContext.Current;

            // Check for any uploaded file  
            if (httpContext.Request.Files.Count <= 0) return BadRequest("NO FILES???");

            // Create new folder if does not exist.
            try
            {
                string folder = HostingEnvironment.MapPath(@"~/App_Data/tmp/");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder ?? throw new InvalidOperationException("Folder not found"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            // Schedule update will be from today to end of month...
            DateTime fromDt = DateTime.Now.Date;
            DateTime todt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

            // Although there'll be only one file, it's nice to loop through it
            for (int i = 0; i < httpContext.Request.Files.Count;)
            {
                HttpPostedFile postedFile = httpContext.Request.Files[i];

                if (postedFile.ContentLength <= 0) return BadRequest("Passed file is null!");

                try
                {
                    string fileExt = Path.GetExtension(postedFile.FileName);
                    if (fileExt != ".csv") return BadRequest("Invalid file extension.");

                    //Save file to tmp directory for later reviews

                    postedFile.SaveAs(
                        HostingEnvironment.MapPath(@"~/App_Data/tmp/") +
                        "u-" + empUnqId + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv");

                    //don't get openmonth from db, as after 25th, the same will be changed to next month
                    //for uploading of next month's ss
                    int openMonth = Convert.ToInt32(DateTime.Now.Year + DateTime.Now.Month.ToString("00"));

                    using (var reader = new StreamReader(postedFile.InputStream))
                    {
                        // This will just rip off the first Header line off the excel file
                        var header = reader.ReadLine()?.Split(',');

                        // Add check for column header here...
                        // first 7 columns are :
                        // empunqid, name, deptname, statname, desig, category, remarks

                        if (header != null && header[7].Substring(0, 2) != fromDt.Day.ToString("00"))
                            return BadRequest("check date column. Download template and upload it.");

                        if (header != null && header.Length - 1 != (todt - fromDt).Days + 7)
                            return BadRequest("File format invalid.");

                        var schedules = new List<ShiftScheduleDto>();

                        while (!reader.EndOfStream)
                        {
                            var row = reader.ReadLine()?.Split(',');
                            if (row == null) continue;

                            // the employee of current row
                            string tmpEmpId = row[0];

                            string remarks = row[6].Trim();
                            remarks = Helpers.CustomHelper.RemoveSpecialCharacters(remarks);

                            if (remarks == "")
                                return BadRequest("Remarks column is mandatory for all employees for schedule change.");

                            var sch = new ShiftScheduleDto
                            {
                                YearMonth = openMonth,
                                EmpUnqId = tmpEmpId,
                                ReleaseGroupCode = ReleaseGroups.ShiftSchedule,
                                ReleaseStrategy = tmpEmpId,
                                ReleaseStatusCode = ReleaseStatus.PartiallyReleased,
                                AddDt = DateTime.Now,
                                AddUser = empUnqId,
                                Remarks = remarks,
                                ShiftScheduleDetails = new List<ShiftScheduleDetailDto>()
                            };

                            //first get the schedule for upto FromDate 
                            //and add it to details

                            int extScheduleId = _context.ShiftSchedules
                                .Where(s => s.EmpUnqId == tmpEmpId &&
                                            s.YearMonth == openMonth &&
                                            s.ReleaseStatusCode == ReleaseStatus.FullyReleased)
                                .Max(s => s.ScheduleId);

                            var existingSch = _context.ShiftScheduleDetails
                                .Where(s => s.YearMonth == openMonth &&
                                            s.ScheduleId == extScheduleId &&
                                            s.EmpUnqId == tmpEmpId
                                )
                                .ToList();

                            // Get current employee rec - only location field required

                            string empLoc = _context.Employees.FirstOrDefault(e => e.EmpUnqId == tmpEmpId)?.Location;

                            // Get schedule from ATTD
                            AttdShiftScheduleDto attdSchedule = Helpers.CustomHelper
                                .GetattdShiftSchedule(sch.YearMonth, sch.EmpUnqId, empLoc);

                            for (int rowIndex = 1; rowIndex < fromDt.Day; rowIndex++)
                            {
                                var existingLine = new ShiftScheduleDetails
                                {
                                    YearMonth = openMonth, ShiftDay = rowIndex
                                };

                                if (attdSchedule.EmpUnqId != null)
                                    existingLine.ShiftCode = attdSchedule["D" + rowIndex.ToString("00")].ToString();
                                else
                                    existingLine = existingSch.FirstOrDefault(s => s.ShiftDay == rowIndex);

                                if (existingLine == null)
                                    return BadRequest(
                                        $"Error: already uploaded shift schedule not found for {sch.EmpUnqId}");

                                var schLine = new ShiftScheduleDetailDto
                                {
                                    YearMonth = existingLine.YearMonth,
                                    ShiftDay = existingLine.ShiftDay,
                                    ShiftCode = existingLine.ShiftCode
                                };
                                sch.ShiftScheduleDetails.Add(schLine);
                            }


                            for (int rowIndex = fromDt.Day; rowIndex <= todt.Day; rowIndex++)
                            {
                                // Check for WO in existing schedule.

                                if (attdSchedule.EmpUnqId != null)
                                {
                                    // If there's a wo in existing schedule in attendnace,
                                    if (attdSchedule["D" + rowIndex.ToString("00")].ToString() == "WO")
                                    {
                                        // and if currently supplied ss do not have WO, throw error
                                        if (row[rowIndex - fromDt.Day + 7] != "WO")
                                        {
                                            return BadRequest(
                                                "Do not change WO from exising one. Check employee " + tmpEmpId);
                                        }
                                    }


                                    // If uploading ss is saying it's a WO,
                                    if (row[rowIndex - fromDt.Day + 7] == "WO")
                                    {
                                        // and attendance system does not have one, throw error
                                        if (attdSchedule["D" + rowIndex.ToString("00")].ToString() != "WO")
                                        {
                                            return BadRequest(
                                                "Do not change WO from exising one. Check employee " + tmpEmpId);
                                        }
                                    }
                                }

                                var schLine = new ShiftScheduleDetailDto
                                {
                                    YearMonth = sch.YearMonth,
                                    ShiftDay = rowIndex,
                                    ShiftCode = row[rowIndex - fromDt.Day + 7]
                                };

                                sch.ShiftScheduleDetails.Add(schLine);
                            }

                            schedules.Add(sch);
                        }

                        var errors = ShiftScheduleController.SchValidate(schedules, empUnqId);

                        // Check if any unreleased schedule exist for any of the employee...

                        var schEmps = schedules.Select(s => s.EmpUnqId).ToArray();
                        var unrelSch = _context.ShiftSchedules
                            .Where(s => s.YearMonth == openMonth &&
                                        schEmps.Contains(s.EmpUnqId) &&
                                        s.ReleaseStatusCode != ReleaseStatus.FullyReleased &&
                                        s.ReleaseStatusCode != ReleaseStatus.ReleaseRejected)
                            .Select(e => e.EmpUnqId)
                            .ToArray();

                        if (unrelSch.Length > 0)
                            errors.Add("A shift schedule already uploaded for " + unrelSch.Length +
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

                                if (emp == null) return BadRequest($"Employee {sch.EmpUnqId} is null!!!");

                                sch.ScheduleId = maxId;
                                sch.CompCode = emp.CompCode;
                                sch.WrkGrp = emp.WrkGrp;
                                sch.UnitCode = emp.UnitCode;
                                sch.DeptCode = emp.DeptCode;
                                sch.StatCode = emp.StatCode;

                                foreach (ShiftScheduleDetailDto detail in sch.ShiftScheduleDetails)
                                {
                                    detail.ScheduleId = maxId;
                                    if (detail.ShiftDay == 0)
                                        return BadRequest($"Shift day is 0 for employee {sch.EmpUnqId}");
                                }

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
                    return BadRequest(ex.ToString());
                }
            }

            return Ok();
        }
    }
}