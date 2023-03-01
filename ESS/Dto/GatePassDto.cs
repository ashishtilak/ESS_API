using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class GatePassDto
    {
        public int Id { get; set; }
        public int YearMonth { get; set; }
        public DateTime GatePassDate { get; set; }
        public int GatePassNo { get; set; }
        public int GatePassItem { get; set; }
        public string EmpUnqId { get; set; }
        public string Mode { get; set; }
        public string PlaceOfVisit { get; set; }
        public string Reason { get; set; }
        public string AddUser { get; set; }
        public DateTime AddDateTime { get; set; }
        public string GatePassStatus { get; set; }
        public DateTime? GateOutDateTime { get; set; }
        public string GateOutUser { get; set; }
        public string GateOutIp { get; set; }
        public DateTime? GateInDateTime { get; set; }
        public string GateInUser { get; set; }
        public string GateInIp { get; set; }

        public string ReleaseGroupCode { get; set; }
        public string ReleaseStrategy { get; set; }
        public string ReleaseStatusCode { get; set; }

        public string GpRemarks { get; set; }
        public DateTime? AttdUpdate { get; set; }
        public string AttdFlag { get; set; }

        public DateTime? AttdGpOutTime { get; set; }
        public DateTime? AttdGpInTime { get; set; }

        public string EmpName { get; set; }
        public string UnitName { get; set; }
        public string DeptName { get; set; }
        public string StatName { get; set; }
        public string ModeName { get; set; }
        public string StatusName { get; set; }

        public string WrkGrp { get; set; }
        public string CatName { get; set; }
        public string DesgName { get; set; }

        public string BarCode { get; set; }

        public string GetMode(string mode)
        {
            if (mode == GatePass.GatePassModes.Official)
                return "Official";
            if (mode == GatePass.GatePassModes.Personal)
                return "Personal";
            if (mode == GatePass.GatePassModes.DutyOff)
                return "Duty Off";

            return "";
        }

        public string GetStatus(string status)
        {
            if (status == GatePass.GatePassStatuses.New)
                return "New";
            if (status == GatePass.GatePassStatuses.Out)
                return "Out";
            if (status == GatePass.GatePassStatuses.In)
                return "In";
            if (status == GatePass.GatePassStatuses.ForceClosed)
                return "Force Closed";

            return "";
        }

        public string GetBarcode(string empUnqId, int id)
        {
            return empUnqId + ":" + id.ToString("00000000");
        }


        public List<ApplReleaseStatusDto> ApplReleaseStatus { get; set; }
    }
}