using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class GpAdviceDto
    {
        public int YearMonth { get; set; }
        public int GpAdviceNo { get; set; }
        public DateTime GpAdviceDate { get; set; }
        public string EmpUnqId { get; set; }
        public string UnitCode { get; set; }
        public string DeptCode { get; set; }
        public string StatCode { get; set; }
        public string GpAdviceType { get; set; }
        public string Purpose { get; set; }
        public string WorkOrderNo { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorAddress2 { get; set; }
        public string VendorAddress3 { get; set; }
        public DateTime? ApproxDateOfReturn { get; set; }
        public string ModeOfTransport { get; set; }
        public string TransporterName { get; set; }

        public string Requisitioner { get; set; }
        public string SapGpNumber { get; set; }
        public string PoTerms { get; set; }

        public string GpAdviceStatus { get; set; }

        public string ReleaseGroupCode { get; set; }
        public string GaReleaseStrategy { get; set; }
        public string ReleaseStatusCode { get; set; }

        public DateTime AddDt { get; set; }
        public string AddUser { get; set; }
        public DateTime? UpdDt { get; set; }
        public string UpdUser { get; set; }

        public string Remarks { get; set; }

        public string PostedUser { get; set; }
        public DateTime? PostedDt { get; set; }

        public List<GpAdviceDetailsDto> GpAdviceDetails { get; set; }
        public List<ApplReleaseStatusDto> ApplReleaseStatus { get; set; }

        public string EmpName { get; set; }
        public string DeptName { get; set; }
        public string StatName { get; set; }
        public string ModeName { get; set; }
        public string StatusName { get; set; }
    }
}