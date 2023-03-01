using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class LeaveBalanceDto
    {
        public string EmpUnqId { get; set; }
        public EmployeeDto Employees { get; set; }

        public string LeaveTypeCode { get; set; }
        public LeaveTypeDto LeaveTypes { get; set; }

        public float Opening { get; set; }

        public float Availed { get; set; }

        public float Balance { get; set; }

        public float Encashed { get; set; }
    }
}