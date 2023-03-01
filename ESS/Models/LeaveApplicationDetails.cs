using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class LeaveApplicationDetails
    {
        [Key, Column(Order = 0)] public int YearMonth { get; set; }

        [Key, Column(Order = 1)] public int LeaveAppId { get; set; }

        [ForeignKey("YearMonth, LeaveAppId")] public LeaveApplications LeaveApplication { get; set; }

        [Key, Column(Order = 2)] public int LeaveAppItem { get; set; }

        [StringLength(2)] public string CompCode { get; set; }

        [ForeignKey("CompCode")] public Company Company { get; set; }

        [StringLength(10)] public string WrkGrp { get; set; }

        [ForeignKey("CompCode, WrkGrp")] public WorkGroups WorkGroup { get; set; }

        public string LeaveTypeCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, LeaveTypeCode")]
        public LeaveTypes LeaveType { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? FromDt { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? ToDt { get; set; }

        [Required] public bool HalfDayFlag { get; set; }

        public float TotalDays { get; set; }

        [StringLength(1)] public string IsPosted { get; set; }

        [StringLength(255)] public string Remarks { get; set; }

        [StringLength(255)] public string PlaceOfVisit { get; set; }

        [StringLength(255)] public string ContactAddress { get; set; }

        public bool? Cancelled { get; set; }
        public int ParentId { get; set; }

        public bool? IsCancellationPosted { get; set; }

        [StringLength(10)] public string PostUser { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? PostedDt { get; set; }

        [StringLength(1)] public string CoMode { get; set; } // W, H, E
        public DateTime? CoDate1 { get; set; }
        public DateTime? CoDate2 { get; set; }

        [StringLength(255)] public string AdditionalRemarks { get; set; }

        public static readonly string NotPosted = "N";
        public static readonly string FullyPosted = "F";
        public static readonly string PartiallyPosted = "P";
        public static readonly string PostingRejected = "R";
        public static readonly string ForcefullyPosted = "O";
    }
}