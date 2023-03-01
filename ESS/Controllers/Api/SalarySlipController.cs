using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Results;
using ESS.Helpers;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class SalarySlipController : ApiController
    {
        private ApplicationDbContext _context;

        public SalarySlipController()
        {
            _context = new ApplicationDbContext();
        }

        private class Months
        {
            public int YearMonth { get; set; }
            public string MonthName { get; set; }
        }

        private class PaySlip
        {
            public int YearMonth { get; set; }
            public string EmpUnqId { get; set; }
        }


        [HttpGet]
        public IHttpActionResult GetLinks(string empUnqId)
        {
            DateTime today = DateTime.Today;
            DateTime sixMonth = today.AddMonths(-7);
            var months = new List<Months>();

            for (DateTime dt = today; dt > sixMonth;)
            {
                if (DateTimeFormatInfo.CurrentInfo != null)
                {
                    Months mon = new Months
                    {
                        YearMonth = Convert.ToInt32(dt.Year + dt.Month.ToString()),
                        MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(dt.Month) + " " + dt.Year
                    };
                    months.Add(mon);
                }

                dt = dt.AddMonths(-1);
            }

            return Ok(months);
        }

        [HttpPost]
        public IHttpActionResult GetSalarySlip([FromBody] object requestData)
        {
            PaySlip payslip;

            try
            {
                payslip = JsonConvert.DeserializeObject<PaySlip>(requestData.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            // Access folder with yearmonth as suffix
            Locations loc = _context.Location.FirstOrDefault();
            if (loc == null)
                return BadRequest("Location configuration error.");

            string path = HostingEnvironment.MapPath(@"~/App_Data/" + payslip.YearMonth);

            if (string.IsNullOrEmpty(path))
                return BadRequest("Path not found.");

            path += "\\" + payslip.EmpUnqId + ".pdf";

            return new FileResult(path, "application/pdf");
        }

        [HttpPost]
        public IHttpActionResult UploadPaySlip(string folderName)
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