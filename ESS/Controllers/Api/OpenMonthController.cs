using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ESS.Models;

namespace ESS.Controllers.Api
{
    public class OpenMonthController : ApiController
    {
        private ApplicationDbContext _context;

        public OpenMonthController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        public IHttpActionResult GetOpenMonth()
        {
            var openMonth = _context.OpenMonth.First();
            if (openMonth == null)
                return BadRequest("Open month is null. Pl check.");


            string year = openMonth.YearMonth.ToString().Substring(0, 4);
            string month = openMonth.YearMonth.ToString().Substring(4, 2);
            DateTime result = new DateTime();
            try
            {
                result = DateTime.ParseExact(string.Format("{0}-{1}-{2}", year, month, "01"), "yyyy-MM-dd", null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }


            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult ChangeOpenMonth(int yearMonth)
        {
            var currentMonth = _context.OpenMonth.First();

            //if (yearMonth < currentMonth.YearMonth)
            //    return BadRequest("New open month cannot be less than current open month.");

            currentMonth.OpenYear = int.Parse(yearMonth.ToString().Substring(0, 4));

            DateTime prevMonth = DateTime.ParseExact(
                String.Format("{0}-{1}-{2}", yearMonth.ToString().Substring(0, 4), yearMonth.ToString().Substring(4, 2),
                    "01"),
                "yyyy-MM-dd", null).AddMonths(-1);

            currentMonth.PrevMonth = int.Parse(prevMonth.Year.ToString() + prevMonth.Month.ToString("00"));
            currentMonth.YearMonth = yearMonth;
            //_context.SaveChanges();

            string sql = "UPDATE OpenMonths " +
                         "Set YearMonth = " + currentMonth.YearMonth + ", " +
                         "OpenYear = " + currentMonth.OpenYear + ", " +
                         "PrevMonth = " + currentMonth.PrevMonth;

            _context.Database.ExecuteSqlCommand(sql);

            return Ok();
        }
    }
}