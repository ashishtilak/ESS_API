using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Helpers;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class VaccinationController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public VaccinationController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetEmpVaccination(string empUnqId)
        {
            try
            {
                VaccinationDto vacc = Mapper.Map<Vaccination, VaccinationDto>(
                    _context.Vaccinations.FirstOrDefault(e => e.EmpUnqId == empUnqId));

                if (vacc == null)
                    return BadRequest("No records found!");

                vacc.Employee = new EmployeeDto();
                vacc.Employee =
                    Mapper.Map<Employees, EmployeeDto>(
                        _context.Employees
                            .Include(e => e.Company)
                            .Include(e => e.Units)
                            .Include(e => e.Departments)
                            .Include(e => e.Stations)
                            .Include(e => e.Categories)
                            .FirstOrDefault(e => e.EmpUnqId == empUnqId));
                return Ok(vacc);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }
        }

        public IHttpActionResult GetVaccination()
        {
            try
            {
                List<VaccinationDto> vacc = _context.Vaccinations
                    .Select(Mapper.Map<Vaccination, VaccinationDto>)
                    .ToList();

                if (!vacc.Any())
                    return BadRequest("No records found.");

                foreach (VaccinationDto dto in vacc)
                {
                    EmployeeDto employee = _context.Employees
                        .Where(e => e.EmpUnqId == dto.EmpUnqId)
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

                    dto.Employee = employee;
                }

                return Ok(vacc);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }
        }

        public IHttpActionResult GetFile(string empUnqId)
        {
            // Access folder with yearmonth as suffix
            string path = HostingEnvironment.MapPath(@"~/App_Data/vaccination");

            if (string.IsNullOrEmpty(path))
                return BadRequest("Path not found.");

            path += "\\" + empUnqId + ".pdf";
            return new FileResult(path, "application/pdf");
        }

        public IHttpActionResult GetAllFiles()
        {
            // Access folder with yearmonth as suffix
            string path = HostingEnvironment.MapPath(@"~/App_Data/vaccination");
            string zipPath = HostingEnvironment.MapPath(@"~/App_Data/") + "\\vaccination.zip";

            if (string.IsNullOrEmpty(path))
                return BadRequest("Path not found.");


            if (File.Exists(zipPath))
                File.Delete(zipPath);

            try
            {
                ZipFile.CreateFromDirectory(path, zipPath, CompressionLevel.Fastest, true);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            path += "\\" + "vaccination.zip";
            return new FileResult(zipPath, "application/zip");
        }

        [HttpPost]
        public IHttpActionResult AddVaccinationData([FromBody] object requestData)
        {
            try
            {
                var vDto = JsonConvert.DeserializeObject<VaccinationDto>(requestData.ToString());
                Vaccination vacc = _context.Vaccinations
                    .FirstOrDefault(e => e.EmpUnqId == vDto.EmpUnqId);

                if (vacc == null)
                {
                    vacc = new Vaccination
                    {
                        EmpUnqId = vDto.EmpUnqId,
                        FirstDoseDate = vDto.FirstDoseDate,
                        SecondDoseDate = vDto.SecondDoseDate,
                        ThirdDoseDate = vDto.ThirdDoseDate,
                        UpdateDate = DateTime.Now
                    };
                    _context.Vaccinations.Add(vacc);
                }
                else
                {
                    vacc.FirstDoseDate = vDto.FirstDoseDate;
                    vacc.SecondDoseDate = vDto.SecondDoseDate;
                    vacc.ThirdDoseDate = vDto.ThirdDoseDate;
                    vacc.UpdateDate = DateTime.Now;
                }

                _context.SaveChanges();

                return Ok(vacc);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }
        }

        [HttpPut]
        public IHttpActionResult CertificateUpload(string empUnqId)
        {
            try
            {
                if (!_context.Vaccinations.Any(e => e.EmpUnqId == empUnqId))
                    return BadRequest("Vaccination data not found.");

                HttpContext httpContext = HttpContext.Current;
                if (httpContext.Request.Files.Count <= 0) return BadRequest("NO FILES???");

                string folder = HostingEnvironment.MapPath(@"~/App_Data/vaccination");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder ?? throw new InvalidOperationException("Folder not found"));
                }

                for (var i = 0; i < httpContext.Request.Files.Count; i++)
                {
                    HttpPostedFile httpPostedFile = httpContext.Request.Files[i];

                    if (httpPostedFile == null) continue;

                    // Construct file save path
                    // save file as empunqid.pdf format
                    string fileSavePath = Path.Combine(folder, empUnqId + ".pdf");

                    if (File.Exists(fileSavePath))
                        File.Delete(fileSavePath);

                    // Save the uploaded file  
                    httpPostedFile.SaveAs(fileSavePath);
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