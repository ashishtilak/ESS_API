using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class Vendors
    {
        [Key] [StringLength(10)] public string VendorCode { get; set; }

        [StringLength(100)] public string VendorName { get; set; }
        [StringLength(255)] public string VendorAddress1 { get; set; }
        [StringLength(255)] public string VendorAddress2 { get; set; }
        [StringLength(255)] public string VendorAddress3 { get; set; }

        public DateTime? UpdDt { get; set; }
        [StringLength(10)] public string UpdUser { get; set; }
    }
}