using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class ShiftScheduleDetailDto
    {
        public int YearMonth { get; set; }
        public int ScheduleId { get; set; }
        public int ShiftDay { get; set; } //Should be 1 to 31 max
        public string ShiftCode { get; set; }
        public ShiftDto Shift { get; set; }
    }
}