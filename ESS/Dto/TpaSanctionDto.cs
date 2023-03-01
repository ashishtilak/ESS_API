using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class TpaSanctionDto
    {
        public int Id { get; set; }

        public string EmpUnqId { get; set; }

        public string EmpName {get;set;}
        public string CatName { get; set; }
        public string DeptName { get; set; }
        public string StatName { get; set; }
        public string GradeName { get; set; }
        public string DesgName { get; set; }

        public DateTime TpaDate { get; set; } //store only date
        public string TpaShiftCode { get; set; } //Shift at the time of entry
        public float RequiredHours { get; set; }
        public string PreJustification { get; set; }
        public string ReleaseGroupCode { get; set; }
        public ReleaseGroupDto ReleaseGroup { get; set; }
        public string ReleaseStrategy { get; set; }
        
        public string PreReleaseStatusCode { get; set; }
        public string PreRemarks { get; set; }

        public DateTime? AddDt { get; set; }
        public string AddUser { get; set; }
        public string AddUserName { get; set; }

        // POST 
        public string ActShiftCode { get; set; } //Actual shift
        public float WrkHours { get; set; } //Actual wrk hours
        public float SanctionTpa { get; set; } //Sanctioned hours

        public DateTime? ConsIn { get; set; }
        public DateTime? ConsOut { get; set; }
        public float ConsWrkHrs { get; set; }
        public float ShiftHrs {get; set;}
        public float ConsOverTime { get; set; }
        public float CalcOverTime { get; set; }
        public string Status { get; set; }
        public bool HalfDay { get; set; }
        public string LeaveType { get; set; }
        public bool LeaveHalf { get; set; }
        public string Earlycome { get; set; }
        public string EarlyGoing { get; set; }
        public string LateCome { get; set; }


        public string PostJustification { get; set; } //Justification if reqd > actual

        public string PostReleaseStatusCode { get; set; }
        public string PostRemarks { get; set; }

        public string HrReleaseStatusCode { get; set; }
        public string HrPostRemarks { get; set; }
        public string HrUser { get; set; }
        public string HrUserName { get; set; }
        public DateTime? HrReleaseDate { get; set; }

        public List<TpaReleaseDto> TpaReleaseStatus { get; set; }
    }
}