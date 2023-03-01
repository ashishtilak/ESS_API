using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class MedDependentDto
    {
        public string EmpUnqId { get; set; }
        public int DepSr { get; set; }
        public string DepName { get; set; }
        public string Relation { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public DateTime? MarriageDate { get; set; }
        public string Pan { get; set; }
        public string Aadhar { get; set; }
        public string BirthCertificateNo { get; set; }
        public DateTime EffectiveDate { get; set; } //by default add date

        public string ReleaseGroupCode { get; set; }
        public string ReleaseStrategy { get; set; }
        public string ReleaseStatusCode { get; set; }
        public DateTime? ReleaseDt { get; set; }
        public string ReleaseUser { get; set; }

        // Dependent Deletion Release fields...
        public string DelReleaseGroupCode { get; set; }
        public string DelReleaseStrategy { get; set; }
        public string DelReleaseStatusCode { get; set; }
        public DateTime? DelReleaseDt { get; set; }
        public string DelReleaseUser { get; set; }

        public string Remarks { get; set; }

        public bool Active { get; set; }

        public string AddUser { get; set; }
        public DateTime AddDate { get; set; }

        public bool IsChanged { get; set; }

        public MedEmpUhidDto UhIds { get; set; }
    }
}