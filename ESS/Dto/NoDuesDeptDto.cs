using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class NoDuesDeptDto
    {
        public string EmpUnqId { get; set; }
        public EmployeeDto Employee { get; set; }
        public string DeptId { get; set; }
        public bool NoDuesFlag { get; set; }
        public string Remarks { get; set; }
        public bool? ApprovalFlag { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }

        public int? No { get; set; }
        public string DeptName { get; set; }

        public List<NoDuesDeptDetailsDto> NoDuesDeptDetails { get; set; }
    }
}