using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class SsOpenMonth
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int YearMonth { get; set; }

        public bool PostingEnabled { get; set; }
    }
}