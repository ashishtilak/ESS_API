using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class TpaRelease
    {
        [Key] public int Id { get; set; }

        [StringLength(2)] public string ReleaseGroupCode { get; set; }
        [ForeignKey("ReleaseGroupCode")] public ReleaseGroups ReleaseGroup { get; set; }

        public int TpaSanctionId { get; set; }

        [StringLength(15)] public string ReleaseStrategy { get; set; }

        public int ReleaseStrategyLevel { get; set; }

        [StringLength(20)] public string ReleaseCode { get; set; }

        [StringLength(1)] public string PreReleaseStatusCode { get; set; }
        [ForeignKey("PreReleaseStatusCode")] public ReleaseStatus PreReleaseStatus { get; set; }

        public DateTime? PreReleaseDate { get; set; }

        [StringLength(10)] public string PreReleaseAuth { get; set; }

        public bool IsFinalRelease { get; set; }

        [StringLength(255)] public string PreRemarks { get; set; }

        // POST
        [StringLength(1)] public string PostReleaseStatusCode { get; set; }
        [ForeignKey("PostReleaseStatusCode")] public ReleaseStatus PostReleaseStatus { get; set; }

        [StringLength(10)] public string PostReleaseAuth { get; set; }

        public DateTime? PostReleaseDate { get; set; }
        [StringLength(255)] public string PostRemarks { get; set; }
    }
}