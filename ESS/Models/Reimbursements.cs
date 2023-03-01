using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class Reimbursements
    {
        [Key, Column(Order = 0)] public int YearMonth { get; set; } //201920
        [Key, Column(Order = 1)] public int ReimbId { get; set; }

        [StringLength(3)] public string ReimbType { get; set; }
        [StringLength(10)] public string EmpUnqId { get; set; }

        public DateTime ReimbDate { get; set; }

        public int PeriodFrom { get; set; }
        public int PeriodTo { get; set; }

        [StringLength(20)] public string InvoiceNo { get; set; }

        public float AmountClaimed { get; set; }
        public float AmountReleased { get; set; }
        [StringLength(50)] public string AmountReleaseRemarks { get; set; }

        [StringLength(10)] public string AddUser { get; set; }
        public DateTime AddDateTime { get; set; }

        [StringLength(2)] public string ReleaseGroupCode { get; set; }
        [ForeignKey("ReleaseGroupCode")] public ReleaseGroups ReleaseGroup { get; set; }

        [StringLength(15)] public string ReleaseStrategy { get; set; }

        [ForeignKey("ReleaseGroupCode, ReleaseStrategy")]
        public ReleaseStrategies RelStrategy { get; set; }

        [StringLength(1)] public string ReleaseStatusCode { get; set; }
        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }

        [StringLength(255)] public string Remarks { get; set; }

        public const string ConveyenceReimb = "CON";
        public const string CarReimb = "CAR";
        public const string TelephoneReimb = "TEL";

        public ICollection<ApplReleaseStatus> ApplReleaseStatus { get; set; }
        public ICollection<ReimbConv> ReimbConv { get; set; }
    }
}