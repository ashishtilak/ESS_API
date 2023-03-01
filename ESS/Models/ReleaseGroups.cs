using System.ComponentModel.DataAnnotations;

namespace ESS.Models
{
    public class ReleaseGroups
    {
        [Key] [Required] [StringLength(2)] public string ReleaseGroupCode { get; set; }

        [StringLength(50)] public string ReleaseGroupDesc { get; set; }

        public static readonly string LeaveApplication = "LA";
        public static readonly string OutStationDuty = "OD";
        public static readonly string GatePass = "GP";
        public static readonly string GatePassAdvice = "GA";
        public static readonly string CompOff = "CO";
        public static readonly string ShiftSchedule = "SS";
        public static readonly string Reimbursement = "RE";
        public static readonly string Mediclaim = "MC";
        public static readonly string NoDues = "ND";
        public static readonly string Tpa = "OT";
        public static readonly string ProgressReview = "PR";
    }
}