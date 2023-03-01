using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class TaxDetailsSukanyaDto
    {
        public int Id { get; set; }
        public int YearMonth { get; set; } //201920
        public string EmpUnqId { get; set; }
        public bool ActualFlag { get; set; } //Provisional v/s Actual
        public string SukanyaName { get; set; }
        public DateTime? SukanyaDate { get; set; }
        public float SukanyaAmount { get; set; }
    }
}