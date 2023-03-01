using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class EmpUniform
    {
        [Key, Column(Order = 0)] public int Year { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        public int TrouserSize { get; set; }
        public int ShirtSize { get; set; }

        public string UpdUser { get; set; }
        public DateTime UpdTime { get; set; }
    }
}