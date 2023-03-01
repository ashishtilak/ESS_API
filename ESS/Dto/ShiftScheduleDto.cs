using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class ShiftScheduleDto
    {
        public int YearMonth { get; set; }
        public int ScheduleId { get; set; }
        public string EmpUnqId { get; set; }
        public EmployeeDto Employee { get; set; }
        public string CompCode { get; set; }
        public CompanyDto Company { get; set; }
        public string WrkGrp { get; set; }
        public WorkGroupDto WorkGroup { get; set; }
        public string UnitCode { get; set; }
        public UnitDto Units { get; set; }
        public string DeptCode { get; set; }
        public DepartmentDto Departments { get; set; }
        public string StatCode { get; set; }
        public StationDto Stations { get; set; }
        public string ReleaseGroupCode { get; set; }
        public ReleaseGroupDto ReleaseGroup { get; set; }
        public string ReleaseStrategy { get; set; }
        public ReleaseStrategyDto RelStrategy { get; set; }
        public string ReleaseStatusCode { get; set; }
        public DateTime? ReleaseDt { get; set; }
        public string ReleaseUser { get; set; }
        public DateTime? AddDt { get; set; }
        public string AddUser { get; set; }
        public string Remarks { get; set; }


        public List<ShiftScheduleDetailDto> ShiftScheduleDetails { get; set; }
        public List<ApplReleaseStatusDto> ApplReleaseStatus { get; set; }

        public string EmpName { get; set; }
        public string DeptName { get; set; }
        public string StatName { get; set; }
        public string ModeName { get; set; }
        public string StatusName { get; set; }


        public string AddUserName { get; set; }
    }
}