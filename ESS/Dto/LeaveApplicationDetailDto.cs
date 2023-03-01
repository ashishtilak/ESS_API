using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class LeaveApplicationDetailDto
    {
        public int YearMonth { get; set; }
        public int LeaveAppId { get; set; }
        public int LeaveAppItem { get; set; }
        public string CompCode { get; set; }
        public string WrkGrp { get; set; }
        public string LeaveTypeCode { get; set; }
        public DateTime FromDt { get; set; }
        public DateTime ToDt { get; set; }
        public bool HalfDayFlag { get; set; }
        public float TotalDays { get; set; }
        public string IsPosted { get; set; }

        public string Remarks { get; set; }
        public string PlaceOfVisit { get; set; }
        public string ContactAddress { get; set; }

        public bool Cancelled { get; set; }
        public int ParentId { get; set; }

        public string PostUser { get; set; }
        public DateTime? PostedDt { get; set; }

        public string CoMode { get; set; } // W, H, E
        public DateTime? CoDate1 { get; set; }
        public DateTime? CoDate2 { get; set; }

        public string AdditionalRemarks { get; set; }

        public bool IsCancellationPosted { get; set; }

        public LeaveTypeDto LeaveType { get; set; }
    }
}