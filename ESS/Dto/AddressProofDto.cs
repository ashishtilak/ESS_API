using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class AddressProofDto
    {
        public int Id { get; set; }
        public string EmpUnqId { get; set; }

        public DateTime ApplicationDate { get; set; }
        public string Purpose { get; set; }
        public DateTime AddDate { get; set; }

        public string Proof { get; set; }
        public string HrReleaseStatusCode { get; set; }
        public string HrRemarks { get; set; }
        public string HrUser { get; set; }
        public DateTime? HrReleaseDate { get; set; }
    }
}