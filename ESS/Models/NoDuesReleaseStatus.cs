using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class NoDuesReleaseStatus
    {
        [Key, Column(Order = 0)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [Key, Column(Order = 1)]
        [Required, StringLength(2)]
        public string ReleaseGroupCode { get; set; }

        [ForeignKey("ReleaseGroupCode")] public ReleaseGroups ReleaseGroup { get; set; }

        [Key, Column(Order = 3), StringLength(15)]
        public string ReleaseStrategy { get; set; }

        [Key, Column(Order = 4)] public int ReleaseStrategyLevel { get; set; }

        [Required] [StringLength(20)] public string ReleaseCode { get; set; }

        [Required] [StringLength(1)] public string ReleaseStatusCode { get; set; }

        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [StringLength(10)] public string ReleaseAuth { get; set; }

        public bool IsFinalRelease { get; set; }

        [StringLength(255)] public string Remarks { get; set; }
    }
}