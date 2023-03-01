using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.Models
{
    public class ReleaseStrategyLevels
    {
        [Key, Column(Order = 0)]
        [Required]
        [StringLength(15)]
        public string ReleaseGroupCode { get; set; }

        public ReleaseGroups ReleaseGroup { get; set; }

        [Key, Column(Order = 1)]
        [Required]
        [StringLength(15)]
        public string ReleaseStrategy { get; set; }

        [Key, Column(Order = 2)] [Required] public int ReleaseStrategyLevel { get; set; }

        [Required] [StringLength(20)] public string ReleaseCode { get; set; }

        public bool IsFinalRelease { get; set; }

        [ForeignKey("ReleaseGroupCode, ReleaseStrategy")]
        public ReleaseStrategies ReleaseStrategies { get; set; }
    }
}