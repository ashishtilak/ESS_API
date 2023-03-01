using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class LeaveRules
    {
        [Key] public int Id { get; set; }

        [StringLength(5)] public string Location { get; set; }

        [StringLength(2)] public string LeaveTypeCode { get; set; }

        [StringLength(50)] public string LeaveRule { get; set; }

        public bool LeaveAllowed { get; set; }
        public float DaysAllowed { get; set; }
        public bool Active { get; set; }
    }
}