using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using ESS.Helpers;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class AppraisalLetterController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public AppraisalLetterController()
        {
            _context = new ApplicationDbContext();
        }

        private class AppDto
        {
            public string AppraisalYear { get; set; }
            public string EmpUnqId { get; set; }
        }

        [HttpGet]
        public IHttpActionResult GetLinks(string empUnqId)
        {
            DateTime today = DateTime.Today;
            var years = new List<string> { (today.Year - 1) + "-" + (today.Year) };

            return Ok(years);
        }

        [HttpPost]
        public IHttpActionResult GetAppraisal([FromBody] object requestData)
        {
            AppDto appDto;

            try
            {
                appDto = JsonConvert.DeserializeObject<AppDto>(requestData.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            
            // Access folder with year as suffix then Form16 and Form12 folders
            var loc = _context.Location.FirstOrDefault();
            if (loc == null)
                return BadRequest("Location configuration error.");

            //var empDetails = Helpers.CustomHelper.GetEmpDetails(form16.EmpUnqId);
            var empUnqId = _context.Employees.FirstOrDefault(e => e.EmpUnqId == appDto.EmpUnqId)?.EmpUnqId;

            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/app_" + appDto.AppraisalYear);

            if (string.IsNullOrEmpty(path))
                return BadRequest("Path not found.");


            path += "\\" + empUnqId + ".pdf";

            return new FileResult(path, "application/pdf");
        }



        [HttpPost]
        public IHttpActionResult UploadForm(string folderName)
        {
            System.Web.HttpContext httpContext = HttpContext.Current;

            // Check for any uploaded file  
            if (httpContext.Request.Files.Count <= 0) return BadRequest("NO FILES???");

            // Create new folder if does not exist.

            try
            {
                var folder = HostingEnvironment.MapPath(@"~/App_Data/" + folderName);

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder ?? throw new InvalidOperationException("Folder not found"));
                }

                //Loop through uploaded files 
                for (int i = 0; i < httpContext.Request.Files.Count; i++)
                {
                    HttpPostedFile httpPostedFile = httpContext.Request.Files[i];

                    if (httpPostedFile == null) continue;

                    // Construct file save path
                    var fileSavePath = Path.Combine(folder ?? throw new InvalidOperationException("Folder not found"),
                        httpPostedFile.FileName);

                    // Save the uploaded file  
                    httpPostedFile.SaveAs(fileSavePath);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            // Return status code  
            return Ok();
        }
    }
}
