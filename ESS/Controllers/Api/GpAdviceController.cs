using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class GpAdviceController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public GpAdviceController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetGpAdvice(int yearMonth, int gpAdviceNo)
        {
            var gpAdvices = _context.GpAdvices
                .Where(g => g.YearMonth == yearMonth && g.GpAdviceNo == gpAdviceNo)
                .Include(d => d.GpAdviceDetails)
                .ToList()
                .Select(Mapper.Map<GpAdvices, GpAdviceDto>).ToList();

            foreach (var advice in gpAdvices)
            {
                //Get apprelease status
                var app = _context.ApplReleaseStatus
                    .Where(l =>
                        l.YearMonth == advice.YearMonth &&
                        l.ReleaseGroupCode == advice.ReleaseGroupCode &&
                        l.ApplicationId == advice.GpAdviceNo)
                    .ToList()
                    .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>);

                var emp = _context.Employees
                    .Include(d => d.Departments)
                    .Include(s => s.Stations)
                    .FirstOrDefault(e => e.EmpUnqId == advice.EmpUnqId);
                if (emp != null)
                {
                    advice.EmpName = emp.EmpName;
                    advice.DeptName = emp.Departments.DeptName;
                    advice.StatName = emp.Stations.StatName;
                    advice.ModeName = advice.GpAdviceType == GpAdviceTypes.ReturnableGatePassAdvice ? "RGP" : "NRGP";
                }

                advice.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
                advice.ApplReleaseStatus.AddRange(app);
            }

            return Ok(gpAdvices);
        }

        public IHttpActionResult GetGpAdvice(string empUnqId)
        {
            var gpAdvices = _context.GpAdvices
                .Where(g => g.EmpUnqId == empUnqId)
                .Include(d => d.GpAdviceDetails)
                .ToList()
                .Select(Mapper.Map<GpAdvices, GpAdviceDto>).ToList();

            foreach (var advice in gpAdvices)
            {
                //Get apprelease status
                var app = _context.ApplReleaseStatus
                    .Where(l =>
                        l.YearMonth == advice.YearMonth &&
                        l.ReleaseGroupCode == advice.ReleaseGroupCode &&
                        l.ApplicationId == advice.GpAdviceNo)
                    .ToList()
                    .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>);

                advice.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
                advice.ApplReleaseStatus.AddRange(app);
            }

            return Ok(gpAdvices);
        }

        public IHttpActionResult GetGpAdviceForPosting(DateTime fromDt, DateTime toDt, bool posted)
        {
            List<GpAdviceDto> result = new List<GpAdviceDto>();

            List<GpAdviceDto> gpAdvices;

            if (posted)
            {
                gpAdvices = _context.GpAdvices
                    .Where(g =>
                        g.GpAdviceDate >= fromDt &&
                        g.GpAdviceDate <= toDt &&
                        g.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                        g.GpAdviceStatus == GpAdviceStatuses.NotPosted
                    )
                    .Include(d => d.GpAdviceDetails)
                    .ToList()
                    .Select(Mapper.Map<GpAdvices, GpAdviceDto>).ToList();
            }
            else
            {
                gpAdvices = _context.GpAdvices
                    .Where(g =>
                        g.GpAdviceDate >= fromDt &&
                        g.GpAdviceDate <= toDt
                    )
                    .Include(d => d.GpAdviceDetails)
                    .ToList()
                    .Select(Mapper.Map<GpAdvices, GpAdviceDto>).ToList();
            }

            foreach (var advice in gpAdvices)
            {
                var emp = _context.Employees
                    .Include(d => d.Departments)
                    .Include(s => s.Stations)
                    .FirstOrDefault(e => e.EmpUnqId == advice.EmpUnqId);

                if (emp != null)
                {
                    advice.EmpName = emp.EmpName;
                    advice.DeptName = emp.Departments.DeptName;
                    advice.StatName = emp.Stations.StatName;
                }

                //Get apprelease status
                var app = _context.ApplReleaseStatus
                    .Where(l =>
                        l.YearMonth == advice.YearMonth &&
                        l.ReleaseGroupCode == advice.ReleaseGroupCode &&
                        l.ApplicationId == advice.GpAdviceNo)
                    .ToList()
                    .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>);

                advice.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
                advice.ApplReleaseStatus.AddRange(app);
            }

            result.AddRange(gpAdvices);
            return Ok(result);
        }

        public IHttpActionResult GetGpAdvice(string empUnqId, DateTime fromDt, DateTime toDt)
        {
            //Get list of employees under this releaser.
            //get employee details
            var releaser = _context.Employees.Single(e => e.EmpUnqId == empUnqId);
            if (releaser == null)
                return BadRequest("Invalid employee code.");

            //return if employee is not a releaser
            if (!releaser.IsGaReleaser)
                return BadRequest("Employee is not authorized to release (check flag).");

            //if he's a releaser, get his release code
            //and based on the code, get all his release strategy levels
            var relCode = _context.ReleaseAuth.Where(r => r.EmpUnqId == releaser.EmpUnqId).ToList();

            List<GpAdviceDto> result = new List<GpAdviceDto>();
            foreach (var releaseAuth in relCode)
            {
                var relStrategyLevel = _context.GaReleaseStrategyLevels
                    .Include(r => r.GaReleaseStrategies)
                    .Where(r => r.ReleaseCode == releaseAuth.ReleaseCode
                                && r.ReleaseGroupCode == ReleaseGroups.GatePassAdvice)
                    .ToList();

                var relStrategy = relStrategyLevel.Select(level => level.GaReleaseStrategies).ToList();

                //and for each strategy we found above,
                //search for employee who match the release criteria
                foreach (var strategy in relStrategy)
                {
                    var relEmployee = _context.Employees
                        .Include(d => d.Departments)
                        .Include(s => s.Stations)
                        .Where(
                            e => e.EmpUnqId == strategy.GaReleaseStrategy &&
                                 strategy.Active
                        )
                        .ToList()
                        .Select(Mapper.Map<Employees, EmployeeDto>).ToList();

                    //fill details of employee
                    foreach (var emp in relEmployee)
                    {
                        var gpAdvices = _context.GpAdvices
                            .Where(
                                g => g.EmpUnqId == emp.EmpUnqId &&
                                     g.GpAdviceDate >= fromDt &&
                                     g.GpAdviceDate <= toDt
                            )
                            .Include(d => d.GpAdviceDetails)
                            .ToList()
                            .Select(Mapper.Map<GpAdvices, GpAdviceDto>).ToList();

                        foreach (var advice in gpAdvices)
                        {
                            advice.EmpName = emp.EmpName;
                            advice.DeptName = emp.DeptName;
                            advice.StatName = emp.StatName;

                            //Get apprelease status
                            var app = _context.ApplReleaseStatus
                                .Where(l =>
                                    l.YearMonth == advice.YearMonth &&
                                    l.ReleaseGroupCode == advice.ReleaseGroupCode &&
                                    l.ApplicationId == advice.GpAdviceNo)
                                .ToList()
                                .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>);

                            advice.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
                            advice.ApplReleaseStatus.AddRange(app);
                        }

                        result.AddRange(gpAdvices);
                    }
                }
            }

            result = result.GroupBy(r => r.GpAdviceNo).Select(r => r.First()).ToList();
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult CreateGpAdvice([FromBody] object requestData)
        {
            var advice = JsonConvert.DeserializeObject<GpAdviceDto>(requestData.ToString());
            if (!ModelState.IsValid)
                return BadRequest("Invalid Model State.");
            if (advice.GpAdviceNo != 0) return BadRequest("Why there's GpAdvice No?");

            //get the next id from db and fill GpAdvice data
            int maxId;
            try

            {
                maxId = _context.GpAdvices.Max(i => i.GpAdviceNo);
            }
            catch
            {
                maxId = 0;
            }

            advice.GpAdviceNo = maxId + 1;
            foreach (var details in advice.GpAdviceDetails)
            {
                details.GpAdviceNo = advice.GpAdviceNo;
            }

            var relStrat = _context.GaReleaseStrategies
                .FirstOrDefault(
                    r =>
                        r.ReleaseGroupCode == advice.ReleaseGroupCode &&
                        r.GaReleaseStrategy == advice.EmpUnqId &&
                        r.Active == true
                );
            if (relStrat == null)
                return BadRequest("Release strategy not configured.");
            advice.ReleaseGroupCode = advice.ReleaseGroupCode;
            advice.GaReleaseStrategy = advice.GaReleaseStrategy;
            advice.ReleaseStatusCode = ReleaseStatus.NotReleased;

            //get release strategy levels
            var relStratLevels = _context.GaReleaseStrategyLevels
                .Where(
                    rl =>
                        rl.ReleaseGroupCode == advice.ReleaseGroupCode &&
                        rl.GaReleaseStrategy == relStrat.GaReleaseStrategy
                ).ToList();
            relStrat.GaReleaseStrategyLevels = relStratLevels;

            //Now for each release strategy details record create ApplReleaseStatus record

            //create a temp collection to be added to leaveapplicationdto later on
            List<ApplReleaseStatusDto> appReleaseStrategies = new List<ApplReleaseStatusDto>();
            foreach (var relLevel in relStrat.GaReleaseStrategyLevels)
            {
                //get releaser ID from ReleaseAuth model
                var relAuth = _context.ReleaseAuth
                    .FirstOrDefault(ra => ra.ReleaseCode == relLevel.ReleaseCode);
                if (relAuth == null) return BadRequest("Release auth configuration error.");

                ApplReleaseStatus appRelStrat = new ApplReleaseStatus
                {
                    YearMonth = advice.YearMonth,
                    ReleaseGroupCode = advice.ReleaseGroupCode,
                    ApplicationId = advice.GpAdviceNo,
                    ReleaseStrategy = relLevel.GaReleaseStrategy,
                    ReleaseStrategyLevel = relLevel.GaReleaseStrategyLevel,
                    ReleaseCode = relLevel.ReleaseCode,
                    ReleaseStatusCode =
                        relLevel.GaReleaseStrategyLevel == 1
                            ? ReleaseStatus.InRelease
                            : ReleaseStatus.NotReleased,
                    ReleaseDate = null,
                    ReleaseAuth = relAuth.EmpUnqId,
                    IsFinalRelease = relLevel.IsFinalRelease
                };

                //add to collection
                appReleaseStrategies.Add(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>(appRelStrat));
                _context.ApplReleaseStatus.Add(appRelStrat);
            }

            advice.AddDt = DateTime.Now;
            _context.GpAdvices.Add(Mapper.Map<GpAdviceDto, GpAdvices>(advice));
            _context.SaveChanges();

            //Create app release status object and add all app release lines
            advice.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
            advice.ApplReleaseStatus.AddRange(appReleaseStrategies);
            return Created(new Uri(Request.RequestUri + "?gpAdviceNo=" + advice.GpAdviceNo), advice);
        }

        public class GpAdvicePostObject
        {
            public int YearMonth { get; set; }
            public int GpAdviceNo { get; set; }
            public string PostingStatus { get; set; }
            public string EmpUnqId { get; set; }
            public string SapGpNumber { get; set; }
            public string Remarks { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostGpAdvice([FromBody] object requestData, bool flag)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid model state.");

            try
            {
                GpAdvicePostObject obj = JsonConvert.DeserializeObject<GpAdvicePostObject>(requestData.ToString());

                //put this in transaction as multiple tables are updated
                using (var transaction = _context.Database.BeginTransaction())
                {
                    //if Posting status is Fully Posted, then only update status
                    var advice = _context.GpAdvices
                        .FirstOrDefault(g =>
                            g.YearMonth == obj.YearMonth &&
                            g.GpAdviceNo == obj.GpAdviceNo);
                    if (advice == null)
                        return BadRequest("Invalid gate pass advice.");


                    advice.GpAdviceStatus = GpAdviceStatuses.FullyPosted;
                    advice.PostedDt = DateTime.Now;
                    advice.PostedUser = obj.EmpUnqId;
                    advice.SapGpNumber = obj.SapGpNumber;
                    advice.Remarks = obj.Remarks;

                    if (obj.PostingStatus == GpAdviceStatuses.PostingRejected)
                    {
                        //here update the GpAdvice statua
                        //also set release rejected in app release table also

                        var apps = _context.ApplReleaseStatus
                            .Where(a =>
                                a.ReleaseGroupCode == ReleaseGroups.GatePassAdvice &&
                                a.YearMonth == advice.YearMonth &&
                                a.ApplicationId == advice.GpAdviceNo)
                            .ToList();

                        foreach (var app in apps)
                        {
                            app.ReleaseStatusCode = ReleaseStatus.ReleaseRejected;
                            app.Remarks = "Posting rejected.";
                        }

                        advice.GpAdviceStatus = GpAdviceStatuses.PostingRejected;
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// For Updating Gatepass Advice
        /// </summary>
        /// <param name="requestData">GpAdvice Dto Object</param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult UpdateGpAdvice([FromBody] object requestData)
        {
            var adviceDto = JsonConvert.DeserializeObject<GpAdviceDto>(requestData.ToString());
            if (!ModelState.IsValid)
                return BadRequest("Invalid Model State.");
            GpAdvices advice;
            if (adviceDto.GpAdviceNo != 0)
            {
                advice = _context.GpAdvices
                    .Where(g => g.YearMonth == adviceDto.YearMonth && g.GpAdviceNo == adviceDto.GpAdviceNo)
                    .Include(d => d.GpAdviceDetails)
                    .FirstOrDefault();
                if (advice == null) return BadRequest("Invalid Gatepass Advice number.");

//update properties:
                advice.GpAdviceDate = DateTime.Now.Date;
                advice.EmpUnqId = adviceDto.EmpUnqId;
                advice.UnitCode = adviceDto.UnitCode;
                advice.DeptCode = adviceDto.DeptCode;
                advice.StatCode = adviceDto.StatCode;
                advice.GpAdviceType = adviceDto.GpAdviceType;
                advice.Purpose = adviceDto.Purpose;
                advice.WorkOrderNo = adviceDto.WorkOrderNo;
                advice.VendorCode = adviceDto.VendorCode;
                advice.VendorAddress1 = adviceDto.VendorAddress1;
                advice.VendorAddress2 = adviceDto.VendorAddress2;
                advice.VendorAddress3 = adviceDto.VendorAddress3;
                advice.ApproxDateOfReturn = adviceDto.ApproxDateOfReturn;
                advice.ModeOfTransport = adviceDto.ModeOfTransport;
                advice.TransporterName = adviceDto.TransporterName;
                advice.GpAdviceStatus = adviceDto.GpAdviceStatus;
                advice.ReleaseGroupCode = adviceDto.ReleaseGroupCode;
                advice.GaReleaseStrategy = adviceDto.GaReleaseStrategy;
                advice.ReleaseStatusCode = ReleaseStatus.NotReleased;
                advice.Requisitioner = adviceDto.Requisitioner;
                advice.PoTerms = adviceDto.PoTerms;
                advice.UpdDt = DateTime.Now;
                advice.UpdUser = adviceDto.EmpUnqId;
            }
            else
                return BadRequest("Invalid Gatepass Advice number.");

//Delete already existing line items 
            foreach (var detail in advice.GpAdviceDetails.ToList())
            {
                advice.GpAdviceDetails.Remove(detail);
            }

//and add new one from received data
            foreach (var detail in adviceDto.GpAdviceDetails)
            {
                var newdetail = Mapper.Map<GpAdviceDetailsDto, GpAdviceDetails>(detail);
                advice.GpAdviceDetails.Add(newdetail);
            }

//Recheck release strategy
            var relStrat = _context.GaReleaseStrategies
                .FirstOrDefault(
                    r =>
                        r.ReleaseGroupCode == advice.ReleaseGroupCode &&
                        r.GaReleaseStrategy == advice.EmpUnqId &&
                        r.Active == true
                );
            if (relStrat == null)
                return BadRequest("Release strategy not configured.");
            advice.ReleaseGroupCode = advice.ReleaseGroupCode;
            advice.GaReleaseStrategy = advice.GaReleaseStrategy;
            advice.ReleaseStatusCode = ReleaseStatus.NotReleased;

//get release strategy levels
            var relStratLevels = _context.GaReleaseStrategyLevels
                .Where(
                    rl =>
                        rl.ReleaseGroupCode == advice.ReleaseGroupCode &&
                        rl.GaReleaseStrategy == relStrat.GaReleaseStrategy
                ).ToList();
            relStrat.GaReleaseStrategyLevels = relStratLevels;

//First Remove all app release strategy details 
            var apps = _context.ApplReleaseStatus
                .Where(a => a.ReleaseGroupCode == advice.ReleaseGroupCode
                            && a.ApplicationId == advice.GpAdviceNo)
                .ToList();
            var val = _context.ApplReleaseStatus.RemoveRange(apps);

//Now for each release strategy details record create ApplReleaseStatus record

//create a temp collection to be added to leaveapplicationdto later on
            List<ApplReleaseStatusDto> appReleaseStrategies = new List<ApplReleaseStatusDto>();
            foreach (var relLevel in relStrat.GaReleaseStrategyLevels)
            {
//get releaser ID from ReleaseAuth model
                var relAuth = _context.ReleaseAuth
                    .FirstOrDefault(ra => ra.ReleaseCode == relLevel.ReleaseCode);
                if (relAuth == null) return BadRequest("Release auth configuration error.");
                ApplReleaseStatus appRelStrat = new ApplReleaseStatus
                {
                    YearMonth = advice.YearMonth,
                    ReleaseGroupCode = advice.ReleaseGroupCode,
                    ApplicationId = advice.GpAdviceNo,
                    ReleaseStrategy = relLevel.GaReleaseStrategy,
                    ReleaseStrategyLevel = relLevel.GaReleaseStrategyLevel,
                    ReleaseCode = relLevel.ReleaseCode,
                    ReleaseStatusCode =
                        relLevel.GaReleaseStrategyLevel == 1
                            ? ReleaseStatus.InRelease
                            : ReleaseStatus.NotReleased,
                    ReleaseDate = null,
                    ReleaseAuth = relAuth.EmpUnqId,
                    IsFinalRelease = relLevel.IsFinalRelease
                };

//add to collection
                appReleaseStrategies.Add(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>(appRelStrat));
                _context.ApplReleaseStatus.Add(appRelStrat);
            }

            _context.SaveChanges();
            var result = Mapper.Map<GpAdvices, GpAdviceDto>(advice);

//Create app release status object and add all app release lines
            result.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
            result.ApplReleaseStatus.AddRange(appReleaseStrategies);
            return Ok(result);
        }
    }
}