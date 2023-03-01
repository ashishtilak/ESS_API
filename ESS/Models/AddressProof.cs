using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class AddressProof
    {
        [Key] public int Id { get; set; }
        [StringLength(10)] public string EmpUnqId { get; set; }
        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }
        [Column(TypeName = "datetime2")] public DateTime ApplicationDate { get; set; }
        [StringLength(50)] public string Purpose { get; set; }
        [Column(TypeName = "datetime2")] public DateTime AddDate { get; set; }

        [StringLength(50)] public string Proof { get; set; }
        [StringLength(1)] public string HrReleaseStatusCode { get; set; }
        [ForeignKey("HrReleaseStatusCode")] public ReleaseStatus HrReleaseStatus { get; set; }
        [StringLength(255)] public string HrRemarks { get; set; }
        [StringLength(10)] public string HrUser { get; set; }
        public DateTime? HrReleaseDate { get; set; }
    }
}