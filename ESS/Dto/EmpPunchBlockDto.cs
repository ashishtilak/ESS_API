using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class EmpPunchBlockDto
    {
        public string EmpUnqId { get; set; }
        public bool PunchingBlocked { get; set; }
        public DateTime? BlockDt { get; set; }
        public string BlockRemark { get; set; }
        public string BlockBy { get; set; }
        public int Tid { get; set; }

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

        public DateTime? FirstDoseDate { get; set; }
        public DateTime? SecondDoseDate { get; set; }
        public DateTime? ThirdDoseDate { get; set; }
    }
}