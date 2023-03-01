using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class ReleaseStrategyLevelDto
    {
        public string ReleaseGroupCode { get; set; }
        public string ReleaseStrategy { get; set; }
        public int ReleaseStrategyLevel { get; set; }
        public string ReleaseCode { get; set; }
        public bool IsFinalRelease { get; set; }

        public ReleaseGroupDto ReleaseGroup { get; set; }
        public ReleaseStrategyDto ReleaseStrategies { get; set; }

        //these are releaser details
        public string EmpUnqId { get; set; }
        public string EmpName { get; set; }
    }
}