using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class NoDuesDept
    {
        [Key, Column(Order = 0)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(3)]
        public string DeptId { get; set; }

        public bool NoDuesFlag { get; set; }

        [StringLength(20)] public string Remarks { get; set; }
        public bool? ApprovalFlag { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? ApprovalDate { get; set; }
        [StringLength(10)] public string ApprovedBy { get; set; }
    }
}