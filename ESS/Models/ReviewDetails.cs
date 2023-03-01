using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class ReviewDetails
    {
        [Key, Column(Order = 0)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        [Key, Column(Order = 1)] public int ReviewQtrNo { get; set; }
        public bool IsConfirmation { get; set; }

        [Column(TypeName = "date")] public DateTime ReviewDate { get; set; }
        [Column(TypeName = "date")] public DateTime PeriodFrom { get; set; }
        [Column(TypeName = "date")] public DateTime PeriodTo { get; set; }

        [StringLength(255)] public string Assignments { get; set; }
        [StringLength(255)] public string Strength { get; set; }
        [StringLength(255)] public string Improvements { get; set; }
        [StringLength(255)] public string Suggestions { get; set; }

        [StringLength(1)] public string Rating { get; set; } //

        [StringLength(100)] public string Remarks { get; set; }
        [StringLength(1)] public string Recommendation { get; set; } // N, C, E, T

        public static readonly string NotProcessed = "N";
        public static readonly string Confirm = "C";
        public static readonly string Extend = "E";
        public static readonly string Terminate = "T";


        // FIRST LEVEL RELEASER DETAILS
        public DateTime? AddDt { get; set; }
        [StringLength(20)] public string AddReleaseCode { get; set; }
        [StringLength(10)] public string AddUser { get; set; }
        [StringLength(1)] public string AddReleaseStatusCode { get; set; }
        [ForeignKey("AddReleaseStatusCode")] public ReleaseStatus AddReleaseStatus { get; set; }


        [StringLength(2)] public string ReleaseGroupCode { get; set; }
        [ForeignKey("ReleaseGroupCode")] public ReleaseGroups ReleaseGroup { get; set; }
        [StringLength(15)] public string ReleaseStrategy { get; set; }

        // STORE ONLY SECOND LEVEL RELEASER DETAILS
        [StringLength(20)] public string ReleaseCode { get; set; }
        public DateTime? ReleaseDate { get; set; }
        [StringLength(1)] public string ReleaseStatusCode { get; set; }
        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }
        [StringLength(100)] public string HodRemarks { get; set; }

        [StringLength(10)] public string HrUser { get; set; }
        public DateTime? HrReleaseDate { get; set; }
        [StringLength(1)] public string HrReleaseStatusCode { get; set; }
        [ForeignKey("HrReleaseStatusCode")] public ReleaseStatus HrReleaseStatus { get; set; }
        [StringLength(100)] public string HrRemarks { get; set; }
    }
}