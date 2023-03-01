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
using ESS.Migrations;
using ESS.Models;
using Newtonsoft.Json;
using TpaSanction = ESS.Models.TpaSanction;

namespace ESS.Controllers.Api
{
    public class TpaController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public TpaController()
        {
            _context = new ApplicationDbContext();
        }

        // Get TPA for given date range for employees under specified releaser
        [HttpGet, ActionName("getpretpa")]
        public IHttpActionResult GetTpa(DateTime fromDate, DateTime toDate, string empUnqId)
        {
            List<ReleaseAuth> relAuth = _context.ReleaseAuth
                .Where(r => r.EmpUnqId == empUnqId && r.Active)
                .ToList();

            relAuth.RemoveAll(r => r.ReleaseCode.StartsWith("GP_"));

            IEnumerable<string> relCodes = relAuth.Select(r => r.ReleaseCode).ToList();
            List<ReleaseStrategyLevels> relStrLvl = _context.ReleaseStrategyLevels
                .Where(r =>
                    r.ReleaseGroupCode == ReleaseGroups.Tpa &&
                    relCodes.Contains(r.ReleaseCode))
                .ToList();

            if (!relStrLvl.Any())
                return BadRequest("No records found.");

            List<string> allEmps = relStrLvl.Select(r => r.ReleaseStrategy).Distinct().ToList();

            List<string> inActiveEmp = _context.Employees.Where(
                e => allEmps.Contains(e.EmpUnqId) && e.Active == false).Select(e => e.EmpUnqId).ToList();

            List<string> emps = allEmps.Except(inActiveEmp).ToList();

            List<TpaSanction> alreadyTpaEntries = _context.TpaSanctions.Where(
                    t => emps.Contains(t.EmpUnqId) &&
                         t.TpaDate >= fromDate && t.TpaDate <= toDate &&
                         t.PreReleaseStatusCode != ReleaseStatus.ReleaseRejected)
                .ToList();

            var result = new List<TpaSanctionDto>();

            foreach (string emp in emps)
            {
                // get total year months between date range
                int fromYearMonth = int.Parse(fromDate.Year.ToString("0000") + fromDate.Month.ToString("00"));
                int toYearMonth = int.Parse(toDate.Year.ToString("0000") + toDate.Month.ToString("00"));


                // get all fully released schedules 
                var lastSchedule =
                    _context.ShiftSchedules
                        .Where(e => (e.YearMonth >= fromYearMonth && e.YearMonth <= toYearMonth) &&
                                    e.EmpUnqId == emp &&
                                    e.ReleaseStatusCode == ReleaseStatus.FullyReleased)
                        .GroupBy(t => new { t.YearMonth, t.EmpUnqId })
                        .Select(g => new
                        {
                            YearMonth = g.Key.YearMonth,
                            EmpUnqId = g.Key.EmpUnqId,
                            ScheduleId = g.Max(x => x.ScheduleId)
                        })
                        .ToList();

                int[] scheduleIds = lastSchedule.Select(s => s.ScheduleId).ToArray();

                IQueryable<ShiftScheduleDetails> lastScheduleDetails = _context.ShiftScheduleDetails
                    .Where(e =>
                        (e.YearMonth >= fromYearMonth && e.YearMonth <= toYearMonth) &&
                        scheduleIds.Contains(e.ScheduleId) &&
                        e.EmpUnqId == emp);

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
                    .FirstOrDefault(e => e.EmpUnqId == emp);


                for (DateTime dt = fromDate; dt <= toDate;)
                {
                    var newTpa = new TpaSanctionDto
                    {
                        Id = 0,
                        EmpUnqId = emp,
                        TpaDate = dt,
                        RequiredHours = 0,
                        ReleaseGroupCode = ReleaseGroups.Tpa,
                        PreReleaseStatusCode = ReleaseStatus.InRelease,
                        PreRemarks = "",
                        ReleaseStrategy = emp,
                    };

                    TpaSanction already = alreadyTpaEntries.FirstOrDefault(t => t.TpaDate == dt &&
                        t.EmpUnqId == emp);
                    if (already != null)
                    {
                        newTpa.Id = already.Id;
                        newTpa.RequiredHours = already.RequiredHours;
                        newTpa.PreReleaseStatusCode = already.PreReleaseStatusCode;
                        newTpa.PreRemarks = already.PreRemarks;
                    }

                    int yearMonth = int.Parse(dt.Year.ToString("0000") + dt.Month.ToString("00"));

                    if (!lastSchedule.Any())
                    {
                        newTpa.TpaShiftCode = "";
                    }
                    else
                    {
                        var schedule = lastSchedule.FirstOrDefault(s => s.YearMonth == yearMonth && s.EmpUnqId == emp);
                        int scheduleId;
                        if (schedule == null)
                        {
                            newTpa.TpaShiftCode = "";
                        }
                        else
                        {
                            scheduleId = schedule.ScheduleId;

                            newTpa.TpaShiftCode = lastScheduleDetails
                                .FirstOrDefault(e =>
                                    e.YearMonth == yearMonth &&
                                    e.ScheduleId == scheduleId &&
                                    e.ShiftDay == dt.Day &&
                                    e.EmpUnqId == emp)
                                ?.ShiftCode;
                        }
                    }

                    if (employeeDto != null)
                    {
                        newTpa.EmpName = employeeDto.EmpName;
                        newTpa.CatName = employeeDto.CatName;
                        newTpa.DeptName = employeeDto.DeptName;
                        newTpa.StatName = employeeDto.StatName;
                        newTpa.GradeName = employeeDto.GradeName;
                        newTpa.DesgName = employeeDto.DesgName;
                    }

                    result.Add(newTpa);

                    dt = dt.AddDays(1);
                }
            }

            return Ok(result);
        }

        [HttpPost, ActionName("pretparequest")]
        public IHttpActionResult PreTpa(string empUnqId, [FromBody] object requestData)
        {
            var errors = new List<string>();

            try
            {
                var tpaSanctionDto = JsonConvert.DeserializeObject<List<TpaSanctionDto>>(requestData.ToString());

                if (tpaSanctionDto == null)
                    return BadRequest("Invalid payload.");

                var dup = tpaSanctionDto.GroupBy(t => new { t.EmpUnqId, t.TpaDate })
                    .Where(g => g.Count() > 1)
                    .Select(e => e.Key);

                if (dup.Any())
                    return Content(HttpStatusCode.BadRequest, "List contains duplicate values: " + dup.ToString());

                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    foreach (TpaSanctionDto dto in tpaSanctionDto)
                    {
                        var tpaSanction = new TpaSanction
                        {
                            Id = 0,
                            EmpUnqId = dto.EmpUnqId,
                            TpaDate = dto.TpaDate,
                            TpaShiftCode = dto.TpaShiftCode,
                            RequiredHours = dto.RequiredHours,
                            PreJustification = dto.PreJustification,
                            ReleaseGroupCode = ReleaseGroups.Tpa,
                            ReleaseStrategy = dto.ReleaseStrategy,
                            PreReleaseStatusCode = ReleaseStatus.InRelease,
                            PreRemarks = dto.PreRemarks,
                            AddDt = DateTime.Now,
                            AddUser = dto.AddUser
                        };

                        _context.TpaSanctions.Add(tpaSanction);
                        _context.SaveChanges();

                        //get release info and store in tparelease
                        var relStrLvl = _context.ReleaseStrategyLevels
                            .Where(r => r.ReleaseGroupCode == ReleaseGroups.Tpa &&
                                        r.ReleaseStrategy == dto.EmpUnqId)
                            .ToList();

                        foreach (ReleaseStrategyLevels level in relStrLvl)
                        {
                            ReleaseAuth relAuth = _context.ReleaseAuth
                                .FirstOrDefault(ra => ra.ReleaseCode == level.ReleaseCode &&
                                                      ra.Active);

                            if (relAuth == null)
                            {
                                errors.Add("Release auth not found for emp: " + dto.EmpUnqId);
                                continue;
                            }

                            var tpaRelease = new TpaRelease
                            {
                                Id = 0,
                                ReleaseGroupCode = dto.ReleaseGroupCode,
                                TpaSanctionId = tpaSanction.Id,
                                ReleaseStrategy = level.ReleaseStrategy,
                                ReleaseStrategyLevel = level.ReleaseStrategyLevel,
                                ReleaseCode = level.ReleaseCode,
                                PreReleaseStatusCode =
                                    level.ReleaseStrategyLevel == 1
                                        ? ReleaseStatus.InRelease
                                        : ReleaseStatus.NotReleased,
                                PreReleaseDate = null,
                                PreReleaseAuth = relAuth.EmpUnqId,
                                PostReleaseAuth = relAuth.EmpUnqId,
                                IsFinalRelease = level.IsFinalRelease
                            };

                            _context.TpaReleases.Add(tpaRelease);
                            _context.SaveChanges();
                        }
                    }

                    if (errors.Count > 0)
                        return Content(HttpStatusCode.BadRequest, errors);

                    //_context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                errors.Add("Error: " + ex);
                return Content(HttpStatusCode.BadRequest, errors);
            }

            return Ok();
        }

        [HttpGet, ActionName("getprerequestedlist")]
        public IHttpActionResult GetPreRelease(string empUnqId)
        {
            var relAuthList = _context.ReleaseAuth
                .Where(r => r.EmpUnqId == empUnqId)
                .ToList();

            relAuthList.RemoveAll(r => r.ReleaseCode.StartsWith("GP_"));
            string[] relAuth = relAuthList.Select(r => r.ReleaseCode).ToArray();

            List<TpaReleaseDto> tpaReleaseDtos = _context.TpaReleases
                .Where(l =>
                    relAuth.Contains(l.ReleaseCode) &&
                    l.PreReleaseStatusCode == ReleaseStatus.InRelease
                ).AsEnumerable()
                .Select(Mapper.Map<TpaRelease, TpaReleaseDto>)
                .ToList();

            int[] ids = tpaReleaseDtos.Select(a => a.TpaSanctionId).ToArray();


            List<TpaSanctionDto> tpaSanctionList = _context.TpaSanctions
                .Include(e => e.Employee)
                .Include(r => r.ReleaseGroup)
                .Include(rs => rs.RelStrategy)
                .Where(l => ids.Contains(l.Id)).ToList()
                .Select(Mapper.Map<TpaSanction, TpaSanctionDto>)
                .ToList();

            foreach (TpaSanctionDto dto in tpaSanctionList)
            {
                List<TpaReleaseDto> apps = tpaReleaseDtos.Where(t => t.TpaSanctionId == dto.Id).ToList();

                dto.TpaReleaseStatus = new List<TpaReleaseDto>();
                foreach (TpaReleaseDto app in apps)
                {
                    List<ReleaseAuth> relCode = relAuthList
                        .Where(r => r.ReleaseCode == app.ReleaseCode)
                        .ToList();

                    foreach (ReleaseAuth unused in relCode.Where(auth => auth.EmpUnqId == empUnqId))
                        app.PreReleaseAuth = empUnqId;

                    dto.TpaReleaseStatus.Add(app);
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

                dto.EmpName = employeeDto.EmpName;
                dto.CatName = employeeDto.CatName;
                dto.DeptName = employeeDto.DeptName;
                dto.StatName = employeeDto.StatName;
                dto.GradeName = employeeDto.GradeName;
                dto.DesgName = employeeDto.DesgName;
            }

            return Ok(tpaSanctionList);
        }

        [HttpPost, ActionName("preapproval")]
        public IHttpActionResult PreReleaseTpa([FromBody] object requestData)
        {
            try
            {
                var tpaRleaseDtos = JsonConvert.DeserializeObject<List<TpaReleaseDto>>(requestData.ToString());

                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    foreach (TpaReleaseDto dto in tpaRleaseDtos)
                    {
                        TpaRelease tpaRelease = _context.TpaReleases
                            .FirstOrDefault(t =>
                                t.TpaSanctionId == dto.TpaSanctionId &&
                                t.ReleaseStrategyLevel == dto.ReleaseStrategyLevel);

                        if (tpaRelease == null)
                            return BadRequest("Invalid release details.");

                        tpaRelease.PreRemarks = dto.PreRemarks;

                        if (tpaRelease.PreReleaseStatusCode != ReleaseStatus.InRelease)
                            return BadRequest("Application is not in release state.");


                        //first verfy if release code is correct based on the relase code
                        DateTime today = DateTime.Now;

                        ReleaseAuth relAuth = _context.ReleaseAuth
                            .Single(
                                r =>
                                    r.ReleaseCode == tpaRelease.ReleaseCode &&
                                    r.EmpUnqId == tpaRelease.PreReleaseAuth &&
                                    r.Active &&
                                    today >= r.ValidFrom &&
                                    today <= r.ValidTo
                            );

                        if (relAuth == null)
                            BadRequest("Invalid releaser code. Check Active, Valid From, Valid to.");


                        //releaser is Ok. Now find release strategy level details
                        ReleaseStrategyLevels relStrLevel = _context.ReleaseStrategyLevels
                            .Single(
                                r =>
                                    r.ReleaseGroupCode == tpaRelease.ReleaseGroupCode &&
                                    r.ReleaseStrategy == tpaRelease.ReleaseStrategy &&
                                    r.ReleaseStrategyLevel == tpaRelease.ReleaseStrategyLevel &&
                                    r.ReleaseCode == tpaRelease.ReleaseCode
                            );

                        if (relStrLevel == null)
                            return BadRequest("Release strategy detail not found:");

                        TpaSanction tpaSanction = _context.TpaSanctions
                            .FirstOrDefault(t => t.Id == tpaRelease.TpaSanctionId);

                        if (tpaSanction == null)
                            return BadRequest("Corresponding TPA sanction request is not found!");

                        tpaSanction.PreRemarks = dto.PreRemarks;


                        if (dto.PreReleaseStatusCode == ReleaseStatus.ReleaseRejected)
                        {
                            tpaRelease.PreReleaseStatusCode = ReleaseStatus.ReleaseRejected;
                            tpaSanction.PreReleaseStatusCode = ReleaseStatus.ReleaseRejected;
                        }
                        else if (dto.PreReleaseStatusCode == ReleaseStatus.FullyReleased)
                        {
                            if (!tpaRelease.IsFinalRelease)
                            {
                                TpaRelease nextRelease = _context.TpaReleases
                                    .FirstOrDefault(t =>
                                        t.TpaSanctionId == dto.TpaSanctionId &&
                                        t.ReleaseStrategyLevel == dto.ReleaseStrategyLevel + 1);
                                if (nextRelease == null)
                                    return BadRequest(
                                        "This is not final release, and next level not found! Chk app release");
                                nextRelease.PreReleaseStatusCode = ReleaseStatus.InRelease;
                                tpaSanction.PreReleaseStatusCode = ReleaseStatus.PartiallyReleased;
                            }
                            else
                            {
                                tpaSanction.PreReleaseStatusCode = ReleaseStatus.FullyReleased;
                            }

                            tpaRelease.PreReleaseStatusCode = ReleaseStatus.FullyReleased;
                            tpaRelease.PreReleaseDate = DateTime.Now;
                            tpaRelease.PreReleaseAuth = dto.PreReleaseAuth;
                        }

                        _context.SaveChanges();

                        //string strSql = "update TpaReleases set ReleaseStrategy = '" + tpaRelease.ReleaseStrategy + "' " +
                        //                "where ReleaseGroupCode = 'OT' " +
                        //                "and TpaSanctionId = " + tpaRelease.TpaSanctionId + "";
                        //_context.Database.ExecuteSqlCommand(strSql);
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            return Ok();
        }

        // POST SANCTION
        [HttpGet, ActionName("getposttpa")]
        public IHttpActionResult GetPostTpa(DateTime fromDate, DateTime toDate, string empUnqId)
        {
            // get all sanctioned employee records between date range
            var relAuth = _context.ReleaseAuth
                .Where(r => r.EmpUnqId == empUnqId)
                .ToList();

            relAuth.RemoveAll(r => r.ReleaseCode.StartsWith("GP_"));

            IEnumerable<string> relCodes = relAuth.Select(r => r.ReleaseCode).ToList();
            var relStrLvl = _context.ReleaseStrategyLevels
                .Where(r =>
                    r.ReleaseGroupCode == ReleaseGroups.Tpa &&
                    relCodes.Contains(r.ReleaseCode))
                .ToList();

            if (!relStrLvl.Any())
                return BadRequest("No records found.");

            List<string> emps = relStrLvl.Select(r => r.ReleaseStrategy).ToList();

            // get location from one of the employees
            // if it is null, default to "IPU" manually
            string loc = _context.Employees
                .Where(e => e.EmpUnqId == emps.FirstOrDefault())
                .Select(e => e.Location)
                .FirstOrDefault() ?? "IPU";

            // create a string for SQL query
            string employees = "'" + string.Join("','", emps) + "'";

            var tpaAttdDto = new List<TpaAttdDto>();

            tpaAttdDto.AddRange(Helpers.CustomHelper.GetTpa(fromDate, toDate, employees, loc));

            List<TpaSanctionDto> preApprovedTpa = _context.TpaSanctions
                .Where(t =>
                    emps.Contains(t.EmpUnqId) &&
                    t.TpaDate >= fromDate && t.TpaDate <= toDate
                )
                .AsEnumerable()
                .Select(Mapper.Map<TpaSanction, TpaSanctionDto>)
                .ToList();

            var result = new List<TpaSanctionDto>();

            foreach (TpaAttdDto attdDto in tpaAttdDto)
            {
                // find already pre approved record. IF not found, create blank one
                TpaSanctionDto preDto = preApprovedTpa.FirstOrDefault(e =>
                    e.EmpUnqId == attdDto.EmpUnqId && e.TpaDate == attdDto.AttdDate) ?? new TpaSanctionDto
                {
                    Id = 0,
                    EmpUnqId = attdDto.EmpUnqId,
                    TpaDate = attdDto.AttdDate,
                    TpaShiftCode = attdDto.ScheDuleShift,
                    ReleaseGroupCode = ReleaseGroups.Tpa,
                    ReleaseStrategy = attdDto.EmpUnqId,
                };

                if (preDto.Id != 0)
                {
                    if (preDto.PostReleaseStatusCode == ReleaseStatus.InRelease ||
                        preDto.PostReleaseStatusCode == ReleaseStatus.PartiallyReleased ||
                        preDto.PostReleaseStatusCode == ReleaseStatus.FullyReleased
                       )
                    {
                        continue;
                    }
                }

                preDto.TpaShiftCode = attdDto.ScheDuleShift;

                //// IF WeeklyOff, then No OT -- Commented on 2022-09-26
                // if(preDto.TpaShiftCode == LeaveTypes.WeekOff)
                //    continue;

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
                    .Single(e => e.EmpUnqId == preDto.EmpUnqId);


                preDto.EmpName = employeeDto.EmpName;
                preDto.CatName = employeeDto.CatName;
                preDto.DeptName = employeeDto.DeptName;
                preDto.StatName = employeeDto.StatName;
                preDto.GradeName = employeeDto.GradeName;
                preDto.DesgName = employeeDto.DesgName;


                preDto.WrkHours = attdDto.ConsWrkHrs;


                preDto.ActShiftCode = attdDto.ConsShift;
                preDto.CalcOverTime = attdDto.CalcOverTime;

                if (preDto.TpaShiftCode == LeaveTypes.WeekOff)
                    if (preDto.CalcOverTime >= 10)
                        preDto.CalcOverTime = attdDto.CalcOverTime - 8;
                    else
                        preDto.CalcOverTime = 0;


                preDto.SanctionTpa = attdDto.CalcOverTime;
                preDto.ConsIn = attdDto.ConsIn;
                preDto.ConsOut = attdDto.ConsOut;
                preDto.ConsWrkHrs = attdDto.ConsWrkHrs;
                preDto.Status = attdDto.Status;
                preDto.HalfDay = attdDto.HalfDay;
                preDto.LeaveType = attdDto.LeaveType;
                preDto.LeaveHalf = attdDto.LeaveHalf;
                preDto.Earlycome = attdDto.Earlycome;
                preDto.EarlyGoing = attdDto.EarlyGoing;
                preDto.LateCome = attdDto.LateCome;

                result.Add(preDto);
            }

            return Ok(result);
        }

        [HttpPost, ActionName("sanctiontpa")]
        public IHttpActionResult PostTpa([FromBody] object requestData, string empUnqId)
        {
            var errors = new List<string>();

            try
            {
                var tpaSanctionDto = JsonConvert.DeserializeObject<List<TpaSanctionDto>>(requestData.ToString());
                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    foreach (TpaSanctionDto dto in tpaSanctionDto)
                    {
                        var tpaSanction = _context.TpaSanctions.FirstOrDefault(t =>
                            t.Id == dto.Id);

                        if (tpaSanction == null)
                        {
                            // create TpaSanction record 
                            tpaSanction = new TpaSanction
                            {
                                Id = 0,
                                EmpUnqId = dto.EmpUnqId,
                                TpaDate = dto.TpaDate,
                                TpaShiftCode = dto.TpaShiftCode,
                                RequiredHours = 0,
                                PreJustification = "",
                                ReleaseGroupCode = ReleaseGroups.Tpa,
                                ReleaseStrategy = dto.ReleaseStrategy,
                                PreReleaseStatusCode = ReleaseStatus.InRelease,
                                PreRemarks = "",
                                AddDt = DateTime.Now,
                                AddUser = empUnqId,
                                ActShiftCode = dto.ActShiftCode,
                                WrkHours = dto.WrkHours,
                                SanctionTpa = dto.SanctionTpa,
                                PostJustification = dto.PostJustification,
                                PostReleaseStatusCode = ReleaseStatus.InRelease,
                                PostRemarks = dto.PostRemarks,
                                HrUser = "114199", // TODO: HARD CODED HR HEAD ID TO BE REMOVED
                                HrReleaseStatusCode = ReleaseStatus.NotReleased,
                            };

                            _context.TpaSanctions.Add(tpaSanction);
                            _context.SaveChanges();

                            //get release info and store in tparelease
                            var relStrLvl = _context.ReleaseStrategyLevels
                                .Where(r => r.ReleaseGroupCode == ReleaseGroups.Tpa &&
                                            r.ReleaseStrategy == dto.EmpUnqId)
                                .ToList();

                            foreach (ReleaseStrategyLevels level in relStrLvl)
                            {
                                ReleaseAuth relAuth = _context.ReleaseAuth
                                    .FirstOrDefault(ra => ra.ReleaseCode == level.ReleaseCode);

                                if (relAuth == null)
                                {
                                    errors.Add("Release auth not found for emp: " + dto.EmpUnqId);
                                    continue;
                                }

                                var tpaRelease = new TpaRelease
                                {
                                    Id = 0,
                                    ReleaseGroupCode = dto.ReleaseGroupCode,
                                    TpaSanctionId = tpaSanction.Id,
                                    ReleaseStrategy = level.ReleaseStrategy,
                                    ReleaseStrategyLevel = level.ReleaseStrategyLevel,
                                    ReleaseCode = level.ReleaseCode,
                                    PreReleaseStatusCode = ReleaseStatus.InRelease,
                                    PostReleaseStatusCode =
                                        level.ReleaseStrategyLevel == 1
                                            ? ReleaseStatus.InRelease
                                            : ReleaseStatus.NotReleased,
                                    PreReleaseDate = null,
                                    PreReleaseAuth = relAuth.EmpUnqId,
                                    PostReleaseAuth = relAuth.EmpUnqId,
                                    IsFinalRelease = level.IsFinalRelease
                                };

                                _context.TpaReleases.Add(tpaRelease);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            if (tpaSanction.PreReleaseStatusCode == ReleaseStatus.InRelease ||
                                tpaSanction.PreReleaseStatusCode == ReleaseStatus.PartiallyReleased)
                            {
                                errors.Add("Record is not released for employee " + dto.EmpUnqId + " for date " +
                                           dto.TpaDate.ToString("dd/MM/yyy"));
                                continue;
                            }

                            tpaSanction.ActShiftCode = dto.ActShiftCode;
                            tpaSanction.WrkHours = dto.WrkHours;
                            tpaSanction.SanctionTpa = dto.SanctionTpa;
                            tpaSanction.PostJustification = dto.PostJustification;
                            tpaSanction.PostReleaseStatusCode = ReleaseStatus.InRelease;
                            tpaSanction.PostRemarks = dto.PostRemarks;

                            var tpaReleases = _context.TpaReleases
                                .Where(t => t.TpaSanctionId == dto.Id)
                                .ToList();
                            foreach (TpaRelease tpaRelease in tpaReleases)
                            {
                                tpaRelease.PostReleaseAuth = tpaRelease.PreReleaseAuth;
                                tpaRelease.PostReleaseStatusCode = tpaRelease.ReleaseStrategyLevel == 1
                                    ? ReleaseStatus.InRelease
                                    : ReleaseStatus.NotReleased;
                            }
                        }

                        _context.SaveChanges();
                    }

                    if (errors.Count > 0)
                        return Content(HttpStatusCode.BadRequest, errors);

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                errors.Add("Error: " + ex);
            }

            if (errors.Count > 0)
                return Content(HttpStatusCode.BadRequest, errors);

            return Ok();
        }

        [HttpGet, ActionName("getsanctionlist")]
        public IHttpActionResult GetPostRelease(string empUnqId)
        {
            string[] relAuth = _context.ReleaseAuth
                .Where(r => r.EmpUnqId == empUnqId)
                .Select(r => r.ReleaseCode)
                .ToArray();


            List<TpaReleaseDto> tpaReleaseDtos = _context.TpaReleases
                .Where(l =>
                    relAuth.Contains(l.ReleaseCode) &&
                    l.PostReleaseStatusCode == ReleaseStatus.InRelease
                ).AsEnumerable()
                .Select(Mapper.Map<TpaRelease, TpaReleaseDto>)
                .ToList();

            int[] ids = tpaReleaseDtos.Select(a => a.TpaSanctionId).ToArray();


            List<TpaSanctionDto> tpaSanctionList = _context.TpaSanctions
                .Include(e => e.Employee)
                .Include(r => r.ReleaseGroup)
                .Include(rs => rs.RelStrategy)
                .Where(l => ids.Contains(l.Id)).ToList()
                .Select(Mapper.Map<TpaSanction, TpaSanctionDto>)
                .ToList();

            foreach (TpaSanctionDto dto in tpaSanctionList)
            {
                List<TpaReleaseDto> apps = tpaReleaseDtos.Where(t => t.TpaSanctionId == dto.Id).ToList();

                dto.TpaReleaseStatus = new List<TpaReleaseDto>();
                foreach (TpaReleaseDto app in apps)
                {
                    List<ReleaseAuth> relCode = _context.ReleaseAuth
                        .Where(r => r.ReleaseCode == app.ReleaseCode)
                        .ToList();

                    foreach (ReleaseAuth unused in relCode.Where(auth => auth.EmpUnqId == empUnqId))
                        app.PreReleaseAuth = empUnqId;

                    dto.TpaReleaseStatus.Add(app);
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

                dto.EmpName = employeeDto.EmpName;
                dto.CatName = employeeDto.CatName;
                dto.DeptName = employeeDto.DeptName;
                dto.StatName = employeeDto.StatName;
                dto.GradeName = employeeDto.GradeName;
                dto.DesgName = employeeDto.DesgName;
            }

            return Ok(tpaSanctionList);
        }

        [HttpPost, ActionName("postapproval")]
        public IHttpActionResult PostReleaseTpa([FromBody] object requestData)
        {
            var postErrors = new List<string>();

            try
            {
                var tpaRleaseDtos = JsonConvert.DeserializeObject<List<TpaReleaseDto>>(requestData.ToString());

                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    foreach (TpaReleaseDto dto in tpaRleaseDtos)
                    {
                        TpaRelease tpaRelease = _context.TpaReleases
                            .FirstOrDefault(t =>
                                t.TpaSanctionId == dto.TpaSanctionId &&
                                t.ReleaseStrategyLevel == dto.ReleaseStrategyLevel);

                        if (tpaRelease == null)
                            return BadRequest("Invalid release details.");

                        tpaRelease.PostRemarks = dto.PostRemarks;

                        if (tpaRelease.PostReleaseStatusCode != ReleaseStatus.InRelease)
                            return BadRequest("Application is not in release state.");


                        //first verfy if release code is correct based on the relase code
                        DateTime today = DateTime.Now;

                        ReleaseAuth relAuth = _context.ReleaseAuth
                            .Single(
                                r =>
                                    r.ReleaseCode == tpaRelease.ReleaseCode &&
                                    r.EmpUnqId == tpaRelease.PostReleaseAuth &&
                                    r.Active &&
                                    today >= r.ValidFrom &&
                                    today <= r.ValidTo
                            );
                        if (relAuth == null)
                            BadRequest("Invalid releaser code. Check Active, Valid From, Valid to.");

                        //releaser is Ok. Now find release strategy level details
                        ReleaseStrategyLevels relStrLevel = _context.ReleaseStrategyLevels
                            .FirstOrDefault(
                                r =>
                                    r.ReleaseGroupCode == tpaRelease.ReleaseGroupCode &&
                                    r.ReleaseStrategy == tpaRelease.ReleaseStrategy &&
                                    r.ReleaseStrategyLevel == tpaRelease.ReleaseStrategyLevel &&
                                    r.ReleaseCode == tpaRelease.ReleaseCode
                            );

                        if (relStrLevel == null)
                            return BadRequest("Release strategy detail not found: " + tpaRelease.ReleaseStrategy);

                        TpaSanction tpaSanction = _context.TpaSanctions
                            .FirstOrDefault(t => t.Id == tpaRelease.TpaSanctionId);

                        if (tpaSanction == null)
                            return BadRequest("Corresponding TPA sanction request is not found!");

                        tpaSanction.PostRemarks = dto.PostRemarks;

                        if (dto.PostReleaseStatusCode == ReleaseStatus.ReleaseRejected)
                        {
                            tpaRelease.PostReleaseStatusCode = ReleaseStatus.ReleaseRejected;
                            tpaSanction.PostReleaseStatusCode = ReleaseStatus.ReleaseRejected;
                        }
                        else if (dto.PostReleaseStatusCode == ReleaseStatus.FullyReleased)
                        {
                            if (!tpaRelease.IsFinalRelease)
                            {
                                TpaRelease nextRelease = _context.TpaReleases
                                    .FirstOrDefault(t =>
                                        t.TpaSanctionId == dto.TpaSanctionId &&
                                        t.ReleaseStrategyLevel == dto.ReleaseStrategyLevel + 1);
                                if (nextRelease == null)
                                    return BadRequest(
                                        "This is not final release, and next level not found! Chk app release");

                                nextRelease.PostReleaseStatusCode = ReleaseStatus.InRelease;
                                tpaSanction.PostReleaseStatusCode = ReleaseStatus.PartiallyReleased;
                            }
                            else
                            {
                                // final release
                                tpaSanction.PostReleaseStatusCode = ReleaseStatus.FullyReleased;

                                // set status for hr release if eligible
                                if (tpaSanction.HrReleaseStatusCode == ReleaseStatus.NotReleased)
                                    tpaSanction.HrReleaseStatusCode = ReleaseStatus.InRelease;
                            }

                            tpaRelease.PostReleaseStatusCode = ReleaseStatus.FullyReleased;
                            tpaRelease.PostReleaseDate = DateTime.Now;
                            tpaRelease.PostReleaseAuth = dto.PostReleaseAuth;
                        }

                        _context.SaveChanges();
                    }

                    transaction.Commit();

                    //TODO: COMMENT THIS 
                    // LOOP FOR AUTOPOSTING in ATTENDANCE API

                    foreach (TpaReleaseDto dto in tpaRleaseDtos)
                    {
                        if (dto.PostReleaseStatusCode != ReleaseStatus.FullyReleased ||
                            dto.IsFinalRelease != true)
                            continue;


                        TpaSanction tpaSanction = _context.TpaSanctions
                            .FirstOrDefault(t =>
                                t.Id == dto.TpaSanctionId &&
                                t.PostReleaseStatusCode == ReleaseStatus.FullyReleased);

                        if (tpaSanction == null) continue;


                        var catg = _context.Employees.FirstOrDefault(e => e.EmpUnqId == tpaSanction.EmpUnqId)?.CatCode;

                        if (catg != null && catg != "003")
                            continue;


                        var justification = "";
                        if (tpaSanction.PostJustification == null ||
                            string.IsNullOrEmpty(tpaSanction.PostJustification.Trim()))
                            justification = tpaSanction.PreJustification;
                        else
                            justification = tpaSanction.PostJustification;
                        
                        if(justification.Length > 100)
                            justification = justification.Substring(0,99);
                        
                        var obj = new AutoPostObject
                        {
                            SanctionDt = tpaSanction.TpaDate,
                            SanTime = dto.PostReleaseDate ?? DateTime.Now,
                            EmpUnqID = tpaSanction.EmpUnqId,
                            SanFlg = "T",
                            TPAHours = Convert.ToInt32(tpaSanction.SanctionTpa),
                            Reason = justification,
                            PostedFlg = false,
                            AttdUser = dto.PostReleaseAuth,
                            Location = "IPU"
                        };

                        AutoPostObject autoPost = AutoPost(obj, "IPU", out bool result);

                        if (!result)
                        {
                            postErrors.Add("Error posting TPA ID: " + tpaSanction.Id + "for Emp " +
                                           tpaSanction.EmpUnqId + autoPost.ERROR);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            if (postErrors.Count > 0)
                return Content(HttpStatusCode.BadRequest, postErrors);

            return Ok();
        }

        private class AutoPostObject
        {
            public DateTime SanctionDt { get; set; }
            public string EmpUnqID { get; set; }
            public DateTime SanTime { get; set; }
            public string SanFlg { get; set; } // I for in, O for out, T for TPA hours
            public int TPAHours { get; set; }
            public string Reason { get; set; }
            public bool PostedFlg { get; set; }
            public string ERROR { get; set; }
            public string AttdUser { get; set; }
            public string Location { get; set; }
        }

        [HttpGet, ActionName("sanctionreport")]
        public IHttpActionResult SanctionReport(DateTime fromDate, DateTime toDate)
        {
            var result = _context.TpaSanctions
                .Where(t => t.TpaDate >= fromDate && t.TpaDate <= toDate &&
                            t.PostReleaseStatusCode == ReleaseStatus.FullyReleased).AsEnumerable()
                .Select(Mapper.Map<TpaSanction, TpaSanctionDto>)
                .ToList();

            if (result.Count == 0)
                return BadRequest("No records found.");

            var empUnqIds = result.Select(e => e.EmpUnqId).Distinct();
            var employees = _context.Employees
                .Where(e => empUnqIds.Contains(e.EmpUnqId))
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
                .ToList();

            if (employees.Count == 0)
                return BadRequest("No emp found");

            string emps = "'" + string.Join("','", empUnqIds) + "'";

            var attdRecords = Helpers.CustomHelper.GetTpa(fromDate, toDate, emps, employees.First().Location);


            var sanctionIds = result.Select(t => t.Id).Distinct().ToArray();

            var tpaReleases = _context.TpaReleases
                .Where(t => sanctionIds.Contains(t.TpaSanctionId)).AsEnumerable()
                .Select(Mapper.Map<TpaRelease, TpaReleaseDto>)
                .ToList();

            foreach (TpaSanctionDto dto in result)
            {
                var releaseReco = tpaReleases.Where(t => t.TpaSanctionId == dto.Id).ToList();
                dto.TpaReleaseStatus = new List<TpaReleaseDto>();
                dto.TpaReleaseStatus.AddRange(releaseReco);

                var attdDto = attdRecords.FirstOrDefault(a => a.EmpUnqId == dto.EmpUnqId && a.AttdDate == dto.TpaDate);
                if (attdDto == null) continue;


                var d = employees.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (d != null)
                {
                    dto.EmpName = d.EmpName;
                    dto.CatName = d.CatName;
                    dto.DeptName = d.DeptName;
                    dto.StatName = d.StatName;
                    dto.GradeName = d.GradeName;
                    dto.DesgName = d.DesgName;
                }

                dto.WrkHours = attdDto.ConsWrkHrs;
                dto.ActShiftCode = attdDto.ConsShift;
                dto.CalcOverTime = attdDto.CalcOverTime;
                dto.SanctionTpa = attdDto.CalcOverTime;
                dto.ConsIn = attdDto.ConsIn;
                dto.ConsOut = attdDto.ConsOut;
                dto.ConsWrkHrs = attdDto.ConsWrkHrs;
                dto.Status = attdDto.Status;
                dto.HalfDay = attdDto.HalfDay;
                dto.LeaveType = attdDto.LeaveType;
                dto.LeaveHalf = attdDto.LeaveHalf;
                dto.Earlycome = attdDto.Earlycome;
                dto.EarlyGoing = attdDto.EarlyGoing;
                dto.LateCome = attdDto.LateCome;
            }

            return Ok(result);
        }

        [HttpGet, ActionName("tpareport")]
        public IHttpActionResult TpaReport(DateTime fromDate, DateTime toDate, string empUnqId)
        {
            try
            {
                var relAuth = _context.ReleaseAuth
                    .Where(r => r.EmpUnqId == empUnqId && r.Active)
                    .ToList();
                relAuth.RemoveAll(x => x.ReleaseCode.StartsWith("GP_"));

                var relCodes = relAuth.Select(r => r.ReleaseCode).ToList();
                var relStrLevel = _context.ReleaseStrategyLevels
                    .Where(r => r.ReleaseGroupCode == ReleaseGroups.Tpa && relCodes.Contains(r.ReleaseCode))
                    .ToList();

                if (!relStrLevel.Any())
                    return BadRequest("No records found...");

                var emps = relStrLevel.Select(r => r.ReleaseStrategy).Distinct().ToList();

                var tpaSanctionList = _context.TpaSanctions.Where(
                        t => t.TpaDate >= fromDate && t.TpaDate <= toDate &&
                             emps.Contains(t.EmpUnqId)
                    ).AsEnumerable()
                    .Select(Mapper.Map<TpaSanction, TpaSanctionDto>)
                    .ToList();

                var tpaSanctionIds = tpaSanctionList.Select(t => t.Id).ToArray();

                var tpaReleaseList = _context.TpaReleases.Where(
                        t => tpaSanctionIds.Contains(t.TpaSanctionId)
                    ).AsEnumerable()
                    .Select(Mapper.Map<TpaRelease, TpaReleaseDto>)
                    .ToList();

                var preReleaseAuthEmp = tpaReleaseList.Select(t => t.PreReleaseAuth).ToArray();
                var postReleaseAuthEmp = tpaReleaseList.Select(t => t.PostReleaseAuth).ToArray();

                emps.AddRange(preReleaseAuthEmp);
                emps.AddRange(postReleaseAuthEmp);

                var empDtos = _context.Employees
                    .Where(e => emps.Contains(e.EmpUnqId))
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
                    .ToList();

                foreach (TpaSanctionDto dto in tpaSanctionList)
                {
                    List<TpaReleaseDto> releaseDetails = tpaReleaseList.Where(t => t.TpaSanctionId == dto.Id).ToList();
                    foreach (TpaReleaseDto releaseDto in releaseDetails)
                    {
                        releaseDto.PreReleaseName = empDtos.FirstOrDefault(e => e.EmpUnqId == releaseDto.PreReleaseAuth)
                            ?.EmpName;
                        releaseDto.PostReleaseName =
                            empDtos.FirstOrDefault(e => e.EmpUnqId == releaseDto.PostReleaseAuth)?.EmpName;
                    }

                    dto.TpaReleaseStatus = new List<TpaReleaseDto>();
                    dto.TpaReleaseStatus = releaseDetails;

                    var employeeDto = empDtos.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                    if (employeeDto != null)
                    {
                        dto.EmpName = employeeDto.EmpName;
                        dto.CatName = employeeDto.CatName;
                        dto.DeptName = employeeDto.DeptName;
                        dto.StatName = employeeDto.StatName;
                        dto.GradeName = employeeDto.GradeName;
                        dto.DesgName = employeeDto.DesgName;
                    }
                }

                return Ok(tpaSanctionList);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }
        }

        [HttpGet, ActionName("gethrlist")]
        public IHttpActionResult GetHrRelease()
        {
            List<TpaSanctionDto> tpaSanctionList = _context.TpaSanctions
                .Include(e => e.Employee)
                .Include(r => r.ReleaseGroup)
                .Include(rs => rs.RelStrategy)
                .Where(l => l.HrReleaseStatusCode == ReleaseStatus.InRelease).ToList()
                .Select(Mapper.Map<TpaSanction, TpaSanctionDto>)
                .ToList();

            int[] ids = tpaSanctionList.Select(t => t.Id).Distinct().ToArray();

            List<TpaReleaseDto> tpaReleaseDtos = _context.TpaReleases
                .Where(t => ids.Contains(t.TpaSanctionId)).AsEnumerable()
                .Select(Mapper.Map<TpaRelease, TpaReleaseDto>)
                .ToList();

            foreach (TpaSanctionDto dto in tpaSanctionList)
            {
                List<TpaReleaseDto> apps = tpaReleaseDtos.Where(t => t.TpaSanctionId == dto.Id).ToList();

                dto.TpaReleaseStatus = new List<TpaReleaseDto>();

                foreach (TpaReleaseDto app in
                         from app in apps
                         let relCode = _context.ReleaseAuth
                             .Where(r => r.ReleaseCode == app.ReleaseCode)
                             .ToList()
                         select app)
                {
                    dto.TpaReleaseStatus.Add(app);
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

                dto.EmpName = employeeDto.EmpName;
                dto.CatName = employeeDto.CatName;
                dto.DeptName = employeeDto.DeptName;
                dto.StatName = employeeDto.StatName;
                dto.GradeName = employeeDto.GradeName;
                dto.DesgName = employeeDto.DesgName;
            }

            return Ok(tpaSanctionList);
        }

        [HttpPost, ActionName("posthr")]
        public IHttpActionResult PostHrRelease([FromBody] object requestData)
        {
            try
            {
                var tpaSanctionDtos = JsonConvert.DeserializeObject<List<TpaSanctionDto>>(requestData.ToString());

                string[] hrUserList = tpaSanctionDtos.Select(e => e.HrUser).Distinct().ToArray();

                foreach (string user in hrUserList)
                {
                    if (!_context.ReleaseStrategy
                            .Any(r =>
                                r.ReleaseGroupCode == ReleaseGroups.Tpa &&
                                r.ReleaseStrategy == user &&
                                r.IsHod == true))
                        return BadRequest("Employee " + user + " not authorized to release.");
                }

                int[] ids = tpaSanctionDtos.Select(t => t.Id).Distinct().ToArray();

                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    List<TpaSanction> tpaSanctions = _context.TpaSanctions.Where(t => ids.Contains(t.Id)).ToList();

                    foreach (TpaSanction sanction in tpaSanctions)
                    {
                        TpaSanctionDto dto = tpaSanctionDtos.FirstOrDefault(t => t.Id == sanction.Id);
                        if (dto == null) continue;

                        sanction.HrReleaseStatusCode = dto.HrReleaseStatusCode;
                        sanction.HrPostRemarks = dto.HrPostRemarks;
                        sanction.HrUser = dto.HrUser;
                        sanction.HrReleaseDate = DateTime.Now;
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            return Ok();
        }

        private AutoPostObject AutoPost(AutoPostObject punchData, string location, out bool output)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Helpers.CustomHelper.GetAttendanceServerApi(location));

                StringContent content = new StringContent(JsonConvert.SerializeObject(punchData),
                    Encoding.UTF8, "application/json");

                var responseTask = client.PostAsync("/api/sanction/postsanction", content);
                responseTask.Wait();

                HttpResponseMessage result = responseTask.Result;
                output = result.IsSuccessStatusCode;

                var autoPost = new AutoPostObject();

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<AutoPostObject>();
                    readTask.Wait();

                    autoPost = readTask.Result;

                    return autoPost;
                }
                else
                {
                    var readTask = result.Content.ReadAsAsync<AutoPostObject>();
                    readTask.Wait();

                    autoPost = readTask.Result;
                    // Some error was there, return it without changing posting flags

                    return autoPost;
                }
            }
        }
    }
}