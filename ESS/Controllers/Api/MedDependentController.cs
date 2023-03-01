using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Migrations;
using ESS.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class MedDependentController : ApiController
    {
        private readonly ApplicationDbContext _context;

        private const string ReleaseStrategy = "MC_ADD";
        private const string DelReleaseStrategy = "MC_DEL";

        public MedDependentController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet, ActionName("getdependents")]
        public IHttpActionResult GetDependents(bool mode, int policyYear, string empUnqId = null)
        {
            // MODE will be true if all records of all employees are requested
            // If Mode is false, then empUnqId must have been provided
            try
            {
                List<MedDependentDto> dep;
                List<MedEmpUhidDto> uhids;

                //get dependents 

                if (!mode) //for single emp
                {
                    if (empUnqId == null)
                        return BadRequest("Provide empunqid.");

                    dep = _context.MedDependents
                        .Where(d => d.EmpUnqId == empUnqId)
                        .AsEnumerable()
                        .Select(Mapper.Map<MedDependent, MedDependentDto>)
                        .ToList();

                    if (dep.Count == 0)
                        return BadRequest("No records found.");

                    // get all corresponding UhIds for provided year

                    uhids = _context.MedEmpUhids
                        .Where(e => e.EmpUnqId == empUnqId && e.PolicyYear == policyYear)
                        .AsEnumerable()
                        .Select(Mapper.Map<MedEmpUhid, MedEmpUhidDto>)
                        .ToList();
                }
                else
                {
                    dep = _context.MedDependents
                        .Where(d => d.Active)
                        .AsEnumerable()
                        .Select(Mapper.Map<MedDependent, MedDependentDto>)
                        .ToList();

                    if (dep.Count == 0)
                        return BadRequest("No records found.");

                    // get all corresponding UhIds for provided year

                    var empList = dep.Select(d => d.EmpUnqId).ToArray();

                    uhids = _context.MedEmpUhids
                        .Where(e => e.PolicyYear == policyYear
                                    && empList.Contains(e.EmpUnqId)
                                    && e.Active)
                        .AsEnumerable()
                        .Select(Mapper.Map<MedEmpUhid, MedEmpUhidDto>)
                        .ToList();
                }

                // attach uhid to dto

                foreach (MedDependentDto dto in dep)
                {
                    MedEmpUhidDto uhid = uhids.FirstOrDefault(u => u.EmpUnqId == dto.EmpUnqId && u.DepSr == dto.DepSr);
                    if (dto.UhIds == null)
                        dto.UhIds = new MedEmpUhidDto();
                    dto.UhIds = uhid;
                }

                return Ok(dep);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet, ActionName("getreleased")]
        public IHttpActionResult GetReleasedList(DateTime fromDt, DateTime toDt)
        {
            // list of dependents who are added or deleted
            // and released between given date range

            List<MedDependentDto> dep = _context.MedDependents
                .Where(d => (d.ReleaseStatusCode == ReleaseStatus.FullyReleased ||
                             d.DelReleaseStatusCode == ReleaseStatus.FullyReleased) &&
                            ((d.ReleaseDt >= fromDt && d.ReleaseDt <= toDt) ||
                             (d.DelReleaseDt >= fromDt && d.DelReleaseDt <= toDt))
                )
                .AsEnumerable()
                .Select(Mapper.Map<MedDependent, MedDependentDto>)
                .ToList();

            if (dep.Count == 0)
                return BadRequest("No records found.");

            // get all corresponding UhIds for provided year

            var empList = dep.Select(d => d.EmpUnqId).ToArray();

            List<MedEmpUhidDto> uhids = _context.MedEmpUhids
                .Where(e => empList.Contains(e.EmpUnqId)
                            && e.Active)
                .AsEnumerable()
                .Select(Mapper.Map<MedEmpUhid, MedEmpUhidDto>)
                .ToList();

            foreach (MedDependentDto dto in dep)
            {
                MedEmpUhidDto uhid = uhids.FirstOrDefault(u => u.EmpUnqId == dto.EmpUnqId && u.DepSr == dto.DepSr);
                if (dto.UhIds == null)
                    dto.UhIds = new MedEmpUhidDto();
                dto.UhIds = uhid;
            }

            return Ok(dep);
        }

        [HttpPost]
        public IHttpActionResult CreateDependents([FromBody] object request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid form.");

            List<MedDependentDto> medDependentDto;
            string emp;
            try
            {
                medDependentDto = JsonConvert.DeserializeObject<List<MedDependentDto>>(request.ToString());
                emp = medDependentDto.FirstOrDefault(e => e.DepSr == 0)?.EmpUnqId;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }


            try
            {
                // get current active records of employee
                var currentData = _context.MedDependents.Where(e => e.EmpUnqId == emp && e.Active);

                foreach (MedDependentDto dto in medDependentDto)
                {
                    // if the field has a changed flag
                    if (!dto.IsChanged) continue;

                    // look in current data
                    MedDependent currentRec = currentData.FirstOrDefault(d => d.DepSr == dto.DepSr);

                    // If record exist, then this must be a deleted member
                    if (currentRec != null)
                    {
                        // if record is not deleted in DTO, there's some mistake...
                        // loop over
                        if (dto.Active != false) continue;

                        // Set deletion flag and Deletion release flags accordingly

                        // keep it true.  Will be changed once approved
                        currentRec.Active = true;
                        currentRec.DelReleaseGroupCode = ReleaseGroups.Mediclaim;
                        currentRec.DelReleaseStrategy = DelReleaseStrategy;
                        currentRec.DelReleaseStatusCode = ReleaseStatus.InRelease;
                        currentRec.IsChanged = true;
                        currentRec.AddDate = dto.AddDate;
                        currentRec.AddUser = dto.AddUser;
                    }
                    else
                    {
                        // Record not found in current master
                        // So it must have been added.
                        // Add it to context
                        var newRec = new MedDependent
                        {
                            EmpUnqId = emp,
                            DepSr = dto.DepSr,
                            DepName = dto.DepName.ToUpper(),
                            BirthDate = dto.BirthDate,
                            Gender = dto.Gender,
                            MarriageDate = dto.MarriageDate,
                            EffectiveDate = dto.EffectiveDate,
                            ReleaseGroupCode = ReleaseGroups.Mediclaim,
                            ReleaseStrategy = ReleaseStrategy,
                            ReleaseStatusCode = ReleaseStatus.InRelease,
                            Active = false,
                            Pan = dto.Pan,
                            Aadhar = dto.Aadhar,
                            Relation = dto.Relation,
                            BirthCertificateNo = dto.BirthCertificateNo,
                            AddUser = dto.AddUser,
                            AddDate = DateTime.Now,
                            IsChanged = true // keep this false, will be true upon approval
                        };

                        _context.MedDependents.Add(newRec);
                    }
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex);
            }

            return Ok();
        }

        public IHttpActionResult GetRelease(string releaser)
        {
            var relCode = _context.ReleaseAuth.Where(e => e.EmpUnqId == releaser).Select(r => r.ReleaseCode).ToArray();
            var releasestrategylevels = _context.ReleaseStrategyLevels
                .Where(r => r.ReleaseGroupCode == ReleaseGroups.Mediclaim &&
                            relCode.Contains(r.ReleaseCode))
                .ToList();

            var empList = new List<string>();

            try
            {
                foreach (ReleaseStrategyLevels level in releasestrategylevels)
                {
                    // for release of added records
                    var added = _context.MedDependents
                        .Where(m => m.ReleaseGroupCode == ReleaseGroups.Mediclaim &&
                                    m.ReleaseStrategy == level.ReleaseStrategy &&
                                    m.ReleaseStatusCode == ReleaseStatus.InRelease).AsEnumerable()
                        .Select(e => e.EmpUnqId)
                        .ToArray();

                    // for release of removed records
                    var removed = _context.MedDependents
                        .Where(m => m.DelReleaseGroupCode == ReleaseGroups.Mediclaim &&
                                    m.DelReleaseStrategy == level.ReleaseStrategy &&
                                    m.DelReleaseStatusCode == ReleaseStatus.InRelease).AsEnumerable()
                        .Select(e => e.EmpUnqId)
                        .ToArray();

                    empList.AddRange(added);
                    empList.AddRange(removed);
                }

                if (empList.Count == 0)
                    return BadRequest("No records found.");

                // get all records of current employee which are active
                // or "not active" and delReleaseStatus != F (deleted but not approved)

                var result = _context.MedDependents
                    .Where(e => empList.Contains(e.EmpUnqId) &&
                                !(e.Active == false && e.IsChanged == false)
                    ).AsEnumerable()
                    .Select(Mapper.Map<MedDependent, MedDependentDto>)
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }
        }

        [HttpPut]
        public IHttpActionResult ReleaseDependents([FromBody] object request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Error: Invalid model.");

            try
            {
                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    var medDependents = JsonConvert.DeserializeObject<List<MedDependentDto>>(request.ToString());

                    // run loop for changed items only
                    foreach (MedDependentDto dto in medDependents.Where(dto => dto.IsChanged == true))
                    {
                        // This record was deleted and is under release...
                        MedDependent dependent = _context.MedDependents
                            .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId && e.DepSr == dto.DepSr);

                        if (dependent == null)
                            return BadRequest("Emp:" + dto.EmpUnqId + ", Sr:" + dto.DepSr + " not found.");

                        if (dto.Active == true)
                        {
                            // if fully released
                            if (dto.DelReleaseStatusCode == ReleaseStatus.FullyReleased)
                            {
                                dependent.Active = false;
                                dependent.IsChanged = false;
                                dependent.DelReleaseUser = dto.DelReleaseUser;
                                dependent.DelReleaseDt = DateTime.Now;
                                dependent.DelReleaseStatusCode = dto.DelReleaseStatusCode;
                                dependent.Remarks = dto.Remarks;
                            }
                            else
                            {
                                dependent.Active = true;
                                dependent.IsChanged = false;
                                dependent.DelReleaseUser = dto.DelReleaseUser;
                                dependent.DelReleaseDt = DateTime.Now;
                                dependent.DelReleaseStatusCode = dto.DelReleaseStatusCode;
                                dependent.Remarks = dto.Remarks;
                            }
                        }
                        else
                        {
                            // if fully released, mark the employee as active
                            if (dto.ReleaseStatusCode == ReleaseStatus.FullyReleased)
                            {
                                dependent.Active = true;
                                dependent.IsChanged = false;
                                dependent.ReleaseUser = dto.ReleaseUser;
                                dependent.ReleaseDt = DateTime.Now;
                                dependent.ReleaseStatusCode = dto.ReleaseStatusCode;
                                dependent.Remarks = dto.Remarks;
                            }
                            else
                            {
                                dependent.Active = false;
                                dependent.IsChanged = false;
                                dependent.ReleaseUser = dto.ReleaseUser;
                                dependent.ReleaseDt = DateTime.Now;
                                dependent.ReleaseStatusCode = dto.ReleaseStatusCode;
                                dependent.Remarks = dto.Remarks;
                            }
                        }

                        _context.SaveChanges();
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
    }
}