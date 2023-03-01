using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class MissedPunchDto
    {
        public int Id { get; set; }
        public DateTime AddDate { get; set; }

        public string EmpUnqId { get; set; }
        public EmployeeDto Employee { get; set; }

        public string Reason { get; set; }

        public string Remarks { get; set; }

        public string ReleaseStrategy { get; set; }
        public string ReleaseStatusCode { get; set; }

        public DateTime? InTime { get; set; }

        public string InTimeUser { get; set; }

        public DateTime? OutTime { get; set; }

        public string OutTimeUser { get; set; }

        public bool PostingFlag { get; set; }

        public string PunchType { get; set; }     // A for attd, M for In/Out missed
        public string InOutFlag { get; set; }     // I for in, O for out
        public DateTime? AttdPunchTime { get; set; }

        public List<MissedPunchReleaseStatusDto> MissedPunchReleaseStatus { get; set; }
    }
}