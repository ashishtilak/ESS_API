using System.ComponentModel.DataAnnotations;

namespace ESS.Models
{
    public class ReleaseStatus
    {
        [Key] [Required] [StringLength(1)] public string ReleaseStatusCode { get; set; }

        [StringLength(20)] public string ReleaseStatusDesc { get; set; }


        //ADDING STATIC MEMBERS TO ELIMINATE MAGIC STRINGS
        public static readonly string NotReleased = "N";
        public static readonly string FullyReleased = "F";
        public static readonly string InRelease = "I";
        public static readonly string PartiallyReleased = "P";
        public static readonly string ReleaseRejected = "R";
        public static readonly string Closed = "C"; //used in GpAdvice when store has posted the same
    }
}