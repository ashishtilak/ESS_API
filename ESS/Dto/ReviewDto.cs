using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class ReviewDto
    {
        public string EmpUnqId { get; set; }
        public EmployeeDto Employee { get; set; }

        public int ReviewQtr { get; set; }
        public DateTime JoinDt { get; set; }

        public string ConfirmationStatus { get; set; }

        public DateTime? AddDt { get; set; }
        public string AddUser { get; set; }
       
    }
}