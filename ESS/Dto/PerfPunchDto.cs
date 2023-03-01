using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class PerfPunchDto
    {
        public DateTime? PunchDate { get; set; }
        public string IoFlag { get; set; }
        public string MachineIp { get; set; }
        public string MachineDesc { get; set; }
    }
}