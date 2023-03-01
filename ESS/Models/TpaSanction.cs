using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class TpaSanction
    {
        [Key] public int Id { get; set; }

        [StringLength(10)] public string EmpUnqId { get; set; }
        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        [Column(TypeName = "Date")] public DateTime TpaDate { get; set; } //store only date

        [StringLength(2)] public string TpaShiftCode { get; set; } //Shift at the time of entry
        //[ForeignKey("TpaShiftCode")] public Shifts Shift { get; set; }

        public float RequiredHours { get; set; }
        [StringLength(255)] public string PreJustification { get; set; }

        [StringLength(2)] public string ReleaseGroupCode { get; set; }
        [ForeignKey("ReleaseGroupCode")] public ReleaseGroups ReleaseGroup { get; set; }

        [StringLength(15)] public string ReleaseStrategy { get; set; }

        [ForeignKey("ReleaseGroupCode, ReleaseStrategy")]
        public ReleaseStrategies RelStrategy { get; set; }

        [StringLength(1)] public string PreReleaseStatusCode { get; set; }
        [ForeignKey("PreReleaseStatusCode")] public ReleaseStatus PreReleaseStatus { get; set; }

        [StringLength(255)] public string PreRemarks { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? AddDt { get; set; }
        [StringLength(10)] public string AddUser { get; set; }


        // POST 
        [StringLength(2)] public string ActShiftCode { get; set; } //Actual shift
        //[ForeignKey("ActShiftCode")] public Shifts ActShift { get; set; }

        public float WrkHours { get; set; } //Actual wrk hours
        public float SanctionTpa { get; set; } //Sanctioned hours
        [StringLength(255)] public string PostJustification { get; set; } //Justification if reqd > actual

        [StringLength(1)] public string PostReleaseStatusCode { get; set; }
        [ForeignKey("PostReleaseStatusCode")] public ReleaseStatus PostReleaseStatus { get; set; }

        [StringLength(255)] public string PostRemarks { get; set; }

        [StringLength(1)] public string HrReleaseStatusCode { get; set; }
        [ForeignKey("HrReleaseStatusCode")] public ReleaseStatus HrReleaseStatus { get; set; }
        [StringLength(255)] public string HrPostRemarks { get; set; }
        [StringLength(10)] public string HrUser { get; set; }
        public DateTime? HrReleaseDate { get; set; }

    }
}