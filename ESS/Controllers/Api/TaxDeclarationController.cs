using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Reflection;
using System.Web.Mvc;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class TaxDeclarationController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public TaxDeclarationController()
        {
            _context = new ApplicationDbContext();
        }

        /// <summary>
        /// Get's current config for Tax Declaration: year, actual flag, start date, end date
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetTaxDeclarationConfig()
        {
            var config = _context.TaxConfig.FirstOrDefault();
            if (config == null) return BadRequest("Configuration not available.");

            return Ok(config);
        }

        /// <summary>
        /// Get's Tax Declaration details for an Employee
        /// </summary>
        /// <param name="empUnqId"> Employee unique Id</param>
        /// <param name="yearMonth"> Year Month</param>
        /// <param name="actualFlag"> Actual [1] or Provisional [0] </param>
        /// <returns></returns>
        public IHttpActionResult GetTaxDeclaration(string empUnqId, int yearMonth, bool actualFlag)
        {
            var taxDetails = _context.TaxDeclarations
                .Include(i => i.InsuranceDetails)
                .Include(m => m.MutualFundDetails)
                .Include(n => n.NscDetails)
                .Include(p => p.PpfDetails)
                .Include(b => b.BankDeposits)
                .Include(s => s.SukanyaDetails)
                .Include(u => u.UlipDetails)
                .Include(r => r.RentDetails)
                .Where(t => t.EmpUnqId == empUnqId && t.YearMonth == yearMonth)
                .Select(Mapper.Map<TaxDeclarations, TaxDeclarationDto>)
                .ToList();
            // if (taxDetails.Count == 0)
            // {
            //     taxDetails = _context.TaxDeclarations
            //         .Include(i => i.InsuranceDetails)
            //         .Include(m => m.MutualFundDetails)
            //         .Include(n => n.NscDetails)
            //         .Include(p => p.PpfDetails)
            //         .Include(b => b.BankDeposits)
            //         .Include(s => s.SukanyaDetails)
            //         .Include(u => u.UlipDetails)
            //         .Include(r => r.RentDetails)
            //         .Where(t => t.EmpUnqId == empUnqId && t.YearMonth == yearMonth && t.ActualFlag == false)
            //         .Select(Mapper.Map<TaxDeclarations, TaxDeclarationDto>)
            //         .ToList();
            // }

            return Ok(taxDetails);
        }

        /// <summary>
        /// Get tax declaration report for all employees. To be used for finance people.
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <returns></returns>
        public IHttpActionResult GetTaxDeclarations(int yearMonth)
        {
            var taxDeclarations = _context.TaxDeclarations
                .Include(i => i.InsuranceDetails)
                .Include(m => m.MutualFundDetails)
                .Include(n => n.NscDetails)
                .Include(p => p.PpfDetails)
                .Include(b => b.BankDeposits)
                .Include(u => u.UlipDetails)
                .Include(s => s.SukanyaDetails)
                .Include(r => r.RentDetails)
                .Where(t => t.YearMonth == yearMonth && t.IsSubmitted == true)
                .Select(Mapper.Map<TaxDeclarations, TaxDeclarationDto>)
                .ToList();

            var employees = taxDeclarations.Select(e => e.EmpUnqId).Distinct().ToList();

            List<TaxDeclReportDto> report = new List<TaxDeclReportDto>();

            foreach (var emp in employees)
            {
                var empobj = _context.Employees
                    .FirstOrDefault(e => e.EmpUnqId == emp);

                if (empobj == null) return BadRequest("Employee not found : " + emp);

                var taxconfig = _context.TaxConfig.FirstOrDefault();
                if (taxconfig == null) return BadRequest("Tax config not found. ");

                var empRec = new TaxDeclReportDto();

                //get Provisional details:
                var prov = taxDeclarations.FirstOrDefault(e => e.EmpUnqId == emp && e.ActualFlag == false);
                if (prov != null)
                {
                    empRec.YearMonth = prov.YearMonth;
                    empRec.EmpUnqId = prov.EmpUnqId;
                    empRec.SapId = empobj.SapId;
                    empRec.EmpName = empobj.EmpName;
                    empRec.StartDt = taxconfig.StartDt;
                    empRec.EndDt = taxconfig.EndDt;

                    empRec.InsCode = 1; //TODO: MAKE DYNAMIC - PROBABLY FROM A TABLE
                    empRec.InsPro = prov.TotalInsurancePremium;

                    empRec.UlipCode = 4;
                    empRec.UlipPro = prov.TotalUlip;

                    empRec.MfCode = 6;
                    empRec.MfPro = prov.TotalMutualFund;

                    empRec.PpfCode = 7;
                    empRec.PpfPro = prov.TotalPpfAmt;

                    empRec.NscCode = 11;
                    empRec.NscPro = prov.TotalNscAmount;

                    empRec.HomeLoanCode = 14;
                    empRec.HomeLoanPro = prov.HouseLoanPrincipal;

                    empRec.HomeLoanCode2 = 14;
                    empRec.HomeLoanPro2 = prov.HouseLoanPrincipal2;

                    empRec.NotifiedMfCode = 18;
                    empRec.NotifiedMfPro = prov.TotalSukanya;

                    empRec.Child1Code = 24;
                    empRec.Child1Pro = prov.TuitionFeeChild1;

                    empRec.Child2Code = 25;
                    empRec.Child2Pro = prov.TuitionFeeChild2;

                    empRec.TermDepoCode = 26;
                    empRec.TermDepoPro = prov.TotalBankDepositAmount;

                    empRec.Total80C = empRec.InsPro + empRec.UlipPro + empRec.MfPro + empRec.PpfPro + empRec.NscPro +
                                      empRec.HomeLoanPro + empRec.HomeLoanPro2 + empRec.NotifiedMfPro +
                                      empRec.Child1Pro +
                                      empRec.Child2Pro + empRec.TermDepoPro;


                    empRec.LongTermMf = 0; //TODO: CONFIRM
                    empRec.MedicalPremium = prov.MedicalPremiumSelf + prov.MedicalPremiumParents;
                    empRec.EduLoanInterest = prov.EducationLoanInterest;
                    empRec.Nps = prov.NationalPensionScheme;

                    empRec.InterestOnLoan = prov.InterestOnLoan;
                    empRec.InterestPreConstruction = prov.InterestPreConstruction;
                    empRec.RentReceived = prov.RentalIncomePerMonth;
                    empRec.BankName = prov.LoanBank;
                    empRec.BankPan = prov.LoanBankPan;

                    empRec.InterestOnLoan2 = prov.InterestOnLoan2;
                    empRec.InterestPreConstruction2 = prov.InterestPreConstruction2;
                    empRec.RentReceived2 = prov.RentalIncomePerMonth2;
                    empRec.BankName2 = prov.LoanBank2;
                    empRec.BankPan2 = prov.LoanBankPan2;

                    empRec.AccomodationType = empobj.CompanyAcc ? "3" : "1";


                    //Average out rent

                    if (prov.RentDetails.Count > 0)
                    {
                        int months = 0;

                        if (prov.RentDetails[0].April > 0) months++;
                        if (prov.RentDetails[0].May > 0) months++;
                        if (prov.RentDetails[0].June > 0) months++;
                        if (prov.RentDetails[0].July > 0) months++;
                        if (prov.RentDetails[0].August > 0) months++;
                        if (prov.RentDetails[0].September > 0) months++;
                        if (prov.RentDetails[0].October > 0) months++;
                        if (prov.RentDetails[0].November > 0) months++;
                        if (prov.RentDetails[0].December > 0) months++;
                        if (prov.RentDetails[0].January > 0) months++;
                        if (prov.RentDetails[0].February > 0) months++;
                        if (prov.RentDetails[0].March > 0) months++;

                        if (months == 0) empRec.RentPaidAprilPro = 0;
                        else
                        {
                            var totalRent = prov.RentDetails[0].April +
                                            prov.RentDetails[0].May +
                                            prov.RentDetails[0].June +
                                            prov.RentDetails[0].July +
                                            prov.RentDetails[0].August +
                                            prov.RentDetails[0].September +
                                            prov.RentDetails[0].October +
                                            prov.RentDetails[0].November +
                                            prov.RentDetails[0].December +
                                            prov.RentDetails[0].January +
                                            prov.RentDetails[0].February +
                                            prov.RentDetails[0].March;

                            // ReSharper disable once PossibleLossOfFraction
                            empRec.RentPaidAprilPro = totalRent / months;
                        }
                    }

                    //empRec.RentPaidAprilPro = prov.RentDetails.Count > 0 ? prov.RentDetails[0].April : 0;
                    empRec.RentPaidPro = prov.TotalRentPaid;
                    empRec.LandLordName = prov.LandLordName;
                    empRec.LandLordPan = prov.LandLordPan;

                    empRec.RajivGandhiEquity = prov.RajivGandhiEquity;
                    empRec.MedicalPremiumSelf = prov.MedicalPremiumSelf;
                    empRec.MedicalPremiumParents = prov.MedicalPremiumParents;
                    empRec.MedicalPreventiveHealthCheckup = prov.MedicalPreventiveHealthCheckup;
                    empRec.PhysicalDisability = prov.PhysicalDisability;
                    empRec.SevereDisability = prov.SevereDisability;

                    empRec.DisableDependent = prov.DisableDependent;
                    empRec.MedicalExpenditure = prov.MedicalExpenditure;
                    empRec.MunicipalTax = prov.MunicipalTax;

                    empRec.LockEntry = prov.LockEntry;
                    empRec.ActualFlag = false;

                    empRec.TaxRegime = prov.TaxRegime;
                }


                //get Provisional details:
                var act = taxDeclarations.FirstOrDefault(e => e.EmpUnqId == emp && e.ActualFlag == true);
                if (act != null)
                {
                    empRec.YearMonth = act.YearMonth;
                    empRec.EmpUnqId = act.EmpUnqId;
                    empRec.SapId = empobj.SapId;
                    empRec.EmpName = empobj.EmpName;
                    empRec.StartDt = taxconfig.StartDt;
                    empRec.EndDt = taxconfig.EndDt;

                    empRec.InsCode = 1; //TODO: MAKE DYNAMIC - PROBABLY FROM A TABLE
                    empRec.InsAct = act.TotalInsurancePremium;

                    empRec.UlipCode = 4;
                    empRec.UlipAct = act.TotalUlip;

                    empRec.MfCode = 6;
                    empRec.MfAct = act.TotalMutualFund;

                    empRec.PpfCode = 7;
                    empRec.PpfAct = act.TotalPpfAmt;

                    empRec.NscCode = 11;
                    empRec.NscAct = act.TotalNscAmount;

                    empRec.HomeLoanCode = 14;
                    empRec.HomeLoanAct = act.HouseLoanPrincipal;

                    empRec.HomeLoanCode2 = 14;
                    empRec.HomeLoanAct2 = act.HouseLoanPrincipal2;

                    empRec.NotifiedMfCode = 18;
                    empRec.NotifiedMfAct = act.TotalSukanya;

                    empRec.Child1Code = 24;
                    empRec.Child1Act = act.TuitionFeeChild1;

                    empRec.Child2Code = 25;
                    empRec.Child2Act = act.TuitionFeeChild2;

                    empRec.TermDepoCode = 26;
                    empRec.TermDepoAct = act.TotalBankDepositAmount;

                    empRec.Total80C = empRec.InsAct + empRec.UlipAct + empRec.MfAct + empRec.PpfAct + empRec.NscAct +
                                      empRec.HomeLoanAct + empRec.HomeLoanAct2 + empRec.NotifiedMfAct +
                                      empRec.Child1Act +
                                      empRec.Child2Act + empRec.TermDepoAct;


                    empRec.LongTermMf = 0; //TODO: CONFIRM
                    empRec.MedicalPremium = act.MedicalPremiumSelf + act.MedicalPremiumParents;
                    empRec.EduLoanInterest = act.EducationLoanInterest;
                    empRec.Nps = act.NationalPensionScheme;

                    empRec.InterestOnLoan = act.InterestOnLoan;
                    empRec.InterestPreConstruction = act.InterestPreConstruction;
                    empRec.RentReceived = act.RentalIncomePerMonth;
                    empRec.BankName = act.LoanBank;
                    empRec.BankPan = act.LoanBankPan;

                    empRec.InterestOnLoan2 = act.InterestOnLoan2;
                    empRec.InterestPreConstruction2 = act.InterestPreConstruction2;
                    empRec.RentReceived2 = act.RentalIncomePerMonth2;
                    empRec.BankName2 = act.LoanBank2;
                    empRec.BankPan2 = act.LoanBankPan2;


                    empRec.AccomodationType = empobj.CompanyAcc ? "3" : "1";


                    //Average out rent

                    if (act.RentDetails.Count > 0)
                    {
                        int months = 0;

                        if (act.RentDetails[0].April > 0) months++;
                        if (act.RentDetails[0].May > 0) months++;
                        if (act.RentDetails[0].June > 0) months++;
                        if (act.RentDetails[0].July > 0) months++;
                        if (act.RentDetails[0].August > 0) months++;
                        if (act.RentDetails[0].September > 0) months++;
                        if (act.RentDetails[0].October > 0) months++;
                        if (act.RentDetails[0].November > 0) months++;
                        if (act.RentDetails[0].December > 0) months++;
                        if (act.RentDetails[0].January > 0) months++;
                        if (act.RentDetails[0].February > 0) months++;
                        if (act.RentDetails[0].March > 0) months++;

                        if (months == 0) empRec.RentPaidAprilPro = 0;
                        else
                        {
                            var totalRent = act.RentDetails[0].April +
                                            act.RentDetails[0].May +
                                            act.RentDetails[0].June +
                                            act.RentDetails[0].July +
                                            act.RentDetails[0].August +
                                            act.RentDetails[0].September +
                                            act.RentDetails[0].October +
                                            act.RentDetails[0].November +
                                            act.RentDetails[0].December +
                                            act.RentDetails[0].January +
                                            act.RentDetails[0].February +
                                            act.RentDetails[0].March;

                            // ReSharper disable once PossibleLossOfFraction
                            empRec.RentPaidAprilAct = totalRent / months;
                        }
                    }

                    //empRec.RentPaidAprilAct = act.RentDetails.Count > 0 ? act.RentDetails[0].April : 0;
                    empRec.RentPaidAct = act.TotalRentPaid;
                    empRec.LandLordName = act.LandLordName;
                    empRec.LandLordPan = act.LandLordPan;

                    empRec.RajivGandhiEquity = act.RajivGandhiEquity;
                    empRec.MedicalPremiumSelf = act.MedicalPremiumSelf;
                    empRec.MedicalPremiumParents = act.MedicalPremiumParents;
                    empRec.MedicalPreventiveHealthCheckup = act.MedicalPreventiveHealthCheckup;
                    empRec.PhysicalDisability = act.PhysicalDisability;
                    empRec.SevereDisability = act.SevereDisability;

                    empRec.DisableDependent = act.DisableDependent;
                    empRec.MedicalExpenditure = act.MedicalExpenditure;
                    empRec.MunicipalTax = act.MunicipalTax;

                    empRec.LockEntry = act.LockEntry;
                    empRec.ActualFlag = true;

                    empRec.TaxRegime = act.TaxRegime;
                }

                report.Add(empRec);
            }

            return Ok(report);
        }

        public IHttpActionResult GetBankNames(string flag)
        {
            var banks = _context.Banks.ToList().Select(Mapper.Map<Banks, BankDto>);
            return Ok(banks);
        }

        /// <summary>
        /// Create/Update Tax declaration of employee
        /// </summary>
        /// <param name="requestData">Data in form of TaxDeclaration json object </param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public IHttpActionResult CreateTaxDeclaration([FromBody] object requestData)
        {
            var config = _context.TaxConfig.FirstOrDefault();
            if (config == null) return BadRequest("Configuration not found");

            //if (config.CloseFlag) return BadRequest("Posting is closed. Contact finance department.");

            var sentData = JsonConvert.DeserializeObject<TaxDeclarationDto>(requestData.ToString());

            if (!ModelState.IsValid)
                return BadRequest("Model state is invalid.");

            var found = _context.TaxDeclarations
                .Any(t => t.EmpUnqId == sentData.EmpUnqId &&
                          t.YearMonth == sentData.YearMonth &&
                          t.ActualFlag == sentData.ActualFlag);


            TaxDeclarations taxDeclaration;

            if (!found)
            {
                taxDeclaration = new TaxDeclarations();

                taxDeclaration.PpfDetails = new List<TaxDetailsPpf>();
                taxDeclaration.BankDeposits = new List<TaxDetailsBankDeposit>();
                taxDeclaration.InsuranceDetails = new List<TaxDetailsInsurance>();
                taxDeclaration.NscDetails = new List<TaxDetailsNsc>();
                taxDeclaration.MutualFundDetails = new List<TaxDetailsMutualFunds>();
                taxDeclaration.UlipDetails = new List<TaxDetailsUlip>();
                taxDeclaration.SukanyaDetails = new List<TaxDetailsSukanya>();
                taxDeclaration.RentDetails = new List<TaxDetailsRent>();
            }
            else
            {
                taxDeclaration = _context.TaxDeclarations
                    .FirstOrDefault(t => t.EmpUnqId == sentData.EmpUnqId &&
                                         t.YearMonth == sentData.YearMonth &&
                                         t.ActualFlag == sentData.ActualFlag);

                if (taxDeclaration != null && taxDeclaration.LockEntry)
                {
                    if (sentData.UpdateUserId == "102971" ||
                        sentData.UpdateUserId == "104065" ||
                        sentData.UpdateUserId == "113052" ||
                        sentData.UpdateUserId == "112213")
                    {
                        if (taxDeclaration.FinLock)
                        {
                            return BadRequest("Entry is locked for you sir.");
                        }
                    }
                    else
                        return BadRequest("Entry is locked for you sir.");
                }
            }

            TaxDeclarationHistory history = new TaxDeclarationHistory();

            var tmp = Mapper.Map<TaxDeclarationDto, TaxDeclarations>(sentData);

            foreach (PropertyInfo prop in tmp.GetType().GetProperties())
            {
                if (!prop.CanRead) continue; //if property is readable... 
                if (prop.PropertyType != typeof(string) &&
                    typeof(IEnumerable).IsAssignableFrom(prop.PropertyType)) continue;
                object value = prop.GetValue(tmp, null);
                prop.SetValue(taxDeclaration, value);
            }


            var tmpH = Mapper.Map<TaxDeclarationDto, TaxDeclarationHistory>(sentData);

            foreach (PropertyInfo prop in tmpH.GetType().GetProperties())
            {
                if (!prop.CanRead) continue; //if property is readable... 

                object value = prop.GetValue(tmpH, null);
                prop.SetValue(history, value);
            }


            if (taxDeclaration == null) return BadRequest("Error!!!");

            foreach (var detail in taxDeclaration.InsuranceDetails.ToList())
            {
                taxDeclaration.InsuranceDetails.Remove(detail);
            }

            foreach (var detail in taxDeclaration.MutualFundDetails.ToList())
            {
                taxDeclaration.MutualFundDetails.Remove(detail);
            }

            foreach (var detail in taxDeclaration.NscDetails.ToList())
            {
                taxDeclaration.NscDetails.Remove(detail);
            }

            foreach (var detail in taxDeclaration.PpfDetails.ToList())
            {
                taxDeclaration.PpfDetails.Remove(detail);
            }

            foreach (var detail in taxDeclaration.BankDeposits.ToList())
            {
                taxDeclaration.BankDeposits.Remove(detail);
            }

            foreach (var detail in taxDeclaration.UlipDetails.ToList())
            {
                taxDeclaration.UlipDetails.Remove(detail);
            }

            foreach (var detail in taxDeclaration.SukanyaDetails.ToList())
            {
                taxDeclaration.SukanyaDetails.Remove(detail);
            }

            foreach (var detail in taxDeclaration.RentDetails.ToList())
            {
                taxDeclaration.RentDetails.Remove(detail);
            }

            if (sentData.InsuranceDetails.Count > 0)
            {
                foreach (var insuranceDto in sentData.InsuranceDetails)
                {
                    taxDeclaration.InsuranceDetails.Add(
                        Mapper.Map<TaxDetailsInsuranceDto, TaxDetailsInsurance>(insuranceDto));
                }
            }

            if (sentData.MutualFundDetails.Count > 0)
            {
                foreach (var mfDto in sentData.MutualFundDetails)
                {
                    taxDeclaration.MutualFundDetails.Add(
                        Mapper.Map<TaxDetailsMutualFundsDto, TaxDetailsMutualFunds>(mfDto));
                }
            }

            if (sentData.NscDetails.Count > 0)
            {
                foreach (var nscDto in sentData.NscDetails)
                {
                    taxDeclaration.NscDetails.Add(
                        Mapper.Map<TaxDetailsNscDto, TaxDetailsNsc>(nscDto));
                }
            }

            if (sentData.PpfDetails.Count > 0)
            {
                foreach (var ppfDto in sentData.PpfDetails)
                {
                    taxDeclaration.PpfDetails.Add(
                        Mapper.Map<TaxDetailsPpfDto, TaxDetailsPpf>(ppfDto));
                }
            }

            if (sentData.BankDeposits.Count > 0)
            {
                foreach (var bankDto in sentData.BankDeposits)
                {
                    taxDeclaration.BankDeposits.Add(
                        Mapper.Map<TaxDetailsBankDepositDto, TaxDetailsBankDeposit>(bankDto));
                }
            }

            if (sentData.UlipDetails.Count > 0)
            {
                foreach (var ulipDto in sentData.UlipDetails)
                {
                    taxDeclaration.UlipDetails.Add(
                        Mapper.Map<TaxDetailsUlipDto, TaxDetailsUlip>(ulipDto));
                }
            }

            if (sentData.SukanyaDetails.Count > 0)
            {
                foreach (var sukanyaDto in sentData.SukanyaDetails)
                {
                    taxDeclaration.SukanyaDetails.Add(
                        Mapper.Map<TaxDetailsSukanyaDto, TaxDetailsSukanya>(sukanyaDto));
                }
            }

            if (sentData.RentDetails.Count > 0)
            {
                foreach (var rentDto in sentData.RentDetails)
                {
                    taxDeclaration.RentDetails.Add(
                        Mapper.Map<TaxDetailsRentDto, TaxDetailsRent>(rentDto));
                }
            }

            taxDeclaration.IsSubmitted = true;

            if (!found)
            {
                _context.TaxDeclarations.Add(taxDeclaration);
                _context.Entry(taxDeclaration).State = EntityState.Added;
            }

            _context.TaxDeclarationHistories.Add(history);

            _context.SaveChanges();
            return Ok(Mapper.Map<TaxDeclarations, TaxDeclarationDto>(taxDeclaration));
        }
    }
}