using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class FullAndFinal
    {
        [Key] [StringLength(10)] public string EmpUnqId { get; set; }
        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        public DateTime? RelieveDate { get; set; }

        [StringLength(50)] public string DocumentNo { get; set; }
        public float? RecoveryAmount { get; set; }
        [StringLength(50)] public string CashDeposited { get; set; }
        [Column(TypeName = "date")] public DateTime? DepositDate { get; set; }

        [StringLength(255)] public string Remarks { get; set; }

        public bool GratuityFlag { get; set; }

        [StringLength(10)] public string AddUser { get; set; }
        [Column(TypeName = "datetime2")] public DateTime AddDate { get; set; }
    }
}