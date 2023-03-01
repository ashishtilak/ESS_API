using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class ProgressReviewController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public ProgressReviewController()
        {
            _context = new ApplicationDbContext();
        }

        // GET all performance review records between dates
        [HttpGet, ActionName("getallreviews")]
        public IHttpActionResult GetAllReviews(DateTime fromDate, DateTime toDate, bool pendingOnly)
        {
            var qry = _context.ReviewDetails
                .Where(r => r.ReviewDate >= fromDate && r.ReviewDate <= toDate);

            // if pending only is true only return pending  reviews (not confirmation)

            if (!pendingOnly)
                qry = qry.Where(p => p.HrReleaseStatusCode == ReleaseStatus.FullyReleased);

            List<ReviewDetailDto> reviewDtls = qry.AsEnumerable()
                .Select(Mapper.Map<ReviewDetails, ReviewDetailDto>)
                .ToList();

            if (reviewDtls.Count == 0)
                return BadRequest("No records found!");

            foreach (ReviewDetailDto dto in reviewDtls)
            {
                EmployeeDto employeeDto = _context.Employees
                    .Select(e => new EmployeeDto
                    {
                        EmpUnqId = e.EmpUnqId,
                        EmpName = e.EmpName,
                        FatherName = e.FatherName,
                        Active = e.Active,
                        Pass = e.Pass,

                        CompCode = e.CatCode,
                        WrkGrp = e.WrkGrp,
                        UnitCode = e.UnitCode,
                        DeptCode = e.DeptCode,
                        StatCode = e.StatCode,
                        CatCode = e.CatCode,
                        EmpTypeCode = e.EmpTypeCode,
                        GradeCode = e.GradeCode,
                        DesgCode = e.DesgCode,
                        IsHod = e.IsHod,

                        CompName = e.Company.CompName,
                        WrkGrpDesc = e.WorkGroup.WrkGrpDesc,
                        UnitName = e.Units.UnitName,
                        DeptName = e.Departments.DeptName,
                        StatName = e.Stations.StatName,
                        CatName = e.Categories.CatName,
                        EmpTypeName = e.EmpTypes.EmpTypeName,
                        GradeName = e.Grades.GradeName,
                        DesgName = e.Designations.DesgName,
                        JoinDate = e.JoinDate,
                        Location = e.Location
                    })
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                dto.Employee = employeeDto;
                dto.ReleaseEmpName = _context.Employees.FirstOrDefault(e=>e.EmpUnqId == dto.ReleaseCode)?.EmpName;
                dto.AddEmpName = _context.Employees.FirstOrDefault(e=>e.EmpUnqId == dto.AddUser)?.EmpName;
                dto.HrEmpName = _context.Employees.FirstOrDefault(e=>e.EmpUnqId == dto.HrUser)?.EmpName;

            }

            return Ok(reviewDtls);
        }

        // GET PENDING REVIEW for Given Supervisor (first level releaser)
        [HttpGet, ActionName("getreviews")]
        public IHttpActionResult GetReviews(string empUnqId, bool relFlag)
        {
            // IF relFlag is false, provide 1st level details, else 2nd level

            string[] relCodes = _context.ReleaseAuth
                .Where(r => r.EmpUnqId == empUnqId && !r.ReleaseCode.Contains("GP_"))
                .Select(r => r.ReleaseCode).ToArray();

            IQueryable<ReleaseStrategyLevels> relStrQry = _context.ReleaseStrategyLevels
                .Where(r =>
                    r.ReleaseGroupCode == ReleaseGroups.ProgressReview &&
                    relCodes.Contains(r.ReleaseCode));

            relStrQry = relFlag
                ? relStrQry.Where(r => r.ReleaseStrategyLevel == 2)
                : relStrQry.Where(r => r.ReleaseStrategyLevel == 1);


            string[] relStr = relStrQry.Select(r => r.ReleaseStrategy).ToArray();

            var targetDate = DateTime.Today.AddDays(10);

            var reviewsQry = _context.ReviewDetails
                .Where(r => relStr.Contains(r.EmpUnqId) &&
                            r.ReviewDate <= targetDate);


            // if relflag is true (for second level releaser)
            // check for release status code,
            // else check for add release status code
            reviewsQry = relFlag == true
                ? reviewsQry.Where(r => r.ReleaseStatusCode == ReleaseStatus.InRelease)
                : reviewsQry.Where(r => r.AddReleaseStatusCode == ReleaseStatus.InRelease);

            List<ReviewDetailDto> reviews = reviewsQry.AsEnumerable()
                .Select(Mapper.Map<ReviewDetails, ReviewDetailDto>)
                .ToList();

            foreach (ReviewDetailDto dto in reviews)
            {
                EmployeeDto employeeDto = _context.Employees
                    .Select(e => new EmployeeDto
                    {
                        EmpUnqId = e.EmpUnqId,
                        EmpName = e.EmpName,
                        FatherName = e.FatherName,
                        Active = e.Active,
                        Pass = e.Pass,

                        CompCode = e.CatCode,
                        WrkGrp = e.WrkGrp,
                        UnitCode = e.UnitCode,
                        DeptCode = e.DeptCode,
                        StatCode = e.StatCode,
                        CatCode = e.CatCode,
                        EmpTypeCode = e.EmpTypeCode,
                        GradeCode = e.GradeCode,
                        DesgCode = e.DesgCode,
                        IsHod = e.IsHod,

                        CompName = e.Company.CompName,
                        WrkGrpDesc = e.WorkGroup.WrkGrpDesc,
                        UnitName = e.Units.UnitName,
                        DeptName = e.Departments.DeptName,
                        StatName = e.Stations.StatName,
                        CatName = e.Categories.CatName,
                        EmpTypeName = e.EmpTypes.EmpTypeName,
                        GradeName = e.Grades.GradeName,
                        DesgName = e.Designations.DesgName,
                        JoinDate = e.JoinDate,
                        Location = e.Location
                    })
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                dto.Employee = employeeDto;
            }

            return Ok(reviews);
        }

        [HttpGet, ActionName("getreviewqtr")]
        public IHttpActionResult GetReviewQtr(string empUnqId)
        {
            Employees emp = _context.Employees.FirstOrDefault(e => e.EmpUnqId == empUnqId);
            if (emp == null)
                return BadRequest("Invalid employee code. Try to sync first!");

            var revQtr = _context.GradeReviews.FirstOrDefault(g => g.GradeCode == emp.GradeCode);

            if (revQtr == null)
                return BadRequest("Review qtr not maintained for grade: " + emp.GradeCode);

            return Ok(revQtr.ReviewQtr);
        }

        // FIRST ENTRY BY HR USER
        [HttpPost, ActionName("PostReview")]
        public IHttpActionResult PostReview([FromBody] object requestData)
        {
            var revDto = JsonConvert.DeserializeObject<ReviewDto>(requestData.ToString());

            var review = _context.Reviews
                .FirstOrDefault(r => r.EmpUnqId == revDto.EmpUnqId);

            if (review != null)
                return BadRequest("Record already exist.");

            var emp = _context.Employees.FirstOrDefault(e => e.EmpUnqId == revDto.EmpUnqId);
            if (emp == null)
                return BadRequest("Employee does not exist. Try to sync first!");

            using (DbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                review = new Reviews
                {
                    EmpUnqId = revDto.EmpUnqId,
                    JoinDt = emp.JoinDate ?? DateTime.Today,
                    AddDt = DateTime.Now,
                    AddUser = revDto.AddUser,
                    ReviewQtr = revDto.ReviewQtr,
                    ConfirmationStatus = Reviews.NotStarted
                };

                _context.Reviews.Add(review);

                for (var i = 0; i < revDto.ReviewQtr; i++)
                {
                    int qtrNo = i + 1;
                    var dtl = new ReviewDetails
                    {
                        EmpUnqId = revDto.EmpUnqId,
                        ReviewQtrNo = qtrNo,
                        IsConfirmation = i == revDto.ReviewQtr - 1,
                        ReviewDate = review.JoinDt.AddMonths(qtrNo * 3),
                        PeriodFrom = review.JoinDt.AddMonths(i * 3),
                        PeriodTo = review.JoinDt.AddMonths(qtrNo * 3).AddDays(-1),
                        Recommendation = ReviewDetails.NotProcessed,
                        AddReleaseStatusCode = i==0 ? ReleaseStatus.InRelease : ReleaseStatus.NotReleased,
                        ReleaseGroupCode = ReleaseGroups.ProgressReview,
                        ReleaseStrategy = revDto.EmpUnqId,
                        ReleaseStatusCode = ReleaseStatus.NotReleased
                    };
                    _context.ReviewDetails.Add(dtl);
                }

                _context.SaveChanges();
                transaction.Commit();
            }

            return Ok();
        }

        // FOR SUPERVISOR REVIEW SUBMIT
        [HttpPost, ActionName("submitreview")]
        public IHttpActionResult SubmitReview([FromBody] object requestData)
        {
            var dto = JsonConvert.DeserializeObject<ReviewDetailDto>(requestData.ToString());
            var reviewDtl = _context.ReviewDetails.FirstOrDefault(
                r => r.EmpUnqId == dto.EmpUnqId && r.ReviewQtrNo == dto.ReviewQtrNo);

            if (reviewDtl == null)
                return BadRequest("Review details not found!");

            // get first level releaser details
            var rel = _context.ReleaseStrategyLevels.FirstOrDefault(
                r => r.ReleaseGroupCode == ReleaseGroups.ProgressReview &&
                     r.ReleaseStrategy == dto.EmpUnqId &&
                     r.ReleaseStrategyLevel == 1 &&
                     r.IsFinalRelease == false
            );
            if (rel == null)
                return BadRequest("First level Release is not configured.");


            //TODO: ADD CHECKS AND VALIDATIONS HERE

            reviewDtl.Assignments = dto.Assignments;
            reviewDtl.Strength = dto.Strength;
            reviewDtl.Improvements = dto.Improvements;
            reviewDtl.Suggestions = dto.Suggestions;
            reviewDtl.Rating = dto.Rating;
            reviewDtl.Remarks = dto.Remarks;
            reviewDtl.Recommendation = dto.Recommendation;
            reviewDtl.AddDt = DateTime.Now;
            reviewDtl.AddReleaseCode = dto.AddReleaseCode;
            reviewDtl.AddReleaseStatusCode = ReleaseStatus.FullyReleased;
            reviewDtl.AddUser = dto.AddUser;

            reviewDtl.ReleaseGroupCode = ReleaseGroups.ProgressReview;
            reviewDtl.ReleaseStrategy = dto.EmpUnqId;
            reviewDtl.ReleaseCode = rel.ReleaseCode;
            reviewDtl.ReleaseStatusCode = ReleaseStatus.InRelease;

            _context.SaveChanges();

            return Ok(Mapper.Map<ReviewDetails, ReviewDetailDto>(reviewDtl));
        }

        // FOR HOD REVIEW RELEASE
        [HttpPost, ActionName("ReleaseReview")]
        public IHttpActionResult ReleaseReview([FromBody] object requestData)
        {
            var dto = JsonConvert.DeserializeObject<ReviewDetailDto>(requestData.ToString());
            var reviewDtl = _context.ReviewDetails.FirstOrDefault(
                r => r.EmpUnqId == dto.EmpUnqId && r.ReviewQtrNo == dto.ReviewQtrNo);

            if (reviewDtl == null)
                return BadRequest("Review details not found!");

            // get second level releaser details
            var rel = _context.ReleaseStrategyLevels.FirstOrDefault(
                r => r.ReleaseGroupCode == ReleaseGroups.ProgressReview &&
                     r.ReleaseStrategy == dto.EmpUnqId &&
                     r.ReleaseStrategyLevel == 2 &&
                     r.IsFinalRelease == true
            );
            if (rel == null)
                return BadRequest("Second level Release is not configured.");


            //TODO: ADD CHECKS AND VALIDATIONS HERE

            reviewDtl.Recommendation = dto.Recommendation;
            reviewDtl.Rating = dto.Rating;
            reviewDtl.ReleaseCode = dto.ReleaseCode;
            reviewDtl.ReleaseDate = DateTime.Now;
            reviewDtl.ReleaseStatusCode = dto.ReleaseStatusCode;
            reviewDtl.HrReleaseStatusCode = ReleaseStatus.InRelease;
            reviewDtl.HodRemarks = dto.HodRemarks;

            if (reviewDtl.ReleaseStatusCode == ReleaseStatus.ReleaseRejected)
            {
                reviewDtl.Recommendation = ReviewDetails.NotProcessed;
                reviewDtl.AddReleaseStatusCode = ReleaseStatus.InRelease;
            }

            Reviews review = _context.Reviews.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
            if (review != null)
            {
                if (reviewDtl.IsConfirmation && dto.Recommendation == ReviewDetails.Extend)
                {
                    review.ConfirmationStatus = Reviews.Extended;
                }
                else if (reviewDtl.IsConfirmation && dto.Recommendation == ReviewDetails.Confirm)
                {
                    review.ConfirmationStatus = Reviews.Confirmed;
                }
                else if (!reviewDtl.IsConfirmation)
                {
                    review.ConfirmationStatus = Reviews.UnderReview;
                }
            }

            _context.SaveChanges();
            return Ok();
        }

        // HR GET
        [HttpGet, ActionName("getreviewshr")]
        public IHttpActionResult GetReviewsHr()
        {
            List<ReviewDetailDto> reviews = _context.ReviewDetails
                .Where(r => r.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                            !(r.HrReleaseStatusCode == ReleaseStatus.FullyReleased ||
                              r.HrReleaseStatusCode == ReleaseStatus.ReleaseRejected)
                ).AsEnumerable()
                .Select(Mapper.Map<ReviewDetails, ReviewDetailDto>)
                .ToList();

            foreach (ReviewDetailDto dto in reviews)
            {
                EmployeeDto employeeDto = _context.Employees
                    .Select(e => new EmployeeDto
                    {
                        EmpUnqId = e.EmpUnqId,
                        EmpName = e.EmpName,
                        FatherName = e.FatherName,
                        Active = e.Active,
                        Pass = e.Pass,

                        CompCode = e.CatCode,
                        WrkGrp = e.WrkGrp,
                        UnitCode = e.UnitCode,
                        DeptCode = e.DeptCode,
                        StatCode = e.StatCode,
                        CatCode = e.CatCode,
                        EmpTypeCode = e.EmpTypeCode,
                        GradeCode = e.GradeCode,
                        DesgCode = e.DesgCode,
                        IsHod = e.IsHod,

                        CompName = e.Company.CompName,
                        WrkGrpDesc = e.WorkGroup.WrkGrpDesc,
                        UnitName = e.Units.UnitName,
                        DeptName = e.Departments.DeptName,
                        StatName = e.Stations.StatName,
                        CatName = e.Categories.CatName,
                        EmpTypeName = e.EmpTypes.EmpTypeName,
                        GradeName = e.Grades.GradeName,
                        DesgName = e.Designations.DesgName,
                        JoinDate = e.JoinDate,
                        Location = e.Location
                    })
                    .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                dto.Employee = employeeDto;
            }

            return Ok(reviews);
        }

        // HR RELEASE
        [HttpPost, ActionName("HrRelease")]
        public IHttpActionResult HrRelease([FromBody] object requestData)
        {
            var dto = JsonConvert.DeserializeObject<ReviewDetailDto>(requestData.ToString());

            try
            {
                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    ReviewDetails reviewDtl = _context.ReviewDetails.FirstOrDefault(
                        r => r.EmpUnqId == dto.EmpUnqId && r.ReviewQtrNo == dto.ReviewQtrNo);

                    if (reviewDtl == null)
                        return BadRequest("Review details not found!");

                    //TODO: ADD CHECKS AND VALIDATIONS HERE

                    reviewDtl.HrReleaseStatusCode = dto.HrReleaseStatusCode;
                    reviewDtl.HrUser = dto.HrUser;
                    reviewDtl.HrReleaseDate = DateTime.Now;
                    reviewDtl.HrRemarks = dto.HrRemarks;


                    Reviews review = _context.Reviews.FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);
                    if (review != null)
                    {
                        if (reviewDtl.ReleaseStatusCode == ReleaseStatus.ReleaseRejected)
                        {
                            // IF HR REJECTS THE REQUEST THEN...
                            // RESET ALL Details FLAGS
                            reviewDtl.Recommendation = ReviewDetails.NotProcessed;
                            reviewDtl.AddReleaseStatusCode = ReleaseStatus.InRelease;
                            reviewDtl.ReleaseStatusCode = ReleaseStatus.NotReleased;
                            review.ConfirmationStatus = Reviews.UnderReview;
                        }
                        else
                        {
                            // HR HAS APPROVED.
                            // If this is confirmation, then 
                            // if extended, then add row in details table
                            // if not extended then close the review
                            if (reviewDtl.IsConfirmation && dto.Recommendation == ReviewDetails.Extend)
                            {
                                review.ConfirmationStatus = Reviews.Extended;

                                var dtl = new ReviewDetails
                                {
                                    EmpUnqId = reviewDtl.EmpUnqId,
                                    ReviewQtrNo = reviewDtl.ReviewQtrNo + 1,
                                    IsConfirmation = true,
                                    ReviewDate = reviewDtl.ReviewDate.AddMonths(3),
                                    PeriodFrom = reviewDtl.PeriodFrom.AddMonths(3),
                                    PeriodTo = reviewDtl.PeriodTo.AddMonths(3),
                                    Recommendation = ReviewDetails.NotProcessed,
                                    AddReleaseStatusCode = ReleaseStatus.InRelease,
                                    ReleaseGroupCode = ReleaseGroups.ProgressReview,
                                    ReleaseStrategy = reviewDtl.EmpUnqId,
                                    ReleaseStatusCode = ReleaseStatus.NotReleased
                                };
                                _context.ReviewDetails.Add(dtl);
                            }
                            else if (reviewDtl.IsConfirmation && dto.Recommendation != ReviewDetails.Extend)
                            {
                                review.ConfirmationStatus = Reviews.Confirmed;
                            }
                            else if (!reviewDtl.IsConfirmation)
                            {
                                review.ConfirmationStatus = Reviews.UnderReview;

                                ReviewDetails nextDtl = _context.ReviewDetails
                                    .FirstOrDefault(r=> r.EmpUnqId == dto.EmpUnqId && r.ReviewQtrNo == dto.ReviewQtrNo + 1);
                                if(nextDtl != null)
                                    nextDtl.AddReleaseStatusCode = ReleaseStatus.InRelease;

                            }
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error : " + ex);
            }

            return Ok();
        }
    }
}