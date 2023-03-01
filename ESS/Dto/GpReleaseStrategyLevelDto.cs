using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class GpReleaseStrategyLevelDto
    {
        public string ReleaseGroupCode { get; set; }
        public string GpReleaseStrategy { get; set; }
        public int GpReleaseStrategyLevel { get; set; }
        public string ReleaseCode { get; set; }
        public bool IsFinalRelease { get; set; }

        public ReleaseGroupDto ReleaseGroup { get; set; }
        public GpReleaseStrategyDto GpReleaseStrategies { get; set; }

        //these are releaser details
        public string EmpUnqId { get; set; }
        public string EmpName { get; set; }

        public bool IsGpNightReleaser { get; set; }
    }
}