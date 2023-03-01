using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class ManageLeaveController : ApiController
    {
        private ApplicationDbContext _context;

        public ManageLeaveController()
        {
            _context = new ApplicationDbContext();
        }


        [HttpPost]
        public IHttpActionResult ManageLeave([FromBody] object requestData, string empUnqId)
        {
            List<string> error = new List<string>();

            LeaveApplicationDetailDto changedLeave;
            try
            {
                changedLeave = JsonConvert.DeserializeObject<LeaveApplicationDetailDto>(requestData.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            var lDto = _context.LeaveApplicationDetails
                .FirstOrDefault(
                    l => l.YearMonth == changedLeave.YearMonth &&
                         l.LeaveAppId == changedLeave.LeaveAppId &&
                         l.LeaveAppItem == changedLeave.LeaveAppItem);

            if (lDto == null)
                return BadRequest("Invalid leave details. Pl chck in Db.");


            //Check half day totaldays
            lDto.TotalDays = changedLeave.HalfDayFlag ? 0.5f : (changedLeave.ToDt - changedLeave.FromDt).Days + 1;

            var emp = _context.Employees.FirstOrDefault(e => e.EmpUnqId == empUnqId);
            if (emp == null)
                return BadRequest("Invalid employee. Pl chck in Db.");

            //Get list of holidays
            List<DateTime> holidays =
                ESS.Helpers.CustomHelper.GetHolidays(changedLeave.FromDt, changedLeave.ToDt, lDto.CompCode, lDto.WrkGrp,
                    emp.Location);

            lDto.TotalDays -= holidays.Count;

            if (Math.Abs(lDto.TotalDays) < 0)
                error.Add("You cannot take " + lDto.LeaveTypeCode + " on a holiday.");

            // Get weekly offs between the selected range
            List<DateTime> weekOffs =
                ESS.Helpers.CustomHelper.GetWeeklyOff(changedLeave.FromDt, changedLeave.ToDt, empUnqId);


            //Check if weekoff is on holiday. If it is, then remove it.
            if (holidays.Count > 0)
            {
                weekOffs.RemoveAll(w => holidays.Any(h => h == w));
            }

            lDto.TotalDays -= weekOffs.Count;

            if (Math.Abs(lDto.TotalDays) < 0)
                error.Add("You cannot take " + lDto.LeaveTypeCode + " on weekly off day.");


            //If there are errors, return error object
            if (error.Count > 0)
                return Content(HttpStatusCode.BadRequest, error);


            //Modify current Db detail object from change request object
            lDto.FromDt = changedLeave.FromDt;
            lDto.ToDt = changedLeave.ToDt;
            lDto.LeaveTypeCode = changedLeave.LeaveTypeCode;
            lDto.HalfDayFlag = changedLeave.HalfDayFlag;
            lDto.Remarks = changedLeave.Remarks;

            _context.SaveChanges();

            return Ok();
        }
    }
}