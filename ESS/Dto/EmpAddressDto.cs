using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class EmpAddressDto
    {
        public string EmpUnqId { get; set; }
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
        public DateTime UpdDt { get; set; }
        public bool HrVerified { get; set; }
        public bool HrUser { get; set; }
        public DateTime? HrVerificationDate { get; set; }

        public string HrCity { get; set; }
        public string HrSociety { get; set; }
        public string HrRemarks { get; set; }
    }
}