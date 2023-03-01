using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ESS.Dto;

namespace ESS.Controllers.Api
{
    public class HolidayController : ApiController
    {
        public IHttpActionResult GetHolidays(string compCode, string wrkGrp, int tYear, string location,
            bool optionalFlg)
        {
            if (!optionalFlg)
            {
                try
                {
                    List<HolidayDto> holidays = Helpers.CustomHelper.GetHolidays(compCode, wrkGrp, tYear, location);
                    return Ok(holidays);
                }
                catch (Exception e)
                {
                    return BadRequest(e.ToString());
                }
            }
            else
            {
                try
                {
                    List<HolidayDto> holidays = Helpers.CustomHelper.GetOptionalHolidays(tYear, location);
                    return Ok(holidays);
                }
                catch (Exception e)
                {
                    return BadRequest(e.ToString());
                }
            }
        }


        public IHttpActionResult GetHolidays(DateTime fromDt, DateTime toDt, string compCode, string wrkGrp,
            string location)
        {
            try
            {
                var holidays = Helpers.CustomHelper.GetHolidays(fromDt, toDt, compCode, wrkGrp, location);
                return Ok(holidays);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}