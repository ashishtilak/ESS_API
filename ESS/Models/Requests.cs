using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class Requests
    {
        [Key, Column(Order = 0)] public int RequestId { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? RequestDate { get; set; }

        [StringLength(255)] public string Remarks { get; set; }

        [StringLength(2)] public string ReleaseGroupCode { get; set; }

        [ForeignKey("ReleaseGroupCode")] public ReleaseGroups ReleaseGroup { get; set; }

        [StringLength(15)] public string ReleaseStrategy { get; set; }

        [ForeignKey("ReleaseGroupCode, ReleaseStrategy")]
        public ReleaseStrategies RelStrategy { get; set; }

        [StringLength(1)] public string ReleaseStatusCode { get; set; }

        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? AddDt { get; set; }

        [StringLength(10)] public string AddUser { get; set; }

        [StringLength(1)] public string IsPosted { get; set; }
        [StringLength(10)] public string PostUser { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? PostedDt { get; set; }

        public ICollection<RequestDetails> RequestDetails { get; set; }
        public ICollection<RequestRelease> RequestReleases { get; set; }
    }
}