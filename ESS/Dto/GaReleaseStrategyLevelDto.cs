using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class GaReleaseStrategyLevelDto
    {
        public string ReleaseGroupCode { get; set; }
        public string GaReleaseStrategy { get; set; }
        public int GaReleaseStrategyLevel { get; set; }
        public string ReleaseCode { get; set; }
        public bool IsFinalRelease { get; set; }

        public ReleaseGroupDto ReleaseGroup { get; set; }
        public GaReleaseStrategyDto GaReleaseStrategies { get; set; }

        //these are releaser details
        public string EmpUnqId { get; set; }
        public string EmpName { get; set; }
    }
}