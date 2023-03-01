using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESS.Models;

namespace ESS.Dto
{
    public class TaxDeclarationDto
    {
        public int YearMonth { get; set; } //201920
        public string EmpUnqId { get; set; }
        public bool ActualFlag { get; set; } //Provisional v/s Actual
        public float TotalRentPaid { get; set; }
        public string RentHouseAddress { get; set; }
        public string LandLordName { get; set; }
        public string LandLordPan { get; set; }
        public float PrevCompSalary { get; set; }
        public float PrevCompTds { get; set; }

        public float TotalPpfAmt { get; set; } //Details in table TaxDetailsPpf
        public float TotalBankDepositAmount { get; set; }
        public float TotalInsurancePremium { get; set; } //Deatils in table TaxDetailsInsurance
        public float TotalNscAmount { get; set; } //Deatils in table TaxDetailsNsc
        public float TotalMutualFund { get; set; } //Deatils in table TaxDetailsMutualFunds
        public float TotalUlip { get; set; }
        public float TotalSukanya { get; set; }

        public float HouseLoanPrincipal { get; set; }
        public float HouseLoanPrincipal2 { get; set; }

        public string Child1Name { get; set; }
        public float TuitionFeeChild1 { get; set; }
        public string Child2Name { get; set; }
        public float TuitionFeeChild2 { get; set; }

        public float NotifiedPensionScheme { get; set; }

        public string Others1Desc { get; set; }
        public float Others1Amount { get; set; }
        public string Others2Desc { get; set; }
        public float Others2Amount { get; set; }

        //80D

        public float RajivGandhiEquity { get; set; }
        public float MedicalPremiumSelf { get; set; }
        public float MedicalPremiumParents { get; set; }
        public float MedicalPremiumParentsAge { get; set; }
        public float MedicalPreventiveHealthCheckup { get; set; }

        public float EducationLoanInterest { get; set; }
        public float PhysicalDisability { get; set; }
        public float SevereDisability { get; set; }
        public float NationalPensionScheme { get; set; }

        public float DisableDependent { get; set; }
        public float MedicalExpenditure { get; set; }

        public string PropertyAddress { get; set; }
        public string PropertyStatus { get; set; } //s=self; v=vacant; l=letout
        public string LoanBank { get; set; } //Bank drop down required...
        public string LoanBankPan { get; set; }

        public float LoanAmount { get; set; }
        public DateTime? LoanDate { get; set; }

        public string Purpose { get; set; } //P-purchase; c=construction; r=repairs
        public DateTime? ConstructionCompDate { get; set; }
        public DateTime? PossessionDate { get; set; }

        public string Ownership { get; set; } // j=joint; s=sole
        public string JointOwnerName { get; set; }
        public string JointOwnerRelation { get; set; }

        public float JointOwnerShare { get; set; } //percentage
        public float RentalIncomePerMonth { get; set; }
        public float MunicipalTax { get; set; }

        public float InterestOnLoan { get; set; }
        public float InterestPreConstruction { get; set; }


        // second home loan

        public string PropertyAddress2 { get; set; }
        public string PropertyStatus2 { get; set; } //s=self; v=vacant; l=letout
        public string LoanBank2 { get; set; } //Bank drop down required...
        public string LoanBankPan2 { get; set; }

        public float LoanAmount2 { get; set; }
        public DateTime? LoanDate2 { get; set; }

        public string Purpose2 { get; set; } //P-purchase; c=construction; r=repairs
        public DateTime? ConstructionCompDate2 { get; set; }
        public DateTime? PossessionDate2 { get; set; }

        public string Ownership2 { get; set; } // j=joint; s=sole
        public string JointOwnerName2 { get; set; }
        public string JointOwnerRelation2 { get; set; }

        public float JointOwnerShare2 { get; set; } //percentage
        public float RentalIncomePerMonth2 { get; set; }
        public float MunicipalTax2 { get; set; }

        public float InterestOnLoan2 { get; set; }
        public float InterestPreConstruction2 { get; set; }


        public float OtherInterest { get; set; }
        public string OtherIncomeDesc { get; set; }
        public float OtherIncomeAmount { get; set; }

        public string TaxRegime { get; set; } //O=Old regime, N=New Regime (from april 2020).

        public string UpdateUserId { get; set; }
        public DateTime UpdateDate { get; set; }

        public bool LockEntry { get; set; }
        public bool FinLock { get; set; }

        public bool IsSubmitted { get; set; }

        public List<TaxDetailsPpfDto> PpfDetails { get; set; }
        public List<TaxDetailsInsuranceDto> InsuranceDetails { get; set; }
        public List<TaxDetailsNscDto> NscDetails { get; set; }
        public List<TaxDetailsMutualFundsDto> MutualFundDetails { get; set; }
        public List<TaxDetailsBankDepositDto> BankDeposits { get; set; }
        public List<TaxDetailsUlipDto> UlipDetails { get; set; }
        public List<TaxDetailsSukanyaDto> SukanyaDetails { get; set; }
        public List<TaxDetailsRentDto> RentDetails { get; set; }
    }
}