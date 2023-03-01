using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.Models
{
    public class ReleaseStrategies
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

        [StringLength(100)] public string ReleaseStrategyName { get; set; }


        public bool IsHod { get; set; }

        public bool Active { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? UpdDt { get; set; }

        [StringLength(10)] public string UpdUser { get; set; }


        public List<ReleaseStrategyLevels> ReleaseStrategyLevels;
    }
}