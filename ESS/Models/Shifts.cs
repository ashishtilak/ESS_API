using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class Shifts
    {
        [Key] [StringLength(2)] public string ShiftCode { get; set; }

        [StringLength(50)] public string ShiftDesc { get; set; }

        public TimeSpan? ShiftStart { get; set; }
        public TimeSpan? ShiftEnd { get; set; }
    }
}