using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class VehicleReq
    {
        [Key] public int ReqId { get; set; }

        [StringLength(10)] public string EmpUnqId { get; set; }
        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        [Column(TypeName = "datetime2")] public DateTime ReqDate { get; set; }
        [Column(TypeName = "Date")] public DateTime BookingDate { get; set; }

        public int BookingSlot { get; set; }

        public bool BookingStatus { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? PickupTime { get; set; } //date will be booking date
        [StringLength(50)] public string PickupLocation { get; set; }
        [StringLength(50)] public string DropLocation { get; set; }
        public int NumberOfPass { get; set; }
        [StringLength(50)] public string Remarks { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? AddDt { get; set; }
        [StringLength(10)] public string AddUser { get; set; }


        // RELEASE DETAILS

        [StringLength(2)] public string ReleaseGroupCode { get; set; }
        [ForeignKey("ReleaseGroupCode")] public ReleaseGroups ReleaseGroup { get; set; }

        [StringLength(15)] public string ReleaseStrategy { get; set; }

        [ForeignKey("ReleaseGroupCode, ReleaseStrategy")]
        public ReleaseStrategies RelStrategy { get; set; }

        [StringLength(20)] public string ReleaseCode { get; set; }

        [StringLength(1)] public string ReleaseStatusCode { get; set; }
        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }

        [StringLength(255)] public string ReleaseRemarks { get; set; }

        [StringLength(10)] public string ReleaseAuth { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? ReleaseDate { get; set; }

        [StringLength(1)] public string AdminReleaseStatusCode { get; set; }
        [ForeignKey("AdminReleaseStatusCode")] public ReleaseStatus AdminReleaseStatus { get; set; }

        [StringLength(10)] public string AdminUser { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? AdminReleaseDate { get; set; }
        [StringLength(255)] public string AdminReleaseRemarks { get; set; }

        public static readonly int Morning = 1;
        public static readonly int Evening = 2;
        public static readonly int Night = 3;
    }
}