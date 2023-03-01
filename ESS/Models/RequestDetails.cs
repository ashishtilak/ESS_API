using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class RequestDetails
    {
        [Key, Column(Order = 0)] public int RequestId { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [Key, Column(Order = 2)] public int Sr { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? FromDt { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? ToDt { get; set; }

        [StringLength(2)] public string ShiftCode { get; set; }
        [ForeignKey("ShiftCode")] public Shifts Shift { get; set; }

        [StringLength(255)] public string Reason { get; set; }
    }
}