using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class OpenMonth
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int YearMonth { get; set; }

        public int OpenYear { get; set; }

        public int PrevMonth { get; set; }
    }
}