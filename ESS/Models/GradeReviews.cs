using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class GradeReviews
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
        public string GradeCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, GradeCode")]
        public Grades Grades { get; set; }

        public int ReviewQtr { get; set; }
    }
}