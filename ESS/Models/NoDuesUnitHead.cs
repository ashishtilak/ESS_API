using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class NoDuesUnitHead
    {
        [Key, Column(Order = 0)]
        [StringLength(2)]
        public string CompCode { get; set; }

        [ForeignKey("CompCode")] public Company Company { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(10)]
        public string WrkGrp { get; set; }

        [ForeignKey("CompCode, WrkGrp")] public WorkGroups WorkGroup { get; set; }

        [Key, Column(Order = 2)]
        [StringLength(3)]
        public string UnitCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, UnitCode")]
        public Units Units { get; set; }

        [Key, Column(Order = 3)]
        [StringLength(3)]
        public string DeptCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, UnitCode, DeptCode")]
        public Departments Departments { get; set; }

        [StringLength(2)] public string DeptLoB { get; set; } //LD or DI

        [StringLength(10)] public string UnitHead { get; set; }
    }
}