using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class MissedPunch
    {
        [Key] public int Id { get; set; }
        public DateTime AddDate { get; set; }

        [StringLength(10)] public string EmpUnqId { get; set; }
        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        [StringLength(50)] public string Reason { get; set; }

        [StringLength(255)] public string Remarks { get; set; }

        [StringLength(15)] public string ReleaseStrategy { get; set; }

        [StringLength(1)] public string ReleaseStatusCode { get; set; }

        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }

        public DateTime? InTime { get; set; }

        [StringLength(10)] public string InTimeUser { get; set; }

        public DateTime? OutTime { get; set; }

        [StringLength(10)] public string OutTimeUser { get; set; }

        public bool PostingFlag { get; set; }

        public ICollection<MissedPunchReleaseStatus> MissedPunchReleaseStatus { get; set; }

        [StringLength(1)] public string PunchType { get; set; }     // A for attd, M for In/Out missed
        [StringLength(1)] public string InOutFlag { get; set; }     // I for in, O for out
        public DateTime? AttdPunchTime { get; set; }

        public static readonly string AttdPunch = "A";
        public static readonly string InOutPunch = "M";

    }
}