using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class NoDuesDeptDetailsDto
    {
        public string EmpUnqId { get; set; }

        public EmployeeDto Employee { get; set; }

        public string DeptId { get; set; }

        public int Sr { get; set; }

        public string Particulars { get; set; }
        public string Remarks { get; set; }
        public float? Amount { get; set; }

        public string AddUser { get; set; }
        public DateTime? AddDate { get; set; }
    }
}