using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class MedIntimationDto
    {
        public int Id { get; set; }
        public DateTime IntimationDate { get; set; }

        public string EmpUnqId { get; set; }
        public EmployeeDto Employee { get; set; }

        public string InsuredMobileNo { get; set; }
        public string PatientName { get; set; }
        public string Relation { get; set; }
        public string IntimatorEmpUnqId { get; set; }
        public string IntimatorName { get; set; }
        public string IntimatorMobileNo { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string DoctorName { get; set; }
        public string DoctorRegistrationNumber { get; set; }
        public string Diagnosis { get; set; }
        public string HospitalName { get; set; }
        public string HospitalRegistrationNumber { get; set; }
        public string HospitalAddress { get; set; }
        public string Pin { get; set; }

        public string AddUser { get; set; }

        public string HrUser { get; set; }
        public DateTime? HrApproveDate { get; set; }
        public string HrRemarks { get; set; }

        public string ReleaseStatusCode { get; set; }
    }
}