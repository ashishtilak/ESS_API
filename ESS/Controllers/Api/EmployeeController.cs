using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class EmployeeController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController()
        {
            _context = new ApplicationDbContext();
        }

        //get /api/employee
        public IHttpActionResult GetEmployees(string location)
        {
            var employeeDto = _context.Employees
                .Where(e => e.Active && (e.WrkGrp == "COMP" || e.WrkGrp == "OUTSOURCE"))
                .Select(
                    e => new EmployeeDto
                    {
                        EmpUnqId = e.EmpUnqId,
                        EmpName = e.EmpName,
                        FatherName = e.FatherName,
                        Active = e.Active,
                        Pass = e.Pass,

                        CompCode = e.CompCode,
                        WrkGrp = e.WrkGrp,
                        UnitCode = e.UnitCode,
                        DeptCode = e.DeptCode,
                        StatCode = e.StatCode,
                        //SecCode = e.SecCode,
                        CatCode = e.CatCode,
                        EmpTypeCode = e.EmpTypeCode,
                        GradeCode = e.GradeCode,
                        DesgCode = e.DesgCode,


                        CompName = e.Company.CompName,
                        WrkGrpDesc = e.WorkGroup.WrkGrpDesc,
                        UnitName = e.Units.UnitName,
                        DeptName = e.Departments.DeptName,
                        StatName = e.Stations.StatName,
                        //SecName = e.Sections.SecName,
                        CatName = e.Categories.CatName,
                        EmpTypeName = e.EmpTypes.EmpTypeName,
                        GradeName = e.Grades.GradeName,
                        DesgName = e.Designations.DesgName,

                        IsHod = e.IsHod,
                        IsHrUser = e.IsHrUser,
                        IsReleaser = e.IsReleaser,
                        Email = e.Email,

                        Location = e.Location,
                        SapId = e.SapId,
                        CompanyAcc = e.CompanyAcc
                    }
                )
                .ToList();

            //get all employee per address
            var empPerAdd = Helpers.CustomHelper.GetEmpPerAddress(location);


            foreach (var dto in employeeDto)
            {
                var empAdd = _context.EmpAddress
                    .OrderByDescending(e => e.Counter)
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (empAdd == null) continue;

                dto.Counter = empAdd.Counter;
                dto.EmpUnqId = empAdd.EmpUnqId;
                dto.HouseNumber = empAdd.HouseNumber;
                dto.SocietyName = empAdd.SocietyName;
                dto.AreaName = empAdd.AreaName;
                dto.LandMark = empAdd.LandMark;
                dto.Tehsil = empAdd.Tehsil;
                dto.PoliceStation = empAdd.PoliceStation;
                dto.PreDistrict = empAdd.PreDistrict;
                dto.PreCity = empAdd.PreCity;
                dto.PreState = empAdd.PreState;
                dto.PrePin = empAdd.PrePin;
                dto.PrePhone = empAdd.PrePhone;
                dto.PreResPhone = empAdd.PreResPhone;
                dto.PreEmail = empAdd.PreEmail;
                dto.HrVerified = empAdd.HrVerified;
                dto.HrUser = empAdd.HrUser;
                dto.HrVerificationDate = empAdd.HrVerificationDate;
                dto.HrCity = empAdd.HrCity;
                dto.HrSociety = empAdd.HrSociety;
                dto.HrRemarks = empAdd.HrRemarks;

                dto.UpdDt = empAdd.UpdDt;

                var perAdd = empPerAdd.SingleOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (perAdd == null) continue;

                dto.EmpUnqId = perAdd.EmpUnqId;
                dto.PerAdd1 = perAdd.PerAdd1;
                dto.PerAdd2 = perAdd.PerAdd2;
                dto.PerAdd3 = perAdd.PerAdd3;
                dto.PerAdd4 = perAdd.PerAdd4;
                dto.PerDistrict = perAdd.PerDistrict;
                dto.PerCity = perAdd.PerCity;
                dto.PerState = perAdd.PerState;
                dto.PerPin = perAdd.PerPin;
                dto.PerPhone = perAdd.PerPhone;
                dto.PerPoliceSt = perAdd.PerPoliceSt;
            }

            return Ok(employeeDto);
        }

        [HttpGet]
        [ActionName("GetEmployee")]
        public IHttpActionResult GetEmployee(string empUnqId)
        {
            var employee = _context.Employees
                .Where(e => e.EmpUnqId == empUnqId)
                .Select(
                    e => new EmployeeDto
                    {
                        EmpUnqId = e.EmpUnqId,
                        EmpName = e.EmpName,
                        FatherName = e.FatherName,
                        Active = e.Active,
                        Pass = e.Pass,

                        CompCode = e.CompCode,
                        WrkGrp = e.WrkGrp,
                        UnitCode = e.UnitCode,
                        DeptCode = e.DeptCode,
                        StatCode = e.StatCode,
                        //SecCode = e.SecCode,
                        CatCode = e.CatCode,
                        EmpTypeCode = e.EmpTypeCode,
                        GradeCode = e.GradeCode,
                        DesgCode = e.DesgCode,


                        CompName = e.Company.CompName,
                        WrkGrpDesc = e.WorkGroup.WrkGrpDesc,
                        UnitName = e.Units.UnitName,
                        DeptName = e.Departments.DeptName,
                        StatName = e.Stations.StatName,
                        //SecName = e.Sections.SecName,
                        CatName = e.Categories.CatName,
                        EmpTypeName = e.EmpTypes.EmpTypeName,
                        GradeName = e.Grades.GradeName,
                        DesgName = e.Designations.DesgName,

                        IsHod = e.IsHod,
                        IsHrUser = e.IsHrUser,
                        IsReleaser = e.IsReleaser,
                        Email = e.Email,

                        Location = e.Location,
                        SapId = e.SapId,
                        CompanyAcc = e.CompanyAcc,

                        BirthDate = e.BirthDate,
                        Pan = e.Pan,
                        JoinDate = e.JoinDate
                    }
                )
                .ToList();

            if (employee.Count == 0)
                return NotFound();

            foreach (EmployeeDto employeeDto in employee)
            {
                employeeDto.NoDuesFlag = _context.NoDuesMaster.Any(e => e.EmpUnqId == employeeDto.EmpUnqId);

                var empAdd = _context.EmpAddress
                    .OrderByDescending(e => e.Counter)
                    .FirstOrDefault(e => e.EmpUnqId == empUnqId);

                if (empAdd != null)
                {
                    employeeDto.PrePhone = empAdd.PrePhone;
                    employeeDto.PreResPhone = empAdd.PreResPhone;
                    employeeDto.PreEmail = empAdd.PreEmail;
                }
            }


            return Ok(employee);
        }

        [HttpGet]
        [ActionName("GetEmpDetails")]
        public IHttpActionResult GetEmpDetails(string empUnqId, int mode)
        {
            if (mode == 1) //Employee details
            {
                var result = Helpers.CustomHelper.GetEmpDetails(empUnqId);
                return Ok(result);
            }
            else if (mode == 2) //Employee education details
            {
                var result = Helpers.CustomHelper.GetEmpEduDetails(empUnqId);
                return Ok(result);
            }
            else if (mode == 3) //Family details
            {
                var result = Helpers.CustomHelper.GetEmpFamilyDetails(empUnqId);
                return Ok(result);
            }

            return NotFound();
        }

        private class EmpRelease
        {
            public string EmpUnqId { get; set; }
            public string EmpName { get; set; }
            public string WrkGrp { get; set; }
            public string UnitCode { get; set; }
            public string DeptCode { get; set; }
            public string DeptName { get; set; }
            public string StatCode { get; set; }

            public string StatName { get; set; }

            //public string SecCode { get; set; }
            //public string SecName { get; set; }
            public string Email { get; set; }
            public string ReleaseGroup { get; set; }
            public string ReleaseStrategy { get; set; }

            public string ReleaseCode1 { get; set; }
            public string ReleaseEmpUnqId1 { get; set; }
            public string ReleaseName1 { get; set; }

            public string ReleaseCode2 { get; set; }
            public string ReleaseEmpUnqId2 { get; set; }
            public string ReleaseName2 { get; set; }

            public string ReleaseCode3 { get; set; }
            public string ReleaseEmpUnqId3 { get; set; }
            public string ReleaseName3 { get; set; }
        }

        public IHttpActionResult GetEmpRelease(bool flag)
        {
            var emps = _context.Employees
                .Include(d => d.Departments)
                .Include(s => s.Stations)
                //.Include(e => e.Sections)
                .Where(e => e.Active && (e.WrkGrp == "COMP" || e.WrkGrp == "OUTSOURCE"))
                .ToList();

            List<EmpRelease> result = new List<EmpRelease>();

            foreach (var emp in emps)
            {
                EmpRelease e = new EmpRelease
                {
                    EmpUnqId = emp.EmpUnqId,
                    EmpName = emp.EmpName,
                    WrkGrp = emp.WrkGrp,
                    UnitCode = emp.UnitCode
                };

                if (emp.DeptCode != null)
                {
                    e.DeptCode = emp.DeptCode;
                    e.DeptName = emp.Departments.DeptName;
                }

                if (emp.StatCode != null)
                {
                    e.StatCode = emp.StatCode;
                    e.StatName = emp.Stations.StatName;
                }

                //if (emp.SecCode != null)
                //{
                //    e.SecCode = emp.SecCode;
                //    e.SecName = emp.Sections.SecName;
                //}

                e.Email = emp.Email;

                var relStr = _context.ReleaseStrategy
                    .SingleOrDefault(r =>
                        r.ReleaseStrategy == e.EmpUnqId &&
                        r.ReleaseGroupCode == ReleaseGroups.LeaveApplication
                    );

                if (relStr == null)
                {
                    result.Add(e);
                    continue;
                }


                var relStrLvl = _context.ReleaseStrategyLevels
                    .Where(r =>
                        r.ReleaseGroupCode == relStr.ReleaseGroupCode &&
                        r.ReleaseStrategy == relStr.ReleaseStrategy
                    ).ToList();
                foreach (var level in relStrLvl)
                {
                    var relAuth = _context.ReleaseAuth
                        .Include(re => re.Employee)
                        .Where(r => r.ReleaseCode == level.ReleaseCode)
                        .ToList();

                    string relEmpUnqId = "";
                    string relName = "";
                    foreach (var auth in relAuth)
                    {
                        if (relEmpUnqId == "")
                            relEmpUnqId = auth.EmpUnqId;
                        else
                            relEmpUnqId += "/" + auth.EmpUnqId;

                        if (relName == "")
                            relName = auth.Employee.EmpName;
                        else
                            relName += "/" + auth.Employee.EmpName;
                    }

                    e.ReleaseGroup = relStr.ReleaseGroupCode;
                    e.ReleaseStrategy = relStr.ReleaseStrategy;

                    switch (level.ReleaseStrategyLevel)
                    {
                        case 1:
                            e.ReleaseCode1 = level.ReleaseCode;
                            e.ReleaseEmpUnqId1 = relEmpUnqId;
                            e.ReleaseName1 = relName;
                            break;
                        case 2:
                            e.ReleaseCode2 = level.ReleaseCode;
                            e.ReleaseEmpUnqId2 = relEmpUnqId;
                            e.ReleaseName2 = relName;
                            break;
                        case 3:
                            e.ReleaseCode3 = level.ReleaseCode;
                            e.ReleaseEmpUnqId3 = relEmpUnqId;
                            e.ReleaseName3 = relName;
                            break;
                    }
                }

                result.Add(e);
            }


            return Ok(result);
        }

        [HttpGet]
        [ActionName("perfattd")]
        public IHttpActionResult PerfAttd(string empUnqId, string flag, DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            if (flag == "PERF")
            {
                var result = Helpers.CustomHelper.GetPerfAttd(empUnqId, fromDate, toDate);
                return Ok(result);
            }
            else if (flag == "PUNCH")
            {
                var result = Helpers.CustomHelper.GetPerfPunch(empUnqId);
                return Ok(result);
            }
            else
            {
                return BadRequest("Invalid parameter...");
            }
        }

        [HttpGet]
        [ActionName("gettripodpunch")]
        public IHttpActionResult GetTripodPunch(string empUnqId, DateTime tDate, string ioFlag)
        {
            var result = Helpers.CustomHelper.GetTripodPunch(empUnqId, tDate, ioFlag);
            return Ok(result);
        }


        private class UserPassword
        {
            public string EmpUnqId { get; set; }
            public string Pass { get; set; }
        }

        [HttpPost]
        [ActionName("changepassword")]
        public IHttpActionResult ChangePassword([FromBody] object requestData)
        {
            var userPassword = JsonConvert.DeserializeObject<UserPassword>(requestData.ToString());

            var employee = _context.Employees
                .Single(e => e.EmpUnqId == userPassword.EmpUnqId);

            if (employee == null)
                return BadRequest("Invalid employee unique Id.");

            employee.Pass = userPassword.Pass;
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [ActionName("updateemail")]
        public IHttpActionResult UpdateEmail(string empUnqId, string email)
        {
            var emp = _context.Employees.Single(e => e.EmpUnqId == empUnqId);

            if (emp == null)
                return BadRequest("Invalid employee code");

            emp.Email = email;
            _context.SaveChanges();

            return Ok();
        }


        [HttpGet]
        [ActionName("getsapid")]
        public IHttpActionResult GetSapId()
        {
            List<SapUserIdDto> result = _context.SapUserIds.Select(Mapper.Map<SapUserIds, SapUserIdDto>).ToList();
            foreach (SapUserIdDto dto in result)
            {
                Employees emp = _context.Employees
                    .Include(c => c.Company)
                    .Include(w => w.WorkGroup)
                    .Include(u => u.Units)
                    .Include(d => d.Departments)
                    .Include(s => s.Stations)
                    .Include(g => g.Grades)
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp == null) continue;
                
                dto.EmpName = emp.EmpName;
                dto.Active = emp.Active;
                dto.CompName = emp.Company?.CompName;
                dto.WrkGrpDesc = emp.WorkGroup?.WrkGrpDesc;
                dto.UnitName = emp.Units?.UnitName;
                dto.Dept = emp.Departments?.DeptName;
                dto.StatName = emp.Stations?.StatName;
                dto.GradeName = emp.Grades?.GradeName;
                dto.JoinDate = emp.JoinDate;

                NoDuesMaster nodues = _context.NoDuesMaster.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (nodues == null) continue;
                dto.NoDuesFlag = nodues.ClosedFlag;
                dto.ResignDate = nodues.ResignDate;
                dto.RelieveDate = nodues.RelieveDate;
            }

            return Ok(result);
        }
    }
}