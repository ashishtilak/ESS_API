using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class TaxDetailsInsuranceDto
    {
        public int Id { get; set; }
        public int YearMonth { get; set; } //201920
        public string EmpUnqId { get; set; }
        public bool ActualFlag { get; set; } //Provisional v/s Actual
        public string PolicyNo { get; set; }
        public DateTime? PolicyDate { get; set; }
        public float SumInsured { get; set; }
        public float AnnualPremiumAmount { get; set; }
        public DateTime? PremiumPaidDate { get; set; }
        public DateTime? PremiumDueDate { get; set; }
    }
}