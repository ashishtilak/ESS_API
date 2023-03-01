using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class FandFController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public FandFController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        public IHttpActionResult GetFandF(string empUnqId)
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
                .FirstOrDefault();

            if (employee == null)
                return BadRequest("Invalid employee.");


            NoDuesMaster nd = _context.NoDuesMaster.FirstOrDefault(e => e.EmpUnqId == empUnqId);

            if (nd == null)
                return BadRequest("No dues is not done for this employee");


            FullAndFinalDto fnf = _context.FullAndFinals
                                      .Where(e => e.EmpUnqId == empUnqId).AsEnumerable()
                                      .Select(Mapper.Map<FullAndFinal, FullAndFinalDto>)
                                      .FirstOrDefault()
                                  ?? new FullAndFinalDto();

            fnf.EmpUnqId = empUnqId;
            fnf.Employee = employee;

            fnf.JoinDate = nd.JoinDate;
            fnf.RelieveDate = nd.RelieveDate;
            fnf.NoDuesStartDate = nd.NoDuesStartDate;
            fnf.NoticePeriod = nd.NoticePeriod;
            fnf.NoticePeriodUnit = nd.NoticePeriodUnit;
            fnf.LastWorkingDate = nd.LastWorkingDate;
            fnf.ModeOfLeaving = nd.ModeOfLeaving;
            fnf.ClosedFlag = nd.ClosedFlag;

            if (fnf.LastWorkingDate != null && fnf.RelieveDate.Value.Year - fnf.LastWorkingDate.Value.Year >= 5)
                fnf.GratuityFlag = true;

            return Ok(fnf);
        }

        [HttpGet]
        public IHttpActionResult GetFandF(DateTime fromDt, DateTime toDt)
        {
            List<FullAndFinalDto> fnfs = _context.FullAndFinals
                .Where(e => e.RelieveDate >= fromDt && e.RelieveDate <= toDt).AsEnumerable()
                .Select(Mapper.Map<FullAndFinal, FullAndFinalDto>)
                .ToList();

            if (fnfs.Count == 0)
                return BadRequest("No records found.");

            foreach (FullAndFinalDto fnf in fnfs)
            {
                EmployeeDto employee = _context.Employees
                    .Where(e => e.EmpUnqId == fnf.EmpUnqId)
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
                    .FirstOrDefault();

                if (employee == null)
                    continue;


                NoDuesMaster nd = _context.NoDuesMaster.FirstOrDefault(e => e.EmpUnqId == fnf.EmpUnqId);

                if (nd == null)
                    continue;

                fnf.Employee = employee;

                fnf.JoinDate = nd.JoinDate;
                fnf.RelieveDate = nd.RelieveDate;
                fnf.NoDuesStartDate = nd.NoDuesStartDate;
                fnf.NoticePeriod = nd.NoticePeriod;
                fnf.NoticePeriodUnit = nd.NoticePeriodUnit;
                fnf.LastWorkingDate = nd.LastWorkingDate;
                fnf.ModeOfLeaving = nd.ModeOfLeaving;
                fnf.ClosedFlag = nd.ClosedFlag;

                if (fnf.RelieveDate.Value.Year - fnf.JoinDate.Value.Year >= 5)
                    fnf.GratuityFlag = true;
            }

            return Ok(fnfs);
        }

        [HttpPost]
        public IHttpActionResult CreateFandF([FromBody] object requestData)
        {
            var dto = JsonConvert.DeserializeObject<FullAndFinalDto>(requestData.ToString());

            if (_context.FullAndFinals.Any(e => e.EmpUnqId == dto.EmpUnqId))
                return BadRequest("Full and Final already exists.");

            var fnf = new FullAndFinal
            {
                EmpUnqId = dto.EmpUnqId,
                RelieveDate = dto.RelieveDate,
                DocumentNo = dto.DocumentNo,
                RecoveryAmount = dto.RecoveryAmount,
                CashDeposited = dto.CashDeposited,
                DepositDate = dto.DepositDate,
                Remarks = dto.Remarks,
                GratuityFlag = dto.GratuityFlag,
                AddUser = dto.AddUser,
                AddDate = DateTime.Now
            };

            try
            {
                _context.FullAndFinals.Add(fnf);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            return Ok(dto);
        }

        [HttpPut]
        public IHttpActionResult ChangeFandF([FromBody] object requestData)
        {
            var dto = JsonConvert.DeserializeObject<FullAndFinalDto>(requestData.ToString());

            var fnf = _context.FullAndFinals.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
            if (fnf == null)
                return BadRequest("Full and Final does not exist.");

            fnf.EmpUnqId = dto.EmpUnqId;
            fnf.RelieveDate = dto.RelieveDate;
            fnf.DocumentNo = dto.DocumentNo;
            fnf.RecoveryAmount = dto.RecoveryAmount;
            fnf.CashDeposited = dto.CashDeposited;
            fnf.DepositDate = dto.DepositDate;
            fnf.Remarks = dto.Remarks;
            fnf.GratuityFlag = dto.GratuityFlag;
            fnf.AddUser = dto.AddUser;
            fnf.AddDate = DateTime.Now;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            return Ok(dto);
        }
    }
}