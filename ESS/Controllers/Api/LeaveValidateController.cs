using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using ESS.Dto;
using ESS.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class LeaveValidateController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public LeaveValidateController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        public IHttpActionResult IsValid([FromBody] object requestData)
        {
            var error = new List<string>();

            LeaveApplicationDto leaveApplicationDto;
            try
            {
                leaveApplicationDto = JsonConvert.DeserializeObject<LeaveApplicationDto>(requestData.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            //get leave balance each leave wise for current year

            Employees emp = _context.Employees.Single(e => e.EmpUnqId == leaveApplicationDto.EmpUnqId);

            int yearMonth = _context.OpenMonth.Select(t => t.YearMonth).Single();
            int year = _context.OpenMonth.Select(t => t.OpenYear).Single();

            DateTime monthFirst = DateTime.ParseExact(
                $"{yearMonth.ToString().Substring(0, 4)}-{yearMonth.ToString().Substring(4, 2)}-01",
                "yyyy-MM-dd", null).AddMonths(-1);

            DateTime monthLast = DateTime.ParseExact(
                $"{yearMonth.ToString().Substring(0, 4)}-{yearMonth.ToString().Substring(4, 2)}-{"01"}",
                "yyyy-MM-dd", null).AddMonths(4).AddDays(-1);


            // CHECK 1 Reject if any leave is pending for release
            var pendingLeave = _context.LeaveApplications
                .Where(l => l.EmpUnqId == leaveApplicationDto.EmpUnqId &&
                            (l.ReleaseStatusCode == ReleaseStatus.PartiallyReleased ||
                             l.ReleaseStatusCode == ReleaseStatus.NotReleased))
                .ToList();

            if (pendingLeave.Count > 0)
            {
                error.Add("Cannot take leave if any application is pending for release.");
                return Content(HttpStatusCode.BadRequest, error);
            }
            // CHECK 1 OVER


            // Use helper class to get leavebalance form attendance server
            var leaveBalDto = Helpers.CustomHelper.GetLeaveBalance(leaveApplicationDto.EmpUnqId, year);

            //
            // Raag Bhoopali:
            // Aaroh: Sa Re Ga Pa Dha Sa.  Avroh: Sa. Dha Pa Ga Re Sa
            // Pakad: Ga Re Sa .Dh, Sa Re Ga, Pa Ga, Dha Pa Ga, Re Sa
            //

            //store weekly off and holidays in this variable
            //then deduct it in date continuation check

            float offDays = 0;

            #region ForEach_on_leaveAppDetails

            foreach (LeaveApplicationDetailDto details in leaveApplicationDto.LeaveApplicationDetails)
            {
                if (details.TotalDays <= 0)
                {
                    error.Add("Leave days less then zero????");
                    continue;
                }

                // check for PL date < 15 
                // by pass if location table flag is on

                if (!IsPlDateValid(details, emp.Location))
                {
                    error.Add("Apply for EL before 15 days.");
                    continue;
                }

                //check2: check if leave type is there in balance table
                bool leaveExist = leaveBalDto.Any(l => l.LeaveTypeCode == details.LeaveTypeCode);

                if (!leaveExist &&
                    (details.LeaveTypeCode != LeaveTypes.LeaveWithoutPay &&
                     details.LeaveTypeCode != LeaveTypes.CompOff &&
                     details.LeaveTypeCode != LeaveTypes.OtCOff &&
                     details.LeaveTypeCode != LeaveTypes.OutdoorDuty &&
                     details.LeaveTypeCode != LeaveTypes.WeekOff &&
                     details.LeaveTypeCode != LeaveTypes.Lockdown
                    ))
                {
                    error.Add("There is no balance available for leave type: " + details.LeaveTypeCode);
                    continue;
                }
                //check2 over

                //Check3 if dates are within open month
                if (details.FromDt < monthFirst || details.ToDt > monthLast)
                    error.Add(details.LeaveTypeCode + " leave date must be within open month: "
                                                    + monthFirst.ToShortDateString() + " - " +
                                                    monthLast.ToShortDateString());
                //check3 over


                if (details.LeaveTypeCode == LeaveTypes.OutdoorDuty ||
                    details.LeaveTypeCode == LeaveTypes.WeekOff ||
                    details.LeaveTypeCode == LeaveTypes.Lockdown
                   )
                    continue;

                // Re calculate days because, days passed from client are from grid
                // which are actual days (total days - holidays - weekoffs)
                // which are again deducted

                details.TotalDays = details.HalfDayFlag ? 0.5f : (details.ToDt - details.FromDt).Days + 1;

                // for CO, Take days == 1 only, as 
                // start date will be Week Off date,
                // End date will be date of CO.

                if (details.LeaveTypeCode == LeaveTypes.CompOff ||
                    details.LeaveTypeCode == LeaveTypes.OtCOff)
                    details.TotalDays = 1;

                // allow half CO in case of JFL tembhurni
                if (emp.Location == Locations.Jfl &&
                    details.LeaveTypeCode == LeaveTypes.CompOff && details.HalfDayFlag == true)
                    details.TotalDays = 0.5f;

                if (emp.Location == Locations.Ipu &&
                    details.LeaveTypeCode == LeaveTypes.OtCOff && details.HalfDayFlag == true)
                    details.TotalDays = 0.5f;

                //check4 that the date should not overlap with existing leave taken
                var existingLeave = _context.LeaveApplicationDetails
                    .Where(l =>
                        l.LeaveApplication.EmpUnqId == leaveApplicationDto.EmpUnqId &&
                        ((l.FromDt <= details.ToDt && l.ToDt >= details.FromDt) ||
                         (l.ToDt <= details.FromDt && l.FromDt >= details.ToDt)) &&
                        l.LeaveApplication.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                        l.Cancelled == false
                    )
                    .ToList();

                if (existingLeave.Count > 0)
                {
                    if (existingLeave.Any(e => e.LeaveTypeCode != LeaveTypes.OutdoorDuty))
                    {
                        error.Add(details.LeaveTypeCode + " leave date must not overlap with leave already taken. ");
                        continue;
                    }
                }

                //check4 over


                // Get holidays between from/to date and add them to off days
                var holidays =
                    Helpers.CustomHelper.GetHolidays(details.FromDt, details.ToDt, leaveApplicationDto.CompCode,
                        leaveApplicationDto.WrkGrp, emp.Location);

                details.TotalDays -= holidays.Count;

                if (details.TotalDays == 0)
                    error.Add("You cannot take " + details.LeaveTypeCode + " on a holiday.");

                offDays += holidays.Count;

                // Get weekly offs between the selected range
                var weekOffs =
                    Helpers.CustomHelper.GetWeeklyOff(details.FromDt, details.ToDt, leaveApplicationDto.EmpUnqId);

                //Check if weekoff is on holiday. If it is, then remove it.
                if (holidays.Count > 0)
                {
                    weekOffs.RemoveAll(w => holidays.Any(h => h == w));
                }

                details.TotalDays -= weekOffs.Count;

                if (details.TotalDays == 0)
                    error.Add("You cannot take " + details.LeaveTypeCode + " on weekly off day.");

                offDays += weekOffs.Count;



                //CHECK6: check leave balance other than CO

                if (details.LeaveTypeCode != LeaveTypes.CompOff &&
                    details.LeaveTypeCode != LeaveTypes.OtCOff &&
                    details.LeaveTypeCode != LeaveTypes.LeaveWithoutPay &&
                    details.LeaveTypeCode != LeaveTypes.OutdoorDuty)
                {
                    LeaveBalanceDto lb = leaveBalDto.Single(l => l.LeaveTypeCode == details.LeaveTypeCode);
                    float bal = lb.Opening - lb.Availed - lb.Encashed;

                    if (bal < details.TotalDays)
                        error.Add("Insufficient balance of " + details.LeaveTypeCode + ". Current Balance is: " + bal);
                }
                //CHECK6 OVER


                //Checks involving previous and next days leaves

                // NEW LOGIC FOR LEAVE RULE CHECKS
                // DONE ON 21-11-2022
                // 

                error.AddRange(PrevNextLeaveChecks(leaveApplicationDto.EmpUnqId, leaveApplicationDto.CompCode,
                    leaveApplicationDto.WrkGrp, emp.Location, details.LeaveTypeCode, details.TotalDays,
                    details.FromDt, details.ToDt));

                //
                // END OF NEW LEAVE RULE CHECKS DONE ON 21-11-2022
                // Get previous 6 days data in dictionary
                //

                
                //break out of loop in case of LWP                                                                                            
                if (details.LeaveTypeCode == LeaveTypes.LeaveWithoutPay)
                {
                    // check for first and last day week off 
                    // added on 05/02/2021

                    if (weekOffs.Any(fDt => fDt.Equals(details.FromDt)) ||
                        weekOffs.Any(tDt => tDt.Equals(details.ToDt))
                       )
                        error.Add("Cannot start or end LWP on week off day.");

                    if (holidays.Any(fDt => fDt.Equals(details.FromDt)) ||
                        holidays.Any(tDt => tDt.Equals(details.ToDt))
                       )
                        error.Add("Cannot start or end LWP on Holiday.");


                    continue;
                }


                // ***********************************************************************************
                // COMMENTED ON 26/11/2022
                // ***********************************************************************************
                ////var prevLeave = new Dictionary<DateTime, string>();
                ////DateTime d = details.FromDt.AddDays(-1);
                ////string strPrevLeave = "";
                ////while (d >= details.FromDt.AddDays(-6))
                ////{
                ////    prevLeave.Add(d, GetLeaveOnDate(d, leaveApplicationDto.EmpUnqId));

                ////    if (!string.IsNullOrEmpty(prevLeave[d]))
                ////        strPrevLeave += prevLeave[d] + ",";
                ////    else
                ////    {
                ////        if (Helpers.CustomHelper.GetWeeklyOff(d, d, leaveApplicationDto.EmpUnqId).Count > 0)
                ////            strPrevLeave += "WO,";
                ////        else if (Helpers.CustomHelper.GetHolidays(d, d, leaveApplicationDto.CompCode,
                ////                     leaveApplicationDto.WrkGrp, emp.Location).Count > 0)
                ////            strPrevLeave += "HL,";
                ////        else
                ////            strPrevLeave += prevLeave[d] + ",";
                ////    }

                ////    d = d.AddDays(-1);
                ////}


                //////got data in array along with WO


                //////Get next 6 days data in dictionary
                ////var nextLeave = new Dictionary<DateTime, string>();
                ////d = details.ToDt.AddDays(1);
                ////string strNextLeave = "";
                ////while (d <= details.ToDt.AddDays(6))
                ////{
                ////    nextLeave.Add(d, GetLeaveOnDate(d, leaveApplicationDto.EmpUnqId));


                ////    if (!string.IsNullOrEmpty(nextLeave[d]))
                ////        strNextLeave += nextLeave[d] + ",";
                ////    else
                ////    {
                ////        if (Helpers.CustomHelper.GetWeeklyOff(d, d, leaveApplicationDto.EmpUnqId).Count > 0)
                ////            strNextLeave += "WO,";
                ////        else if (Helpers.CustomHelper.GetHolidays(d, d, leaveApplicationDto.CompCode,
                ////                     leaveApplicationDto.WrkGrp, emp.Location).Count > 0)
                ////            strNextLeave += "HL,";
                ////        else
                ////            strNextLeave += nextLeave[d] + ",";
                ////    }

                ////    d = d.AddDays(1);
                ////}
                //////got data in array along with WO


                //////Get leave rules for both prev & next
                ////LeaveRules prevLeaveRules = _context.LeaveRules
                ////    .FirstOrDefault(l =>
                ////        strPrevLeave.StartsWith(l.LeaveRule) && details.LeaveTypeCode == l.LeaveTypeCode);

                ////LeaveRules nextLeaveRules = _context.LeaveRules
                ////    .FirstOrDefault(l =>
                ////        strNextLeave.StartsWith(l.LeaveRule) && details.LeaveTypeCode == l.LeaveTypeCode);


                // ***********************************************************************************
                // END COMMENTED ON 26/11/2022
                // ***********************************************************************************


                // *****************************************
                //  LEAVE TYPE WISE CHECKS................
                // *****************************************

                //CHECKS FOR CL
                if (details.LeaveTypeCode == LeaveTypes.CasualLeave)
                {
                    // CL can't be more than 3 days
                    if (details.TotalDays > 3)
                        error.Add("CL cannot be more than 3 days");

                    // If some leave rule is found for prev/next leaves, check it here...

                    // ***********************************************************************************
                    // COMMENTED ON 26/11/2022
                    // ***********************************************************************************

                    //if (prevLeaveRules != null && prevLeaveRules.Active)
                    //    if (details.TotalDays > prevLeaveRules.DaysAllowed)
                    //        error.Add("Cannot take this CL. Check leaves taken on previous days.");

                    //if (nextLeaveRules != null && nextLeaveRules.Active)
                    //    if (details.TotalDays > nextLeaveRules.DaysAllowed)
                    //        error.Add("Cannot take this CL. Check leaves taken on next days.");
                    // ***********************************************************************************
                    // END COMMENTED ON 26/11/2022
                    // ***********************************************************************************
                }

                //CHECKS FOR EL/PL
                if (details.LeaveTypeCode == LeaveTypes.PaidLeave)
                {
                    // EL Must be at least 3 days
                    if (details.TotalDays < 3)
                        error.Add("EL cannot be less than 3 days");

                    // ***********************************************************************************
                    // COMMENTED ON 26/11/2022
                    // ***********************************************************************************

                    ////// If some leave rule is found for prev/next leaves, check it here...

                    ////if (prevLeaveRules != null && prevLeaveRules.Active)
                    ////    if (details.TotalDays > prevLeaveRules.DaysAllowed || !prevLeaveRules.LeaveAllowed)
                    ////        error.Add("Cannot take this EL. Check leaves taken on previous days.");

                    ////if (nextLeaveRules != null && nextLeaveRules.Active)
                    ////    if (details.TotalDays > nextLeaveRules.DaysAllowed || !nextLeaveRules.LeaveAllowed)
                    ////        error.Add("Cannot take this EL. Check leaves taken on next days.");
                    //
                    // ***********************************************************************************
                    // END COMMENTED ON 26/11/2022
                    // ***********************************************************************************
                }

                // ***********************************************************************************
                // COMMENTED ON 26/11/2022
                // ***********************************************************************************

                //CHECKS FOR SL
                ////if (details.LeaveTypeCode == LeaveTypes.SickLeave)
                ////{
                ////    // If some leave rule is found for prev/next leaves, check it here...
                ////    if (prevLeaveRules != null && prevLeaveRules.Active)
                ////        if (details.TotalDays > prevLeaveRules.DaysAllowed || !prevLeaveRules.LeaveAllowed)
                ////            error.Add("Cannot take this SL. Check leaves taken on previous days.");

                ////    if (nextLeaveRules != null && nextLeaveRules.Active)
                ////        if (details.TotalDays > nextLeaveRules.DaysAllowed || !nextLeaveRules.LeaveAllowed)
                ////            error.Add("Cannot take this SL. Check leaves taken on next days.");
                ////}

                // ***********************************************************************************
                // END COMMENTED ON 26/11/2022
                // ***********************************************************************************

                if (details.LeaveTypeCode == LeaveTypes.OptionalLeave)
                {
                    if (details.TotalDays > 1)
                        error.Add("Only one OH is allowed.");

                    if (details.HalfDayFlag)
                        error.Add("Half day OH is no allowed.");

                    if (!Helpers.CustomHelper.GetOptionalHolidays(details.FromDt, emp.Location))
                        error.Add("Invalid Optional holiday. Pl verify date.");

                    // ***********************************************************************************
                    // COMMENTED ON 26/11/2022
                    // ***********************************************************************************


                    ////// If some leave rule is found for prev/next leaves, check it here...
                    ////if (prevLeaveRules != null && prevLeaveRules.Active)
                    ////    if (details.TotalDays > prevLeaveRules.DaysAllowed || !prevLeaveRules.LeaveAllowed)
                    ////        error.Add("Cannot take this OL. Check leaves taken on previous days.");

                    ////if (nextLeaveRules != null && nextLeaveRules.Active)
                    ////    if (details.TotalDays > nextLeaveRules.DaysAllowed || !nextLeaveRules.LeaveAllowed)
                    ////        error.Add("Cannot take this OL. Check leaves taken on next days.");
                    // ***********************************************************************************
                    // END COMMENTED ON 26/11/2022
                    // ***********************************************************************************
                }

                // ***********************************************************************************
                // COMMENTED ON 26/11/2022
                // ***********************************************************************************

                //////CHECKS FOR OC
                ////if (details.LeaveTypeCode == LeaveTypes.OtCOff)
                ////{
                ////    // If some leave rule is found for prev/next leaves, check it here...
                ////    if (prevLeaveRules != null && prevLeaveRules.Active)
                ////        if (details.TotalDays > prevLeaveRules.DaysAllowed)
                ////            error.Add("Cannot take this OT Coff. Check leaves taken on previous days.");

                ////    if (nextLeaveRules != null && nextLeaveRules.Active)
                ////        if (details.TotalDays > nextLeaveRules.DaysAllowed)
                ////            error.Add("Cannot take this OT Coff. Check leaves taken on next days.");
                ////}

                // ***********************************************************************************
                // END COMMENTED ON 26/11/2022
                // ***********************************************************************************
            }

            #endregion

            //CHECKS FOR WHOLE LEAVE OBJECTS COMBINING LEAVE DETAIL DATA

            //if there are multiple leaves and one of them is CL, throw error
            if (leaveApplicationDto.LeaveApplicationDetails.Any(x => x.LeaveTypeCode == LeaveTypes.CasualLeave)
                && leaveApplicationDto.LeaveApplicationDetails.Count > 1)
            {
                var leaves =
                    leaveApplicationDto.LeaveApplicationDetails.Where(x => x.LeaveTypeCode != LeaveTypes.CasualLeave);

                //find if there's a leave type other than LWP
                bool found = leaves.Any(d => d.LeaveTypeCode != LeaveTypes.LeaveWithoutPay &&
                                             d.LeaveTypeCode != LeaveTypes.OptionalLeave);

                if (found)
                    error.Add("CL cannot be clubbed with any other leaves.");
            }


            // added on 21/08/2021 by ashish
            //if there are multiple leaves and if OL is clubbed with SL, throw error
            if (leaveApplicationDto.LeaveApplicationDetails.Any(x => x.LeaveTypeCode == LeaveTypes.OptionalLeave)
                && leaveApplicationDto.LeaveApplicationDetails.Count > 1)
            {
                var leaves =
                    leaveApplicationDto.LeaveApplicationDetails.Where(x => x.LeaveTypeCode == LeaveTypes.SickLeave);

                if (leaves.Any())
                    error.Add("OL(RH) cannot be clubbed with Sick leave.");
            }


            foreach (var leaveType in leaveApplicationDto.LeaveApplicationDetails.Select(l => l.LeaveTypeCode)
                         .Distinct())
            {
                if (leaveType != LeaveTypes.CasualLeave && leaveType != LeaveTypes.SickLeave &&
                    leaveType != LeaveTypes.PaidLeave)
                    continue;

                float leaveCount = leaveApplicationDto.LeaveApplicationDetails
                    .Where(detail => detail.LeaveTypeCode == leaveType).Sum(detail => detail.TotalDays);

                if (leaveCount > 3 && leaveType == LeaveTypes.CasualLeave)
                    error.Add("CL cannot be more than 3 days");

                LeaveBalanceDto lb = leaveBalDto.FirstOrDefault(l => l.LeaveTypeCode == leaveType);
                if (lb == null)
                {
                    error.Add("Invalid leave type: " + leaveType + "");
                    continue;
                }

                float bal = lb.Opening - lb.Availed - lb.Encashed;

                if (bal < leaveCount)
                    error.Add("Insufficient balance of " + leaveType + ". Current Balance is: " + bal);
            }


            //throw error if multiple CL are applied in the same application:
            if (leaveApplicationDto.LeaveApplicationDetails.Count(x => x.LeaveTypeCode == LeaveTypes.CasualLeave) > 1)
            {
                //check for half CL...
                bool found = leaveApplicationDto.LeaveApplicationDetails.Any(detail => detail.HalfDayFlag);

                bool findOptional =
                    leaveApplicationDto.LeaveApplicationDetails.Any(detail =>
                        detail.LeaveTypeCode == LeaveTypes.OptionalLeave);

                if (!found)
                {
                    if (!findOptional)
                        error.Add("Cannot take multiple CLs in single Leave Application.");
                }
            }

            //throw error if multiple OH are applied in the same application:
            if (leaveApplicationDto.LeaveApplicationDetails.Count(x => x.LeaveTypeCode == LeaveTypes.OptionalLeave) > 1)
            {
                error.Add("Cannot take multiple Optional Leaves in single Leave Application.");
            }

            //Date range check
            DateTime start = leaveApplicationDto.LeaveApplicationDetails.Select(x => x.FromDt).Min();
            DateTime end = leaveApplicationDto.LeaveApplicationDetails.Select(x => x.ToDt).Max();
            double total = leaveApplicationDto.LeaveApplicationDetails
                .Select(x => (x.ToDt.Subtract(x.FromDt).TotalDays + 1)).Sum();

            //subtract off days (w/offs + holidays) from above total
            total -= offDays;

            if (Math.Abs(total - ((end.Subtract(start).TotalDays + 1) - offDays)) > 0)
                error.Add("Date ranges must be continuous. No gaps allowed");


            //Check if ranges overlap skip in case of CO
            if (leaveApplicationDto.LeaveApplicationDetails.All(x => x.LeaveTypeCode != LeaveTypes.CompOff))
            {
                var overlaps = leaveApplicationDto.LeaveApplicationDetails.SelectMany(
                        x1 => leaveApplicationDto.LeaveApplicationDetails,
                        (x1, x2) => new { x1, x2 })
                    .Where(t => !(Equals(t.x1, t.x2)))
                    .Where(t => (t.x1.FromDt <= t.x2.ToDt) && (t.x1.ToDt >= t.x2.FromDt))
                    .Select(t => t.x2);

                if (overlaps.Any())
                    error.Add("Date ranges must be consicutive, should not overlap.");
            }

            // CHECKS FOR COMP OFF ( CO )

            // check if start date is a Holiday
            if (leaveApplicationDto.LeaveApplicationDetails.Any(x => x.LeaveTypeCode == LeaveTypes.CompOff))
            {
                if (emp.Location == Locations.Nashik || emp.Location == Locations.Ipu ||
                    emp.Location == Locations.Bellary)
                {
                    // W - weekoff, H - holiday

                    var coDate = (DateTime)leaveApplicationDto.LeaveApplicationDetails[0].CoDate1;

                    var tpa = Helpers.CustomHelper.GetPerfAttd(emp.EmpUnqId, coDate, coDate);

                    double hours = tpa[0].ConsShift == "CC" ? 7.5 : 8.0;

                    // if hours are 7.5, convert them to 8 for locations other than nashik
                    if (hours == 7.5)
                        hours = emp.Location == Locations.Nashik ? 7.5 : 8.0;


                    if (tpa[0].ConsWrkHrs < hours)
                    {
                        error.Add("Work hours are less than 8 hours. Comp. Off cannot be availed.");
                    }

                    if (leaveApplicationDto.LeaveApplicationDetails[0].CoMode == "H")
                    {
                        if (Helpers.CustomHelper.GetHolidays(coDate, coDate, leaveApplicationDto.CompCode,
                                leaveApplicationDto.WrkGrp, emp.Location).Count != 1)
                        {
                            error.Add("Date selected is not Holiday.");
                        }
                        else
                        {
                            if (Helpers.CustomHelper.GetWeeklyOff(start, start, leaveApplicationDto.EmpUnqId).Count ==
                                1)
                            {
                                error.Add("Comp. Off cannot be taken on a Week Off day.");
                            }

                            // Check if CO date is <= 7 days from WO day
                            if (emp.Location == Locations.Ipu)
                            {
                                if (end.Subtract(start).TotalDays > 30)
                                    error.Add("Comp. Off can be taken within 30 days.");
                            }
                            else if (emp.Location == Locations.Jfl)
                            {
                                if (end.Subtract(start).TotalDays > 30)
                                    error.Add("Comp. Off can be taken within 30 days.");
                            }
                            else
                            {
                                if (end.Subtract(start).TotalDays > 90)
                                    error.Add("Comp. Off can be taken within 90 days.");
                            }
                        }
                    }
                    else if (leaveApplicationDto.LeaveApplicationDetails[0].CoMode == "W")
                    {
                        if (tpa[0].LeaveType != "WO")
                        {
                            error.Add("Date selected is not Week Off day.");
                        }
                        else
                        {
                            if (Helpers.CustomHelper.GetWeeklyOff(start, start, leaveApplicationDto.EmpUnqId).Count ==
                                1)
                            {
                                error.Add("Comp. Off cannot be taken on a Week Off day.");
                            }

                            // Check if CO date is <= 7 days from WO day
                            if (emp.Location == Locations.Ipu)
                            {
                                if (end.Subtract(start).TotalDays > 3)
                                    error.Add("Comp. Off can be taken within 3 days.");
                            }
                            else if (emp.Location == Locations.Jfl)
                            {
                                if (end.Subtract(start).TotalDays > 3)
                                    error.Add("Comp. Off can be taken within 3 days.");
                            }
                            else
                            {
                                if (end.Subtract(start).TotalDays > 3)
                                    error.Add("Comp. Off can be taken within 3 days.");
                            }
                        }
                    }

                    // check if emp has already taken coff against this holiday/weekoff

                    var coDate1 = leaveApplicationDto.LeaveApplicationDetails[0].CoDate1;

                    List<LeaveApplications> already = _context.LeaveApplications
                        .Include(la => la.LeaveApplicationDetails)
                        .Where(l => l.EmpUnqId == leaveApplicationDto.EmpUnqId &&
                                    l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                                    l.Cancelled == false &&
                                    _context.LeaveApplicationDetails.Any(
                                        ld => ld.CoDate1 == coDate1 &&
                                              ld.LeaveAppId == l.LeaveAppId)
                        ).ToList();

                    if (already.Count > 0)
                    {
                        error.Add("Already taken Comp Off against PH/WO " +
                                  leaveApplicationDto.LeaveApplicationDetails[0].CoDate1);
                    }
                    // end of already taken coff against holiday check
                }
                else
                {
                    if (start != end)
                    {
                        if (Helpers.CustomHelper.GetHolidays(start, start, leaveApplicationDto.CompCode,
                                leaveApplicationDto.WrkGrp, emp.Location).Count != 1)
                        {
                            error.Add("Date selected is not Holiday.");
                        }
                        else
                        {
                            if (Helpers.CustomHelper.GetWeeklyOff(start, start, leaveApplicationDto.EmpUnqId).Count ==
                                1)
                            {
                                error.Add("Comp. Off cannot be taken against a Week Off day.");
                            }

                            // Check if CO date is <= 7 days from WO day
                            if (emp.Location == Locations.Ipu)
                            {
                                if (end.Subtract(start).TotalDays > 30)
                                    error.Add("Comp. Off can be taken within 7 days.");
                            }
                            else if (emp.Location == Locations.Jfl)
                            {
                                if (end.Subtract(start).TotalDays > 30)
                                    error.Add("Comp. Off can be taken within 30 days.");
                            }
                            else
                            {
                                if (end.Subtract(start).TotalDays > 90)
                                    error.Add("Comp. Off can be taken within 90 days.");
                            }
                        }

                        if (Helpers.CustomHelper.GetWeeklyOff(end, end, leaveApplicationDto.EmpUnqId).Count == 1)
                        {
                            error.Add("Comp. Off cannot be taken on Week Off day.");
                        }
                    }
                }
            } //  COMP OFF CHECKS

            if (leaveApplicationDto.LeaveApplicationDetails.Any(x => x.LeaveTypeCode == LeaveTypes.OtCOff))
            {
                foreach (LeaveApplicationDetailDto detail in leaveApplicationDto.LeaveApplicationDetails)
                {
                    float otHours1 = 0;
                    float otHours2 = 0;
                    if (detail.CoDate1 != null)
                    {
                        if ((detail.FromDt - detail.CoDate1.Value).TotalDays > 180)
                            error.Add("Sanction OT not found on date " + detail.CoDate1);


                        var otDate1 = _context.TpaSanctions.FirstOrDefault(
                            e => e.EmpUnqId == emp.EmpUnqId &&
                                 e.TpaDate == detail.CoDate1 &&
                                 e.PostReleaseStatusCode == ReleaseStatus.FullyReleased);
                        if (otDate1 == null)
                            error.Add("Sanction OT not found on date " + detail.CoDate1);

                        otHours1 = otDate1.SanctionTpa;
                    }

                    if (detail.CoDate2 != null)
                    {
                        if ((detail.FromDt - detail.CoDate2.Value).TotalDays > 180)
                            error.Add("Sanction OT not found on date " + detail.CoDate1);

                        var otDate2 = _context.TpaSanctions.FirstOrDefault(
                            e => e.EmpUnqId == emp.EmpUnqId &&
                                 e.TpaDate == detail.CoDate2 &&
                                 e.PostReleaseStatusCode == ReleaseStatus.FullyReleased);
                        if (otDate2 == null)
                            error.Add("Sanction OT not found on date " + detail.CoDate2);
                        otHours2 = otDate2.SanctionTpa;
                    }

                    if (detail.TotalDays == 1.0)
                    {
                        if (otHours1 >= 8) continue;

                        if (otHours1 < 4)
                        {
                            if (detail.CoDate1 != null)
                                error.Add("Sanctioned OT on " + detail.CoDate1.Value.ToString("dd/MM/yyyy") +
                                          " should be at least 4 hours.");
                        }
                        else if (otHours2 < 4)
                        {
                            if (detail.CoDate2 != null)
                                error.Add("Sanctioned OT on " + detail.CoDate2.Value.ToString("dd/MM/yyyy") +
                                          " should be at least 4 hours.");
                        }
                    }
                    else if (detail.TotalDays == 0.5 && otHours1 < 4)
                    {
                        if (detail.CoDate1 != null)
                            error.Add("Sanctioned OT on " + detail.CoDate1.Value.ToString("dd/MM/yyyy") +
                                      " should be at least 4 hours.");
                    }


                    // check if emp has already taken coff against this holiday/weekoff

                    var coDate1 = detail.CoDate1;
                    var coDate2 = detail.CoDate2;


                    var cOffDetails = _context.LeaveApplicationDetails
                        .Where(l =>
                            (l.LeaveTypeCode == LeaveTypes.CompOff || l.LeaveTypeCode == LeaveTypes.OtCOff) &&
                            ((l.CoDate1 != null && l.CoDate1 == coDate1) ||
                             (l.CoDate1 != null && l.CoDate1 == coDate2) ||
                             (l.CoDate2 != null && l.CoDate2 == coDate1) ||
                             (l.CoDate2 != null && l.CoDate2 == coDate2)
                            )
                        )
                        .Select(l => l.LeaveAppId).ToArray();


                    List<LeaveApplications> already = _context.LeaveApplications
                        .Include(la => la.LeaveApplicationDetails)
                        .Where(l => l.EmpUnqId == leaveApplicationDto.EmpUnqId &&
                                    l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                                    l.Cancelled == false &&
                                    cOffDetails.Contains(l.LeaveAppId)
                        ).ToList();

                    //List<LeaveApplications> already = _context.LeaveApplications
                    //    .Include(la => la.LeaveApplicationDetails)
                    //    .Where(l => l.EmpUnqId == leaveApplicationDto.EmpUnqId &&
                    //                l.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                    //                _context.LeaveApplicationDetails.Any(
                    //                    ld =>
                    //                        (ld.LeaveTypeCode == LeaveTypes.CompOff ||
                    //                         ld.LeaveTypeCode == LeaveTypes.OtCOff) &&
                    //                        (ld.CoDate1 == coDate1 || ld.CoDate1 == coDate2 ||
                    //                         ld.CoDate2 == coDate1 || ld.CoDate2 == coDate2) &&
                    //                        ld.LeaveAppId == l.LeaveAppId)
                    //    ).ToList();

                    if (already.Count > 0)
                    {
                        error.Add("Already taken Comp Off against OT Date " +
                                  coDate1 + "/" + coDate2);
                    }
                    // end of already taken coff against holiday check
                }
            }

            // DONE. If there's no error, return success
            if (error.Count == 0)
                return Ok(leaveApplicationDto);


            // If there are errors, return bad request.

            return Content(HttpStatusCode.BadRequest, error);
        }

        // PL application before 15 days check.
        private bool IsPlDateValid(LeaveApplicationDetailDto details, string location)
        {
            if (details.LeaveTypeCode != LeaveTypes.PaidLeave)
                return true;

            bool? plChk = _context.Location.FirstOrDefault()?.PlCheck ?? true;

            if (location != Locations.Nashik || plChk != true)
                return true;

            return (details.FromDt - DateTime.Today).Days >= 15;
        }

        // ***********************************************************************************
        // COMMENTED ON 26/11/2022
        // ***********************************************************************************

        ////private string GetLeaveOnDate(DateTime dt, string empUnqId)
        ////{
        ////    if (Helpers.CustomHelper.GetWeeklyOff(dt, dt, empUnqId).Count > 0)
        ////        return "WO";

        ////    LeaveApplicationDetails leave = _context.LeaveApplicationDetails
        ////        .FirstOrDefault(l => l.LeaveApplication.EmpUnqId == empUnqId &&
        ////                             (dt >= l.FromDt && dt <= l.ToDt) &&
        ////                             l.Cancelled == false &&
        ////                             l.LeaveApplication.ReleaseStatusCode != ReleaseStatus.ReleaseRejected);

        ////    return leave != null ? leave.LeaveTypeCode : "";
        ////}
        // ***********************************************************************************
        // END COMMENTED ON 26/11/2022
        // ***********************************************************************************


        private List<string> PrevNextLeaveChecks(string empUnqId, string compCode, string wrkGrp, string location,
            string leaveTypeCode, float totalDays, DateTime fromDate, DateTime toDate)
        {
            var errors = new List<string>();

            var nextLeaves = new Dictionary<DateTime, string>();
            var nextDay = toDate.AddDays(1);

            // BUILD THE NEXT LEAVES ARRAY
            while (true)
            {
                //1: CHECK IF NEXT DAY IS WOFF OR HL
                if (Helpers.CustomHelper.GetWeeklyOff(nextDay, nextDay, empUnqId).Count > 0)
                {
                    nextLeaves.Add(nextDay, "WO");
                    nextDay = nextDay.AddDays(1);
                    continue;
                }
                else if (Helpers.CustomHelper.GetHolidays(nextDay, nextDay, compCode,
                             wrkGrp, location).Count > 0)
                {
                    nextLeaves.Add(nextDay, "HL");
                    nextDay = nextDay.AddDays(1);
                    continue;
                }

                //2: GET LEAVE APP DTL FROM NEXT DAY
                var existingNextLeave = _context.LeaveApplicationDetails
                    .Where(l =>
                        l.LeaveApplication.EmpUnqId == empUnqId &&
                        l.FromDt == nextDay &&
                        l.LeaveApplication.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                        l.LeaveTypeCode != LeaveTypes.OutdoorDuty &&
                        l.Cancelled == false
                    )
                    .ToList();

                if (existingNextLeave.Count > 0)
                {
                    foreach (var leave in existingNextLeave)
                    {
                        for (DateTime? d = leave.FromDt; d <= leave.ToDt;)
                        {
                            nextLeaves.Add(d.Value, leave.LeaveTypeCode);
                            d = d.Value.AddDays(1);
                        }

                        if (leave.ToDt != null) nextDay = leave.ToDt.Value.AddDays(1);
                    }
                }
                else
                {
                    // NO MORE LEAVES FOUND... EXIT THE LOOP
                    break;
                }
            }


            ////////////////////////////////////////

            var prevLeaves = new Dictionary<DateTime, string>();
            var prevDay = fromDate.AddDays(-1);

            // BUILD THE NEXT LEAVES ARRAY
            while (true)
            {
                //1: CHECK IF NEXT DAY IS WOFF OR HL
                if (Helpers.CustomHelper.GetWeeklyOff(prevDay, prevDay, empUnqId).Count > 0)
                {
                    prevLeaves.Add(prevDay, "WO");
                    prevDay = prevDay.AddDays(-1);
                    continue;
                }
                else if (Helpers.CustomHelper.GetHolidays(prevDay, prevDay, compCode,
                             wrkGrp, location).Count > 0)
                {
                    prevLeaves.Add(prevDay, "HL");
                    prevDay = prevDay.AddDays(-1);
                    continue;
                }

                //2: GET LEAVE APP DTL FROM NEXT DAY
                var existingPrevLeave = _context.LeaveApplicationDetails
                    .Where(l =>
                        l.LeaveApplication.EmpUnqId == empUnqId &&
                        l.ToDt == prevDay &&
                        l.LeaveApplication.ReleaseStatusCode == ReleaseStatus.FullyReleased &&
                        l.LeaveTypeCode != LeaveTypes.OutdoorDuty &&
                        l.Cancelled == false
                    )
                    .ToList();

                if (existingPrevLeave.Count > 0)
                {
                    foreach (var leave in existingPrevLeave)
                    {
                        for (DateTime? d = leave.ToDt; d >= leave.FromDt;)
                        {
                            prevLeaves.Add(d.Value, leave.LeaveTypeCode);
                            d = d.Value.AddDays(-1);
                        }

                        if (leave.FromDt != null) prevDay = leave.FromDt.Value.AddDays(-1);
                    }
                }
                else
                {
                    // NO MORE LEAVES FOUND... EXIT THE LOOP
                    break;
                }
            }

            ////////////////////////////////////////


            ////////////////////////////////////////

            // GET RECORD FROM LEAVE CHECK TABLE

            string leaveString = string.Join(",", nextLeaves.Distinct().Select(x => x.Value).ToArray());
            var leaveChecks =
                _context.LeaveChecks
                    .Where(l => l.LeaveTaken == leaveTypeCode && leaveString.Contains(l.LeaveCheck)).ToList();


            // CHECK VALIDITY OF LEAVE

            float clubbedLeave = totalDays;

            foreach (var leave in nextLeaves)
            {
                var chk = leaveChecks.FirstOrDefault(l => l.LeaveCheck == leave.Value);
                if (chk == null)
                {
                    errors.Add("Rule not added for " + leaveTypeCode + " and " + leave.Value);
                    continue;
                }

                // NOT ALLOWED
                if (!chk.IsAllowed)
                {
                    errors.Add("Clubbing not allowed of leave types " + leaveTypeCode + " and " + leave.Value);
                    break;
                }
                else
                {
                    // ALLOWED WITH RESTRICTED DAYS
                    if (chk.DaysAllowed > 0)
                    {
                        if (leaveTypeCode == leave.Value)
                            clubbedLeave++;

                        if (clubbedLeave > chk.DaysAllowed)
                        {
                            errors.Add("Total days allowed are " + chk.DaysAllowed + ".  Check previous/next leaves");
                            break;
                        }
                    }
                }
            }

            ////////////////////////////////////////

            leaveString = string.Join(",", prevLeaves.Distinct().Select(x => x.Value).ToArray());
            leaveChecks =
                _context.LeaveChecks
                    .Where(l => l.LeaveTaken == leaveTypeCode && leaveString.Contains(l.LeaveCheck)).ToList();

            // CHECK VALIDITY OF LEAVE

            clubbedLeave = totalDays;

            var sortedPrevLeaves = prevLeaves.OrderBy(i => i.Key);

            foreach (var leave in sortedPrevLeaves)
            {
                var chk = leaveChecks.FirstOrDefault(l => l.LeaveCheck == leave.Value);
                if (chk == null)
                {
                    errors.Add("Rule not added for " + leaveTypeCode + " and " + leave.Value);
                    continue;
                }

                // NOT ALLOWED
                if (!chk.IsAllowed)
                {
                    errors.Add("Clubbing not allowed of leave types " + leaveTypeCode + " and " + leave.Value);
                    break;
                }
                else
                {
                    // ALLOWED WITH RESTRICTED DAYS
                    if (chk.DaysAllowed > 0)
                    {
                        if (leaveTypeCode == leave.Value)
                            clubbedLeave++;

                        if (clubbedLeave > chk.DaysAllowed)
                        {
                            errors.Add("Total days allowed are " + chk.DaysAllowed + ".  Check previous/next leaves");
                            break;
                        }
                    }
                }
            }

            ////////////////////////////////////////


            // BUILD THE ARRAY OF FULL LEAVES STARTING FROM PREVIOUS ENDING IN NEXT LEAVES

            if (nextLeaves.Count > 0 && prevLeaves.Count > 0)
            {
                Dictionary<DateTime, string> fullleaves = prevLeaves.ToDictionary(item => item.Key, item => item.Value);

                for (DateTime? d = fromDate; d <= toDate;)
                {
                    fullleaves.Add(d.Value, leaveTypeCode);
                    d = d.Value.AddDays(1);
                }

                foreach (KeyValuePair<DateTime, string> item in nextLeaves)
                {
                    fullleaves.Add(item.Key, item.Value);
                }

                var fullLeavesSorted = fullleaves.OrderBy(i => i.Key).ToArray();

                string fullLeaveString = string.Join(",", fullLeavesSorted.Distinct().Select(x => x.Value).ToArray());


                // CHECK FOR PL+OH+PL

                var ploh = string.Join(",",
                    fullLeavesSorted
                        .Where(x => x.Value == LeaveTypes.PaidLeave ||
                                    x.Value == LeaveTypes.OptionalLeave)
                        .Select(x => x.Value).ToArray());

                if (ploh.Contains("PL,OH,PL") || ploh.Contains("EL,OH,EL"))
                {
                    errors.Add("Can't take Optional leave in between two PLs.");
                    return errors;
                }

                foreach (var thisLeave in fullLeavesSorted)
                {
                    leaveChecks = _context.LeaveChecks.Where(l =>
                        l.LeaveTaken == thisLeave.Value && fullLeaveString.Contains(l.LeaveCheck)).ToList();
                    bool found = false;

                    foreach (var checkLeave in fullLeavesSorted)
                    {
                        if (thisLeave.Key == checkLeave.Key) continue;

                        var chk = leaveChecks.FirstOrDefault(l => l.LeaveCheck == checkLeave.Value);
                        if (chk == null)
                        {
                            errors.Add("Rule not added for " + thisLeave.Value + " and " + checkLeave.Value);
                            continue;
                        }

                        if (!chk.IsAllowed)
                        {
                            errors.Add("Clubbing not allowed of leave types " + thisLeave.Value + " and " +
                                       checkLeave.Value);
                            found = true;
                            break;
                        }
                    }

                    if (found)
                        break;
                }
            }

            return errors;
        }
    }
}