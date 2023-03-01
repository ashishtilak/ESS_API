using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.Models
{
    public class MedPolicy
    {
        [Key] [Column(Order = 0)] public int PolicyYear { get; set; } //202021

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string PolicyNumber { get; set; }

        [StringLength(50)] public string InsurerName { get; set; }

        [StringLength(1)] public string PolicyType { get; set; } //M-Mediclaim, G-GPA, T-GTLI

        [StringLength(2)] public string CompCode { get; set; }
        [ForeignKey("CompCode")] public Company Company { get; set; }

        [StringLength(10)] public string WrkGrp { get; set; }
        [ForeignKey("CompCode, WrkGrp")] public WorkGroups WorkGroup { get; set; }

        [StringLength(3)] public string UnitCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, UnitCode")]
        public Units Units { get; set; }

        [StringLength(50)] public string TpaName { get; set; }

        [StringLength(50)] public string ContactPerson { get; set; }

        [StringLength(20)] public string ContactNumber { get; set; }

        [StringLength(20)] public string AltContactNumber { get; set; }

        [StringLength(50)] public string ContactEmail { get; set; }

        [Column(TypeName = "datetime2")] public DateTime ValidFrom { get; set; }
        [Column(TypeName = "datetime2")] public DateTime ValidTo { get; set; }
    }
}