using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class MedicalFitness
    {
        [Key] public int Id { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? TestDate { get; set; }

        [StringLength(10)] public string EmpUnqId { get; set; }
        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? CardBlockedOn { get; set; }
        public int CardBlockedDays { get; set; }

        [StringLength(50)] public string CardBlockedReason { get; set; }

        public bool FitnessFlag { get; set; } // True = fit, False = Unfit

        [StringLength(50)] public string Remarks { get; set; }

        [Column(TypeName = "datetime2")] public DateTime AddDt { get; set; }
        [StringLength(10)] public string AddUser { get; set; }
    }
}