using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class EmpSeparation
    {
        [Key] public int Id { get; set; }
        [StringLength(10)] public string EmpUnqId { get; set; }
        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }
        [Column(TypeName = "datetime2")] public DateTime ApplicationDate { get; set; }
        [StringLength(1)] public string Mode { get; set; }
        [StringLength(50)] public string Reason { get; set; }
        [StringLength(50)] public string ReasonOther { get; set; }
        [Column(TypeName = "date")] public DateTime RelieveDate { get; set; }
        [StringLength(1500)] public string ResignText { get; set; }

        [StringLength(1)] public string ReleaseStatusCode { get; set; }
        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }

        public bool FurtherReleaseRequired { get; set; }
        [StringLength(10)] public string FurtherReleaser { get; set; }
        [StringLength(1)] public string FurtherReleaseStatusCode { get; set; }

        [ForeignKey("FurtherReleaseStatusCode")]
        public ReleaseStatus FurtherReleaseStatus { get; set; }

        [Column(TypeName = "datetime2")] public DateTime FurtherReleaseDate { get; set; }

        public bool StatusHr { get; set; }
        [StringLength(10)] public string HrUser { get; set; }
        [Column(TypeName = "datetime2")] public DateTime HrStatusDate { get; set; }
    }
}