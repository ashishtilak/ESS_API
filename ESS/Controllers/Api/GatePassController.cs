using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class GatePassController : ApiController
    {
        private readonly ApplicationDbContext _context;
        private const int TIME_LIMIT = 5;

        public GatePassController()
        {
            _context = new ApplicationDbContext();
        }


        // GET /api/GatePass


        /// <summary>
        /// Following class is used for output to VMS
        /// </summary>
        private class OutGatePass
        {
            public int Id { get; set; }
            public string EmpUnqID { get; set; }
            public string EmpName { get; set; }
            public string DeptName { get; set; }
            public string StatName { get; set; }
            public DateTime? GateOutDateTime { get; set; }
            public DateTime? GateInDateTime { get; set; }
            public string PlaceOfVisit { get; set; }
            public string Mode { get; set; }
            public string Reason { get; set; }
            public string GatePassStatus { get; set; }
            public string ErrorMessage { get; set; }
        }

        /// <summary>
        /// This method is used by VMS. It'll post appropriate status.
        /// </summary>
        /// <param name="request">EmpUnqId:GatePassId - 20005116:00000008</param>
        /// <returns>Returns OutGatePass type object</returns>
        /// 
        /// 
        public IHttpActionResult GetGatePass(string request)
        {
            string[] str = request.Split(':');

            string empUnqId = str[0];
            int gpNumber = 0;
            int.TryParse(str[1], out gpNumber);

            try
            {
                var gpDto = _context.GatePass.FirstOrDefault(
                    g => g.Id == gpNumber && g.EmpUnqId == empUnqId);

                if (gpDto == null)
                    return BadRequest("Gatepass not found");

                var emp = _context.Employees
                    .Include(e => e.Departments)
                    .Include(e => e.Stations)
                    .FirstOrDefault
                        (e => e.EmpUnqId == gpDto.EmpUnqId);

                if (emp == null)
                    return BadRequest("Employee not found");


                OutGatePass ogp = new OutGatePass
                {
                    Id = gpDto.Id,
                    EmpUnqID = gpDto.EmpUnqId,
                    EmpName = emp.EmpName,
                    DeptName = emp.Departments == null ? "" : emp.Departments.DeptName,
                    StatName = emp.Stations == null ? "" : emp.Stations.StatName,
                    GateOutDateTime = gpDto.GateOutDateTime,
                    GateInDateTime = gpDto.GateInDateTime,
                    PlaceOfVisit = gpDto.PlaceOfVisit,
                    Mode = gpDto.Mode,
                    Reason = gpDto.Reason,
                    GatePassStatus = gpDto.GatePassStatus,
                    ErrorMessage = ""
                };


                //Now chech the status of the gatepass
                //If status is "N": set the status to Out, set gatepassout time, set status to "O"
                //If status is "O" and gatepassout time is < TIME_LIMIT min, do nothing
                //If status is "O" and mode is "D", Send error message, duty off gatepass already out, along with object
                //If status is "O": if Mode is not "D", then set status to In, and set gatepassin time
                //If status is "I": Send error message, gatepass already in along with object


                //If status = "N", set gateout time and set status to "O"
                if (gpDto.GatePassStatus == GatePass.GatePassStatuses.New)
                {
                    gpDto.GatePassStatus = GatePass.GatePassStatuses.Out;
                    gpDto.GateOutDateTime = DateTime.Now;

                    gpDto.AttdFlag = "O";

                    ogp.GatePassStatus = gpDto.GatePassStatus;
                    ogp.GateOutDateTime = gpDto.GateOutDateTime;

                    ogp.ErrorMessage = "Gate out done successfully.";
                    //TODO: ADD IP ADDRESS
                }
                else if (gpDto.GatePassStatus == GatePass.GatePassStatuses.Out)
                {
                    //Check for TIME LIMIT window

                    if (DateTime.Now.Subtract(gpDto.GateOutDateTime ?? DateTime.Now).TotalMinutes <= TIME_LIMIT)
                    {
                        //NOTHING TO BE DONE...
                        ogp.ErrorMessage = "Gate out done within " + TIME_LIMIT + " minutes.";
                        return Content(HttpStatusCode.BadRequest, ogp);
                    }

                    if (gpDto.Mode == GatePass.GatePassModes.DutyOff)
                    {
                        //Duty off wala case hai bhai, already out to ho gaya
                        //Ab kyu scan kar raha hai?
                        ogp.ErrorMessage = "Duty off gatepass is already out.";
                        return Content(HttpStatusCode.BadRequest, ogp);
                    }

                    gpDto.GatePassStatus = GatePass.GatePassStatuses.In;
                    gpDto.GateInDateTime = DateTime.Now;

                    ogp.GatePassStatus = gpDto.GatePassStatus;
                    ogp.GateInDateTime = gpDto.GateInDateTime;

                    ogp.ErrorMessage = "Gate In done successfully.";
                    //TODO: ADD IP ADDRESS
                }
                else if (gpDto.GatePassStatus == GatePass.GatePassStatuses.In)
                {
                    ogp.ErrorMessage = "Gatepass already IN.";
                    return Content(HttpStatusCode.BadRequest, ogp);
                }


                //raag kalingada notes are same as raag bhairav
                //kalingada is not as serious as bhairav -- its chanchal raag
                //it's sung during last phase of night
                //kalingada emphasis is given to pancham and shadaj (opposed to dhaivat and shadaj/rishabh in Bhairav)
                //Ramkali raag is also similar to bhairav
                //In ramkali sometimes m tivra is used in avroh

                _context.SaveChanges();

                return Ok(ogp);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        //for printing purpose
        public IHttpActionResult GetGatePass(string empUnqId, DateTime fromDt, DateTime toDt, string reportType)
        {
            //Get list of employees under this releaser.
            //get employee details
            var releaser = _context.Employees.Single(e => e.EmpUnqId == empUnqId);
            if (releaser == null)
                return BadRequest("Invalid employee code.");

            //return if employee is not a releaser
            if (!releaser.IsGpReleaser)
                return BadRequest("Employee is not authorized to release (check flag).");


            //if he's a releaser, get his release code
            //and based on the code, get all his release strategy levels

            var relCode = _context.ReleaseAuth.Where(r => r.EmpUnqId == releaser.EmpUnqId).ToList();

            List<GatePassDto> result = new List<GatePassDto>();

            //loop through all release codes found
            foreach (var releaseAuth in relCode)
            {
                var relStrategyLevel = _context.GpReleaseStrategyLevels
                    .Include(r => r.GpReleaseStrategies)
                    .Where(r => r.ReleaseCode == releaseAuth.ReleaseCode && r.ReleaseGroupCode == "GP")
                    .ToList();


                var relStrategy = relStrategyLevel.Select(level => level.GpReleaseStrategies).ToList();


                //and for each strategy we found above,
                //search for employee who match the release criteria
                foreach (var strategy in relStrategy)
                {
                    var relEmployee = _context.Employees
                        .Include(d => d.Departments)
                        .Include(s => s.Stations)
                        .Where(
                            e => e.EmpUnqId == strategy.GpReleaseStrategy &&
                                 strategy.Active
                        )
                        .Select(Mapper.Map<Employees, EmployeeDto>).ToList();


                    var relEmployee2 = _context.Employees
                        .Include(d => d.Departments)
                        .Include(s => s.Stations)
                        .Where(
                            e => e.CompCode == strategy.CompCode &&
                                 e.WrkGrp == strategy.WrkGrp &&
                                 e.UnitCode == strategy.UnitCode &&
                                 e.DeptCode == strategy.DeptCode &&
                                 e.StatCode == strategy.StatCode &&
                                 e.Active
                        )
                        .Select(Mapper.Map<Employees, EmployeeDto>).ToList();


                    relEmployee.AddRange(relEmployee2);

                    foreach (var emp in relEmployee)
                    {
                        var gatepass = _context.GatePass
                            .Where(
                                g => g.EmpUnqId == emp.EmpUnqId &&
                                     g.GatePassDate >= fromDt &&
                                     g.GatePassDate <= toDt &&
                                     g.GatePassStatus != GatePass.GatePassStatuses.ForceClosed
                            )
                            .Select(Mapper.Map<GatePass, GatePassDto>)
                            .ToList();


                        foreach (var dto in gatepass)
                        {
                            dto.EmpName = emp.EmpName;
                            dto.DeptName = emp.DeptName;
                            dto.StatName = emp.StatName;

                            dto.ModeName = dto.GetMode(dto.Mode);
                            dto.StatusName = dto.GetStatus(dto.GatePassStatus);
                            dto.BarCode = dto.GetBarcode(dto.EmpUnqId, dto.Id);
                        }

                        result.AddRange(gatepass);
                    }
                }
            }

            var labourGp = _context.GatePass
                .Where(
                    g => g.AddUser == empUnqId &&
                         g.GatePassDate >= fromDt &&
                         g.GatePassDate <= toDt &&
                         g.GatePassStatus != GatePass.GatePassStatuses.ForceClosed
                )
                .Select(Mapper.Map<GatePass, GatePassDto>)
                .ToList();

            foreach (var dto in labourGp)
            {
                var emp = _context.Employees
                    .Include(d => d.Departments)
                    .Include(s => s.Stations)
                    .SingleOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp != null)
                {
                    dto.EmpName = emp.EmpName;
                    dto.DeptName = emp.Departments.DeptName;
                    dto.StatName = emp.Stations.StatName;
                }

                dto.ModeName = dto.GetMode(dto.Mode);
                dto.StatusName = dto.GetStatus(dto.GatePassStatus);
                dto.BarCode = dto.GetBarcode(dto.EmpUnqId, dto.Id);
            }

            result.AddRange(labourGp);

            result = result.GroupBy(r => r.Id).Select(r => r.First()).ToList();

            return Ok(result);
        }

        //Gatepass details for an employee in date range...
        public IHttpActionResult GetGatePass(int gpNo, DateTime gpDate)
        {
            var gatepass = _context.GatePass
                .Where(
                    g => g.GatePassNo == gpNo &&
                         g.GatePassDate == gpDate &&
                         g.GatePassStatus != GatePass.GatePassStatuses.ForceClosed
                )
                .Select(Mapper.Map<GatePass, GatePassDto>)
                .ToList();

            foreach (var dto in gatepass)
            {
                var emp = _context.Employees
                    .Include(d => d.Departments)
                    .Include(s => s.Stations)
                    .SingleOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp == null)
                    return BadRequest("Invalid employee code.");


                dto.EmpName = emp.EmpName;
                dto.DeptName = emp.Departments == null ? "" : emp.Departments.DeptName.Trim();
                dto.StatName = emp.Stations == null ? "" : emp.Stations.StatName;

                dto.ModeName = dto.GetMode(dto.Mode);
                dto.StatusName = dto.GetStatus(dto.GatePassStatus);
                dto.BarCode = dto.GetBarcode(dto.EmpUnqId, dto.Id);
            }

            return Ok(gatepass);
        }

        //Gatepass details for an employee in date range...
        public IHttpActionResult GetGatePass(string empUnqId, DateTime fromDt, DateTime toDt)
        {
            var emp = _context.Employees
                .Include(d => d.Departments)
                .Include(s => s.Stations)
                .SingleOrDefault(e => e.EmpUnqId == empUnqId);

            if (emp == null)
                return BadRequest("Invalid employee code.");

            var gatepass = _context.GatePass
                .Where(
                    g => g.EmpUnqId == empUnqId &&
                         g.GatePassDate >= fromDt &&
                         g.GatePassDate <= toDt &&
                         g.GatePassStatus != GatePass.GatePassStatuses.ForceClosed
                )
                .Select(Mapper.Map<GatePass, GatePassDto>)
                .ToList();


            foreach (var dto in gatepass)
            {
                dto.EmpName = emp.EmpName;

                dto.DeptName = emp.Departments == null ? "" : emp.Departments.DeptName.Trim();
                dto.StatName = emp.Stations == null ? "" : emp.Stations.StatName;

                dto.ModeName = dto.GetMode(dto.Mode);
                dto.StatusName = dto.GetStatus(dto.GatePassStatus);
                dto.BarCode = dto.GetBarcode(dto.EmpUnqId, dto.Id);
            }

            return Ok(gatepass);
        }

        //All gatepass details between date range
        //Report for HR and Admin-security
        public IHttpActionResult GetGatePass(DateTime fromDt, DateTime toDt)
        {
            var gatepass = _context.GatePass
                .Where(
                    g => g.GatePassDate >= fromDt &&
                         g.GatePassDate <= toDt &&
                         g.GatePassStatus != GatePass.GatePassStatuses.ForceClosed
                )
                .Select(Mapper.Map<GatePass, GatePassDto>)
                .ToList();

            foreach (var dto in gatepass)
            {
                var emp = _context.Employees
                    .Include(u => u.Units)
                    .Include(d => d.Departments)
                    .Include(s => s.Stations)
                    .Include(c => c.Categories)
                    .Include(g => g.Designations)
                    .SingleOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp == null) continue;

                dto.EmpName = emp.EmpName;
                dto.UnitName = emp.Units == null ? "" : emp.Units.UnitName.Trim();
                dto.DeptName = emp.Departments == null ? "" : emp.Departments.DeptName.Trim();
                dto.StatName = emp.Stations == null ? "" : emp.Stations.StatName;
                dto.WrkGrp = emp.WrkGrp;
                dto.CatName = emp.Categories == null ? "" : emp.Categories.CatName;
                dto.DesgName = emp.Designations == null ? "" : emp.Designations.DesgName;

                dto.ModeName = dto.GetMode(dto.Mode);
                dto.StatusName = dto.GetStatus(dto.GatePassStatus);
                dto.BarCode = dto.GetBarcode(dto.EmpUnqId, dto.Id);
            }

            return Ok(gatepass);
        }

        //For posting:
        private class RequestGp
        {
            public int GatePassItem { get; set; }
            public string EmpUnqId { get; set; }
            public string Mode { get; set; }
            public string PlaceOfVisit { get; set; }
            public string Reason { get; set; }
            public string AddUser { get; set; }
            public string ReleaseGroupCode { get; set; }
            public int YearMonth { get; set; }
        }

        [HttpPost]
        [ActionName("CreateGatePass")]
        public IHttpActionResult CreateGatePass([FromBody] object requestData)
        {
            try
            {
                List<RequestGp> dto = JsonConvert.DeserializeObject<List<RequestGp>>(requestData.ToString());

                if (!ModelState.IsValid)
                    return BadRequest();

                int maxId, maxGpId;

                try
                {
                    maxId = _context.GatePass.Max(g => g.Id);
                }
                catch
                {
                    maxId = 0;
                }

                try
                {
                    maxGpId = _context.GatePass.Where(
                            i => DbFunctions.TruncateTime(i.GatePassDate) == DbFunctions.TruncateTime(DateTime.Now))
                        .Max(g => g.GatePassNo);
                }
                catch
                {
                    maxGpId = 0;
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    List<ApplReleaseStatusDto> result = new List<ApplReleaseStatusDto>();

                    List<GatePass> gps = new List<GatePass>();

                    foreach (var gp in dto)
                    {
                        var emp = _context.Employees
                            .SingleOrDefault(e => e.EmpUnqId == gp.EmpUnqId);

                        if (emp == null)
                            return BadRequest("Employee not found: " + gp.EmpUnqId.ToString());


                        //first get release strategy details based on comp, wrkgrp, unit, dept, stat and cat code
                        var relStrat = _context.GpReleaseStrategy
                            .FirstOrDefault(
                                r =>
                                    r.ReleaseGroupCode == gp.ReleaseGroupCode &&
                                    r.GpReleaseStrategy == gp.EmpUnqId &&
                                    r.Active
                            );

                        if (relStrat == null)
                            return BadRequest("Release strategy not configured for Employee " + gp.EmpUnqId);


                        //get release strategy levels
                        var relStratLevels = _context.GpReleaseStrategyLevels
                            .Where(
                                rl =>
                                    rl.ReleaseGroupCode == gp.ReleaseGroupCode &&
                                    rl.GpReleaseStrategy == relStrat.GpReleaseStrategy
                            ).ToList();


                        relStrat.GpReleaseStrategyLevels = relStratLevels;


                        //loop through all GP details 
                        GatePass newGp = new GatePass
                        {
                            YearMonth = gp.YearMonth,
                            Id = maxId + gp.GatePassItem,
                            GatePassDate = DateTime.Now.Date,
                            GatePassNo = maxGpId + 1,
                            GatePassItem = gp.GatePassItem,
                            EmpUnqId = gp.EmpUnqId,
                            Mode = gp.Mode,
                            PlaceOfVisit = gp.PlaceOfVisit,
                            Reason = gp.Reason,
                            ReleaseGroupCode = gp.ReleaseGroupCode,
                            ReleaseStatusCode = ReleaseStatus.InRelease,
                            GpReleaseStrategy = relStrat.GpReleaseStrategy,
                            AddUser = gp.AddUser,
                            AddDateTime = DateTime.Now,
                            GatePassStatus = GatePass.GatePassStatuses.New
                        };

                        gps.Add(newGp);


                        //create a temp collection to be added to leaveapplicationdto later on
                        List<ApplReleaseStatusDto> apps = new List<ApplReleaseStatusDto>();

                        foreach (var relStratReleaseStrategyLevel in relStrat.GpReleaseStrategyLevels)
                        {
                            //get releaser ID from ReleaseAuth model
                            var relAuth = _context.ReleaseAuth
                                .FirstOrDefault(ra => ra.ReleaseCode == relStratReleaseStrategyLevel.ReleaseCode);


                            ApplReleaseStatus appRelStat = new ApplReleaseStatus
                            {
                                YearMonth = gp.YearMonth,
                                ReleaseGroupCode = gps.Last().ReleaseGroupCode,
                                ApplicationId = gps.Last().Id,
                                ReleaseStrategy = relStratReleaseStrategyLevel.GpReleaseStrategy,
                                ReleaseStrategyLevel = relStratReleaseStrategyLevel.GpReleaseStrategyLevel,
                                ReleaseCode = relStratReleaseStrategyLevel.ReleaseCode,
                                ReleaseStatusCode =
                                    relStratReleaseStrategyLevel.GpReleaseStrategyLevel == 1
                                        ? ReleaseStatus.InRelease
                                        : ReleaseStatus.NotReleased,
                                ReleaseDate = null,
                                ReleaseAuth = relAuth.EmpUnqId,
                                IsFinalRelease = relStratReleaseStrategyLevel.IsFinalRelease
                            };

                            //add to collection
                            apps.Add(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>(appRelStat));
                            result.Add(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>(appRelStat));

                            _context.ApplReleaseStatus.Add(appRelStat);
                        }

                        _context.GatePass.Add(newGp);
                    }

                    _context.SaveChanges();

                    transaction.Commit();

                    return Ok(result);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.ToString());
            }
        }


        [HttpPost]
        [ActionName("CreateLabourGatePass")]
        public IHttpActionResult CreateLabourGatePass([FromBody] object requestData, string workGroup)
        {
            try
            {
                List<RequestGp> dto = JsonConvert.DeserializeObject<List<RequestGp>>(requestData.ToString());

                if (!ModelState.IsValid)
                    return BadRequest();

                int maxId, maxGpId;

                try
                {
                    maxId = _context.GatePass.Max(g => g.Id);
                }
                catch
                {
                    maxId = 0;
                }

                try
                {
                    maxGpId = _context.GatePass.Where(
                            i => DbFunctions.TruncateTime(i.GatePassDate) == DbFunctions.TruncateTime(DateTime.Now))
                        .Max(g => g.GatePassNo);
                }
                catch
                {
                    maxGpId = 0;
                }


                List<GatePass> gps = new List<GatePass>();

                foreach (var gp in dto)
                {
                    var emp = _context.Employees
                        .SingleOrDefault(e => e.EmpUnqId == gp.EmpUnqId);

                    if (emp == null)
                        return BadRequest("Employee not found: " + gp.EmpUnqId.ToString());

                    //loop through all GP details 
                    GatePass newGp = new GatePass
                    {
                        YearMonth = gp.YearMonth,
                        Id = maxId + gp.GatePassItem,
                        GatePassDate = DateTime.Now.Date,
                        GatePassNo = maxGpId + 1,
                        GatePassItem = gp.GatePassItem,
                        EmpUnqId = gp.EmpUnqId,
                        Mode = gp.Mode,
                        PlaceOfVisit = gp.PlaceOfVisit,
                        Reason = gp.Reason,
                        ReleaseGroupCode = gp.ReleaseGroupCode,
                        ReleaseStatusCode = ReleaseStatus.FullyReleased,
                        AddUser = gp.AddUser,
                        AddDateTime = DateTime.Now,
                        GatePassStatus = GatePass.GatePassStatuses.New
                    };

                    gps.Add(newGp);

                    _context.GatePass.Add(newGp);
                }

                _context.SaveChanges();
                return Ok(gps);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.ToString());
            }
        }
    }
}