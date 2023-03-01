using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class ReimbConv
    {
        [Key, Column(Order = 0)] public int YearMonth { get; set; } //201920
        [Key, Column(Order = 1)] public int ReimbId { get; set; }
        [Key, Column(Order = 2)] public int Sr { get; set; }

        [StringLength(3)] public string ReimbType { get; set; }
        [StringLength(10)] public string EmpUnqId { get; set; }

        public DateTime ConvDate { get; set; }
        [StringLength(15)] public string VehicleNo { get; set; }
        [StringLength(100)] public string Particulars { get; set; }
        public int MeterFrom { get; set; }
        public int Distance { get; set; }
        public int MeterTo { get; set; }
        public float Rate { get; set; }
        public float Amount { get; set; }
        [StringLength(20)] public string Remarks { get; set; }
    }
}