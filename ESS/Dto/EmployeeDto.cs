using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class EmployeeDto
    {
        public string EmpUnqId { get; set; }
        public string EmpName { get; set; }
        public string FatherName { get; set; }
        public bool Active { get; set; }
        public string Pass { get; set; }

        public string CompCode { get; set; }
        public string WrkGrp { get; set; }
        public string UnitCode { get; set; }
        public string DeptCode { get; set; }

        public string StatCode { get; set; }

        //public string SecCode { get; set; }
        public string CatCode { get; set; }
        public string EmpTypeCode { get; set; }
        public string GradeCode { get; set; }
        public string DesgCode { get; set; }

        public bool OtFlag { get; set; }

        public bool IsReleaser { get; set; }
        public bool IsHrUser { get; set; }
        public bool IsHod { get; set; }
        public bool IsGpReleaser { get; set; }
        public bool IsGaReleaser { get; set; }
        public bool IsSecUser { get; set; }
        public bool IsAdmin { get; set; }
        public string Email { get; set; }

        public string CompName { get; set; }
        public string WrkGrpDesc { get; set; }
        public string UnitName { get; set; }
        public string DeptName { get; set; }
        public string StatName { get; set; }
        public string SecName { get; set; }
        public string CatName { get; set; }
        public string EmpTypeName { get; set; }
        public string GradeName { get; set; }
        public string DesgName { get; set; }


        public int Counter { get; set; }
        public string HouseNumber { get; set; }
        public string SocietyName { get; set; }
        public string AreaName { get; set; }
        public string LandMark { get; set; }
        public string Tehsil { get; set; }
        public string PoliceStation { get; set; }
        public string PreDistrict { get; set; }
        public string PreCity { get; set; }
        public string PreState { get; set; }
        public string PrePin { get; set; }
        public string PrePhone { get; set; }
        public string PreResPhone { get; set; }
        public string PreEmail { get; set; }
        public DateTime? UpdDt { get; set; } //present address update date

        public string PerAdd1 { get; set; }
        public string PerAdd2 { get; set; }
        public string PerAdd3 { get; set; }
        public string PerAdd4 { get; set; }
        public string PerDistrict { get; set; }
        public string PerCity { get; set; }
        public string PerState { get; set; }
        public string PerPin { get; set; }
        public string PerPhone { get; set; }
        public string PerPoliceSt { get; set; }

        public bool HrVerified { get; set; }
        public bool HrUser { get; set; }
        public DateTime? HrVerificationDate { get; set; }

        public string HrCity { get; set; }
        public string HrSociety { get; set; }
        public string HrRemarks { get; set; }

        public string Location { get; set; }

        public string SapId { get; set; }
        public bool CompanyAcc { get; set; }

        public int RoleId { get; set; }

        public DateTime? BirthDate { get; set; }
        public string Pan { get; set; }

        public DateTime? JoinDate { get; set; }

        public bool NoDuesFlag { get; set; }

        public int YearlyCount { get; set; }

        public bool PlCheck { get; set; }
    }
}