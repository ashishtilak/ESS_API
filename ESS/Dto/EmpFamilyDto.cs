using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class EmpFamilyDto
    {
        public string EmpUnqId { get; set; }
        public int Sr { get; set; }
        public string Name { get; set; }
        public string Relation { get; set; }
        public DateTime? BirthDt { get; set; }
        public string Education { get; set; }
        public string Occupation { get; set; }
    }
}