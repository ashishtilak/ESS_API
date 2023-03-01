using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class EmpUniformDto
    {
        public string EmpUnqId { get; set; }
        public int Year { get; set; }

        public int TrouserSize;
        public int ShirtSize;

        public string UpdUser;
        public DateTime UpdTime;
    }
}