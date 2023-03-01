using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class Materials
    {
        [Key] [StringLength(16)] public string MaterialCode { get; set; }

        [StringLength(50)] public string MaterialDesc { get; set; }

        [StringLength(5)] public string Uom { get; set; }

        [StringLength(10)] public string HsnCode { get; set; }

        public DateTime? UpdDt { get; set; }
        [StringLength(10)] public string UpdUser { get; set; }
    }
}