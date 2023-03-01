using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class TaxDetailsNscDto
    {
        public int Id { get; set; }
        public int YearMonth { get; set; } //201920
        public string EmpUnqId { get; set; }
        public bool ActualFlag { get; set; } //Provisional v/s Actual
        public string NscNumber { get; set; }

        public DateTime? NscPurchaseDate { get; set; }
        public float NscAmount { get; set; }
        public float NscInterestAmount { get; set; }
    }
}