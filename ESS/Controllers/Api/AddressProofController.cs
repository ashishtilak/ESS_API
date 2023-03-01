using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Migrations;
using ESS.Models;
using Newtonsoft.Json;
using AddressProof = ESS.Models.AddressProof;

namespace ESS.Controllers.Api
{
    public class AddressProofController : ApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly int _maxReq = 5;

        public AddressProofController()
        {
            _context = new ApplicationDbContext();
        }

        // for self address proof requests

        [HttpGet, ActionName("getempaddproof")]
        public IHttpActionResult GetEmpAddProof(string empUnqId)
        {
            List<AddressProofDto> result = _context.AddressProofs.Where(e => e.EmpUnqId == empUnqId).AsEnumerable()
                .Select(Mapper.Map<AddressProof, AddressProofDto>)
                .ToList();

            if (result.Any())
                return Ok(result);

            return BadRequest("No records found!");
        }

        [HttpGet, ActionName("getalladdproof")]
        public IHttpActionResult GetAddressProofs(DateTime fromDate, DateTime toDate)
        {
            List<AddressProofDto> result = _context.AddressProofs
                .Where(e => e.AddDate >= fromDate && e.AddDate <= toDate).AsEnumerable()
                .Select(Mapper.Map<AddressProof, AddressProofDto>)
                .ToList();

            if (result.Any())
                return Ok(result);

            return BadRequest("No records found!");
        }

        // User will create request for Address proof
        [HttpPost]
        public IHttpActionResult AddressProofRequest([FromBody] object requestData)
        {
            try
            {
                var dto = JsonConvert.DeserializeObject<AddressProofDto>(requestData.ToString());

                //TODO: check for duplicate requests...???
                // check for already existing - pending requests
                bool pending = _context.AddressProofs
                    .Any(e => e.EmpUnqId == dto.EmpUnqId &&
                              (e.HrReleaseStatusCode != ReleaseStatus.FullyReleased &&
                               e.HrReleaseStatusCode != ReleaseStatus.ReleaseRejected));
                if (pending)
                    return BadRequest("Can't create new request as pending address proof requests exist.");

                int cnt = _context.AddressProofs
                    .Count(e => e.EmpUnqId == dto.EmpUnqId && e.AddDate.Year == DateTime.Today.Year);

                if(cnt >= _maxReq)
                    return BadRequest("Can't create more than 5 requests per year.");

                // create new record
                var newProof = new AddressProof
                {
                    EmpUnqId = dto.EmpUnqId,
                    ApplicationDate = DateTime.Now.Date,
                    Purpose = dto.Purpose,
                    AddDate = DateTime.Now,
                    HrReleaseStatusCode = ReleaseStatus.FullyReleased,
                    HrReleaseDate = DateTime.Now,
                    HrUser = "Admin",
                    HrRemarks = "auto released"
                };

                _context.AddressProofs.Add(newProof);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            return Ok();
        }

        [HttpGet, ActionName("getrequests")]
        public IHttpActionResult GetPendingRequests()
        {
            // list of requests with status in release
            List<AddressProofDto> result = _context.AddressProofs
                .Where(e => e.HrReleaseStatusCode == ReleaseStatus.InRelease).AsEnumerable()
                .Select(Mapper.Map<AddressProof, AddressProofDto>)
                .ToList();

            if (result.Any())
                return Ok(result);

            return BadRequest("No records found!");
        }

        [HttpPut]
        public IHttpActionResult ReleaseRequest([FromBody] object requestData)
        {
            try
            {
                var dto = JsonConvert.DeserializeObject<AddressProofDto>(requestData.ToString());

                AddressProof addressProof = _context.AddressProofs
                    .FirstOrDefault(e => e.Id == dto.Id);

                if (addressProof == null)
                    return BadRequest("Address proof request not found!");

                if (addressProof.HrReleaseStatusCode != ReleaseStatus.InRelease)
                    return BadRequest("Address proof request is not in release state.");

                if (dto.HrReleaseStatusCode == ReleaseStatus.ReleaseRejected &&
                    string.IsNullOrEmpty(dto.HrRemarks))
                    return BadRequest("Remarks mandatory for rejection.");

                addressProof.HrReleaseStatusCode = dto.HrReleaseStatusCode;
                addressProof.HrRemarks = dto.HrRemarks;
                addressProof.HrUser = dto.HrUser;
                addressProof.HrReleaseDate = DateTime.Now;

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            return Ok();
        }
    }
}