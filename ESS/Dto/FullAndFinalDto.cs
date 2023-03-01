using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class FullAndFinalDto
    {
        public string EmpUnqId { get; set; }
        public EmployeeDto Employee { get; set; }
        public DateTime? RelieveDate { get; set; }

        public string DocumentNo { get; set; }
        public float? RecoveryAmount { get; set; }
        public string CashDeposited { get; set; }
        public DateTime? DepositDate { get; set; }

        public string Remarks { get; set; }

        public bool GratuityFlag { get; set; }

        public string AddUser { get; set; }
        public DateTime AddDate { get; set; }

        // Nodues fields
        public DateTime? JoinDate { get; set; }
        public DateTime? ResignDate { get; set; }
        public DateTime? NoDuesStartDate { get; set; }
        public string NoticePeriod { get; set; }
        public string NoticePeriodUnit { get; set; } // BASIC or CTC
        public DateTime? LastWorkingDate { get; set; }
        public string ModeOfLeaving { get; set; } // Resign/Retire/Absconding/Termination/etc.
        public bool? ExitInterviewFlag { get; set; }
        public bool ClosedFlag { get; set; }


    }
}