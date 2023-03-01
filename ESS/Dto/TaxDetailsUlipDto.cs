using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class TaxDetailsUlipDto
    {
        public int Id { get; set; }
        public int YearMonth { get; set; } //201920
        public string EmpUnqId { get; set; }
        public bool ActualFlag { get; set; } //Provisional v/s Actual
        public string UlipNo { get; set; }
        public DateTime? UlipDate { get; set; }
        public float UlipAmount { get; set; }
    }
}