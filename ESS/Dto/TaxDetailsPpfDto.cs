using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class TaxDetailsPpfDto
    {
        public int Id { get; set; }
        public int YearMonth { get; set; } //201920
        public string EmpUnqId { get; set; }
        public bool ActualFlag { get; set; } //Provisional v/s Actual
        public string PpfAcNo { get; set; }
        public DateTime? PpfDepositeDate { get; set; }
        public float PpfAmt { get; set; }
    }
}