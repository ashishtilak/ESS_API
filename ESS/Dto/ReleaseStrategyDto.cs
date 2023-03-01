using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class ReleaseStrategyDto
    {
        public string ReleaseGroupCode { get; set; }
        public string ReleaseStrategy { get; set; }
        public string ReleaseStrategyName { get; set; }
        public bool Active { get; set; }

        public string CompCode { get; set; }
        public string WrkGrp { get; set; }
        public string UnitCode { get; set; }
        public string DeptCode { get; set; }
        public string StatCode { get; set; }

        public string SecCode { get; set; }
        //        public string CatCode { get; set; }

        public ReleaseGroupDto ReleaseGroup { get; set; }
        public CompanyDto Company { get; set; }
        public WorkGroupDto WorkGroup { get; set; }
        public UnitDto Unit { get; set; }
        public DepartmentDto Department { get; set; }
        public StationDto Stations { get; set; }

        public SectionDto Sections { get; set; }

        //        public CategoryDto Category { get; set; }
        public bool IsHod { get; set; }

        public DateTime? UpdDt { get; set; }
        public string UpdUser { get; set; }

        public List<ReleaseStrategyLevelDto> ReleaseStrategyLevels;
    }
}