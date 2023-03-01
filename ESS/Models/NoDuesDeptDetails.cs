using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class NoDuesDeptDetails
    {
        [Key, Column(Order = 0)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(3)]
        public string DeptId { get; set; }

        [Key, Column(Order = 2)] public int Sr { get; set; }

        [StringLength(50)] public string Particulars { get; set; }
        [StringLength(200)] public string Remarks { get; set; }
        public float? Amount { get; set; }

        [StringLength(10)] public string AddUser { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? AddDate { get; set; }
    }
}