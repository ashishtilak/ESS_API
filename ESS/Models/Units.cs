﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.Models
{
    public class Units
    {
        [Key, Column(Order = 0)]
        [StringLength(2)]
        public string CompCode { get; set; }

        public virtual Company Company { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(10)]
        public string WrkGrp { get; set; }

        public virtual WorkGroups WorkGroup { get; set; }

        [Key, Column(Order = 2)]
        [Required]
        [StringLength(3)]
        public string UnitCode { get; set; }

        [StringLength(50)] public string UnitName { get; set; }

        [StringLength(5)] public string Location { get; set; }
    }
}