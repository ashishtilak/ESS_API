using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class VehicleReqDto
    {
        public int ReqId { get; set; }

        public string EmpUnqId { get; set; }
        public EmployeeDto Employee { get; set; }

        public DateTime ReqDate { get; set; }
        public DateTime BookingDate { get; set; }

        public int BookingSlot;

        public bool BookingStatus { get; set; }

        public DateTime PickupTime { get; set; } //date will be booking date
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
        public int NumberOfPass { get; set; }
        public string Remarks { get; set; }

        public DateTime? AddDt { get; set; }
        public string AddUser { get; set; }

        // EMPLOYEE DETAILS

        public string EmpName { get; set; }
        public string DeptName { get; set; }
        public string StatName { get; set; }
        public string CatName { get; set; }
        public string GradeName { get; set; }

        // RELEASE DETAILS

        public string ReleaseGroupCode { get; set; }
        public ReleaseGroupDto ReleaseGroup { get; set; }

        public string ReleaseStrategy { get; set; }


        public ReleaseStrategyDto RelStrategy { get; set; }

        public string ReleaseCode { get; set; }

        public string ReleaseStatusCode { get; set; }


        public string ReleaseRemarks { get; set; }

        public string ReleaseAuth { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public string AdminReleaseStatusCode { get; set; }


        public string AdminUser { get; set; }
        public DateTime? AdminReleaseDate { get; set; }
        public string AdminReleaseRemarks { get; set; }
    }
}