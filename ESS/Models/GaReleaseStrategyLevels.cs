using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class GaReleaseStrategyLevels
    {
        [Key, Column(Order = 0)]
        [Required]
        [StringLength(15)]
        public string ReleaseGroupCode { get; set; }

        public ReleaseGroups ReleaseGroup { get; set; }

        [Key, Column(Order = 1)]
        [Required]
        [StringLength(15)]
        public string GaReleaseStrategy { get; set; }

        [Key, Column(Order = 2)] [Required] public int GaReleaseStrategyLevel { get; set; }

        [Required] [StringLength(20)] public string ReleaseCode { get; set; }

        public bool IsFinalRelease { get; set; }

        [ForeignKey("ReleaseGroupCode, GaReleaseStrategy")]
        public GaReleaseStrategies GaReleaseStrategies { get; set; }
    }
}