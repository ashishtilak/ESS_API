using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class MaterialDto
    {
        public string MaterialCode { get; set; }
        public string MaterialDesc { get; set; }
        public string Uom { get; set; }
        public string HsnCode { get; set; }
        public DateTime? UpdDt { get; set; }
        public string UpdUser { get; set; }
    }
}