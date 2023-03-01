using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class NoDuesMasterDto
    {
        public string EmpUnqId { get; set; }

        public EmployeeDto Employee { get; set; }

        public DateTime JoinDate { get; set; }
        public DateTime ResignDate { get; set; }
        public DateTime RelieveDate { get; set; }
        public DateTime NoDuesStartDate { get; set; }

        public string AddUser { get; set; }
        public string AddUserName { get; set; }
        public DateTime? AddDate { get; set; }

        public bool ClosedFlag { get; set; }

        public string DeptParticulars { get; set; }
        public string DeptRemarks { get; set; }
        public float? DeptAmount { get; set; }

        public bool DeptNoDuesFlag { get; set; }
        public bool DeptApprovalFlag { get; set; }

        public string DeptAddUser { get; set; }
        public string DeptAddUserName { get; set; }
        public DateTime? DeptAddDate { get; set; }

        public string EmpName { get; set; }
        public string UnitName { get; set; }
        public string DeptName { get; set; }
        public string StatName { get; set; }
        public string CatName { get; set; }
        public string GradeName { get; set; }
        public string DesgName { get; set; }


        public string NoticePeriod { get; set; }
        public string NoticePeriodUnit { get; set; } // BASIC or CTC
        public DateTime? LastWorkingDate { get; set; }
        public string ModeOfLeaving { get; set; } // Resign/Retire/Absconding/Termination/etc.
        public bool? ExitInterviewFlag { get; set; }

        public string HrAddUser { get; set; }
        public string HrAddUserName { get; set; }
        public DateTime? HrAddDate { get; set; }

        // approval of HR
        public bool? HrApprovalFlag { get; set; }
        public DateTime? HrApprovalDate { get; set; }
        public string HrApprovedBy { get; set; }
        public string HrApprovedByName { get; set; }


        // approval of Unit head
        public string UnitHead { get; set; }
        public bool? UhApprovalFlag { get; set; }
        public DateTime? UhApprovalDate { get; set; }
        public string UhApprovedBy { get; set; }

        // list of all nodues type included here....
        public List<NoDuesReleaseStatusDto> NoDuesReleaseStatus { get; set; }
        public List<NoDuesDeptDto> NoDuesDepts { get; set; }
        public List<NoDuesIt> NoDuesIt { get; set; }
        public NoDuesStatusDto NoDuesStatus { get; set; }
    }
}