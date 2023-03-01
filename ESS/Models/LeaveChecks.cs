using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class LeaveChecks
    {
        [Key] public int Id {get;set;}

        [StringLength(2)] public string LeaveTaken { get; set; }

        [StringLength(2)] public string LeaveCheck { get; set; }

        public bool IsAllowed { get; set; }
        public float DaysAllowed { get; set; }
        public bool Active { get; set; }
    }
}