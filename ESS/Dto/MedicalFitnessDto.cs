using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class MedicalFitnessDto
    {
        public int Id { get; set; }

        public DateTime? TestDate { get; set; }

        public string EmpUnqId { get; set; }
        public EmployeeDto Employee { get; set; }

        public DateTime? CardBlockedOn { get; set; }
        public int CardBlockedDays { get; set; }

        public string CardBlockedReason { get; set; }

        public bool FitnessFlag { get; set; } // True = fit, False = Unfit

        public string Remarks { get; set; }

        public DateTime AddDt { get; set; }
        public string AddUser { get; set; }

        public string CompName { get; set; }
        public string WrkGrpDesc { get; set; }
        public string UnitName { get; set; }
        public string DeptName { get; set; }
        public string StatName { get; set; }
        public string CatName { get; set; }
        public string EmpTypeName { get; set; }
        public string GradeName { get; set; }
        public string DesgName { get; set; }
        public string EmpName { get; set; }
    }
}