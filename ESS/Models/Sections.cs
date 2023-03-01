using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class Sections
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
        [StringLength(3)]
        public string UnitCode { get; set; }

        public virtual Units Unit { get; set; }

        [Key, Column(Order = 3)]
        [StringLength(3)]
        public string DeptCode { get; set; }

        public virtual Departments Department { get; set; }

        [Key, Column(Order = 4)]
        [StringLength(3)]
        public string StatCode { get; set; }

        public virtual Stations Station { get; set; }

        [Key, Column(Order = 5)]
        [StringLength(3)]
        public string SecCode { get; set; }


        [StringLength(100)] public string SecName { get; set; }

        [StringLength(5)] public string Location { get; set; }
    }
}