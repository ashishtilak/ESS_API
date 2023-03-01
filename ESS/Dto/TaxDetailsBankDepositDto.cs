using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class TaxDetailsBankDepositDto
    {
        public int Id { get; set; }
        public int YearMonth { get; set; } //201920
        public string EmpUnqId { get; set; }
        public bool ActualFlag { get; set; } //Provisional v/s Actual
        public string BankAccountNo { get; set; }
        public DateTime? DepositDate { get; set; }
        public float DepositAmount { get; set; }
    }
}