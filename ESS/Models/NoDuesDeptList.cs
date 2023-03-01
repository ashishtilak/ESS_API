using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class NoDuesDeptList
    {
        [Key, Column(Order = 0), StringLength(3)]
        public string DeptId { get; set; }

        public int Index { get; set; }
        [StringLength(20)] public string DeptName { get; set; }

        public static readonly string Admin = "ADM";
        public static readonly string Canteen = "CAN";
        public static readonly string Electrical = "ELE";
        public static readonly string Finance = "FIN";
        public static readonly string HoD = "HOD";
        public static readonly string Ohc = "OHC";
        public static readonly string PrgHr = "PRG";
        public static readonly string Safety = "SAF";
        public static readonly string School = "SCH";
        public static readonly string Security = "SEC";
        public static readonly string Stores = "STO";
        public static readonly string Township = "TOW";
        public static readonly string It = "IT";
        public static readonly string Hr = "HR";
        public static readonly string Er = "ER";
    }
}