using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace ESS.Models
{
    public class MedDependent
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [Key] [Column(Order = 1)] public int DepSr { get; set; }

        [StringLength(50)] public string DepName { get; set; }

        [StringLength(10)] public string Relation { get; set; }

        [Column(TypeName = "datetime2")] public DateTime BirthDate { get; set; }

        [StringLength(1)] public string Gender { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? MarriageDate { get; set; }

        [StringLength(10)] public string Pan { get; set; }
        [StringLength(12)] public string Aadhar { get; set; }
        [StringLength(20)] public string BirthCertificateNo { get; set; }


        [Column(TypeName = "datetime2")] public DateTime EffectiveDate { get; set; } //by default add date

        [StringLength(2)] public string ReleaseGroupCode { get; set; }
        [ForeignKey("ReleaseGroupCode")] public ReleaseGroups ReleaseGroup { get; set; }
        [StringLength(15)] public string ReleaseStrategy { get; set; }

        [ForeignKey("ReleaseGroupCode, ReleaseStrategy")]
        public ReleaseStrategies RelStrategy { get; set; }

        [StringLength(1)] public string ReleaseStatusCode { get; set; }
        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? ReleaseDt { get; set; }
        [StringLength(10)] public string ReleaseUser { get; set; }


        // Dependent Deletion Release fields...
        [StringLength(2)] public string DelReleaseGroupCode { get; set; }
        [ForeignKey("DelReleaseGroupCode")] public ReleaseGroups DelReleaseGroup { get; set; }
        [StringLength(15)] public string DelReleaseStrategy { get; set; }

        [ForeignKey("DelReleaseGroupCode, DelReleaseStrategy")]
        public ReleaseStrategies DelRelStrategy { get; set; }

        [StringLength(1)] public string DelReleaseStatusCode { get; set; }
        [ForeignKey("DelReleaseStatusCode")] public ReleaseStatus DelReleaseStatus { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? DelReleaseDt { get; set; }
        [StringLength(10)] public string DelReleaseUser { get; set; }

        [StringLength(255)] public string Remarks { get; set; }

        public bool Active { get; set; }

        [StringLength(10)] public string AddUser { get; set; }
        [Column(TypeName = "datetime2")] public DateTime AddDate { get; set; }

        public bool IsChanged { get; set; }

        public static readonly string Self = "Self";
        public static readonly string Wife = "Wife";
        public static readonly string Husband = "Husband";
        public static readonly string Daughter = "Daughter";
        public static readonly string Son = "Son";
    }
}