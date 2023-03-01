using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class VendorDto
    {
        public string VendorCode { get; set; }

        public string VendorName { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorAddress2 { get; set; }
        public string VendorAddress3 { get; set; }

        public DateTime? UpdDt { get; set; }
        public string UpdUser { get; set; }
    }
}