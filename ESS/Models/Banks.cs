using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class Banks
    {
        [Key] public int BankId { get; set; }

        [StringLength(150)] public string BankName { get; set; }
        [StringLength(10)] public string BankPan { get; set; }
    }
}