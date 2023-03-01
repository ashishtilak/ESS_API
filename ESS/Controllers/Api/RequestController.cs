using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class RequestController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public RequestController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetRequest(int requestId)
        {
            Requests req = _context.Requests
                .Include(r => r.RequestDetails)
                .Include(r => r.RequestReleases)
                .FirstOrDefault(r => r.RequestId == requestId);

            if (req == null)
                return BadRequest("No data found.");

            return Ok(Mapper.Map<Requests, RequestDto>(req));
        }


        [HttpPost]
        public IHttpActionResult CreateRequest([FromBody] object requestData)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model state is not valid.");

            RequestDto dto;

            try
            {
                dto = JsonConvert.DeserializeObject<RequestDto>(requestData.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex);
            }

            if (dto.RequestId != 0)
                return BadRequest("Request Id should be 0.");


            // VALIDATION
            // Check if any change request is still pending for approval for same employee

            int existing = _context.Requests
                .Count(r => r.EmpUnqId == dto.EmpUnqId &&
                            (r.ReleaseStatusCode == ReleaseStatus.InRelease ||
                             r.ReleaseStatusCode == ReleaseStatus.NotReleased ||
                             r.ReleaseStatusCode == ReleaseStatus.PartiallyReleased));
            if (existing > 0)
                return BadRequest("Some request is pending for release for employee " + dto.EmpUnqId);
            // over

            // check if date range overlaps
            if (dto.RequestDetails.Count > 1)
            {
                //IEnumerable<RequestDetailsDto> overlaps = dto.RequestDetails.SelectMany(
                //        x1 => dto.RequestDetails,
                //        (x1, x2) => new {x1, x2})
                //    .Where(t => (t.x1.FromDt <= t.x2.ToDt) && (t.x1.ToDt >= t.x2.FromDt))
                //    .Select(t => t.x2);
                bool found = dto.RequestDetails
                    .Any(r => dto.RequestDetails
                        .Where(q => q != r)
                        .Any(q => q.FromDt <= r.ToDt && q.ToDt >= r.FromDt)
                    );

                if (found)
                    return BadRequest("Date ranges must not overlap.");
            }
            // daterange check


            // Cretae request
            int requestId;
            try
            {
                requestId = _context.Requests.Select(r => r.RequestId).Max() + 1;
            }
            catch (Exception)
            {
                requestId = 1;
            }

            var request = new Requests
            {
                RequestId = requestId,
                EmpUnqId = dto.EmpUnqId,
                RequestDate = dto.RequestDate,
                Remarks = dto.Remarks,
                ReleaseGroupCode = ReleaseGroups.ShiftSchedule,
                ReleaseStrategy = dto.EmpUnqId,
                AddDt = DateTime.Now,
                AddUser = dto.AddUser,
                RequestDetails = new List<RequestDetails>(),
                RequestReleases = new List<RequestRelease>()
            };

            // add line items
            foreach (RequestDetailsDto detail in dto.RequestDetails)
            {
                var newDetail = new RequestDetails
                {
                    RequestId = requestId,
                    EmpUnqId = detail.EmpUnqId,
                    Sr = detail.Sr,
                    FromDt = detail.FromDt,
                    ToDt = detail.ToDt,
                    ShiftCode = detail.ShiftCode,
                    Reason = detail.Reason
                };

                request.RequestDetails.Add(newDetail);
            }

            // Get Releaser
            ReleaseStrategies relStr = _context.ReleaseStrategy
                .FirstOrDefault(r => r.ReleaseGroupCode == ReleaseGroups.ShiftSchedule &&
                                     r.ReleaseStrategy == dto.ReleaseStrategy);
            if (relStr == null)
                return BadRequest("Invalid release strategy.");

            List<ReleaseStrategyLevels> relStrLvl = _context.ReleaseStrategyLevels
                .Where(r => r.ReleaseGroupCode == relStr.ReleaseGroupCode &&
                            r.ReleaseStrategy == relStr.ReleaseStrategy)
                .ToList();


            foreach (ReleaseStrategyLevels level in relStrLvl)
            {
                var newRelease = new RequestRelease
                {
                    RequestId = dto.RequestId,
                    EmpUnqId = dto.EmpUnqId,
                    ReleaseStrategy = dto.ReleaseStrategy,
                    ReleaseStrategyLevel = level.ReleaseStrategyLevel,
                    ReleaseGroupCode = level.ReleaseGroupCode,
                    ReleaseCode = level.ReleaseCode,
                    ReleaseStatusCode = ReleaseStatus.NotReleased,
                    IsFinalRelease = level.IsFinalRelease
                };

                if (level.ReleaseStrategyLevel == 1)
                {
                    newRelease.ReleaseStatusCode = ReleaseStatus.FullyReleased;
                    newRelease.ReleaseDate = DateTime.Now;
                    newRelease.ReleaseAuth = dto.AddUser;
                }
                else if (level.ReleaseStrategyLevel == 2)
                {
                    newRelease.ReleaseStatusCode = ReleaseStatus.InRelease;
                }

                request.RequestReleases.Add(newRelease);
            }

            try
            {
                _context.Requests.Add(request);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex);
            }

            return Ok(Mapper.Map<Requests, RequestDto>(request));
        }

        [HttpPost]
        public IHttpActionResult ReleaseRequest([FromBody] object requestData, string releaser)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model state is invalid.");

            RequestReleaseDto dto;

            try
            {
                dto = JsonConvert.DeserializeObject<RequestReleaseDto>(requestData.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex);
            }

            RequestRelease reqRelease = _context.RequestReleases.SingleOrDefault(
                r => r.RequestId == dto.RequestId &&
                     r.EmpUnqId == dto.EmpUnqId &&
                     r.ReleaseStrategy == dto.ReleaseStrategy &&
                     r.ReleaseStrategyLevel == dto.ReleaseStrategyLevel);

            if (reqRelease == null)
                return BadRequest("Invalid release object.");

            if (reqRelease.ReleaseStatusCode != ReleaseStatus.InRelease)
                return BadRequest("Application is not in release state.");


            Requests request = _context.Requests
                .FirstOrDefault(r => r.RequestId == dto.RequestId);
            if (request == null)
                return BadRequest("Request not found.");

            request.Remarks = dto.Remarks;


            using (DbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                // if rejected
                if (dto.ReleaseStatusCode == ReleaseStatus.ReleaseRejected)
                {
                    request.ReleaseStatusCode = dto.ReleaseStatusCode;
                    request.Remarks = dto.Remarks;
                }
                else
                {
                    if (dto.IsFinalRelease)
                    {
                        request.ReleaseStatusCode = ReleaseStatus.FullyReleased;
                    }
                    else
                    {
                        request.ReleaseStatusCode = ReleaseStatus.PartiallyReleased;

                        // set release status for next level release, if any
                        RequestRelease nextRec = _context.RequestReleases
                            .FirstOrDefault(
                                r => r.RequestId == dto.RequestId &&
                                     r.EmpUnqId == dto.EmpUnqId &&
                                     r.ReleaseStrategy == dto.ReleaseStrategy &&
                                     r.ReleaseStrategyLevel == dto.ReleaseStrategyLevel + 1);
                        if (nextRec == null)
                            return BadRequest("Next level release not found!");

                        nextRec.ReleaseStatusCode = ReleaseStatus.InRelease;
                    }
                }

                reqRelease.ReleaseStatusCode = dto.ReleaseStatusCode;
                reqRelease.ReleaseDate = DateTime.Now;
                reqRelease.ReleaseAuth = dto.ReleaseAuth;
                reqRelease.Remarks = dto.Remarks;

                _context.SaveChanges();
                transaction.Commit();
            }

            return Ok(Mapper.Map<RequestRelease, RequestReleaseDto>(reqRelease));
        }

        [HttpPut]
        public IHttpActionResult PostRequest(int requestId, string postingFlag, string postUser, string remarks)
        {
            try
            {
                Requests request = _context.Requests
                    .Include(r => r.RequestDetails)
                    .Include(r => r.RequestReleases)
                    .FirstOrDefault(r => r.RequestId == requestId);

                if (request == null)
                    return BadRequest("Request not found.");

                if (request.ReleaseStatusCode != ReleaseStatus.FullyReleased)
                    return BadRequest("Request is not fully released.");

                if (request.IsPosted == ReleaseStatus.FullyReleased)
                    return BadRequest("Request is already posted.");

                // handle rejection
                if (postingFlag == ReleaseStatus.ReleaseRejected)
                {
                    request.ReleaseStatusCode = ReleaseStatus.ReleaseRejected;
                    request.IsPosted = ReleaseStatus.ReleaseRejected;
                }
                else
                {
                    request.IsPosted = ReleaseStatus.FullyReleased;
                }

                request.Remarks = "HR:" + remarks;
                request.PostUser = postUser;
                request.PostedDt = DateTime.Now;

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex);
            }

            return Ok();
        }

        public enum ReportModes
        {
            Emp = 0,
            Supervisor = 1,
            Releaser = 2,
            Hr = 3,
            HrFull = 4,
            HrTemplate = 5
        }

        public IHttpActionResult GetRequest(DateTime fromDt, DateTime toDt, ReportModes mode, string empUnqId = "")
        {
            List<RequestDto> result;

            string[] relCodes;
            int[] relStrLvl;
            switch (mode)
            {
                // for specific employee, get all requests during period
                case ReportModes.Emp:
                    result = _context.Requests
                        .Include(r => r.RequestDetails)
                        .Include(r => r.RequestReleases)
                        .Where(r =>
                            r.EmpUnqId == empUnqId &&
                            r.RequestDate >= fromDt && r.RequestDate <= toDt)
                        .AsEnumerable()
                        .Select(Mapper.Map<Requests, RequestDto>)
                        .ToList();
                    break;

                // for specific supervisor, get all requests under his release during period
                case ReportModes.Supervisor:
                    //He's creator, means releaselevel will be 1
                    // get release code from releaseauth
                    relCodes = _context.ReleaseAuth
                        .Where(e => e.EmpUnqId == empUnqId)
                        .Select(e => e.ReleaseCode)
                        .ToArray();

                    // get all requests of those codes
                    relStrLvl = _context.RequestReleases
                        .Where(r =>
                            r.ReleaseGroupCode == ReleaseGroups.ShiftSchedule &&
                            relCodes.Contains(r.ReleaseCode) &&
                            r.ReleaseStrategyLevel == 1)
                        .Select(r => r.RequestId)
                        .ToArray();

                    // get requests
                    result = _context.Requests
                        .Include(r => r.RequestDetails)
                        .Include(r => r.RequestReleases)
                        .Where(r =>
                            relStrLvl.Contains(r.RequestId) &&
                            r.RequestDate >= fromDt && r.RequestDate <= toDt)
                        .AsEnumerable()
                        .Select(Mapper.Map<Requests, RequestDto>)
                        .ToList();

                    break;

                // for releasers, get requests that are In release
                case ReportModes.Releaser:
                    relCodes = _context.ReleaseAuth
                        .Where(e => e.EmpUnqId == empUnqId)
                        .Select(e => e.ReleaseCode)
                        .ToArray();

                    // get all requests of those codes which are in release
                    relStrLvl = _context.RequestReleases
                        .Where(r =>
                            r.ReleaseGroupCode == ReleaseGroups.ShiftSchedule &&
                            relCodes.Contains(r.ReleaseCode) &&
                            r.ReleaseStatusCode == ReleaseStatus.InRelease)
                        .Select(r => r.RequestId)
                        .ToArray();

                    // get requests
                    result = _context.Requests
                        .Include(r => r.RequestDetails)
                        .Include(r => r.RequestReleases)
                        .Where(r =>
                            relStrLvl.Contains(r.RequestId) &&
                            r.RequestDate >= fromDt && r.RequestDate <= toDt)
                        .AsEnumerable()
                        .Select(Mapper.Map<Requests, RequestDto>)
                        .ToList();

                    break;

                // For HR, get all released requests during period
                case ReportModes.Hr:
                    result = _context.Requests
                        .Include(r => r.RequestDetails)
                        .Include(r => r.RequestReleases)
                        .Where(r =>
                            r.RequestDate >= fromDt && r.RequestDate <= toDt &&
                            r.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                            r.IsPosted != ReleaseStatus.FullyReleased)
                        .AsEnumerable()
                        .Select(Mapper.Map<Requests, RequestDto>)
                        .ToList();
                    break;
                // For HR, get all released requests during period
                case ReportModes.HrFull:
                case ReportModes.HrTemplate:
                    result = _context.Requests
                        .Include(r => r.RequestDetails)
                        .Include(r => r.RequestReleases)
                        .Where(r =>
                            r.RequestDate >= fromDt && r.RequestDate <= toDt &&
                            r.ReleaseStatusCode == ReleaseStatus.FullyReleased)
                        .AsEnumerable()
                        .Select(Mapper.Map<Requests, RequestDto>)
                        .ToList();
                    break;
                default:
                    return BadRequest("Invalid mode.");
            }

            if (result.Count <= 0)
                return BadRequest("No records found.");

            // get employee name of emp and supervisor
            foreach (RequestDto dto in result)
            {
                string[] empList = dto.RequestDetails.Select(e => e.EmpUnqId).Distinct().ToArray();
                var empName = _context.Employees
                    .Where(e => empList.Contains(e.EmpUnqId))
                    .Select(e => new
                    {
                        empUnqId = e.EmpUnqId,
                        empName = e.EmpName,
                    })
                    .ToArray();

                foreach (RequestDetailsDto detail in dto.RequestDetails)
                {
                    detail.EmpName = empName.FirstOrDefault(e => e.empUnqId == detail.EmpUnqId)?.empName;
                }

                dto.EmpName = empName.First().empName;
                dto.AddUserName = _context.Employees
                    .FirstOrDefault(e => e.EmpUnqId == dto.AddUser)?.EmpName;
            }

            // Return OK if template is not required.
            if (mode != ReportModes.HrTemplate)
                return Ok(result);

            // ELSE Generate template and return template
            var returnData = new List<DataTemplate>();

            foreach (RequestDto requestDto in result)
            {
                // get details
                foreach (RequestDetailsDto dto in requestDto.RequestDetails)
                {
                    // loop between date range
                    for (DateTime? dt = dto.FromDt; dt <= dto.ToDt;)
                    {
                        var temp = new DataTemplate
                        {
                            EmpUnqId = requestDto.EmpUnqId,
                            SanDate = dt.Value,
                            InTime = "",
                            OutTime = "",
                            ShiftCode = dto.ShiftCode,
                            TpaHours = "0",
                            Reason = dto.Reason
                        };
                        returnData.Add(temp);
                        dt = dt.Value.AddDays(1);
                    }
                }
            }

            return Ok(returnData);
        }

        private class DataTemplate
        {
            public string EmpUnqId { get; set; }
            public DateTime SanDate { get; set; }
            public string InTime { get; set; }
            public string OutTime { get; set; }
            public string ShiftCode { get; set; }
            public string TpaHours { get; set; }
            public string Reason { get; set; }
        }
    }
}


//EmpUnqID	SanDate	InTime	OutTime	ShiftCode	TPAHours	Reason
//104019	2020-09-26	09:00	18:00			Forgot Both  punch
//104019	2020-09-26	09:00				Machine problem
//112244	2020-09-26		20:00			Forgot Out punch