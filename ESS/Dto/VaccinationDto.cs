using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class VaccinationDto
    {
        public string EmpUnqId { get; set; }
        public EmployeeDto Employee { get; set; }

        public DateTime? FirstDoseDate { get; set; }
        public DateTime? SecondDoseDate { get; set; }
        public DateTime? ThirdDoseDate { get; set; }
        

        public DateTime UpdateDate { get; set; }
    }
}