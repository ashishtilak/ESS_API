using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class VehicleReqController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public VehicleReqController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        public IHttpActionResult CreateReq([FromBody] object requestData)
        {
            try
            {
                var dto = JsonConvert.DeserializeObject<VehicleReqDto>(requestData.ToString());
                if (dto == null) return BadRequest("Invalid request data.");

                bool existing = _context.VehicleReqs
                    .Any(v => v.BookingDate == dto.BookingDate && 
                              v.BookingSlot == dto.BookingSlot &&
                              v.BookingStatus == true);

                if (existing)
                    return BadRequest("Booking date/slot is already full.");

                var req = new VehicleReq
                {
                    EmpUnqId = dto.EmpUnqId,
                    ReqDate = DateTime.Now,
                    BookingDate = dto.BookingDate,
                    BookingSlot = dto.BookingSlot,
                    BookingStatus = false,
                    PickupTime = dto.PickupTime,
                    PickupLocation = dto.PickupLocation,
                    NumberOfPass = dto.NumberOfPass,
                    DropLocation = dto.DropLocation,
                    Remarks = dto.Remarks,
                    AddDt = DateTime.Now,
                    AddUser = dto.AddUser,
                    ReleaseGroupCode = ReleaseGroups.LeaveApplication,
                    ReleaseStrategy = dto.EmpUnqId,
                    ReleaseStatusCode = ReleaseStatus.InRelease,
                    ReleaseRemarks = "",
                    ReleaseAuth = "",
                    AdminReleaseStatusCode = ReleaseStatus.NotReleased
                };

                // get final level releaser
                var releaser = _context.ReleaseStrategyLevels
                    .FirstOrDefault(l => l.ReleaseGroupCode == ReleaseGroups.LeaveApplication &&
                                         l.ReleaseStrategy == req.ReleaseStrategy &&
                                         l.IsFinalRelease == true);

                if (releaser != null)
                    req.ReleaseCode = releaser.ReleaseCode;

                // IF GRADE OF EMPLOYEE IS GM OR ABOVE
                // RELEASE THE REQUEST AUTOMATICALLY

                string wrkgrp = _context.Employees.FirstOrDefault(e=>e.EmpUnqId == dto.EmpUnqId)?.WrkGrp;
                string grade = _context.Employees.FirstOrDefault(e=>e.EmpUnqId == dto.EmpUnqId)?.GradeCode;
                if((wrkgrp != null && grade != null) && int.Parse(grade) <= int.Parse("010"))
                {
                    req.ReleaseStatusCode = ReleaseStatus.FullyReleased;
                    req.ReleaseAuth = dto.EmpUnqId;
                    req.ReleaseDate = DateTime.Now;
                    req.ReleaseRemarks = "Auto self-release (GM & Above).";

                    if (dto.ReleaseStatusCode != ReleaseStatus.ReleaseRejected)
                    {
                        req.AdminReleaseStatusCode = ReleaseStatus.InRelease;
                        req.BookingStatus = true;
                    }
                }

                _context.VehicleReqs.Add(req);
                _context.SaveChanges();

                return Ok(Mapper.Map<VehicleReq, VehicleReqDto>(req));
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }
        }

        public IHttpActionResult GetReqForRel(string empUnqId, bool isAdmin)
        {
            List<VehicleReq> vehicleReqs;

            if (isAdmin)
            {
                // this is for admin user final release
                vehicleReqs = _context.VehicleReqs
                    .Where(v => v.AdminReleaseStatusCode == ReleaseStatus.InRelease)
                    .ToList();
                if (!vehicleReqs.Any())
                    return BadRequest("No pending vehicle requisitions found.");
            }
            else
            {
                // this is for hod release
                var auth = _context.ReleaseAuth
                    .Where(e => e.EmpUnqId == empUnqId && e.Active)
                    .Select(r => r.ReleaseCode);

                if (!auth.Any())
                    return BadRequest("Release auth not found for employee.");

                var relStr = _context.ReleaseStrategyLevels
                    .Where(r => r.ReleaseGroupCode == ReleaseGroups.LeaveApplication &&
                                auth.Contains(r.ReleaseCode) &&
                                r.IsFinalRelease == true)
                    .Select(r => r.ReleaseStrategy);

                if (!relStr.Any())
                    return BadRequest("Release strategy not found for releaser.");

                vehicleReqs = _context.VehicleReqs
                    .Where(v => relStr.Contains(v.EmpUnqId) &&
                                v.ReleaseStatusCode == ReleaseStatus.InRelease)
                    .ToList();
                if (!vehicleReqs.Any())
                    return BadRequest("No pending vehicle requisitions found.");
            }

            List<VehicleReqDto> reqDtos = Mapper.Map<List<VehicleReq>, List<VehicleReqDto>>(vehicleReqs);
            var empList = reqDtos.Select(e => e.EmpUnqId).ToList();

            var empListDto = _context.Employees
                .Where(e => empList.Contains(e.EmpUnqId))
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
                }).ToList();
            foreach (VehicleReqDto dto in reqDtos)
            {
                EmployeeDto dtoEmp = empListDto.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                if (dtoEmp == null) continue;

                dto.EmpName = dtoEmp.EmpName;
                dto.DeptName = dtoEmp.DeptName;
                dto.StatName = dtoEmp.StatName;
                dto.CatName = dtoEmp.CatName;
                dto.GradeName = dtoEmp.GradeName;
            }

            return Ok(reqDtos);
        }

        [HttpPut]
        public IHttpActionResult ReleaseReq(bool isAdmin, [FromBody] object requestData)
        {
            var dto = JsonConvert.DeserializeObject<VehicleReqDto>(requestData.ToString());
            if (dto == null) return BadRequest("Invalid object.");

            if (dto.ReqId == 0)
                return BadRequest("Request id invalid.");

            VehicleReq req = _context.VehicleReqs
                .FirstOrDefault(v => v.ReqId == dto.ReqId);
            if (req == null) return BadRequest("Incorrect request id.");


            if (isAdmin)
            {
                // ADMIN RELEASER
                if (!(req.AdminReleaseStatusCode == ReleaseStatus.InRelease ||
                      req.AdminReleaseStatusCode == ReleaseStatus.FullyReleased))
                {
                    return BadRequest("Request is not in release state.");
                }

                if (req.AdminReleaseStatusCode == ReleaseStatus.FullyReleased &&
                    dto.AdminReleaseStatusCode != ReleaseStatus.ReleaseRejected)
                    return BadRequest("You can only reject fully released req.");

                if (req.ReleaseStatusCode != ReleaseStatus.FullyReleased)
                    return BadRequest("HOD release pending.");

                req.AdminReleaseStatusCode = dto.AdminReleaseStatusCode;
                req.AdminUser = dto.AdminUser;
                req.AdminReleaseDate = DateTime.Now;
                req.AdminReleaseRemarks = dto.AdminReleaseRemarks;
                req.BookingStatus = true;

                if (req.AdminReleaseStatusCode == ReleaseStatus.ReleaseRejected)
                    req.BookingStatus = false;

                _context.SaveChanges();

                return Ok();
            }
            else
            {
                if (req.ReleaseStatusCode != ReleaseStatus.InRelease)
                    return BadRequest("Request is not in release state.");

                var releaseCode = _context.ReleaseStrategyLevels
                    .FirstOrDefault(r => r.ReleaseGroupCode == ReleaseGroups.LeaveApplication &&
                                         r.ReleaseStrategy == dto.EmpUnqId &&
                                         r.IsFinalRelease == true)
                    ?.ReleaseCode;
                if (releaseCode == null) return BadRequest("Release code not found.");
                var relAuth = _context.ReleaseAuth
                    .FirstOrDefault(r => r.ReleaseCode == releaseCode &&
                                         r.EmpUnqId == dto.ReleaseAuth &&
                                         r.Active);
                if (relAuth == null) return BadRequest("Invalid releaser.");

                if (dto.ReleaseStatusCode == ReleaseStatus.ReleaseRejected &&
                    string.IsNullOrEmpty(dto.ReleaseRemarks))
                    return BadRequest("Remarks mandatory for rejection.");

                req.ReleaseStatusCode = dto.ReleaseStatusCode;
                req.ReleaseAuth = dto.ReleaseAuth;
                req.ReleaseDate = DateTime.Now;
                req.ReleaseRemarks = dto.ReleaseRemarks;

                if (dto.ReleaseStatusCode != ReleaseStatus.ReleaseRejected)
                {
                    req.AdminReleaseStatusCode = ReleaseStatus.InRelease;
                    req.BookingStatus = true;
                }

                _context.SaveChanges();
                return Ok();
            }
        }

        public IHttpActionResult GetReqOnDate(DateTime date, int slot)
        {
            bool found = _context.VehicleReqs.Any(v => v.BookingDate == date &&
                                                       v.BookingSlot == slot &&
                                                       v.BookingStatus == true);
            if (found)
                return BadRequest("Request already exist on Date/slot.");

            return Ok();
        }

        public IHttpActionResult GetVehicleReq(DateTime fromDt, DateTime toDt)
        {
            List<VehicleReq> reqs = _context.VehicleReqs
                .Where(v => v.BookingDate >= fromDt &&
                            v.BookingDate <= toDt &&
                            v.ReleaseStatusCode == ReleaseStatus.FullyReleased)
                .ToList();

            if (reqs.Count == 0) return BadRequest("No requisitions found.");

            List<VehicleReqDto> reqDtos = Mapper.Map<List<VehicleReq>, List<VehicleReqDto>>(reqs);
            var empList = reqDtos.Select(e => e.EmpUnqId).ToList();

            var empListDto = _context.Employees
                .Where(e => empList.Contains(e.EmpUnqId))
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
                }).ToList();
            foreach (VehicleReqDto dto in reqDtos)
            {
                EmployeeDto dtoEmp = empListDto.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                if (dtoEmp == null) continue;

                dto.EmpName = dtoEmp.EmpName;
                dto.DeptName = dtoEmp.DeptName;
                dto.StatName = dtoEmp.StatName;
                dto.CatName = dtoEmp.CatName;
                dto.GradeName = dtoEmp.GradeName;
            }

            return Ok(reqDtos);
        }

        public IHttpActionResult GetByEmployee(string empUnqId, DateTime fromDt, DateTime toDt)
        {
            List<VehicleReq> reqs = _context.VehicleReqs
                .Where(v => v.EmpUnqId == empUnqId &&
                            v.BookingDate >= fromDt &&
                            v.BookingDate <= toDt )
                .ToList();

            if (reqs.Count == 0) return BadRequest("No requisitions found.");

            List<VehicleReqDto> reqDtos = Mapper.Map<List<VehicleReq>, List<VehicleReqDto>>(reqs);
            var empListDto = _context.Employees
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
                }).ToList();
            foreach (VehicleReqDto dto in reqDtos)
            {
                EmployeeDto dtoEmp = empListDto.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                if (dtoEmp == null) continue;

                dto.EmpName = dtoEmp.EmpName;
                dto.DeptName = dtoEmp.DeptName;
                dto.StatName = dtoEmp.StatName;
                dto.CatName = dtoEmp.CatName;
                dto.GradeName = dtoEmp.GradeName;
            }

            return Ok(reqDtos);
        }
    }
}