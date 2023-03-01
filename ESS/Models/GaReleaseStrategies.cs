using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.Models
{
    public class GaReleaseStrategies
    {
        public List<GaReleaseStrategyLevels> GaReleaseStrategyLevels;

        public bool Active { get; set; }

        [ForeignKey("CompCode")] public Company Company { get; set; }

        [StringLength(2)] public string CompCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, UnitCode, DeptCode")]
        public Departments Departments { get; set; }

        [Required] [StringLength(3)] public string DeptCode { get; set; }

        [Key, Column(Order = 1)]
        [Required]
        [StringLength(15)]
        public string GaReleaseStrategy { get; set; }

        [StringLength(100)] public string GaReleaseStrategyName { get; set; }

        public ReleaseGroups ReleaseGroup { get; set; }

        [Key, Column(Order = 0)]
        [Required]
        [StringLength(15)]
        public string ReleaseGroupCode { get; set; }

        [Required] [StringLength(3)] public string StatCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, UnitCode, DeptCode, StatCode")]
        public Stations Stations { get; set; }

        [StringLength(3)] public string UnitCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, UnitCode")]
        public Units Units { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? UpdDt { get; set; }

        [StringLength(10)] public string UpdUser { get; set; }

        [ForeignKey("CompCode, WrkGrp")] public WorkGroups WorkGroup { get; set; }

        [StringLength(10)] public string WrkGrp { get; set; }
    }
}