using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class TaxDetailsMutualFunds
    {
        [Key, Column(Order = 0)] public int YearMonth { get; set; } //201920

        [Key, Column(Order = 1)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [Key, Column(Order = 2)] public bool ActualFlag { get; set; } //Provisional v/s Actual

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 3)]
        public int Id { get; set; }

        [StringLength(50)] public string MutualFundName { get; set; }
        public DateTime? MutualFundDate { get; set; }
        public float MutualFundAmount { get; set; }

        public virtual TaxDeclarations TaxDeclaration { get; set; }
    }
}