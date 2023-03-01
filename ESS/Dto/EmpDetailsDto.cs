using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class EmpDetailsDto
    {
        public string EmpUnqId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string ContactNo { get; set; }
        public string BloodGroup { get; set; }

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
        public string PrePoliceSt { get; set; }
        public string PreEmail { get; set; }

        public string IdPrf1 { get; set; }
        public string IdPrf1No { get; set; }
        public DateTime? IdPrf1ExpOn { get; set; }
        public string IdPrf2 { get; set; }
        public string IdPrf2No { get; set; }
        public DateTime? IdPrf2ExpOn { get; set; }
        public string IdPrf3 { get; set; }
        public string IdPrf3No { get; set; }
        public DateTime? IdPrf3ExpOn { get; set; }

        public string Religion { get; set; }
        public string Category { get; set; }

        public string SapId { get; set; }
        public DateTime? JoinDt { get; set; }
        public string BankAcNo { get; set; }
        public string BankName { get; set; }
        public string BankIfsc { get; set; }
        public string AadharNo { get; set; }
    }
}