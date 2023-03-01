using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class TaxConfig
    {
        [Key] public int Id { get; set; } //Only dummy, as there'll be only one record
        public int YearMonth { get; set; } //201920
        public bool ActualFlag { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }
        public bool CloseFlag { get; set; }
    }
}