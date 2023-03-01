using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.Models
{
    public class MedEmpUhid
    {
        [Key] [Column(Order = 0)] public int PolicyYear { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string EmpUnqId { get; set; }

        [Key] [Column(Order = 2)] public int DepSr { get; set; }
        [ForeignKey("EmpUnqId, DepSr")] public MedDependent Dependent { get; set; }

        [StringLength(25)] public string Uhid { get; set; }

        public bool Active { get; set; }
    }
}