using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class RequestDto
    {
        public int RequestId { get; set; }
        public string EmpUnqId { get; set; }
        public string EmpName { get; set; }
        public DateTime? RequestDate { get; set; }
        public string Remarks { get; set; }
        public string ReleaseGroupCode { get; set; }
        public string ReleaseStrategy { get; set; }
        public string ReleaseStatusCode { get; set; }
        public DateTime? AddDt { get; set; }
        public string AddUser { get; set; }
        public string AddUserName { get; set; }

        public string IsPosted { get; set; }
        public string PostUser { get; set; }
        public DateTime? PostedDt { get; set; }

        public List<RequestDetailsDto> RequestDetails { get; set; }
        public List<RequestReleaseDto> RequestReleases { get; set; }
    }
}