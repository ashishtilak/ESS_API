using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class GpAdviceDetailsDto
    {
        public int YearMonth { get; set; }
        public int GpAdviceNo { get; set; }
        public int GpAdviceItem { get; set; }

        public string MaterialCode { get; set; }
        public string MaterialDesc { get; set; }
        public float MaterialQty { get; set; }
        public float ApproxValue { get; set; }
        public string HsnCode { get; set; }
    }
}