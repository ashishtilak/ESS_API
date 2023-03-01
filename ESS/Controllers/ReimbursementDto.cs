using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESS.Dto;

namespace ESS.Controllers
{
    public class ReimbursementDto
    {
        public int YearMonth { get; set; } //201920
        public int ReimbId { get; set; }
        public string ReimbType { get; set; }
        public string EmpUnqId { get; set; }
        public DateTime ReimbDate { get; set; }

        public int PeriodFrom { get; set; }
        public int PeriodTo { get; set; }

        public string InvoiceNo { get; set; }

        public float AmountClaimed { get; set; }
        public float AmountReleased { get; set; }
        public string AmountReleaseRemarks { get; set; }

        public string AddUser { get; set; }
        public DateTime AddDateTime { get; set; }

        public string ReleaseGroupCode { get; set; }
        public string ReleaseStrategy { get; set; }

        public string ReleaseStatusCode { get; set; }

        public string Remarks { get; set; }

        public List<ApplReleaseStatusDto> ApplReleaseStatus { get; set; }
        public List<ReimbConvDto> ReimbConv { get; set; }


        public string DeptName { get; set; }
        public string StatName { get; set; }
        public string EmpName { get; set; }
    }
}