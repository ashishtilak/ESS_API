using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class ReviewDetailDto
    {
        public string EmpUnqId { get; set; }

        public EmployeeDto Employee { get; set; }

        public int ReviewQtrNo { get; set; }
        public bool IsConfirmation { get; set; }

        public DateTime ReviewDate { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }

        public string Assignments { get; set; }
        public string Strength { get; set; }
        public string Improvements { get; set; }
        public string Suggestions { get; set; }

        public string Rating { get; set; } //

        public string Remarks { get; set; }
        public string Recommendation { get; set; } // N, C, E, T

        // FIRST LEVEL RELEASER DETAILS
        public DateTime? AddDt { get; set; }
        public string AddReleaseCode { get; set; }
        public string AddUser { get; set; }
        public string AddEmpName { get; set; }
        public string AddReleaseStatusCode { get; set; }

        public string ReleaseGroupCode { get; set; }
        public string ReleaseStrategy { get; set; }

        // STORE ONLY SECOND LEVEL RELEASER DETAILS
        public string ReleaseCode { get; set; }
        public string ReleaseEmpName { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string ReleaseStatusCode { get; set; }
        public string HodRemarks { get; set; }

        public string HrUser { get; set; }
        public string HrEmpName { get; set; }
        public DateTime? HrReleaseDate { get; set; }
        public string HrReleaseStatusCode { get; set; }
        public string HrRemarks { get; set; }
    }
}