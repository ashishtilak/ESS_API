using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESS.Models
{
    public class TaxDeclarationHistory
    {
        [Key, Column(Order = 0)] public int Id { get; set; }

        public int YearMonth { get; set; } //201920

        [StringLength(10)] public string EmpUnqId { get; set; }

        public bool ActualFlag { get; set; } //Provisional v/s Actual

        public float TotalRentPaid { get; set; }

        [StringLength(200)] public string RentHouseAddress { get; set; }
        [StringLength(50)] public string LandLordName { get; set; }
        [StringLength(10)] public string LandLordPan { get; set; }

        public float PrevCompSalary { get; set; }
        public float PrevCompTds { get; set; }

        public float TotalPpfAmt { get; set; } //Details in table TaxDetailsPpf

        public float TotalBankDepositAmount { get; set; }

        public float TotalInsurancePremium { get; set; } //Deatils in table TaxDetailsInsurance

        public float TotalNscAmount { get; set; } //Deatils in table TaxDetailsNsc

        public float TotalMutualFund { get; set; } //Deatils in table TaxDetailsMutualFunds

        public float TotalUlip { get; set; }

        public float TotalSukanya { get; set; } //sukanya samriddhi 1

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

        // 80DD
        public float DisableDependent { get; set; }

        // 80DDB
        public float MedicalExpenditure { get; set; }

        [StringLength(200)] public string PropertyAddress { get; set; }
        [StringLength(1)] public string PropertyStatus { get; set; } //s=self; v=vacant; l=letout

        [StringLength(150)] public string LoanBank { get; set; } //Bank drop down required...
        [StringLength(10)] public string LoanBankPan { get; set; }

        public float LoanAmount { get; set; }
        public DateTime? LoanDate { get; set; }

        [StringLength(1)] public string Purpose { get; set; } //P-purchase; c=construction; r=repairs
        public DateTime? ConstructionCompDate { get; set; }
        public DateTime? PossessionDate { get; set; }

        [StringLength(1)] public string Ownership { get; set; } // j=joint; s=sole

        [StringLength(50)] public string JointOwnerName { get; set; }
        [StringLength(20)] public string JointOwnerRelation { get; set; }

        public float JointOwnerShare { get; set; } //percentage
        public float RentalIncomePerMonth { get; set; }

        public float MunicipalTax { get; set; }

        public float InterestOnLoan { get; set; }
        public float InterestPreConstruction { get; set; }

        //fields for second home loan

        [StringLength(200)] public string PropertyAddress2 { get; set; }
        [StringLength(1)] public string PropertyStatus2 { get; set; } //s=self; v=vacant; l=letout

        [StringLength(150)] public string LoanBank2 { get; set; } //Bank drop down required...
        [StringLength(10)] public string LoanBankPan2 { get; set; }

        public float LoanAmount2 { get; set; }
        public DateTime? LoanDate2 { get; set; }

        [StringLength(1)] public string Purpose2 { get; set; } //P-purchase; c=construction; r=repairs
        public DateTime? ConstructionCompDate2 { get; set; }
        public DateTime? PossessionDate2 { get; set; }

        [StringLength(1)] public string Ownership2 { get; set; } // j=joint; s=sole

        [StringLength(50)] public string JointOwnerName2 { get; set; }
        [StringLength(20)] public string JointOwnerRelation2 { get; set; }

        public float JointOwnerShare2 { get; set; } //percentage
        public float RentalIncomePerMonth2 { get; set; }

        public float MunicipalTax2 { get; set; }

        public float InterestOnLoan2 { get; set; }
        public float InterestPreConstruction2 { get; set; }


        public float OtherInterest { get; set; }

        [StringLength(1)] public string TaxRegime { get; set; } //O=Old regime, N=New Regime (from april 2020).

        [StringLength(50)] public string OtherIncomeDesc { get; set; }
        public float OtherIncomeAmount { get; set; }

        public bool IsSubmitted { get; set; }

        [StringLength(8)] public string UpdateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}