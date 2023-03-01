using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public class MedicalFitnessController : ApiController
    {
        private ApplicationDbContext _context;

        public MedicalFitnessController()
        {
            _context = new ApplicationDbContext();
        }


        // GET for report
        public IHttpActionResult GetMedFitness(DateTime fromDt, DateTime toDt, string empUnqId = null)
        {
            if (toDt < fromDt)
                return BadRequest("From date is less than To date.");

            toDt = toDt.AddDays(1);

            List<MedicalFitnessDto> fitnessRec;

            if (empUnqId != null)
            {
                fitnessRec = _context.MedicalFitness
                    .Where(m => m.TestDate >= fromDt && m.TestDate <= toDt && m.EmpUnqId == empUnqId)
                    .AsEnumerable()
                    .Select(Mapper.Map<MedicalFitness, MedicalFitnessDto>)
                    .ToList();
                if (fitnessRec.Count <= 0) return BadRequest("No records found.");

                Employees emp = _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.WorkGroup)
                    .Include(e => e.Units)
                    .Include(e => e.Departments)
                    .Include(e => e.Stations)
                    .Include(e => e.Categories)
                    .Include(e => e.EmpTypes)
                    .Include(e => e.Grades)
                    .Include(e => e.Designations)
                    .FirstOrDefault(e => e.EmpUnqId == empUnqId && e.Active);

                if (emp == null) return Ok(fitnessRec);

                foreach (MedicalFitnessDto dto in fitnessRec)
                {
                    dto.EmpName = emp.EmpName;
                    dto.CompName = emp.Company.CompName;
                    dto.WrkGrpDesc = emp.WorkGroup.WrkGrpDesc;
                    dto.UnitName = emp.Units.UnitName;
                    dto.DeptName = emp.Departments.DeptName;
                    dto.StatName = emp.Stations.StatName;
                    dto.CatName = emp.Categories.CatName;
                    dto.EmpTypeName = emp.EmpTypes.EmpTypeName;
                    dto.GradeName = emp.Grades.GradeName;
                    dto.DesgName = emp.Designations.DesgName;
                }
            }
            else
            {
                fitnessRec = _context.MedicalFitness
                    .Where(m => m.TestDate >= fromDt && m.TestDate < toDt)
                    .AsEnumerable()
                    .Select(Mapper.Map<MedicalFitness, MedicalFitnessDto>)
                    .ToList();

                if (fitnessRec.Count <= 0) return BadRequest("No records found.");

                var empUnqIds = fitnessRec
                    .GroupBy(f => f.EmpUnqId).Select(x => x.FirstOrDefault())
                    .Select(e => e.EmpUnqId).ToArray();


                var emps = _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.WorkGroup)
                    .Include(e => e.Units)
                    .Include(e => e.Departments)
                    .Include(e => e.Stations)
                    .Include(e => e.Categories)
                    .Include(e => e.EmpTypes)
                    .Include(e => e.Grades)
                    .Include(e => e.Designations)
                    .Where(e => empUnqIds.Contains(e.EmpUnqId) && e.Active)
                    .ToList();

                if (emps.Count <= 0) return Ok(fitnessRec);

                foreach (Employees emp in emps)
                {
                    foreach (MedicalFitnessDto dto in fitnessRec.Where(dto => dto.EmpUnqId == emp.EmpUnqId))
                    {
                        dto.EmpName = emp.EmpName;
                        dto.CompName = emp.Company.CompName;
                        dto.WrkGrpDesc = emp.WorkGroup.WrkGrpDesc;
                        dto.UnitName = emp.Units.UnitName;
                        dto.DeptName = emp.Departments.DeptName;
                        dto.StatName = emp.Stations.StatName;
                        dto.CatName = emp.Categories.CatName;
                        dto.EmpTypeName = emp.EmpTypes.EmpTypeName;
                        dto.GradeName = emp.Grades.GradeName;
                        dto.DesgName = emp.Designations.DesgName;
                    }
                }
            }

            return Ok(fitnessRec);
        }

        // GET for particular employee, for filling the fitness form
        public IHttpActionResult GetEmpBlock(string empUnqId)
        {
            Employees emp = _context.Employees
                .Include(e => e.Company)
                .Include(e => e.WorkGroup)
                .Include(e => e.Units)
                .Include(e => e.Departments)
                .Include(e => e.Stations)
                .Include(e => e.Categories)
                .Include(e => e.EmpTypes)
                .Include(e => e.Grades)
                .Include(e => e.Designations)
                .FirstOrDefault(e => e.EmpUnqId == empUnqId && e.Active);
            if (emp == null)
                return BadRequest("Employee not found or not active!");

            EmpPunchBlockDto dto = Helpers.CustomHelper.GetEmpPunchBlock(empUnqId, emp.Location);

            dto.EmpName = emp.EmpName;
            dto.CompName = emp.Company.CompName;
            dto.WrkGrpDesc = emp.WorkGroup.WrkGrpDesc;
            dto.UnitName = emp.Units.UnitName;
            dto.DeptName = emp.Departments.DeptName;
            dto.StatName = emp.Stations.StatName;
            dto.CatName = emp.Categories.CatName;
            dto.EmpTypeName = emp.EmpTypes.EmpTypeName;
            dto.GradeName = emp.Grades.GradeName;
            dto.DesgName = emp.Designations.DesgName;


            VaccinationDto vacc = Mapper.Map<Vaccination, VaccinationDto>(
                _context.Vaccinations.FirstOrDefault(e => e.EmpUnqId == empUnqId));

            if (vacc == null) return Ok(dto);

            dto.FirstDoseDate = vacc.FirstDoseDate;
            dto.SecondDoseDate = vacc.SecondDoseDate;
            dto.ThirdDoseDate = vacc.ThirdDoseDate;

            return Ok(dto);
        }

        [HttpPost]
        public IHttpActionResult CreateMedFitness([FromBody] object requestData)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var dto = JsonConvert.DeserializeObject<MedicalFitnessDto>(requestData.ToString());

            Employees emp = _context.Employees.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId && e.Active);
            if (emp == null)
                return BadRequest("Employee does not exist or not active.");

            try
            {
                var newFitness = new MedicalFitness
                {
                    TestDate = dto.TestDate,
                    EmpUnqId = dto.EmpUnqId,
                    CardBlockedOn = dto.CardBlockedOn,
                    CardBlockedDays = dto.CardBlockedDays,
                    CardBlockedReason = dto.CardBlockedReason,
                    FitnessFlag = dto.FitnessFlag,
                    Remarks = dto.Remarks,
                    AddUser = dto.AddUser,
                    AddDt = DateTime.Now
                };

                _context.MedicalFitness.Add(newFitness);
                _context.SaveChanges();

                return Ok(newFitness);
            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.ToString());
            }
        }
    }
}