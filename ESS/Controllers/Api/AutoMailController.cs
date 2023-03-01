using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;

namespace ESS.Controllers.Api
{
    public class AutoMailController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public AutoMailController()
        {
            _context = new ApplicationDbContext();
            _context.Database.Log = s => Debug.WriteLine(s);
        }

        [HttpGet]
        public IHttpActionResult SendMail(string releaseGroupCode, int id, string releaseAuth)
        {
            try
            {
                if (releaseGroupCode == ReleaseGroups.LeaveApplication)
                    return Ok(SendMailLeave(id, releaseAuth));
                else if (releaseGroupCode == ReleaseGroups.GatePass)
                    return Ok(SendMailGatepass(id, releaseAuth));
                else if (releaseGroupCode == ReleaseGroups.ShiftSchedule)
                    return Ok(SendMailShiftSchedule(id, releaseAuth));
                else
                    return BadRequest("Invalid release group code");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        private bool SendMailShiftSchedule(int id, string releaseCode)
        {
            ShiftSchedules ss = _context.ShiftSchedules.FirstOrDefault(
                s => s.ScheduleId == id);
            if (ss == null)
                throw new Exception("Invalid shift schedule id");

            ShiftScheduleDto ssDto = Mapper.Map<ShiftSchedules, ShiftScheduleDto>(ss);

            if (ssDto == null)
                throw new Exception("Invalid shift schedule id");

            // get corresponding app release objects
            var app = _context.ApplReleaseStatus
                .Where(l =>
                    l.YearMonth == ssDto.YearMonth &&
                    l.ApplicationId == ssDto.ScheduleId &&
                    l.ReleaseGroupCode == ssDto.ReleaseGroupCode &&
                    l.ReleaseCode == releaseCode
                ).AsEnumerable()
                .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>)
                .FirstOrDefault();

            // add them to dto
            ssDto.ApplReleaseStatus = new List<ApplReleaseStatusDto>();

            if (app != null)
                ssDto.ApplReleaseStatus.Add(app);
            else
                throw new Exception("Releaser is invalid! No app release line found.");

            //// loop through each app release object
            //foreach (var applReleaseStatusDto in app)
            //{
            //    // Check for release code for supplied releaser 
            //    var relAuth = _context.ReleaseAuth
            //        .SingleOrDefault(r =>
            //            r.ReleaseCode == releaseCode
            //            );

            //    // when release code is matched, add corresponding app rel line to gatepass dto
            //    if (relAuth != null)
            //        ssDto.ApplReleaseStatus.Add(applReleaseStatusDto);
            //}

            // if no app release lines are found, there's no line for this employee to release
            //if (ssDto.ApplReleaseStatus.Count == 0)
            //    throw new Exception("Releaser is invalid! No app release line found.");    

            //// FILL EMP DETAILS....
            Employees emp = _context.Employees
                .Include(d => d.Departments)
                .Include(s => s.Stations)
                .FirstOrDefault(e => e.EmpUnqId == ssDto.AddUser);
            if (emp != null)
            {
                ssDto.EmpName = emp.EmpName;
                ssDto.DeptName = emp.Departments.DeptName;
                ssDto.StatName = emp.Stations.StatName;
            }

            string empCode = ssDto.EmpUnqId;
            string empName = ssDto.EmpName;
            string deptstat = ssDto.DeptName + " / " + ssDto.StatName;

            const string header = @"
                <html lang=""en"">
                    <head>    
                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                        <title>
                            ESS Portal - Automessage
                        </title>
                        <style type=""text/css"">
                            body { font-family: arial, sans-serif; }
                            table {
                                font-family: arial, sans-serif;
                                border-collapse: collapse;
                                width: 80%;
                            }

                            td, th {
                                border: 1px solid #dddddd;
                                text-align: left;
                                padding: 8px;
                            }

                            tr:nth-child(even) {
                                background-color: #dddddd;
                            }
                        </style>
                    </head>
                    <body>
                ";

            string body = "Dear Sir, <br /><br /> " +
                          "Following Shift schedule application requires your attention: <br/> <br />" +
                          "Shif schedule Uploaded by: " + empCode + " <br/>" +
                          "Name: " + empName + " <br/>" +
                          "Dept/Station: " + deptstat + " <br/></br>";

            body = header + body;

            body += "<br/>Kindly review the same in <a href='" + ConfigurationManager.AppSettings["PortalAddress"] +
                    "'>ESS Portal</a>.";

            body += "</body></html>";

            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPClient"], 25)
            {
                //Credentials = new System.Net.NetworkCredential("tilaka@jindalsaw.com", "ashish123$$"),
                UseDefaultCredentials = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false
            };


            var mail = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["MailAddress"], "ESS Portal"),
                //From = new MailAddress("attnd.nkp@jindalsaw.com", "ESS Portal"),
                Subject = "Notification from ESS Portal for Shift schedule release.",
                BodyEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = true,
                Body = body
            };

            //now loop through all app release lines -- there'll be only one line, but anyway...
            foreach (ApplReleaseStatusDto dto in ssDto.ApplReleaseStatus)
            {
                //find the releaser from the releasecode
                var releasers = _context.ReleaseAuth
                    .Where(r => r.ReleaseCode == dto.ReleaseCode).ToList();

                //in case of multiple releasers for single release code,
                foreach (Employees releaser in
                    releasers.Select(r =>
                            _context.Employees.SingleOrDefault(e => e.EmpUnqId == r.EmpUnqId))
                        .Where(releaser => releaser != null).Where(releaser => !string.IsNullOrEmpty(releaser.Email)))
                {
                    mail.To.Add(new MailAddress(releaser.Email));
                    smtpClient.Send(mail);
                    mail.To.Remove(new MailAddress(releaser.Email));
                }
            }

            return true;
        }

        private bool SendMailGatepass(int id, string releaseAuth)
        {
            var gpDto = _context.GatePass
                .Select(Mapper.Map<GatePass, GatePassDto>)
                .FirstOrDefault(g => g.Id == id);

            if (gpDto == null)
                throw new Exception("Invalid gate pass application");

            // get all App Release lines
            var app = _context.ApplReleaseStatus
                .Where(l =>
                    l.YearMonth == gpDto.YearMonth &&
                    l.ReleaseGroupCode == gpDto.ReleaseGroupCode &&
                    l.ApplicationId == gpDto.Id
                )
                .ToList()
                .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>);

            gpDto.ApplReleaseStatus = new List<ApplReleaseStatusDto>();

            foreach (var applReleaseStatusDto in app)
            {
                // Check for release code for supplied releaser 
                var relAuth = _context.ReleaseAuth
                    .SingleOrDefault(r =>
                        r.ReleaseCode == applReleaseStatusDto.ReleaseCode &&
                        r.EmpUnqId == releaseAuth);

                // when release code is matched, add corresponding app rel line to gatepass dto
                if (relAuth != null)
                    gpDto.ApplReleaseStatus.Add(applReleaseStatusDto);
            }

            // if no app release lines are found, there's no line for this employee to release
            if (gpDto.ApplReleaseStatus.Count == 0)
                throw new Exception("Releaser is invalid! No app release line found.");

            //// FILL EMP DETAILS....
            var emp = _context.Employees
                .Include(d => d.Departments)
                .Include(s => s.Stations)
                .FirstOrDefault(e => e.EmpUnqId == gpDto.EmpUnqId);
            if (emp != null)
            {
                gpDto.EmpName = emp.EmpName;
                gpDto.DeptName = emp.Departments.DeptName;
                gpDto.StatName = emp.Stations.StatName;
                gpDto.ModeName = gpDto.GetMode(gpDto.Mode);
            }

            var empCode = gpDto.EmpUnqId;
            var empName = gpDto.EmpName;
            var deptstat = gpDto.DeptName + " / " + gpDto.StatName;

            const string header = @"
                <html lang=""en"">
                    <head>    
                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                        <title>
                            ESS Portal - Automessage
                        </title>
                        <style type=""text/css"">
                            body { font-family: arial, sans-serif; }
                            table {
                                font-family: arial, sans-serif;
                                border-collapse: collapse;
                                width: 80%;
                            }

                            td, th {
                                border: 1px solid #dddddd;
                                text-align: left;
                                padding: 8px;
                            }

                            tr:nth-child(even) {
                                background-color: #dddddd;
                            }
                        </style>
                    </head>
                    <body>
                ";

            string body = "Dear Sir, <br /><br /> " +
                          "Following Gatepass application requires your attention: <br/> <br />" +
                          "Employee Code: " + empCode + " <br/>" +
                          "Employee Name: " + empName + " <br/>" +
                          "Dept/Station: " + deptstat + " <br/>" +
                          "Gatepass Type: " + gpDto.ModeName + "<br/>" +
                          "Reason: " + gpDto.Reason + "<br/><br/>";

            body = header + body;

            body += "<br/>Kindly review the same in <a href='" + ConfigurationManager.AppSettings["PortalAddress"] +
                    "'>ESS Portal</a>.";

            body += "</body></html>";

            SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPClient"], 25)
            {
                //Credentials = new System.Net.NetworkCredential("tilaka@jindalsaw.com", "ashish123$$"),
                UseDefaultCredentials = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false
            };

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["MailAddress"], "ESS Portal"),
                //From = new MailAddress("attnd.nkp@jindalsaw.com", "ESS Portal"),
                Subject = "Notification from ESS Portal for Gatepass release.",
                BodyEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = true,
                Body = body
            };

            //now loop through all app release lines -- there'll be only one line, but anyway...
            foreach (var dto in gpDto.ApplReleaseStatus)
            {
                //find the releaser from the releasecode
                var releasers = _context.ReleaseAuth
                    .Where(r => r.ReleaseCode == dto.ReleaseCode).ToList();

                //in case of multiple releasers for single release code,
                foreach (var r in releasers)
                {
                    // get releaser employee object
                    var releaser = _context.Employees.SingleOrDefault(e => e.EmpUnqId == r.EmpUnqId);

                    if (releaser == null)
                        continue;

                    if (string.IsNullOrEmpty(releaser.Email))
                        continue;

                    mail.To.Add(new MailAddress(releaser.Email));
                    smtpClient.Send(mail);
                    mail.To.Remove(new MailAddress(releaser.Email));
                }
            }

            return true;
        }

        private bool SendMailLeave(int id, string releaseAuth)
        {
            //get the leave application object
            var leaveApps = _context.LeaveApplications
                .Include(e => e.Employee)
                .Include(c => c.Company)
                .Include(cat => cat.Categories)
                .Include(w => w.WorkGroup)
                .Include(d => d.Departments)
                .Include(s => s.Stations)
                .Include(u => u.Units)
                .Include(r => r.ReleaseGroup)
                .Include(rs => rs.RelStrategy)
                .Include(l => l.LeaveApplicationDetails)
                .FirstOrDefault(a => a.LeaveAppId == id);

            var leaveAppDto = Mapper.Map<LeaveApplications, LeaveApplicationDto>(leaveApps);

            if (leaveAppDto == null)
                throw new Exception("Invalid leave application parameters.");

            // get all App Release lines
            var app = _context.ApplReleaseStatus
                .Where(l =>
                    l.YearMonth == leaveAppDto.YearMonth &&
                    l.ReleaseGroupCode == leaveAppDto.ReleaseGroupCode &&
                    l.ApplicationId == leaveAppDto.LeaveAppId
                )
                .ToList()
                .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>);

            foreach (var applReleaseStatusDto in app)
            {
                // Check for release code for supplied releaser 
                var relAuth = _context.ReleaseAuth
                    .SingleOrDefault(r =>
                        r.ReleaseCode == applReleaseStatusDto.ReleaseCode &&
                        r.EmpUnqId == releaseAuth);

                // when release code is matched, add corresponding app rel line to leave dto
                if (relAuth != null)
                    leaveAppDto.ApplReleaseStatus.Add(applReleaseStatusDto);
            }


            // if no app release lines are found, there's no line for this employee to release
            if (leaveAppDto.ApplReleaseStatus.Count == 0)
                throw new Exception("Releaser is invalid! No app release line found.");

            var empCode = leaveAppDto.EmpUnqId;
            var empName = leaveAppDto.Employee.EmpName;
            var deptstat = leaveAppDto.Stations.StatName;

            const string header = @"
                <html lang=""en"">
                    <head>    
                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                        <title>
                            ESS Portal - Automessage
                        </title>
                        <style type=""text/css"">
                            body { font-family: arial, sans-serif; }
                            table {
                                font-family: arial, sans-serif;
                                border-collapse: collapse;
                                width: 80%;
                            }

                            td, th {
                                border: 1px solid #dddddd;
                                text-align: left;
                                padding: 8px;
                            }

                            tr:nth-child(even) {
                                background-color: #dddddd;
                            }
                        </style>
                    </head>
                    <body>
                ";

            string body = "Dear Sir, <br /><br /> " +
                          "Following leave application requires your attention: <br/> <br />" +
                          "Employee Code: " + empCode + " <br/>" +
                          "Employee Name: " + empName + " <br/>" +
                          "Dept/Station: " + deptstat + " <br/><br/>";

            body = header + body;

            if (leaveAppDto.Cancelled)
                body += "<br /><p style='Color:Red'>Please note that this is a leave cancellation. </p> <br /><br />";

            string bodyTable = "<table> " +
                               "<thead>" +
                               "<tr>" +
                               "<th>Leave Type</th>" +
                               "<th>From Date</th>" +
                               "<th>To Date</th>" +
                               "<th>Total Days</th>" +
                               "<th>Half Day</th>" +
                               "<th>Reason</th>" +
                               "</tr>" +
                               "</thead>" +
                               "<tbody>";


            foreach (var detail in leaveAppDto.LeaveApplicationDetails)
            {
                string bodyLine = "<tr>" +
                                  "<td>" + detail.LeaveTypeCode + "</td>" +
                                  "<td>" + detail.FromDt.ToString("dd/MM/yyyy") + "</td>" +
                                  "<td>" + detail.ToDt.ToString("dd/MM/yyyy") + "</td>" +
                                  "<td>" + detail.TotalDays + "</td>" +
                                  "<td>" + (detail.HalfDayFlag ? "Yes" : "No") + "</td>" +
                                  "<td>" + detail.Remarks + "</td>" +
                                  "</tr>";

                bodyTable += bodyLine;
            }

            bodyTable += "</tbody>" +
                         "</table> <br/>";
            body += bodyTable;

            body += "<br/>Kindly review the same in <a href='" + ConfigurationManager.AppSettings["PortalAddress"] +
                    "'>ESS Portal</a>.";

            body += "</body></html>";

            SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPClient"], 25)
            {
                //Credentials = new System.Net.NetworkCredential("tilaka@jindalsaw.com", "ashish123$$"),
                UseDefaultCredentials = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false
            };

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["MailAddress"], "ESS Portal"),
                //From = new MailAddress("attnd.nkp@jindalsaw.com", "ESS Portal"),
                Subject = "Notification from ESS Portal",
                BodyEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = true,
                Body = body
            };


            //now loop through all app release lines -- there'll be only one line, but anyway...
            foreach (var dto in leaveAppDto.ApplReleaseStatus)
            {
                //find the releaser from the releasecode
                var releasers = _context.ReleaseAuth
                    .Where(r => r.ReleaseCode == dto.ReleaseCode).ToList();


                //in case of multiple releasers for single release code,
                foreach (var releaser in
                    releasers.Select(r =>
                            _context.Employees.SingleOrDefault(e => e.EmpUnqId == r.EmpUnqId))
                        .Where(releaser => releaser != null)
                        .Where(releaser => !string.IsNullOrEmpty(releaser.Email)))
                {
                    //return BadRequest("Email not maintained for releaser " + releaser.EmpUnqId + " - " + releaser.EmpName);

                    mail.To.Add(new MailAddress(releaser.Email));
                    smtpClient.Send(mail);
                    mail.To.Remove(new MailAddress(releaser.Email));
                }
            }

            return true;
        }

        [HttpGet]
        public IHttpActionResult SendMail(string releaseGroupCode, int id)
        {
            //This controller is used to send rejection mail
            //whenever HR rejects a leave application.
            //Mail is sent to first level releaser only

            //get the leave application object
            var leaveAppDto = _context.LeaveApplications
                .Include(e => e.Employee)
                .Include(c => c.Company)
                .Include(cat => cat.Categories)
                .Include(w => w.WorkGroup)
                .Include(d => d.Departments)
                .Include(s => s.Stations)
                .Include(u => u.Units)
                .Include(r => r.ReleaseGroup)
                .Include(rs => rs.RelStrategy)
                .Include(l => l.LeaveApplicationDetails)
                .Select(Mapper.Map<LeaveApplications, LeaveApplicationDto>)
                .FirstOrDefault(a => a.LeaveAppId == id);

            if (leaveAppDto == null)
                return BadRequest("Invalid leave application parameters.");


            // get App Release lines for 1st level release strategy only

            var app = _context.ApplReleaseStatus
                .Where(l =>
                    l.YearMonth == leaveAppDto.YearMonth &&
                    l.ReleaseGroupCode == leaveAppDto.ReleaseGroupCode &&
                    l.ApplicationId == leaveAppDto.LeaveAppId &&
                    l.ReleaseStrategyLevel == 1
                )
                .ToList()
                .Select(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>);

            foreach (var applReleaseStatusDto in app)
            {
                // Check for release code for supplied releaser 
                var relAuth = _context.ReleaseAuth
                    .SingleOrDefault(r =>
                        r.ReleaseCode == applReleaseStatusDto.ReleaseCode &&
                        r.Active);

                // when release code is matched, add corresponding app rel line to leave dto
                if (relAuth != null)
                    leaveAppDto.ApplReleaseStatus.Add(applReleaseStatusDto);
            }


            // if no app release lines are found, there's no line for this employee to release
            if (leaveAppDto.ApplReleaseStatus.Count == 0)
                return BadRequest("Releaser is invalid! No app release line found.");

            var empCode = leaveAppDto.EmpUnqId;
            var empName = leaveAppDto.Employee.EmpName;
            var deptstat = leaveAppDto.Stations.StatName;

            const string header = @"
                <html lang=""en"">
                    <head>    
                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                        <title>
                            ESS Portal - Automessage
                        </title>
                        <style type=""text/css"">
                            body { font-family: arial, sans-serif; }
                            table {
                                font-family: arial, sans-serif;
                                border-collapse: collapse;
                                width: 80%;
                            }

                            td, th {
                                border: 1px solid #dddddd;
                                text-align: left;
                                padding: 8px;
                            }

                            tr:nth-child(even) {
                                background-color: #dddddd;
                            }
                        </style>
                    </head>
                    <body>
                ";

            string body = "Dear Sir, <br /><br /> " +
                          "Following leave application HAS BEEN REJECTED BY HR: <br/> <br />" +
                          "Employee Code: " + empCode + " <br/>" +
                          "Employee Name: " + empName + " <br/>" +
                          "Dept/Station: " + deptstat + " <br/><br/>";

            body = header + body;

            if (leaveAppDto.Cancelled)
                body += "<br /><p style='Color:Red'>Please note that this is a leave cancellation. </p> <br /><br />";

            string bodyTable = "<table> " +
                               "<thead>" +
                               "<tr>" +
                               "<th>Leave Type</th>" +
                               "<th>From Date</th>" +
                               "<th>To Date</th>" +
                               "<th>Total Days</th>" +
                               "<th>Half Day</th>" +
                               "<th>Reason</th>" +
                               "<th>HR Rejection Reason</th>" +
                               "</tr>" +
                               "</thead>" +
                               "<tbody>";


            foreach (var detail in leaveAppDto.LeaveApplicationDetails)
            {
                string bodyLine = "<tr>" +
                                  "<td>" + detail.LeaveTypeCode + "</td>" +
                                  "<td>" + detail.FromDt.ToString("dd/MM/yyyy") + "</td>" +
                                  "<td>" + detail.ToDt.ToString("dd/MM/yyyy") + "</td>" +
                                  "<td>" + detail.TotalDays + "</td>" +
                                  "<td>" + (detail.HalfDayFlag ? "Yes" : "No") + "</td>" +
                                  "<td>" + detail.Remarks + "</td>" +
                                  "<td>" + leaveAppDto.Remarks + "</td>" +
                                  "</tr>";

                bodyTable += bodyLine;
            }

            bodyTable += "</tbody>" +
                         "</table> <br/>";
            body += bodyTable;

            body += "<br/>Kindly review the same in <a href='" + ConfigurationManager.AppSettings["PortalAddress"] +
                    "'>ESS Portal</a>.";

            body += "</body></html>";

            SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPClient"], 25)
            {
                UseDefaultCredentials = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false
            };

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["MailAddress"], "ESS Portal"),
                Subject = "Notification from ESS Portal",
                BodyEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = true,
                Body = body
            };


            //now loop through all app release lines -- there'll be only one line, but anyway...
            foreach (var dto in leaveAppDto.ApplReleaseStatus)
            {
                //find the releaser from the releasecode
                var releasers = _context.ReleaseAuth
                    .Where(r => r.ReleaseCode == dto.ReleaseCode).ToList();


                //in case of multiple releasers for single release code,
                foreach (var r in releasers)
                {
                    // get releaser employee object
                    var releaser = _context.Employees.SingleOrDefault(e => e.EmpUnqId == r.EmpUnqId);

                    if (releaser == null)
                        continue;

                    if (string.IsNullOrEmpty(releaser.Email))
                        continue;

                    //return BadRequest("Email not maintained for releaser " + releaser.EmpUnqId + " - " + releaser.EmpName);

                    mail.To.Add(new MailAddress(releaser.Email));
                    smtpClient.Send(mail);
                    mail.To.Remove(new MailAddress(releaser.Email));
                }
            }

            return Ok();
        }

        [HttpGet]
        public IHttpActionResult SendMailNoDues(string releaseGroupCode, string empUnqId, string dept)
        {
            string[] emails;

            // record created, send mail to dept head for intimation
            if (dept.ToUpper() == "HOD")
            {
                // get list of releaers
                var relCodes = _context.NoDuesReleaseStatus
                    .Where(e => e.EmpUnqId == empUnqId)
                    .Select(e => e.ReleaseCode).ToArray();

                var releasers = _context.ReleaseAuth
                    .Where(r => relCodes.Contains(r.ReleaseCode))
                    .Select(e => e.EmpUnqId).ToArray();

                emails = _context.Employees
                    .Where(e => releasers.Contains(e.EmpUnqId))
                    .Select(e => e.Email)
                    .ToArray();
            }
            else
            {
                // Dept head has released, intimate all other GSS depts
                var releasers = _context.NoDuesReleaser
                    .Select(e => e.EmpUnqId).ToArray();
                emails = _context.Employees
                    .Where(e => releasers.Contains(e.EmpUnqId))
                    .Select(e => e.Email)
                    .ToArray();
            }

            if (emails.Length == 0)
                return BadRequest("No emails found to send...");


            Employees emp = _context.Employees
                .Include(e => e.Departments)
                .Include(e => e.Stations)
                .FirstOrDefault(e => e.EmpUnqId == empUnqId);

            if (emp == null)
                return BadRequest("Employee not found!");

            NoDuesMaster noDuesMaster = _context.NoDuesMaster
                .FirstOrDefault(e => e.EmpUnqId == empUnqId);

            if (noDuesMaster == null)
                return BadRequest("No Dues master not found!");


            const string header = @"
                <html lang=""en"">
                    <head>    
                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                        <title>
                            ESS Portal - Automessage
                        </title>
                        <style type=""text/css"">
                            body { font-family: arial, sans-serif; }
                            table {
                                font-family: arial, sans-serif;
                                border-collapse: collapse;
                                width: 80%;
                            }

                            td, th {
                                border: 1px solid #dddddd;
                                text-align: left;
                                padding: 8px;
                            }

                            tr:nth-child(even) {
                                background-color: #dddddd;
                            }
                        </style>
                    </head>
                    <body>
                ";

            string body = "Dear Sir, <br /><br /> " +
                          "Following No Dues/outstanding certification application requires your attention: <br/> <br />" +
                          "Employee Code: " + emp.EmpUnqId + " <br/>" +
                          "Name: " + emp.EmpName + " <br/>" +
                          "Dept/Station: " + emp.Departments.DeptName + "/" + emp.Stations.StatName + " <br/></br>";

            body = header + body;

            body += "The same will be available to you from " +
                    noDuesMaster.NoDuesStartDate.Date.ToString("dd/MM/yyyy");

            body += "<br/>Kindly review the same in <a href='" + ConfigurationManager.AppSettings["PortalAddress"] +
                    "'>ESS Portal</a>.";

            body += "</body></html>";
            try
            {
                var smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPClient"], 25)
                {
                    //Credentials = new System.Net.NetworkCredential("tilaka@jindalsaw.com", "ashish123$$"),
                    UseDefaultCredentials = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = false
                };


                var mail = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["MailAddress"], "ESS Portal"),
                    //From = new MailAddress("attnd.nkp@jindalsaw.com", "ESS Portal"),
                    Subject = "Notification from ESS Portal for No Dues certification",
                    BodyEncoding = System.Text.Encoding.UTF8,
                    IsBodyHtml = true,
                    Body = body
                };

                foreach (string email in emails)
                {
                    mail.To.Add(new MailAddress(email));
                    if (emp.Location == Locations.Ipu)
                    {
                        mail.CC.Add("vallabh.r@jindalsaw.com");
                        mail.CC.Add("mandeep.singh@jindalsaw.com");
                        mail.CC.Add("vinaykumar.singh@jindalsaw.com");
                        mail.CC.Add("careers.ipu@jindalsaw.com");
                        mail.CC.Add("raghuvir.jadeja@jindalsaw.com");
                        mail.Bcc.Add("mohit.parmar@jindalsaw.com");
                        mail.Bcc.Add("ashish.tilak@jindalsaw.com");
                    }

                    smtpClient.Send(mail);
                    mail.To.Remove(new MailAddress(email));
                    if (emp.Location == Locations.Ipu)
                    {
                        mail.CC.Remove(new MailAddress("vallabh.r@jindalsaw.com"));
                        mail.CC.Remove(new MailAddress("mandeep.singh@jindalsaw.com"));
                        mail.CC.Remove(new MailAddress("vinaykumar.singh@jindalsaw.com"));
                        mail.CC.Remove(new MailAddress("careers.ipu@jindalsaw.com"));
                        mail.CC.Remove(new MailAddress("raghuvir.jadeja@jindalsaw.com"));
                        mail.Bcc.Remove(new MailAddress("mohit.parmar@jindalsaw.com"));
                        mail.Bcc.Remove(new MailAddress("ashish.tilak@jindalsaw.com"));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex);
            }

            return Ok();
        }

        [HttpGet]
        public IHttpActionResult SendMailResignation(int id, string furtherReleaser, string empUnqId)
        {
            if (furtherReleaser.ToUpper() == "TRUE" && string.IsNullOrEmpty(empUnqId))
                return BadRequest("Provide further releaser employee id.");

            EmpSeparation resign = _context.EmpSeparations.FirstOrDefault(r => r.Id == id);

            if (resign == null)
                return BadRequest("Invalid resignation id.");

            if (resign.StatusHr)
                return BadRequest("Resignation already approved by HR.");

            string empName = _context.Employees.FirstOrDefault(e => e.EmpUnqId == resign.EmpUnqId)?.EmpName;
            string deptStat = _context.Stations.FirstOrDefault(s => s.CompCode == resign.Employee.CompCode &&
                                                                    s.WrkGrp == resign.Employee.WrkGrp &&
                                                                    s.UnitCode == resign.Employee.UnitCode &&
                                                                    s.DeptCode == resign.Employee.DeptCode &&
                                                                    s.StatCode == resign.Employee.StatCode)?.StatName;

            const string header = @"
                <html lang=""en"">
                    <head>    
                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                        <title>
                            ESS Portal - Automessage
                        </title>
                        <style type=""text/css"">
                            body { font-family: arial, sans-serif; }
                            table {
                                font-family: arial, sans-serif;
                                border-collapse: collapse;
                                width: 80%;
                            }

                            td, th {
                                border: 1px solid #dddddd;
                                text-align: left;
                                padding: 8px;
                            }

                            tr:nth-child(even) {
                                background-color: #dddddd;
                            }
                        </style>
                    </head>
                    <body>
                ";

            string body = "Dear Sir, <br /><br /> " +
                          "Following employee resignation requires your attention: <br/> <br />" +
                          "Employee Code: " + resign.EmpUnqId + " <br/>" +
                          "Employee Name: " + empName + " <br/>" +
                          "Dept/Station: " + deptStat + " <br/>" +
                          "Resign date:" + resign.ApplicationDate.ToString("dd/MM/yyyy") + " <br/>" +
                          "Relieve date:" + resign.RelieveDate.ToString("dd/MM/yyyy") + "<br/><br/>";

            body = header + body;

            body += "<br/>Kindly review the same in <a href='" + ConfigurationManager.AppSettings["PortalAddress"] +
                    "'>ESS Portal</a>.";

            body += "</body></html>";


            if (furtherReleaser.ToUpper() == "FALSE")
            {
                List<ApplReleaseStatus> app = _context.ApplReleaseStatus
                    .Where(l =>
                        l.ReleaseGroupCode == ReleaseGroups.NoDues &&
                        l.ApplicationId == id
                    ).ToList();

                if (app.Count == 0)
                    return BadRequest("App release record not found.");

                string[] relcodes = app.Select(r => r.ReleaseCode).ToArray();

                ReleaseAuth relAuth = _context.ReleaseAuth
                    .SingleOrDefault(r =>
                        relcodes.Contains(r.ReleaseCode) &&
                        r.EmpUnqId == empUnqId);
                if (relAuth == null)
                    return BadRequest("Release auth not found.");

                var smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPClient"], 25)
                {
                    UseDefaultCredentials = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = false
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["MailAddress"], "ESS Portal"),
                    Subject = "Notification from ESS Portal for Employee Resignation",
                    BodyEncoding = System.Text.Encoding.UTF8,
                    IsBodyHtml = true,
                    Body = body
                };

                List<ReleaseAuth> releasers = _context.ReleaseAuth
                    .Where(r => r.ReleaseCode == relAuth.ReleaseCode).ToList();

                //in case of multiple releasers for single release code,
                foreach (Employees releaser in
                    releasers.Select(r =>
                            _context.Employees.SingleOrDefault(e => e.EmpUnqId == r.EmpUnqId))
                        .Where(releaser => releaser != null)
                        .Where(releaser => !string.IsNullOrEmpty(releaser.Email)))
                {
                    mail.To.Add(new MailAddress(releaser.Email));
                    smtpClient.Send(mail);
                    mail.To.Remove(new MailAddress(releaser.Email));
                }

                return Ok();
            }
            else if(furtherReleaser.ToUpper() == "TRUE") // for Further releasers
            {
                if (!resign.FurtherReleaseRequired)
                    return BadRequest("Further release is not required for this resignation.");

                var smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPClient"], 25)
                {
                    UseDefaultCredentials = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = false
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["MailAddress"], "ESS Portal"),
                    Subject = "Notification from ESS Portal for Employee Resignation",
                    BodyEncoding = System.Text.Encoding.UTF8,
                    IsBodyHtml = true,
                    Body = body
                };


                string releaserMail = _context.Employees.FirstOrDefault(e => e.EmpUnqId == empUnqId)?.Email;

                if (string.IsNullOrEmpty(releaserMail)) return BadRequest("Further releaser mail id not found.");

                mail.To.Add(new MailAddress(releaserMail));
                smtpClient.Send(mail);
                mail.To.Remove(new MailAddress(releaserMail));

                return Ok();
            }
            else if(furtherReleaser.ToUpper() == "HR")
            {
                var smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPClient"], 25)
                {
                    UseDefaultCredentials = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = false
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["MailAddress"], "ESS Portal"),
                    Subject = "Notification from ESS Portal for Employee Resignation",
                    BodyEncoding = System.Text.Encoding.UTF8,
                    IsBodyHtml = true,
                    Body = body
                };

                // list of hr mail ids
                var releaserMail = new []
                {
                    "vallabh.r@jindalsaw.com",
                    "mandeep.singh@jindalsaw.com",
                    "navin.bhandari@jindalsaw.com",
                    "careers.ipu@jindalsaw.com",
                    "er.ipu@jindalsaw.com",
                    "raghuvir.jadeja@jindalsaw.com",
                    "rajkumar.mehta@jindalsaw.com",
                    "YERI.MEGHARAJA@jindalsaw.com",

                };

                foreach (string s in releaserMail)
                {
                    mail.To.Add(new MailAddress(s));
                }

                mail.CC.Add(new MailAddress("vinaykumar.singh@jindalsaw.com"));
                mail.Bcc.Add(new MailAddress("ashish.tilak@jindalsaw.com"));
                mail.Bcc.Add(new MailAddress("mohit.parmar@jindalsaw.com"));
                
                smtpClient.Send(mail);

                return Ok();
            }
            else
            {
                return BadRequest("Invalid furtherreleaser flag.");
            }
        }

        [HttpGet]
        public IHttpActionResult SendMailVehReq(int reqId, bool flag)
        {
            var vehReq = _context.VehicleReqs.FirstOrDefault(v=>v.ReqId == reqId);
            if(vehReq == null)
                return BadRequest("Invalid req id.");

            //// FILL EMP DETAILS....
            Employees emp = _context.Employees
                .Include(d => d.Departments)
                .Include(s => s.Stations)
                .FirstOrDefault(e => e.EmpUnqId == vehReq.EmpUnqId);

            if(emp == null)
                return BadRequest("Invalid empunqid in vehicle req.");


            string empCode = vehReq.EmpUnqId;
            string empName = emp.EmpName;
            string deptstat = emp.Departments.DeptName + " / " + emp.Stations.StatName;

            const string header = @"
                <html lang=""en"">
                    <head>    
                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                        <title>
                            ESS Portal - Automessage
                        </title>
                        <style type=""text/css"">
                            body { font-family: arial, sans-serif; }
                            table {
                                font-family: arial, sans-serif;
                                border-collapse: collapse;
                                width: 80%;
                            }

                            td, th {
                                border: 1px solid #dddddd;
                                text-align: left;
                                padding: 8px;
                            }

                            tr:nth-child(even) {
                                background-color: #dddddd;
                            }
                        </style>
                    </head>
                    <body>
                ";

            string body = "Dear Sir, <br /><br /> " +
                          "Following Vehicle Requisition requires your attention: <br/> <br />" +
                          "Requisition for: " + empCode + " <br/>" +
                          "Name: " + empName + " <br/>" +
                          "Dept/Station: " + deptstat + " <br/></br>" + 
                          "Requisition date: " + vehReq.BookingDate.Date.ToString("dd/MM/yyyy") + " <br/>" +
                          "Pick up location: " + vehReq.PickupLocation + " <br/> " +
                          "Drop location: " + vehReq.DropLocation + "<br/> " +
                          "Passengers: " + vehReq.NumberOfPass + "<br/><br/>";

            body = header + body;

            body += "<br/>Kindly review the same in <a href='" + ConfigurationManager.AppSettings["PortalAddress"] +
                    "'>ESS Portal</a>.";

            body += "</body></html>";

            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPClient"], 25)
            {
                //Credentials = new System.Net.NetworkCredential("tilaka@jindalsaw.com", "ashish123$$"),
                UseDefaultCredentials = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false
            };


            var mail = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["MailAddress"], "ESS Portal"),
                //From = new MailAddress("attnd.nkp@jindalsaw.com", "ESS Portal"),
                Subject = "Notification from ESS Portal for Vehicle requisition release.",
                BodyEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = true,
                Body = body
            };

            if (flag)
            {
                //now loop through all app release lines -- there'll be only one line, but anyway...
                string relCode = vehReq.ReleaseCode;

                List<ReleaseAuth> releasers = _context.ReleaseAuth
                    .Where(r => r.ReleaseCode == relCode).ToList();

                //in case of multiple releasers for single release code,
                foreach (Employees releaser in
                         releasers.Select(r =>
                                 _context.Employees.SingleOrDefault(e => e.EmpUnqId == r.EmpUnqId))
                             .Where(releaser => releaser != null).Where(releaser => !string.IsNullOrEmpty(releaser.Email)))
                {
                    mail.To.Add(new MailAddress(releaser.Email));
                    smtpClient.Send(mail);
                    mail.To.Remove(new MailAddress(releaser.Email));
                } 
            }
            else
            {
                var releaserMail = new []
                {
                    "randhir.kumar@jindalsaw.com",
                    "ravinder.singh@jindalsaw.com",
                    "vehicle.ipu@jindalsaw.com",
                };

                foreach (string s in releaserMail)
                {
                    mail.To.Add(new MailAddress(s));
                }
                smtpClient.Send(mail);
            }

            return Ok();
        }
    }
}