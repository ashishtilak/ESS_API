using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class GpAdviceDetails
    {
        [Key, Column(Order = 0)] public int YearMonth { get; set; }
        [Key, Column(Order = 1)] public int GpAdviceNo { get; set; }
        [Key, Column(Order = 2)] public int GpAdviceItem { get; set; }

        [StringLength(20)] public string MaterialCode { get; set; }
        [StringLength(100)] public string MaterialDesc { get; set; }
        public float MaterialQty { get; set; }
        public float ApproxValue { get; set; }
        [StringLength(10)] public string HsnCode { get; set; }
    }
}