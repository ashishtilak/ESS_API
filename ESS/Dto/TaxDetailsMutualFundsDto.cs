using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class TaxDetailsMutualFundsDto
    {
        public int Id { get; set; }
        public int YearMonth { get; set; } //201920
        public string EmpUnqId { get; set; }
        public bool ActualFlag { get; set; } //Provisional v/s Actual
        public string MutualFundName { get; set; }
        public DateTime? MutualFundDate { get; set; }
        public float MutualFundAmount { get; set; }
    }
}