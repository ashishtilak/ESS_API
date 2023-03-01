using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class EmpAddress
    {
        [Key, Column(Order = 0)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [Key, Column(Order = 1)] public int Counter { get; set; }

        [StringLength(100)] public string HouseNumber { get; set; }

        [StringLength(100)] public string SocietyName { get; set; }

        [StringLength(50)] public string AreaName { get; set; }

        [StringLength(50)] public string LandMark { get; set; }

        [StringLength(50)] public string Tehsil { get; set; }

        [StringLength(50)] public string PoliceStation { get; set; }

        [StringLength(50)] public string PreDistrict { get; set; }

        [StringLength(50)] public string PreCity { get; set; }

        [StringLength(50)] public string PreState { get; set; }

        [StringLength(6)] public string PrePin { get; set; }

        [StringLength(20)] public string PrePhone { get; set; }

        [StringLength(20)] public string PreResPhone { get; set; }

        [StringLength(70)] public string PreEmail { get; set; }

        [StringLength(50)] public string HrCity { get; set; }
        [StringLength(50)] public string HrSociety { get; set; }
        [StringLength(50)] public string HrRemarks { get; set; }


        public DateTime UpdDt { get; set; }

        public bool HrVerified { get; set; }
        public bool HrUser { get; set; }
        public DateTime? HrVerificationDate { get; set; }
    }
}