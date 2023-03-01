using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace ESS.Models
{
    public class SapUserIds
    {
        [Key] [StringLength(12)] public string SapUserId { get; set; }
        [StringLength(10)] public string EmpUnqId { get; set; }
        [StringLength(2)] public string LineOfBusiness { get; set; }

        // following is required only for common ids
        [StringLength(50)] public string UserName { get; set; }
        [StringLength(50)] public string DeptName { get; set; }
        public bool IsCommon { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime? ValidTo { get; set; }
        [StringLength(50)] public string Remarks { get; set; }
    }
}