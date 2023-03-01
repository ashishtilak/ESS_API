using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class LeaveEntryDto
    {
        public int YearMonth { get; set; }
        public string EmpUnqId { get; set; }
        public string CompCode { get; set; }
        public string WrkGrp { get; set; }
        public string LeaveTypeCode { get; set; }
        public DateTime FromDt { get; set; }
        public DateTime ToDt { get; set; }
        public bool HalfDayFlag { get; set; }
        public float TotalDays { get; set; }
        public float LeaveDed { get; set; }
    }
}