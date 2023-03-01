using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class GpReleaseStrategyDto
    {
        public string ReleaseGroupCode { get; set; }
        public string GpReleaseStrategy { get; set; }
        public string GpReleaseStrategyName { get; set; }
        public bool Active { get; set; }

        public string CompCode { get; set; }
        public string WrkGrp { get; set; }
        public string UnitCode { get; set; }
        public string DeptCode { get; set; }
        public string StatCode { get; set; }

        public ReleaseGroupDto ReleaseGroup { get; set; }
        public CompanyDto Company { get; set; }
        public WorkGroupDto WorkGroup { get; set; }
        public UnitDto Unit { get; set; }
        public DepartmentDto Department { get; set; }
        public StationDto Stations { get; set; }

        public DateTime? UpdDt { get; set; }
        public string UpdUser { get; set; }

        public bool NightFlag { get; set; }

        public List<GpReleaseStrategyLevelDto> GpReleaseStrategyLevels;
    }
}