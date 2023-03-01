using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class TpaAttdDto
    {
        public DateTime AttdDate { get; set; }
        public string EmpUnqId { get; set; }
        public string ScheDuleShift { get; set; }
        public string ConsShift { get; set; }
        public DateTime? ConsIn { get; set; }
        public DateTime? ConsOut { get; set; }
        public float ConsWrkHrs { get; set; }
        public float ShiftHrs {get; set;}
        public float ConsOverTime { get; set; }
        public float CalcOverTime { get; set; }
        public string Status { get; set; }
        public bool HalfDay { get; set; }
        public string LeaveType { get; set; }
        public bool LeaveHalf { get; set; }
        public string Earlycome { get; set; }
        public string EarlyGoing { get; set; }
        public string LateCome { get; set; }
    }
}