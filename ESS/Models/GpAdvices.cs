using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class GpAdvices
    {
        [Key, Column(Order = 0)] public int YearMonth { get; set; }
        [Key, Column(Order = 1)] public int GpAdviceNo { get; set; }

        public DateTime GpAdviceDate { get; set; }

        [StringLength(10)] public string EmpUnqId { get; set; }

        [StringLength(3)] public string UnitCode { get; set; }
        [StringLength(3)] public string DeptCode { get; set; }
        [StringLength(3)] public string StatCode { get; set; }

        [StringLength(1)] public string GpAdviceType { get; set; }

        [StringLength(255)] public string Purpose { get; set; }

        [StringLength(20)] public string WorkOrderNo { get; set; }

        [StringLength(10)] public string VendorCode { get; set; }

        [StringLength(100)] public string VendorName { get; set; }
        [StringLength(255)] public string VendorAddress1 { get; set; }
        [StringLength(255)] public string VendorAddress2 { get; set; }
        [StringLength(255)] public string VendorAddress3 { get; set; }

        public DateTime? ApproxDateOfReturn { get; set; }

        [StringLength(20)] public string ModeOfTransport { get; set; }
        [StringLength(100)] public string TransporterName { get; set; }

        [StringLength(10)] public string Requisitioner { get; set; }
        [StringLength(10)] public string SapGpNumber { get; set; }
        [StringLength(255)] public string PoTerms { get; set; }

        [StringLength(1)] public string GpAdviceStatus { get; set; }

        [StringLength(2)] public string ReleaseGroupCode { get; set; }

        [ForeignKey("ReleaseGroupCode")] public ReleaseGroups ReleaseGroup { get; set; }

        [StringLength(15)] public string GaReleaseStrategy { get; set; }

        [ForeignKey("ReleaseGroupCode, GaReleaseStrategy")]
        public GaReleaseStrategies RelStrategy { get; set; }

        [StringLength(1)] public string ReleaseStatusCode { get; set; }

        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }

        public DateTime AddDt { get; set; }
        [StringLength(10)] public string AddUser { get; set; }

        public DateTime? UpdDt { get; set; }
        [StringLength(10)] public string UpdUser { get; set; }

        [StringLength(100)] public string Remarks { get; set; }

        [StringLength(10)] public string PostedUser { get; set; }
        public DateTime? PostedDt { get; set; }

        public ICollection<GpAdviceDetails> GpAdviceDetails { get; set; }
    }

    public static class GpAdviceTypes
    {
        public static readonly string ReturnableGatePassAdvice = "R";
        public static readonly string NonReturnableGatePassAdvice = "N";
    }

    public static class GpAdviceStatuses
    {
        public static readonly string NotPosted = "N";
        public static readonly string FullyPosted = "F";
        public static readonly string PostingRejected = "R";
    }
}