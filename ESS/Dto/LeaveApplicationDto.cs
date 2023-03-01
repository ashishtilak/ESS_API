using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class LeaveApplicationDto
    {
        public int YearMonth { get; set; }
        public int LeaveAppId { get; set; }
        public string EmpUnqId { get; set; }
        public string CompCode { get; set; }
        public string WrkGrp { get; set; }
        public string UnitCode { get; set; }
        public string DeptCode { get; set; }
        public string StatCode { get; set; }
        public string SecCode { get; set; }
        public string CatCode { get; set; }
        public bool IsHod { get; set; }
        public string ReleaseGroupCode { get; set; }
        public string ReleaseStrategy { get; set; }
        public string ReleaseStatusCode { get; set; }
        public DateTime AddDt { get; set; }
        public string AddUser { get; set; }
        public string ClientIp { get; set; }
        public DateTime UpdDt { get; set; }
        public string UpdUser { get; set; }
        public string Remarks { get; set; }

        public EmployeeDto Employee { get; set; }
        public CompanyDto Company { get; set; }
        public WorkGroupDto WorkGroup { get; set; }
        public UnitDto Units { get; set; }
        public DepartmentDto Departments { get; set; }
        public StationDto Stations { get; set; }
        public SectionDto Sections { get; set; }
        public CategoryDto Categories { get; set; }
        public ReleaseGroupDto ReleaseGroup { get; set; }
        public ReleaseStrategyDto RelStrategy { get; set; }

        public bool Cancelled { get; set; }
        public int ParentId { get; set; }

        public List<LeaveApplicationDetailDto> LeaveApplicationDetails { get; set; }
        public List<ApplReleaseStatusDto> ApplReleaseStatus { get; set; }
    }
}