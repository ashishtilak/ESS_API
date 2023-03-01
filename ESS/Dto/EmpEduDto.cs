using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class EmpEduDto
    {
        public string EmpUnqId { get; set; }
        public int PassingYear { get; set; }
        public string EduName { get; set; }
        public string Subject { get; set; }
        public string University { get; set; }
        public int Percentage { get; set; }
        public string Remarks { get; set; }
    }
}