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
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class MedIntimationController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public MedIntimationController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetIntimations(DateTime fromDt, DateTime toDt)
        {
            List<MedIntimationDto> intimations = _context.MedIntimations
                .Include(e=>e.Employee)
                .Where(i => i.IntimationDate >= fromDt && i.IntimationDate <= toDt).AsEnumerable()
                .Select(Mapper.Map<MedIntimation,MedIntimationDto>)
                .ToList();

            if (intimations.Count == 0)
                return BadRequest("No records found.");

            foreach (MedIntimationDto dto in intimations)
            {
                Employees emp = _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.WorkGroup)
                    .Include(e => e.Units)
                    .Include(e => e.Departments)
                    .Include(e => e.Stations)
                    .Include(e => e.Categories)
                    .Include(e => e.Grades)
                    .Include(e => e.Designations)
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp == null)
                    continue;

                dto.Employee.UnitName = emp.Units.UnitName;
                dto.Employee.CompName = emp.Company.CompName;
                dto.Employee.WrkGrpDesc = emp.WorkGroup.WrkGrpDesc;
                dto.Employee.DeptName = emp.Departments.DeptName;
                dto.Employee.StatName = emp.Stations.StatName;
                dto.Employee.CatName = emp.Categories.CatName;
                dto.Employee.GradeName = emp.Grades.GradeName;
                dto.Employee.DesgName = emp.Designations.DesgName;
            }

            return Ok(intimations);
        }

        public IHttpActionResult GetIntimationById(int id)
        {
            MedIntimation intimations = _context.MedIntimations
                .Include(e=>e.Employee)
                .FirstOrDefault(i => i.Id == id);

            if (intimations == null)
                return BadRequest("No record found.");

            MedIntimationDto intimationDto = Mapper.Map<MedIntimation,MedIntimationDto>(intimations);

            Employees emp = _context.Employees
                .Include(e => e.Company)
                .Include(e => e.WorkGroup)
                .Include(e => e.Units)
                .Include(e => e.Departments)
                .Include(e => e.Stations)
                .Include(e => e.Categories)
                .Include(e => e.Grades)
                .Include(e => e.Designations)
                .FirstOrDefault(e => e.EmpUnqId == intimations.EmpUnqId);

            if (emp == null)
                return BadRequest("Employee not found.");

            intimationDto.Employee.UnitName = emp.Units.UnitName;
            intimationDto.Employee.CompName = emp.Company.CompName;
            intimationDto.Employee.WrkGrpDesc = emp.WorkGroup.WrkGrpDesc;
            intimationDto.Employee.DeptName = emp.Departments.DeptName;
            intimationDto.Employee.StatName = emp.Stations.StatName;
            intimationDto.Employee.CatName = emp.Categories.CatName;
            intimationDto.Employee.GradeName = emp.Grades.GradeName;
            intimationDto.Employee.DesgName = emp.Designations.DesgName;

            return Ok(intimationDto);
        }

        public IHttpActionResult GetIntimationByEmp(string empUnqId)
        {
            List<MedIntimationDto> intimations = _context.MedIntimations
                .Include(e=>e.Employee)
                .Where(i => i.EmpUnqId == empUnqId).AsEnumerable()
                .Select(Mapper.Map<MedIntimation,MedIntimationDto>)
                .ToList();

            if (intimations.Count == 0)
                return BadRequest("No records found.");

            foreach (MedIntimationDto dto in intimations)
            {
                Employees emp = _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.WorkGroup)
                    .Include(e => e.Units)
                    .Include(e => e.Departments)
                    .Include(e => e.Stations)
                    .Include(e => e.Categories)
                    .Include(e => e.Grades)
                    .Include(e => e.Designations)
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp == null)
                    continue;

                dto.Employee.UnitName = emp.Units.UnitName;
                dto.Employee.CompName = emp.Company.CompName;
                dto.Employee.WrkGrpDesc = emp.WorkGroup.WrkGrpDesc;
                dto.Employee.DeptName = emp.Departments.DeptName;
                dto.Employee.StatName = emp.Stations.StatName;
                dto.Employee.CatName = emp.Categories.CatName;
                dto.Employee.GradeName = emp.Grades.GradeName;
                dto.Employee.DesgName = emp.Designations.DesgName;
            }

            return Ok(intimations);
        }

        [HttpPost]
        public IHttpActionResult CreateIntimation([FromBody] object requestData)
        {
            MedIntimationDto intimation;

            try
            {
                intimation = JsonConvert.DeserializeObject<MedIntimationDto>(requestData.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            intimation.IntimationDate = DateTime.Now;
            intimation.ReleaseStatusCode = ReleaseStatus.InRelease;
            
            MedIntimation medIntimation = Mapper.Map<MedIntimationDto, MedIntimation>(intimation);
            _context.MedIntimations.Add(medIntimation);
            _context.SaveChanges();

            intimation.Id = medIntimation.Id;

            return Ok(intimation);
        }

        [HttpPut]
        public IHttpActionResult ReleaseIntimation([FromBody] object requestData)
        {
            return Ok();
        }
    }
}