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
    public class Form16Controller : ApiController
    {
        private readonly ApplicationDbContext _context;

        public Form16Controller()
        {
            _context = new ApplicationDbContext();
        }

        private class Form16
        {
            public string AssYear { get; set; }
            public string EmpUnqId { get; set; }
            public string FormNumber { get; set; }
        }

        [HttpGet]
        public IHttpActionResult GetLinks(string empUnqId)
        {
            DateTime today = DateTime.Today;
            DateTime threeYears = today.AddYears(-3);
            var assYears = new List<string>();

            for (DateTime dt = today; dt > threeYears;)
            {
                if (DateTimeFormatInfo.CurrentInfo != null)
                {
                    string assYear = (dt.Year - 1) + "-" + (dt.Year);
                    assYears.Add(assYear);
                }

                dt = dt.AddYears(-1);
            }

            return Ok(assYears);
        }


        [HttpPost]
        public IHttpActionResult GetForm16([FromBody] object requestData)
        {
            Form16 form16;

            try
            {
                form16 = JsonConvert.DeserializeObject<Form16>(requestData.ToString());
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
            var pan = _context.Employees.FirstOrDefault(e => e.EmpUnqId == form16.EmpUnqId)?.Pan;

            var path = HostingEnvironment.MapPath(@"~/App_Data/" + form16.AssYear);

            if (string.IsNullOrEmpty(path))
                return BadRequest("Path not found.");


            if (form16.FormNumber == "16")
                path += "\\form16\\" + pan + ".pdf";
            else if(form16.FormNumber == "16B")
                path += "\\form16B\\" + pan + ".pdf";
            else if(form16.FormNumber == "12")
                path += "\\form12\\" + pan + ".pdf";

            return new FileResult(path, "application/pdf");
        }


        [HttpPost]
        public IHttpActionResult UploadForm(string folderName)
        {
            HttpContext httpContext = HttpContext.Current;

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