using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class GatePass
    {
        [Key] public int Id { get; set; }
        public int YearMonth { get; set; }
        public DateTime GatePassDate { get; set; }
        public int GatePassNo { get; set; }
        public int GatePassItem { get; set; }
        [StringLength(10)] public string EmpUnqId { get; set; }

        [StringLength(1)] public string Mode { get; set; }

        [StringLength(100)] public string PlaceOfVisit { get; set; }
        [StringLength(100)] public string Reason { get; set; }

        [StringLength(10)] public string AddUser { get; set; }
        public DateTime AddDateTime { get; set; }

        [StringLength(1)] public string GatePassStatus { get; set; }
        public DateTime? GateOutDateTime { get; set; }
        [StringLength(10)] public string GateOutUser { get; set; }
        public string GateOutIp { get; set; }
        public DateTime? GateInDateTime { get; set; }
        [StringLength(10)] public string GateInUser { get; set; }
        public string GateInIp { get; set; }


        //2018.10.09 Added for Release strategy
        [StringLength(2)] public string ReleaseGroupCode { get; set; }

        [ForeignKey("ReleaseGroupCode")] public ReleaseGroups ReleaseGroup { get; set; }

        [StringLength(15)] public string GpReleaseStrategy { get; set; }

        [ForeignKey("ReleaseGroupCode, GpReleaseStrategy")]
        public GpReleaseStrategies RelStrategy { get; set; }

        [StringLength(1)] public string ReleaseStatusCode { get; set; }

        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }
        //2018.10.09 End Added for Release strategy

        [StringLength(100)] public string GpRemarks { get; set; }

        public DateTime? AttdUpdate { get; set; }
        [StringLength(10)] public string AttdFlag { get; set; }


        //FOR Attendance system
        public DateTime? AttdGpOutTime { get; set; }
        public DateTime? AttdGpInTime { get; set; }

        [StringLength(10)] public string AttdGpFlag { get; set; }
        //

        public static class GatePassStatuses
        {
            public static readonly string New = "N";
            public static readonly string Out = "O";
            public static readonly string In = "I";
            public static readonly string DutyOff = "D";
            public static readonly string ForceClosed = "F";
        }

        public static class GatePassModes
        {
            public static readonly string Official = "O";
            public static readonly string Personal = "P";
            public static readonly string DutyOff = "D";
        }
    }
}