using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class NoDuesStatus
    {
        [Key] [StringLength(10)] public string EmpUnqId { get; set; }
        public bool Hod { get; set; }
        public bool Finance { get; set; }
        public bool Stores { get; set; }
        public bool Admin { get; set; }
        public bool Cafeteria { get; set; }
        public bool Hr { get; set; }
        public bool PrgHr { get; set; }
        public bool Township { get; set; }
        public bool EandI { get; set; }
        public bool It { get; set; }
        public bool Security { get; set; }
        public bool Safety { get; set; }
        public bool Ohc { get; set; }
        public bool School { get; set; }
        public bool Er { get; set; }
        public bool UnitHead { get; set; }
    }
}