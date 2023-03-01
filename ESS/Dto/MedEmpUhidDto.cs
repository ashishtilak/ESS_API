using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class MedEmpUhidDto
    {
        public int PolicyYear { get; set; }
        public string EmpUnqId { get; set; }

        public int DepSr { get; set; }

        // public MedDependentDto Dependent { get; set; }
        public string Uhid { get; set; }
        public bool Active { get; set; }
    }
}