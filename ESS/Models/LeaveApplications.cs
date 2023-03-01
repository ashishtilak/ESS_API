using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.Models
{
    public class LeaveApplications
    {
        [Key, Column(Order = 0)] public int YearMonth { get; set; }

        [Key, Column(Order = 1)] public int LeaveAppId { get; set; }

        [StringLength(10)] public string EmpUnqId { get; set; }

        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        [StringLength(2)] public string CompCode { get; set; }

        [ForeignKey("CompCode")] public Company Company { get; set; }

        [StringLength(10)] public string WrkGrp { get; set; }

        [ForeignKey("CompCode, WrkGrp")] public WorkGroups WorkGroup { get; set; }

        [StringLength(3)] public string UnitCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, UnitCode")]
        public Units Units { get; set; }

        [StringLength(3)] public string DeptCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, UnitCode, DeptCode")]
        public Departments Departments { get; set; }

        [StringLength(3)] public string StatCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, UnitCode, DeptCode, StatCode")]
        public Stations Stations { get; set; }

        [StringLength(3)] public string SecCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, UnitCode, DeptCode, StatCode, SecCode")]
        public Sections Sections { get; set; }

        [StringLength(3)] public string CatCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, CatCode")]
        public Categories Categories { get; set; }

        public bool IsHod { get; set; }

        [StringLength(2)] public string ReleaseGroupCode { get; set; }

        [ForeignKey("ReleaseGroupCode")] public ReleaseGroups ReleaseGroup { get; set; }

        [StringLength(15)] public string ReleaseStrategy { get; set; }

        [ForeignKey("ReleaseGroupCode, ReleaseStrategy")]
        public ReleaseStrategies RelStrategy { get; set; }

        [StringLength(1)] public string ReleaseStatusCode { get; set; }

        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? AddDt { get; set; }

        [StringLength(10)] public string AddUser { get; set; }


        [StringLength(15)] public string ClientIp { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? UpdDt { get; set; }

        [StringLength(10)] public string UpdUser { get; set; }

        [StringLength(255)] public string Remarks { get; set; }

        public bool? Cancelled { get; set; }

        public int? ParentId { get; set; }

        public ICollection<LeaveApplicationDetails> LeaveApplicationDetails { get; set; }
        public ICollection<ApplReleaseStatus> ApplReleaseStatus { get; set; }
    }
}