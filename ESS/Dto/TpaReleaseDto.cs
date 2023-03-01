using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class TpaReleaseDto
    {
        public int Id { get; set; }

        public string ReleaseGroupCode { get; set; }
        public ReleaseGroupDto ReleaseGroup { get; set; }

        public int TpaSanctionId { get; set; }

        public string ReleaseStrategy { get; set; }

        public int ReleaseStrategyLevel { get; set; }

        public string ReleaseCode { get; set; }

        public string PreReleaseStatusCode { get; set; }

        public DateTime? PreReleaseDate { get; set; }

        public string PreReleaseAuth { get; set; }          // release user
        public string PreReleaseName { get; set; }          // release user

        public bool IsFinalRelease { get; set; }

        public string PreRemarks { get; set; }

        // POST
        public string PostReleaseStatusCode { get; set; }

        public string PostReleaseAuth { get; set; }
        public string PostReleaseName { get; set; }

        public DateTime? PostReleaseDate { get; set; }
        public string PostRemarks { get; set; }

    }
}