using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class ReimbursementController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public ReimbursementController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetReimb(string reimbType, DateTime fromDate, DateTime toDate)
        {
            var reimb = _context.Reimbursement
                .Where(r =>
                    r.ReimbType == reimbType &&
                    r.ReimbDate >= fromDate && r.ReimbDate <= toDate)
                .Select(Mapper.Map<Reimbursements, ReimbursementDto>)
                .ToList();

            //Check if record exist
            if (reimb.Count == 0)
                return BadRequest("No records found.");


            switch (reimbType)
            {
                case Reimbursements.CarReimb:
                    return BadRequest("Not implemented.");
                case Reimbursements.ConveyenceReimb:
                    return Ok(GetReimbConv(reimb));
                default:
                    return BadRequest("Incorrect reimbType.");
            }
        }

        [HttpPost]
        public IHttpActionResult CreateReimb([FromBody] object requestData)
        {
            ReimbursementDto reimbDto;
            try
            {
                reimbDto = JsonConvert.DeserializeObject<ReimbursementDto>(requestData.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.ToString());
            }

            if (!ModelState.IsValid)
                return BadRequest("Invalid model state.");

            if (reimbDto.ReimbId != 0)
                return BadRequest("ID should be zero!");

            int maxId;

            try
            {
                maxId = _context.Reimbursement.Max(i => i.ReimbId);
            }
            catch
            {
                maxId = 0;
            }

            reimbDto.ReimbId = maxId + 1;

            try
            {
                //validate and update depending upon reimbursement type
                switch (reimbDto.ReimbType)
                {
                    case Reimbursements.ConveyenceReimb:
                        reimbDto = CreateReimbConv(reimbDto);
                        break;
                    default:
                        break;
                }

                // Now we have Reimbursement dto ready to upload in DB

                // set release status and add date
                reimbDto.ReleaseStatusCode = ReleaseStatus.NotReleased;
                reimbDto.AddDateTime = DateTime.Now;


                // Get release strategy
                ReleaseStrategies relStrat = _context.ReleaseStrategy
                    .FirstOrDefault(
                        r =>
                            r.ReleaseGroupCode == reimbDto.ReleaseGroupCode &&
                            r.ReleaseStrategy == reimbDto.EmpUnqId &&
                            r.Active == true
                    );

                if (relStrat == null)
                    return BadRequest("Release strategy not configured.");


                //get release strategy levels
                var relStratLevels = _context.ReleaseStrategyLevels
                    .Where(
                        rl =>
                            rl.ReleaseGroupCode == reimbDto.ReleaseGroupCode &&
                            rl.ReleaseStrategy == relStrat.ReleaseStrategy
                    ).ToList();

                //add them to release strategy object
                relStrat.ReleaseStrategyLevels = relStratLevels;

                //Now for each release strategy details record create ApplReleaseStatus record

                var apps = new List<ApplReleaseStatusDto>();

                foreach (ReleaseStrategyLevels level in relStrat.ReleaseStrategyLevels)
                {
                    //get releaser ID from ReleaseAuth model
                    ReleaseAuth relAuth = _context.ReleaseAuth
                        .FirstOrDefault(ra => ra.ReleaseCode == level.ReleaseCode);

                    if (relAuth == null)
                        return BadRequest("Release auth not set for release code " + level.ReleaseCode);

                    ApplReleaseStatus appRelStat = new ApplReleaseStatus
                    {
                        YearMonth = reimbDto.YearMonth,
                        ReleaseGroupCode = reimbDto.ReleaseGroupCode,
                        ApplicationId = reimbDto.ReimbId,
                        ReleaseStrategy = level.ReleaseStrategy,
                        ReleaseStrategyLevel = level.ReleaseStrategyLevel,
                        ReleaseCode = level.ReleaseCode,
                        ReleaseStatusCode =
                            level.ReleaseStrategyLevel == 1
                                ? ReleaseStatus.InRelease
                                : ReleaseStatus.NotReleased,
                        ReleaseDate = null,
                        ReleaseAuth = relAuth.EmpUnqId,
                        IsFinalRelease = level.IsFinalRelease
                    };

                    //add to collection
                    apps.Add(Mapper.Map<ApplReleaseStatus, ApplReleaseStatusDto>(appRelStat));

                    _context.ApplReleaseStatus.Add(appRelStat);
                }

                // all rel strategy level records are now in app release

                _context.Reimbursement.Add(Mapper.Map<ReimbursementDto, Reimbursements>(reimbDto));
                _context.SaveChanges();

                reimbDto.ApplReleaseStatus = new List<ApplReleaseStatusDto>();
                reimbDto.ApplReleaseStatus.AddRange(apps);

                return Ok(reimbDto);
            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.ToString());
            }
        }

        private class ReimbUpdt
        {
            public int ReimbId { get; set; }
            public float AmountReleased { get; set; }
        }

        [HttpPost]
        public IHttpActionResult UpdateReimb([FromBody] object requestData, bool flag)
        {
            var req = JsonConvert.DeserializeObject<ReimbUpdt>(requestData.ToString());

            var reimb = _context.Reimbursement.SingleOrDefault(r => r.ReimbId == req.ReimbId);
            if (reimb == null)
                return BadRequest("Reimbursement Id not found.");

            reimb.AmountReleased = req.AmountReleased;
            _context.SaveChanges();
            return Ok();
        }

        private List<ReimbursementDto> GetReimbConv(List<ReimbursementDto> reimb)
        {
            foreach (ReimbursementDto dto in reimb)
            {
                var conv = _context.ReimbConvs
                    .Where(r => r.ReimbId == dto.ReimbId && r.ReimbType == dto.ReimbType)
                    .Select(Mapper.Map<ReimbConv, ReimbConvDto>)
                    .ToList();

                dto.ReimbConv.AddRange(conv);
            }

            return reimb;
        }

        private ReimbursementDto CreateReimbConv(ReimbursementDto reimbDto)
        {
            //also add any validations required....
            var conv = new List<ReimbConvDto>();

            foreach (ReimbConvDto dto in reimbDto.ReimbConv)
            {
                dto.ReimbId = reimbDto.ReimbId;
                dto.YearMonth = reimbDto.YearMonth;
                conv.Add(dto);
            }

            reimbDto.ReimbConv.RemoveAll(e => e.ReimbId != 0);
            reimbDto.ReimbConv.AddRange(conv);

            return reimbDto;
        }
    }
}