using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class EmpSeparationDto
    {
        public int Id { get; set; }
        public string EmpUnqId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Mode { get; set; }
        public string Reason { get; set; }
        public string ReasonOther { get; set; }
        public DateTime RelieveDate { get; set; }
        public string ResignText { get; set; }

        public string ReleaseStatusCode { get; set; }
        public ApplReleaseStatusDto ReleaseStatus { get; set; }

        public bool StatusHr { get; set; }
        public string HrUser { get; set; }
        public string HrUserName { get; set; }
        public DateTime HrStatusDate { get; set; }

        public EmployeeDto Employee { get; set; }
        public List<ApplReleaseStatusDto> ApplReleaseStatus { get; set; }

        public bool FurtherReleaseRequired { get; set; }
        public string FurtherReleaser { get; set; }
        public string FurtherReleaserName { get; set; }
        public string FurtherReleaseStatusCode { get; set; }
        public DateTime FurtherReleaseDate { get; set; }
    }
}