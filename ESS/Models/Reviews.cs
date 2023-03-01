using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class Reviews
    {
        [Key] [StringLength(10)] public string EmpUnqId { get; set; }
        [ForeignKey("EmpUnqId")] public Employees Employee { get; set; }

        public int ReviewQtr { get; set; }
        public DateTime JoinDt { get; set; }

        [StringLength(1)] public string ConfirmationStatus { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? AddDt { get; set; }
        [StringLength(10)] public string AddUser { get; set; }

        public static readonly string NotStarted = "N";
        public static readonly string UnderReview = "P";
        public static readonly string Extended = "E";
        public static readonly string Confirmed = "F";
    }
}