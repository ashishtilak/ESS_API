using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using AutoMapper;
using ESS.Dto;
using ESS.Migrations;
using ESS.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using NoDuesDeptDetails = ESS.Models.NoDuesDeptDetails;

namespace ESS.Controllers.Api
{
    public class NoDuesController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public NoDuesController()
        {
            _context = new ApplicationDbContext();
        }

        // get creator/releaser
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetReleaser(string empUnqId, bool releaseFlag)
        {
            if (releaseFlag)
            {
                var result = _context.NoDuesReleaser
                    .Where(e => e.EmpUnqId == empUnqId).ToList();

                if (result.Count == 0)
                    return BadRequest("Not authorized.");

                var dept = result.Select(d => d.Dept).ToArray();

                return Ok(dept);
            }
            else
            {
                var result = _context.NoDuesCreator
                    .Where(e => e.EmpUnqId == empUnqId).ToList();

                if (result.Count == 0)
                    return BadRequest("Not authorized.");

                var dept = result.Select(d => d.Dept).ToArray();

                return Ok(dept);
            }
        }

        // get closed no dues details date range wise
        public IHttpActionResult GetNoDues(DateTime fromDate, DateTime toDate)
        {
            // Get all master records based on date range
            var noDues = _context.NoDuesMaster
                .Where(n => n.ClosedFlag == true &&
                            n.RelieveDate >= fromDate && n.RelieveDate <= toDate).AsEnumerable()
                .Select(Mapper.Map<NoDuesMaster, NoDuesMasterDto>)
                .ToList();

            if (noDues.Count == 0)
                return BadRequest("No records found.");

            var empArray = noDues.Select(e => e.EmpUnqId).ToArray();

            // Get all emp records based on masters
            var empListDto = _context.Employees
                .Where(e => empArray.Contains(e.EmpUnqId))
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


            // Get all dept records based on masters
            var noDuesDept = _context.NoDuesDept
                .Where(e => empArray.Contains(e.EmpUnqId)).AsEnumerable()
                .Select(Mapper.Map<NoDuesDept, NoDuesDeptDto>)
                .ToList();

            // Get all dept details records based on masters
            var noDuesDeptDtl = _context.NoDuesDeptDetails
                .Where(e => empArray.Contains(e.EmpUnqId)).AsEnumerable()
                .Select(Mapper.Map<NoDuesDeptDetails, NoDuesDeptDetailsDto>)
                .ToList();


            // Loop through master 
            foreach (NoDuesMasterDto noDue in noDues)
            {
                // get corresponding dept
                var deptDto = noDuesDept.Where(e => e.EmpUnqId == noDue.EmpUnqId).ToList();
                foreach (NoDuesDeptDto dto in deptDto)
                {
                    // get corresponding dept details
                    dto.NoDuesDeptDetails = new List<NoDuesDeptDetailsDto>();
                    var dtl = noDuesDeptDtl.Where(d => d.DeptId == dto.DeptId && d.EmpUnqId == dto.EmpUnqId).ToList();
                    dto.NoDuesDeptDetails.AddRange(dtl);

                    // get approver name
                    string appName = _context.Employees.FirstOrDefault(e => e.EmpUnqId == dto.ApprovedBy)?.EmpName;
                    dto.ApprovedByName = appName;

                    dto.No = _context.NoDuesDeptList.FirstOrDefault(d => d.DeptId == dto.DeptId)?.Index;
                    dto.DeptName = _context.NoDuesDeptList.FirstOrDefault(d => d.DeptId == dto.DeptId)?.DeptName;
                }

                // Get IT no dues from ServiceDesk portal
                var noDuesIt = Helpers.CustomHelper.GetNoDuesIt(noDue.EmpUnqId);

                noDue.NoDuesIt = new List<NoDuesIt>();
                noDue.NoDuesIt.AddRange(noDuesIt);

                // get employee
                EmployeeDto emp = empListDto.FirstOrDefault(e => e.EmpUnqId == noDue.EmpUnqId);
                if (emp == null) continue;

                noDue.EmpName = emp.EmpName;
                noDue.UnitName = emp.UnitName;
                noDue.DeptName = emp.DeptName;
                noDue.StatName = emp.StatName;
                noDue.CatName = emp.CatName;
                noDue.GradeName = emp.GradeName;
                noDue.DesgName = emp.DesgName;

                // add dept to master

                noDue.NoDuesDepts = new List<NoDuesDeptDto>();
                noDue.NoDuesDepts.AddRange(deptDto);

                var relStrLvl = _context.NoDuesReleaseStatus
                    .Where(r => r.ReleaseGroupCode == ReleaseGroups.NoDues &&
                                r.EmpUnqId == noDue.EmpUnqId).AsEnumerable()
                    .Select(Mapper.Map<NoDuesReleaseStatus, NoDuesReleaseStatusDto>)
                    .ToList();

                foreach (NoDuesReleaseStatusDto relLvl in relStrLvl)
                {
                    string relName = _context.Employees.FirstOrDefault(e => e.EmpUnqId == relLvl.ReleaseAuth)?.EmpName;
                    relLvl.ReleaserName = relName;
                }

                // get names of HR user and approver
                string hrUser = _context.Employees.FirstOrDefault(e => e.EmpUnqId == noDue.HrAddUser)?.EmpName;
                string hrApprover = _context.Employees.FirstOrDefault(e => e.EmpUnqId == noDue.HrApprovedBy)?.EmpName;

                noDue.HrAddUserName = hrUser;
                noDue.HrApprovedByName = hrApprover;

                noDue.NoDuesReleaseStatus = new List<NoDuesReleaseStatusDto>();
                noDue.NoDuesReleaseStatus.AddRange(relStrLvl);
            }

            return Ok(noDues);
        }

        // get no dues status employee wise:
        public IHttpActionResult GetNoDuesStatus(string empUnqId = "")
        {
            List<NoDuesStatus> status;
            if (empUnqId == "")
            {
                string[] empList = _context.NoDuesMaster
                    .Where(e => e.ClosedFlag == false)
                    .Select(e => e.EmpUnqId).ToArray();
                status = _context.NoDuesStatus.Where(e => empList.Contains(e.EmpUnqId)).ToList();
            }
            else
            {
                NoDuesMaster noDues = _context.NoDuesMaster
                    .FirstOrDefault(e => e.EmpUnqId == empUnqId);

                if (noDues == null)
                    return BadRequest("No records found for employee");

                NoDuesMasterDto noDue = Mapper.Map<NoDuesMaster, NoDuesMasterDto>(noDues);

                var deptDto = _context.NoDuesDept
                    .Where(e => e.EmpUnqId == empUnqId).AsEnumerable()
                    .Select(Mapper.Map<NoDuesDept, NoDuesDeptDto>)
                    .ToList();

                var noDuesDeptDtl = _context.NoDuesDeptDetails
                    .Where(e => e.EmpUnqId == empUnqId).AsEnumerable()
                    .Select(Mapper.Map<NoDuesDeptDetails, NoDuesDeptDetailsDto>)
                    .ToList();

                foreach (NoDuesDeptDto dto in deptDto)
                {
                    dto.NoDuesDeptDetails = new List<NoDuesDeptDetailsDto>();
                    var dtl = noDuesDeptDtl.Where(d => d.DeptId == dto.DeptId).ToList();
                    dto.NoDuesDeptDetails.AddRange(dtl);

                    // get approver name
                    string appName = _context.Employees.FirstOrDefault(e => e.EmpUnqId == dto.ApprovedBy)?.EmpName;
                    dto.ApprovedByName = appName;

                    dto.No = _context.NoDuesDeptList.FirstOrDefault(d => d.DeptId == dto.DeptId)?.Index;
                    dto.DeptName = _context.NoDuesDeptList.FirstOrDefault(d => d.DeptId == dto.DeptId)?.DeptName;
                }

                // Get IT no dues from ServiceDesk portal
                var noDuesIt = Helpers.CustomHelper.GetNoDuesIt(noDue.EmpUnqId);

                noDue.NoDuesIt = new List<NoDuesIt>();
                noDue.NoDuesIt.AddRange(noDuesIt);

                // get employee
                var emp = _context.Employees
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
                    .FirstOrDefault(e => e.EmpUnqId == noDue.EmpUnqId);


                noDue.EmpName = emp.EmpName;
                noDue.UnitName = emp.UnitName;
                noDue.DeptName = emp.DeptName;
                noDue.StatName = emp.StatName;
                noDue.CatName = emp.CatName;
                noDue.GradeName = emp.GradeName;
                noDue.DesgName = emp.DesgName;

                noDue.NoDuesDepts = new List<NoDuesDeptDto>();
                noDue.NoDuesDepts.AddRange(deptDto);


                var relStrLvl = _context.NoDuesReleaseStatus
                    .Where(r => r.ReleaseGroupCode == ReleaseGroups.NoDues &&
                                r.EmpUnqId == noDue.EmpUnqId).AsEnumerable()
                    .Select(Mapper.Map<NoDuesReleaseStatus, NoDuesReleaseStatusDto>)
                    .ToList();

                foreach (NoDuesReleaseStatusDto relLvl in relStrLvl)
                {
                    string relName = _context.Employees.FirstOrDefault(e => e.EmpUnqId == relLvl.ReleaseAuth)?.EmpName;
                    relLvl.ReleaserName = relName;
                }

                // get names of HR user and approver
                string hrUser = _context.Employees.FirstOrDefault(e => e.EmpUnqId == noDue.HrAddUser)?.EmpName;
                string hrApprover = _context.Employees.FirstOrDefault(e => e.EmpUnqId == noDue.HrApprovedBy)?.EmpName;

                noDue.HrAddUserName = hrUser;
                noDue.HrApprovedByName = hrApprover;

                noDue.NoDuesReleaseStatus = new List<NoDuesReleaseStatusDto>();
                noDue.NoDuesReleaseStatus.AddRange(relStrLvl);

                var noDueStatus = _context.NoDuesStatus.FirstOrDefault(e => e.EmpUnqId == empUnqId);
                if (noDueStatus != null)
                {
                    noDue.NoDuesStatus = new NoDuesStatusDto();
                    noDue.NoDuesStatus = Mapper.Map<NoDuesStatus, NoDuesStatusDto>(noDueStatus);
                }

                return Ok(noDue);
            }

            if (status.Count == 0)
                return BadRequest("No records found!");

            List<NoDuesStatusDto> statusDto = Mapper.Map<List<NoDuesStatus>, List<NoDuesStatusDto>>(status);

            string[] emps = status.Select(e => e.EmpUnqId).Distinct().ToArray();
            List<Employees> empName = _context.Employees.Where(e => emps.Contains(e.EmpUnqId)).ToList();

            foreach (NoDuesStatusDto stat in statusDto)
            {
                stat.EmpName = empName.First(e => e.EmpUnqId == stat.EmpUnqId).EmpName;
            }

            return Ok(statusDto);
        }

        // get details based on dept/creator-releaser
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetNoDues(string empUnqId, bool releaseFlag, string dept)
        {
            List<NoDuesMasterDto> noDuesMasters;

            if (dept.ToUpper() == "HOD")
            {
                if (releaseFlag == false) // creator - hod
                {
                    var rel = _context.ReleaseAuth.Where(r => r.EmpUnqId == empUnqId)
                        .Select(s => s.ReleaseCode).ToArray();

                    var relStrLvl = _context.ReleaseStrategyLevels
                        .Where(r => r.ReleaseGroupCode == ReleaseGroups.NoDues && rel.Contains(r.ReleaseCode))
                        .Select(r => r.ReleaseStrategy).ToArray();

                    // No dues start date must be less than or equal to today

                    noDuesMasters = _context.NoDuesMaster.Where(
                            n => relStrLvl.Contains(n.EmpUnqId) &&
                                 n.NoDuesStartDate <= DateTime.Now &&
                                 n.DeptApprovalFlag == false).AsEnumerable()
                        .Select(Mapper.Map<NoDuesMaster, NoDuesMasterDto>)
                        .ToList();
                }
                else // releaser - hod
                {
                    var rel = _context.ReleaseAuth.Where(r => r.EmpUnqId == empUnqId)
                        .Select(s => s.ReleaseCode).ToArray();

                    var relStrLvl = _context.NoDuesReleaseStatus
                        .Where(r =>
                            r.ReleaseGroupCode == ReleaseGroups.NoDues &&
                            rel.Contains(r.ReleaseCode) &&
                            r.ReleaseStatusCode == ReleaseStatus.InRelease).AsEnumerable()
                        .Select(Mapper.Map<NoDuesReleaseStatus, NoDuesReleaseStatusDto>)
                        .ToList();

                    var emp = relStrLvl.Select(e => e.EmpUnqId).ToArray();

                    noDuesMasters = _context.NoDuesMaster
                        .Where(n => emp.Contains(n.EmpUnqId) &&
                                    n.DeptNoDuesFlag == true).AsEnumerable()
                        .Select(Mapper.Map<NoDuesMaster, NoDuesMasterDto>)
                        .ToList();

                    foreach (NoDuesMasterDto noDue in noDuesMasters)
                    {
                        noDue.NoDuesReleaseStatus = new List<NoDuesReleaseStatusDto>();
                        var relLvl = relStrLvl.Where(e => e.EmpUnqId == noDue.EmpUnqId).ToList();
                        noDue.NoDuesReleaseStatus.AddRange(relLvl);
                    }
                }
            }
            else if (dept.ToUpper() == "HR")
            {
                if (releaseFlag == false) // creator
                {
                    var usr = _context.NoDuesCreator
                        .Where(e => e.EmpUnqId == empUnqId && e.Dept == dept)
                        .ToList();
                    if (usr.Count == 0)
                        return BadRequest("User is not authorized for creation");

                    noDuesMasters = _context.NoDuesMaster
                        .Where(e =>
                            e.ClosedFlag == false &&
                            e.DeptNoDuesFlag == true &&
                            e.DeptApprovalFlag == true &&
                            e.HrAddUser == null &&
                            e.HrApprovalFlag == false
                        ).AsEnumerable()
                        .Select(Mapper.Map<NoDuesMaster, NoDuesMasterDto>)
                        .ToList();
                }
                else
                {
                    var usr = _context.NoDuesReleaser
                        .Where(e => e.EmpUnqId == empUnqId && e.Dept == dept)
                        .ToList();
                    if (usr.Count == 0)
                        return BadRequest("User is not authorized for release");

                    noDuesMasters = _context.NoDuesMaster
                        .Where(e =>
                            e.ClosedFlag == false &&
                            e.DeptNoDuesFlag == true &&
                            e.DeptApprovalFlag == true &&
                            e.HrAddUser != null &&
                            e.HrApprovalFlag == false
                        ).AsEnumerable()
                        .Select(Mapper.Map<NoDuesMaster, NoDuesMasterDto>)
                        .ToList();
                }
            }
            else if (dept.ToUpper() == "UH")
            {
                // FOR UNIT HEADS
                noDuesMasters = _context.NoDuesMaster
                    .Where(e =>
                        e.UhApprovalFlag == false &&
                        e.UnitHead == empUnqId &&
                        (e.RelieveDate <= DateTime.Today || e.ClosedFlag == false) &&
                        e.DeptApprovalFlag == true).AsEnumerable()
                    .Select(Mapper.Map<NoDuesMaster, NoDuesMasterDto>)
                    .ToList();

                var emp = noDuesMasters.Select(e => e.EmpUnqId).ToArray();

                var noDuesDept = _context.NoDuesDept
                    .Where(e => emp.Contains(e.EmpUnqId) && e.ApprovalFlag == true)
                    .AsEnumerable()
                    .Select(Mapper.Map<NoDuesDept, NoDuesDeptDto>)
                    .ToList();

                emp = noDuesDept.Select(e => e.EmpUnqId).ToArray();

                var noDuesDeptDetails = _context.NoDuesDeptDetails
                    .Where(e => emp.Contains(e.EmpUnqId)).AsEnumerable()
                    .Select(Mapper.Map<NoDuesDeptDetails, NoDuesDeptDetailsDto>)
                    .ToList();
                foreach (NoDuesMasterDto master in noDuesMasters.ToList())
                {
                    // Check if this emp requires release from unit head or not
                    Employees empl = _context.Employees.FirstOrDefault(e => e.EmpUnqId == master.EmpUnqId);
                    if (empl == null) continue;

                    NoDuesUnitHead empDept = _context.NoDuesUnitHead.FirstOrDefault(
                        e => e.CompCode == empl.CompCode &&
                             e.WrkGrp == empl.WrkGrp &&
                             e.UnitCode == empl.UnitCode &&
                             e.DeptCode == empl.DeptCode);

                    if (empDept == null) continue;

                    // check if emp belongs to DI LOB
                    if (empDept.DeptLoB == "DI")
                    {
                        // If he's below manager, remove him
                        if (int.Parse(empl.DesgCode) > int.Parse("006")) //code for manager
                        {
                            noDuesMasters.Remove(master);
                        }
                    }
                    // check for emp over.

                    var deptDtos = noDuesDept.Where(e => e.EmpUnqId == master.EmpUnqId).ToList();
                    if (deptDtos.Count == 0) continue;

                    foreach (NoDuesDeptDto deptDto in deptDtos)
                    {
                        var detailsDto = noDuesDeptDetails.Where(e => e.EmpUnqId == master.EmpUnqId &&
                                                                      e.DeptId == deptDto.DeptId).ToList();
                        if (detailsDto.Count == 0) continue;
                        deptDto.NoDuesDeptDetails = new List<NoDuesDeptDetailsDto>();
                        deptDto.NoDuesDeptDetails.AddRange(detailsDto);
                    }

                    master.NoDuesDepts = new List<NoDuesDeptDto>();
                    master.NoDuesDepts.AddRange(deptDtos);
                }
            }
            else
            {
                // FOR ALL OTHER DEPTS
                if (releaseFlag == false) //creator
                {
                    // check if user is creator
                    var usr = _context.NoDuesCreator
                        .Where(e => e.EmpUnqId == empUnqId && e.Dept == dept)
                        .ToList();
                    if (usr.Count == 0)
                        return BadRequest("User is not authorized for creation");

                    // check how many Masters are pending
                    noDuesMasters = _context.NoDuesMaster
                        .Where(e =>
                            e.ClosedFlag == false &&
                            e.DeptNoDuesFlag == true &&
                            e.DeptApprovalFlag == true).AsEnumerable()
                        .Select(Mapper.Map<NoDuesMaster, NoDuesMasterDto>)
                        .ToList();

                    // array of all emp
                    var emp = noDuesMasters.Select(e => e.EmpUnqId).ToArray();


                    // get all nodues dept for specified dept
                    var noDuesDept = _context.NoDuesDept
                        .Where(e => emp.Contains(e.EmpUnqId) && e.DeptId == dept)
                        .AsEnumerable()
                        .Select(Mapper.Map<NoDuesDept, NoDuesDeptDto>)
                        .ToList();

                    // list of all such emp found in nodues dept
                    emp = noDuesDept.Select(e => e.EmpUnqId).ToArray();

                    // get dept details
                    var noDuesDeptDetails = _context.NoDuesDeptDetails
                        .Where(e => emp.Contains(e.EmpUnqId) &&
                                    e.DeptId == dept).AsEnumerable()
                        .Select(Mapper.Map<NoDuesDeptDetails, NoDuesDeptDetailsDto>)
                        .ToList();

                    foreach (NoDuesMasterDto master in noDuesMasters.ToList())
                    {
                        // check for PRG
                        if (dept == NoDuesDeptList.PrgHr)
                        {
                            var prg = _context.NoDuesStatus.FirstOrDefault(e => e.EmpUnqId == master.EmpUnqId)?.PrgHr;
                            if (prg != null && prg == true)
                                noDuesMasters.Remove(master);
                        }

                        // search if dept exist for emp
                        var deptDtos = noDuesDept.Where(e => e.EmpUnqId == master.EmpUnqId).ToList();

                        if (noDuesDept.Any(e => e.EmpUnqId == master.EmpUnqId && e.ApprovalFlag == true))
                        {
                            noDuesMasters.Remove(master);
                            continue;
                        }

                        foreach (NoDuesDeptDto deptDto in deptDtos)
                        {
                            var detailsDto = noDuesDeptDetails.Where(e => e.EmpUnqId == master.EmpUnqId &&
                                                                          e.DeptId == deptDto.DeptId).ToList();
                            if (detailsDto.Count == 0) continue;
                            deptDto.NoDuesDeptDetails = new List<NoDuesDeptDetailsDto>();
                            deptDto.NoDuesDeptDetails.AddRange(detailsDto);
                        }

                        master.NoDuesDepts = new List<NoDuesDeptDto>();
                        master.NoDuesDepts.AddRange(deptDtos);
                    }
                }
                else //releaser
                {
                    var usr = _context.NoDuesReleaser
                        .Where(e => e.EmpUnqId == empUnqId && e.Dept == dept)
                        .ToList();
                    if (usr.Count == 0)
                        return BadRequest("User is not authorized for release.");

                    var noDuesDept = _context.NoDuesDept
                        .Where(n =>
                            n.DeptId == dept &&
                            n.ApprovalFlag == false).AsEnumerable()
                        .Select(Mapper.Map<NoDuesDept, NoDuesDeptDto>)
                        .ToList();

                    foreach (NoDuesDeptDto dto in noDuesDept)
                    {
                        var ndDetail = _context.NoDuesDeptDetails
                            .Where(e => e.EmpUnqId == dto.EmpUnqId &&
                                        e.DeptId == dto.DeptId).AsEnumerable()
                            .Select(Mapper.Map<NoDuesDeptDetails, NoDuesDeptDetailsDto>)
                            .ToList();

                        if (ndDetail.Count == 0) continue;

                        dto.NoDuesDeptDetails = new List<NoDuesDeptDetailsDto>();
                        dto.NoDuesDeptDetails.AddRange(ndDetail);
                    }

                    var emp = noDuesDept.Select(e => e.EmpUnqId).ToArray();

                    noDuesMasters = _context.NoDuesMaster
                        .Where(e => emp.Contains(e.EmpUnqId) && e.ClosedFlag == false).AsEnumerable()
                        .Select(Mapper.Map<NoDuesMaster, NoDuesMasterDto>)
                        .ToList();

                    foreach (NoDuesMasterDto dto in noDuesMasters)
                    {
                        var nd = noDuesDept.Where(d => d.EmpUnqId == dto.EmpUnqId).ToList();
                        if (nd.Count == 0) continue;

                        dto.NoDuesDepts = new List<NoDuesDeptDto>();
                        dto.NoDuesDepts.AddRange(nd);
                    }
                }
            }

            if (noDuesMasters.Count == 0)
                return BadRequest("No records found!");

            var empList = noDuesMasters.Select(e => e.EmpUnqId).ToArray();

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

            foreach (NoDuesMasterDto dto in noDuesMasters)
            {
                EmployeeDto dtoEmp = empListDto.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                if (dtoEmp == null) continue;

                dto.EmpName = dtoEmp.EmpName;
                dto.DeptName = dtoEmp.DeptName;
                dto.StatName = dtoEmp.StatName;
                dto.CatName = dtoEmp.CatName;
                dto.GradeName = dtoEmp.GradeName;

                NoDuesStatus noDuesStatusDto = _context.NoDuesStatus.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                if (noDuesStatusDto == null) continue;
                dto.NoDuesStatus = new NoDuesStatusDto();
                dto.NoDuesStatus = Mapper.Map<NoDuesStatus, NoDuesStatusDto>(noDuesStatusDto);
            }

            return Ok(noDuesMasters);
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult CreateNoDues([FromBody] object requestData)
        {
            var dto = JsonConvert.DeserializeObject<NoDuesMasterDto>(requestData.ToString());

            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    // TODO: Date checks:

                    // Get unit head from master

                    dto.EmpUnqId = dto.EmpUnqId.Trim();

                    Employees emp = _context.Employees.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                    if (emp == null)
                        return BadRequest("Employee master not found!");

                    string unitHead = _context.NoDuesUnitHead.FirstOrDefault(
                        d =>
                            d.CompCode == emp.CompCode &&
                            d.WrkGrp == emp.WrkGrp &&
                            d.UnitCode == emp.UnitCode &&
                            d.DeptCode == emp.DeptCode
                    )?.UnitHead;

                    // create no dues master record from supplied info:


                    // If mode of leaving is Absconding, start No dues process immediately
                    // other wise no dues process should start 4 days before relieving date
                    DateTime noDuesStartDate = dto.ModeOfLeaving.ToUpper() == "ABSCONDING"
                        ? dto.RelieveDate
                        : dto.RelieveDate.AddDays(-4);


                    var newNoDues = new NoDuesMaster
                    {
                        EmpUnqId = emp.EmpUnqId,
                        JoinDate = dto.JoinDate,
                        ResignDate = dto.ResignDate,
                        RelieveDate = dto.RelieveDate,
                        ModeOfLeaving = dto.ModeOfLeaving,
                        // Nodues process start date should be 4 days prior to relieve date.
                        NoDuesStartDate = noDuesStartDate,
                        AddUser = dto.AddUser,
                        AddDate = DateTime.Now,
                        ClosedFlag = false,
                        DeptNoDuesFlag = false,
                        DeptApprovalFlag = false,
                        HrApprovalFlag = false,
                        UnitHead = unitHead,
                        UhApprovalFlag = false
                    };

                    _context.NoDuesMaster.Add(newNoDues);

                    // get the release strategy for HOD release

                    ReleaseStrategies relStr = _context.ReleaseStrategy
                        .FirstOrDefault(
                            r =>
                                r.ReleaseGroupCode == ReleaseGroups.NoDues &&
                                r.ReleaseStrategy == dto.EmpUnqId &&
                                r.Active == true
                        );

                    if (relStr == null)
                        return BadRequest("Release strategy not configured.");

                    var relStratLevels = _context.ReleaseStrategyLevels
                        .Where(
                            rl =>
                                rl.ReleaseGroupCode == ReleaseGroups.NoDues &&
                                rl.ReleaseStrategy == relStr.ReleaseStrategy
                        ).ToList();

                    int counter = 0;

                    foreach (ReleaseStrategyLevels level in relStratLevels)
                    {
                        // If 1st releaser is not final releaser, don't add it to table
                        if (level.ReleaseStrategyLevel == 1 && level.IsFinalRelease == false) continue;

                        var noDuesRel = new NoDuesReleaseStatus
                        {
                            EmpUnqId = emp.EmpUnqId,
                            ReleaseGroupCode = ReleaseGroups.NoDues,
                            ReleaseStrategy = level.ReleaseStrategy,
                            ReleaseStrategyLevel = level.ReleaseStrategyLevel,
                            ReleaseCode = level.ReleaseCode,
                            ReleaseStatusCode = ReleaseStatus.InRelease,
                            IsFinalRelease = level.IsFinalRelease
                        };

                        if (counter > 0)
                            noDuesRel.ReleaseStatusCode = ReleaseStatus.NotReleased;

                        counter++;

                        _context.NoDuesReleaseStatus.Add(noDuesRel);
                    }

                    string unitCode = _context.Employees.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId)?.UnitCode;

                    _context.NoDuesStatus.Add(new NoDuesStatus
                    {
                        EmpUnqId = emp.EmpUnqId,
                        Hod = false,
                        Finance = false,
                        Stores = false,
                        Admin = false,
                        Cafeteria = false,
                        Hr = false,
                        PrgHr = unitCode != null && unitCode != "003",
                        Township = false,
                        EandI = false,
                        It = false,
                        Security = false,
                        Safety = false,
                        Ohc = false,
                        School = false,
                        Er = false,
                        UnitHead = false
                    });

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

        [System.Web.Http.HttpPut]
        public IHttpActionResult ChangeNoDues([FromBody] object requestData, bool releaseFlag, string dept)
        {
            var dto = JsonConvert.DeserializeObject<NoDuesMasterDto>(requestData.ToString());
            if (!ModelState.IsValid)
                return BadRequest();

            switch (dept.ToUpper())
            {
                case "HOD":
                    try
                    {
                        if (CreateNoDuesHod(dto, releaseFlag))
                            return Ok();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Error:" + ex);
                    }

                    break;
                case "HR":
                    try
                    {
                        if (CreateNoDuesHr(dto, releaseFlag))
                            return Ok();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Error:" + ex);
                    }

                    break;
                case "UH":
                    try
                    {
                        if (ReleaseNoDuesUh(dto, releaseFlag))
                            return Ok();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Error:" + ex);
                    }

                    break;
                default:
                {
                    if (releaseFlag == false)
                        try
                        {
                            if (CreateNoDuesDept(dto, dept))
                                return Ok();
                        }
                        catch (Exception ex)
                        {
                            return BadRequest("Error: " + ex);
                        }
                    else
                        try
                        {
                            if (ReleaseNoDuesDept(dto, dept))
                                return Ok();
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.ToString());
                        }

                    break;
                }
            }

            return Ok();
        }

        private bool CreateNoDuesHod(NoDuesMasterDto dto, bool releaseFlag)
        {
            if (releaseFlag == false) //record creation
                try
                {
                    NoDuesMaster noDues = _context.NoDuesMaster
                        .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                    if (noDues == null)
                        throw new Exception("Employee not found.");

                    noDues.DeptParticulars = dto.DeptParticulars;
                    noDues.DeptRemarks = dto.DeptRemarks;
                    noDues.DeptAmount = dto.DeptAmount;
                    noDues.DeptNoDuesFlag = true;
                    noDues.DeptApprovalFlag = false;
                    noDues.DeptAddDate = DateTime.Now;
                    noDues.DeptAddUser = dto.DeptAddUser;

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error:" + ex);
                }
            else //releasing
                try
                {
                    using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                    {
                        NoDuesReleaseStatusDto relDto = dto.NoDuesReleaseStatus.First();

                        if (relDto == null)
                            throw new Exception("Error in release status dto.");

                        NoDuesReleaseStatus noDuesRelease = _context.NoDuesReleaseStatus
                            .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId &&
                                                 e.ReleaseStrategy == relDto.ReleaseStrategy &&
                                                 e.ReleaseStrategyLevel == relDto.ReleaseStrategyLevel &&
                                                 e.ReleaseStatusCode == ReleaseStatus.InRelease);

                        if (noDuesRelease == null)
                            throw new Exception("Release status record not found or not in release.");

                        noDuesRelease.ReleaseStatusCode = ReleaseStatus.FullyReleased;
                        noDuesRelease.ReleaseDate = DateTime.Now;
                        noDuesRelease.ReleaseAuth = relDto.ReleaseAuth;
                        noDuesRelease.Remarks = relDto.Remarks;

                        if (noDuesRelease.IsFinalRelease)
                        {
                            NoDuesMaster noDue = _context.NoDuesMaster.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                            if (noDue == null)
                                throw new Exception("Error: master record not found");

                            noDue.DeptApprovalFlag = true;

                            NoDuesStatus noDuesStatus =
                                _context.NoDuesStatus.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                            if (noDuesStatus != null) noDuesStatus.Hod = true;
                        }
                        else
                        {
                            NoDuesReleaseStatus nextRelease = _context.NoDuesReleaseStatus
                                .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId &&
                                                     e.ReleaseStrategy == relDto.ReleaseStrategy &&
                                                     e.ReleaseStrategyLevel == relDto.ReleaseStrategyLevel + 1);
                            if (nextRelease != null)
                            {
                                nextRelease.ReleaseStatusCode = ReleaseStatus.InRelease;
                            }
                        }

                        _context.SaveChanges();
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error:" + ex);
                }

            return true;
        }

        private bool ReleaseNoDuesUh(NoDuesMasterDto dto, bool releaseFlag)
        {
            if (releaseFlag == false)
                throw new Exception("Release flag must be true!");

            try
            {
                NoDuesMaster noDues = _context.NoDuesMaster
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                if (noDues == null)
                    throw new Exception("Employee not found.");

                if (!noDues.DeptApprovalFlag)
                    throw new Exception("Dept Hod approval pending.");

                noDues.UhApprovalFlag = true;
                noDues.UhApprovalDate = DateTime.Now;
                noDues.UhApprovedBy = dto.UhApprovedBy;

                NoDuesStatus status = _context.NoDuesStatus.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                if (status == null)
                    throw new Exception("Status table not found!");

                status.UnitHead = true;

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex);
            }

            return true;
        }

        private bool CreateNoDuesHr(NoDuesMasterDto dto, bool releaseFlag)
        {
            if (releaseFlag == false)
                try
                {
                    NoDuesMaster noDues = _context.NoDuesMaster
                        .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                    if (noDues == null)
                        throw new Exception("Employee not found.");

                    if (!noDues.DeptApprovalFlag)
                        throw new Exception("Dept Hod approval pending.");

                    noDues.NoticePeriod = dto.NoticePeriod;
                    noDues.NoticePeriodUnit = dto.NoticePeriodUnit;
                    noDues.LastWorkingDate = dto.LastWorkingDate;
                    noDues.ModeOfLeaving = dto.ModeOfLeaving;
                    noDues.ExitInterviewFlag = dto.ExitInterviewFlag;
                    noDues.HrAddUser = dto.HrAddUser;
                    noDues.HrAddDate = DateTime.Now;
                    noDues.HrApprovalFlag = false;

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error:" + ex);
                }
            else
                try
                {
                    NoDuesMaster noDues = _context.NoDuesMaster
                        .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                    if (noDues == null)
                        throw new Exception("Employee not found.");

                    if (!noDues.DeptApprovalFlag)
                        throw new Exception("Dept Hod approval pending.");

                    if (noDues.HrAddUser == null)
                        throw new Exception("HR record entry is pending.");

                    noDues.HrApprovalFlag = true;
                    noDues.HrApprovalDate = DateTime.Now;
                    noDues.HrApprovedBy = dto.HrApprovedBy;

                    NoDuesStatus status = _context.NoDuesStatus.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                    if (status == null)
                        throw new Exception("Status table not found!");

                    status.Hr = true;

                    if (status.Admin &&
                        status.Cafeteria &&
                        status.EandI &&
                        status.Finance &&
                        status.Hod &&
                        status.Ohc &&
                        status.PrgHr &&
                        status.Safety &&
                        status.School &&
                        status.Security &&
                        status.Stores &&
                        status.Township &&
                        status.It &&
                        status.Er &&
                        status.Hr)

                        // update if all flags are set.
                        noDues.ClosedFlag = true;

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error:" + ex);
                }

            return true;
        }

        private bool CreateNoDuesDept(NoDuesMasterDto dto, string dept)
        {
            try
            {
                NoDuesMaster noDues = _context.NoDuesMaster
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                if (noDues == null)
                    throw new Exception("Employee not found.");

                if (noDues.DeptNoDuesFlag == false || noDues.DeptApprovalFlag == false)
                    throw new Exception("Dept approval is pending.");

                NoDuesDeptDto dtoDept = dto.NoDuesDepts.FirstOrDefault();

                if (dtoDept == null)
                    throw new Exception("Department no dues details missing!");

                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    // remove any existing no dues dept records
                    NoDuesDept existingNoDuesDept = _context.NoDuesDept
                        .FirstOrDefault(n => n.EmpUnqId == dto.EmpUnqId && n.DeptId == dtoDept.DeptId);
                    if (existingNoDuesDept != null)
                        _context.NoDuesDept.Remove(existingNoDuesDept);
                    //

                    // add new record
                    var noDuesDept = new NoDuesDept
                    {
                        EmpUnqId = dtoDept.EmpUnqId,
                        DeptId = dtoDept.DeptId,
                        NoDuesFlag = true,
                        Remarks = "",
                        ApprovalFlag = false
                    };

                    _context.NoDuesDept.Add(noDuesDept);
                    //

                    // Remove any exisitng no dues dept details
                    var existingNoDuesDeptDetails = _context.NoDuesDeptDetails
                        .Where(e => e.EmpUnqId == dto.EmpUnqId && e.DeptId == dtoDept.DeptId)
                        .ToList();

                    if (existingNoDuesDeptDetails.Count > 0)
                        _context.NoDuesDeptDetails.RemoveRange(existingNoDuesDeptDetails);
                    //

                    // add new details records
                    foreach (NoDuesDeptDetailsDto detail in dtoDept.NoDuesDeptDetails)
                    {
                        var deptDetail = new NoDuesDeptDetails
                        {
                            EmpUnqId = detail.EmpUnqId,
                            DeptId = detail.DeptId,
                            Sr = detail.Sr,
                            Particulars = detail.Particulars,
                            Remarks = detail.Remarks,
                            Amount = detail.Amount,
                            AddUser = detail.AddUser,
                            AddDate = DateTime.Now
                        };

                        _context.NoDuesDeptDetails.Add(deptDetail);
                    }
                    //

                    _context.SaveChanges();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex);
            }

            return true;
        }

        private bool ReleaseNoDuesDept(NoDuesMasterDto dto, string dept)
        {
            try
            {
                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    NoDuesMaster noDues = _context.NoDuesMaster.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                    if (noDues == null)
                        throw new Exception("Employee not found.");

                    if (noDues.DeptNoDuesFlag == false || noDues.DeptApprovalFlag == false)
                        throw new Exception("Dept approval is pending.");

                    NoDuesDeptDto dtoDept = dto.NoDuesDepts.FirstOrDefault();
                    if (dtoDept == null)
                        throw new Exception("Department no dues not found");

                    NoDuesDept noDuesDept = _context.NoDuesDept
                        .FirstOrDefault(n => n.EmpUnqId == dto.EmpUnqId &&
                                             n.DeptId == dtoDept.DeptId);

                    if (noDuesDept == null || noDuesDept.NoDuesFlag == false)
                        throw new Exception("No Dues details are not filled.");

                    if (noDuesDept.ApprovalFlag == true)
                        throw new Exception("Already approved.");

                    noDuesDept.ApprovalFlag = true;
                    noDuesDept.ApprovedBy = dtoDept.ApprovedBy;
                    noDuesDept.ApprovalDate = DateTime.Now;
                    noDuesDept.Remarks = dtoDept.Remarks;

                    NoDuesStatus status = _context.NoDuesStatus.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                    if (status == null)
                        throw new Exception("Status table not found!");

                    switch (dept)
                    {
                        case "ADM":
                            status.Admin = true;
                            break;
                        case "CAN":
                            status.Cafeteria = true;
                            break;
                        case "ELE":
                            status.EandI = true;
                            break;
                        case "FIN":
                            status.Finance = true;
                            break;
                        case "OHC":
                            status.Ohc = true;
                            break;
                        case "PRG":
                            status.PrgHr = true;
                            break;
                        case "SAF":
                            status.Safety = true;
                            break;
                        case "SCH":
                            status.School = true;
                            break;
                        case "SEC":
                            status.Security = true;
                            break;
                        case "STO":
                            status.Stores = true;
                            break;
                        case "TOW":
                            status.Township = true;
                            break;
                        case "IT":
                            status.It = true;
                            break;
                        case "ER":
                            status.Er = true;
                            break;
                        case "HR":
                            status.Hr = true;
                            break;
                    }


                    if (status.Admin &&
                        status.Cafeteria &&
                        status.EandI &&
                        status.Finance &&
                        status.Hod &&
                        status.Ohc &&
                        status.PrgHr &&
                        status.Safety &&
                        status.School &&
                        status.Security &&
                        status.Stores &&
                        status.Township &&
                        status.It &&
                        status.Er &&
                        status.Hr)

                        // update if all flags are set.
                        noDues.ClosedFlag = true;

                    _context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex);
            }

            return true;
        }
    }
}