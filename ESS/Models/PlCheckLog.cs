using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class PlCheckLog
    {
        [Key] public int Id { get; set; }
        public DateTime UpdateDate { get; set; }
        [StringLength(10)] public string EmpUnqId { get; set; }
        public bool OldValue { get; set; }
        public bool NewValue { get; set; }
    }
}