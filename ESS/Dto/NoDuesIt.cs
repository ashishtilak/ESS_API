using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class NoDuesIt
    {
        public string EmpUnqId { get; set; }
        public string TypeName { get; set; }
        public string ComponentName { get; set; }
        public string Asset { get; set; }
        public float Cost { get; set; }
    }
}