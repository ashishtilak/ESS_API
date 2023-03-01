using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESS.Dto
{
    public class TaxDeclReportDto
    {
        public int YearMonth { get; set; } //201920
        public string EmpUnqId { get; set; }
        public string SapId { get; set; }
        public string EmpName { get; set; }

        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }

        public int InsCode { get; set; }
        public float InsPro { get; set; }
        public float InsAct { get; set; }

        public int UlipCode { get; set; }
        public float UlipAct { get; set; }
        public float UlipPro { get; set; }

        public int MfCode { get; set; }
        public float MfAct { get; set; }
        public float MfPro { get; set; }

        public int PpfCode { get; set; }
        public float PpfAct { get; set; }
        public float PpfPro { get; set; }

        public int NscCode { get; set; }
        public float NscAct { get; set; }
        public float NscPro { get; set; }

        public int HomeLoanCode { get; set; }
        public float HomeLoanAct { get; set; }
        public float HomeLoanPro { get; set; }

        public int HomeLoanCode2 { get; set; }
        public float HomeLoanAct2 { get; set; }
        public float HomeLoanPro2 { get; set; }

        public int NotifiedMfCode { get; set; }
        public float NotifiedMfAct { get; set; }
        public float NotifiedMfPro { get; set; }

        public int Child1Code { get; set; }
        public float Child1Act { get; set; }
        public float Child1Pro { get; set; }

        public int Child2Code { get; set; }
        public float Child2Act { get; set; }
        public float Child2Pro { get; set; }

        public int TermDepoCode { get; set; }
        public float TermDepoAct { get; set; }
        public float TermDepoPro { get; set; }

        public float Total80C { get; set; }

        public float LongTermMf { get; set; }
        public float MedicalPremium { get; set; }
        public float EduLoanInterest { get; set; }
        public float Nps { get; set; }

        public float InterestOnLoan { get; set; }
        public float InterestPreConstruction { get; set; }
        public float RentReceived { get; set; }
        public string BankName { get; set; }
        public string BankPan { get; set; }

        public float InterestOnLoan2 { get; set; }
        public float InterestPreConstruction2 { get; set; }
        public float RentReceived2 { get; set; }
        public string BankName2 { get; set; }
        public string BankPan2 { get; set; }


        public string AccomodationType { get; set; }

        public float RentPaidAprilPro { get; set; }
        public float RentPaidAprilAct { get; set; }

        public float RentPaidPro { get; set; }
        public float RentPaidAct { get; set; }
        public string LandLordName { get; set; }
        public string LandLordPan { get; set; }

        public float RajivGandhiEquity { get; set; }
        public float MedicalPremiumSelf { get; set; }
        public float MedicalPremiumParents { get; set; }
        public float MedicalPreventiveHealthCheckup { get; set; }
        public float PhysicalDisability { get; set; }
        public float SevereDisability { get; set; }

        public bool LockEntry { get; set; }

        public bool ActualFlag { get; set; }

        public float DisableDependent { get; set; }
        public float MedicalExpenditure { get; set; }

        public float MunicipalTax { get; set; }

        public string TaxRegime { get; set; } //O=Old regime, N=New Regime (from april 2020).
    }
}