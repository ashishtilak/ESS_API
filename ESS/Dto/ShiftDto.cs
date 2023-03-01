using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class ShiftDto
    {
        public string ShiftCode { get; set; }
        public string ShiftDesc { get; set; }
        public TimeSpan? ShiftStart { get; set; }
        public TimeSpan? ShiftEnd { get; set; }
    }
}