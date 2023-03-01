using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class ReimbConvDto
    {
        public int YearMonth { get; set; } //201920
        public int ReimbId { get; set; }
        public int Sr { get; set; }

        public string ReimbType { get; set; }
        public string EmpUnqId { get; set; }
        public DateTime ConvDate { get; set; }
        public string VehicleNo { get; set; }
        public string Particulars { get; set; }
        public int MeterFrom { get; set; }
        public int Distance { get; set; }
        public int MeterTo { get; set; }
        public float Rate { get; set; }
        public float Amount { get; set; }
        public string Remarks { get; set; }
    }
}