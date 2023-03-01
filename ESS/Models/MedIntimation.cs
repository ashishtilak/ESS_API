using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class MedIntimation
    {
        [Key] public int Id { get; set; }
        [Column(TypeName = "datetime2")] public DateTime IntimationDate { get; set; }

        [StringLength(10)] public string EmpUnqId { get; set; }
        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        [StringLength(15)] public string InsuredMobileNo { get; set; }
        [StringLength(50)] public string PatientName { get; set; }
        [StringLength(10)] public string Relation { get; set; }
        [StringLength(10)] public string IntimatorEmpUnqId { get; set; }
        [StringLength(50)] public string IntimatorName { get; set; }
        [StringLength(15)] public string IntimatorMobileNo { get; set; }
        [Column(TypeName = "datetime2")] public DateTime AdmissionDate { get; set; }
        [StringLength(50)] public string DoctorName { get; set; }
        [StringLength(50)] public string DoctorRegistrationNumber { get; set; }
        [StringLength(255)] public string Diagnosis { get; set; }
        [StringLength(50)] public string HospitalName { get; set; }
        [StringLength(50)] public string HospitalRegistrationNumber { get; set; }
        [StringLength(255)] public string HospitalAddress { get; set; }
        [StringLength(6)] public string Pin { get; set; }

        [StringLength(10)] public string AddUser { get; set; }
        
        [StringLength(10)] public string HrUser { get; set; }
        [Column(TypeName = "datetime2")] public DateTime? HrApproveDate { get; set; }
        [StringLength(50)] public string HrRemarks { get; set; }

        [StringLength(1)] public string ReleaseStatusCode { get; set; }
        [ForeignKey("ReleaseStatusCode")] public ReleaseStatus ReleaseStatus { get; set; }
    }
}