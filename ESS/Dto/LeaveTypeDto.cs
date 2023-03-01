using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class LeaveTypeDto
    {
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        public bool Active { get; set; }
    }
}