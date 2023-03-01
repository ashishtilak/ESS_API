using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class NoDuesCreators
    {
        [Key, Column(Order = 0)]
        [StringLength(3)]
        public string Dept { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }
    }
}