using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.Models
{
    public class LeaveBalance
    {
        [Key, Column(Order = 0), StringLength(2)]
        public string CompCode { get; set; }

        [ForeignKey("CompCode")] public Company Company { get; set; }

        [Key, Column(Order = 1), StringLength(10)]
        public string WrkGrp { get; set; }

        [ForeignKey("CompCode, WrkGrp")] public WorkGroups WorkGroup { get; set; }

        [Key, Column(Order = 2)] public int YearMonth { get; set; }

        [Key, Column(Order = 3), ForeignKey("Employees"), StringLength(10)]
        public string EmpUnqId { get; set; }

        public Employees Employees { get; set; }

        [Key, Column(Order = 4), StringLength(2)]
        public string LeaveTypeCode { get; set; }

        [ForeignKey("CompCode, WrkGrp, LeaveTypeCode")]
        public LeaveTypes LeaveTypes { get; set; }

        public float Opening { get; set; }

        public float Availed { get; set; }

        public float Balance { get; set; }

        public float Encashed { get; set; }
    }
}