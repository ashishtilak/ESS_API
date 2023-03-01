using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class RequestDetailsDto
    {
        public int RequestId { get; set; }
        public string EmpUnqId { get; set; }
        public string EmpName { get; set; }
        public int Sr { get; set; }
        public DateTime? FromDt { get; set; }
        public DateTime? ToDt { get; set; }
        public string ShiftCode { get; set; }
        public string Reason { get; set; }
    }
}