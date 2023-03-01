namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddTaxDeclarationHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TaxDeclarationHistories",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ActualFlag = c.Boolean(nullable: false),
                        TotalRentPaid = c.Single(nullable: false),
                        RentHouseAddress = c.String(maxLength: 200),
                        LandLordName = c.String(maxLength: 50),
                        LandLordPan = c.String(maxLength: 10),
                        PrevCompSalary = c.Single(nullable: false),
                        PrevCompTds = c.Single(nullable: false),
                        TotalPpfAmt = c.Single(nullable: false),
                        TotalBankDepositAmount = c.Single(nullable: false),
                        TotalInsurancePremium = c.Single(nullable: false),
                        TotalNscAmount = c.Single(nullable: false),
                        TotalMutualFund = c.Single(nullable: false),
                        TotalUlip = c.Single(nullable: false),
                        TotalSukanya = c.Single(nullable: false),
                        HouseLoanPrincipal = c.Single(nullable: false),
                        Child1Name = c.String(),
                        TuitionFeeChild1 = c.Single(nullable: false),
                        Child2Name = c.String(),
                        TuitionFeeChild2 = c.Single(nullable: false),
                        NotifiedPensionScheme = c.Single(nullable: false),
                        Others1Desc = c.String(),
                        Others1Amount = c.Single(nullable: false),
                        Others2Desc = c.String(),
                        Others2Amount = c.Single(nullable: false),
                        RajivGandhiEquity = c.Single(nullable: false),
                        MedicalPremiumSelf = c.Single(nullable: false),
                        MedicalPremiumParents = c.Single(nullable: false),
                        MedicalPremiumParentsAge = c.Single(nullable: false),
                        MedicalPreventiveHealthCheckup = c.Single(nullable: false),
                        EducationLoanInterest = c.Single(nullable: false),
                        PhysicalDisability = c.Single(nullable: false),
                        SevereDisability = c.Single(nullable: false),
                        NationalPensionScheme = c.Single(nullable: false),
                        PropertyAddress = c.String(maxLength: 200),
                        PropertyStatus = c.String(maxLength: 1),
                        LoanBank = c.String(maxLength: 150),
                        LoanBankPan = c.String(maxLength: 10),
                        LoanAmount = c.Single(nullable: false),
                        LoanDate = c.DateTime(),
                        Purpose = c.String(maxLength: 1),
                        ConstructionCompDate = c.DateTime(),
                        PossessionDate = c.DateTime(),
                        Ownership = c.String(maxLength: 1),
                        JointOwnerName = c.String(maxLength: 50),
                        JointOwnerRelation = c.String(maxLength: 20),
                        JointOwnerShare = c.Single(nullable: false),
                        RentalIncomePerMonth = c.Single(nullable: false),
                        InterestOnLoan = c.Single(nullable: false),
                        InterestPreConstruction = c.Single(nullable: false),
                        OtherInterest = c.Single(nullable: false),
                        OtherIncomeDesc = c.String(maxLength: 50),
                        OtherIncomeAmount = c.Single(nullable: false),
                        UpdateUserId = c.String(maxLength: 8),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag});

            AddColumn("dbo.TaxDeclarations", "UpdateUserId", c => c.String(maxLength: 8));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxDeclarations", "UpdateUserId");
            DropTable("dbo.TaxDeclarationHistories");
        }
    }
}