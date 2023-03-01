using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class ShiftScheduleDetails
    {
        [Key, Column(Order = 0)] public int YearMonth { get; set; }

        [Key, Column(Order = 1)] public int ScheduleId { get; set; }

        [Key, Column(Order = 2)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [ForeignKey("YearMonth, ScheduleId, EmpUnqId")]
        public ShiftSchedules ShiftSchedule { get; set; }

        [Key, Column(Order = 3)] public int ShiftDay { get; set; } //Should be 1 to 31 max

        [StringLength(2)] public string ShiftCode { get; set; }

        [ForeignKey("ShiftCode")] public Shifts Shift { get; set; }
    }
}