using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class SapUserIdDto
    {
        public string SapUserId { get; set; }
        public string EmpUnqId { get; set; }
        public string LineOfBusiness { get; set; }

        // following is required only for common ids
        public string UserName { get; set; }
        public string DeptName { get; set; }
        public bool IsCommon { get; set; }

        public bool IsActive { get; set; }
        public DateTime? ValidTo { get; set; }
        public string Remarks { get; set; }

        public string EmpName { get; set; }
        public bool Active { get; set; }
        public string CompName { get; set; }
        public string WrkGrpDesc { get; set; }
        public string UnitName { get; set; }
        public string Dept { get; set; }
        public string StatName { get; set; }
        public string GradeName { get; set; }
        public DateTime? JoinDate { get; set; }
        public bool NoDuesFlag { get; set; }
        public DateTime? ResignDate { get; set; }
        public DateTime? RelieveDate { get; set; }

    }
}