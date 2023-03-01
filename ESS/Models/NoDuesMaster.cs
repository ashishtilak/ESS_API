using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class NoDuesMaster
    {
        [Key] [StringLength(10)] public string EmpUnqId { get; set; }

        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        [Column(TypeName = "datetime2")] public DateTime JoinDate { get; set; }
        [Column(TypeName = "datetime2")] public DateTime ResignDate { get; set; }
        [Column(TypeName = "datetime2")] public DateTime RelieveDate { get; set; }
        [Column(TypeName = "datetime2")] public DateTime NoDuesStartDate { get; set; }

        [StringLength(10)] public string AddUser { get; set; }
        [Column(TypeName = "datetime2")] public DateTime AddDate { get; set; }

        public bool ClosedFlag { get; set; }

        // DEPARTMENTAL FIELDS:
        [StringLength(50)] public string DeptParticulars { get; set; }
        [StringLength(200)] public string DeptRemarks { get; set; }
        public float? DeptAmount { get; set; }

        public bool DeptNoDuesFlag { get; set; }
        public bool DeptApprovalFlag { get; set; }

        [StringLength(10)] public string DeptAddUser { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? DeptAddDate { get; set; }

        // HR NODUES FIELDS:

        [StringLength(50)] public string NoticePeriod { get; set; }
        [StringLength(10)] public string NoticePeriodUnit { get; set; } // BASIC or CTC
        [Column(TypeName = "datetime2")] public DateTime? LastWorkingDate { get; set; }
        [StringLength(20)] public string ModeOfLeaving { get; set; } // Resign/Retire/Absconding/Termination/etc.
        public bool? ExitInterviewFlag { get; set; }

        [StringLength(10)] public string HrAddUser { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? HrAddDate { get; set; }

        // approval of HR
        public bool? HrApprovalFlag { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? HrApprovalDate { get; set; }
        [StringLength(10)] public string HrApprovedBy { get; set; }


        // approval of Unit head
        [StringLength(10)] public string UnitHead { get; set; }
        public bool? UhApprovalFlag { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? UhApprovalDate { get; set; }
        [StringLength(10)] public string UhApprovedBy { get; set; }

        public ICollection<NoDuesReleaseStatus> NoDuesReleaseStatus { get; set; }
    }
}