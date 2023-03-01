namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class TaxDeclarationTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TaxDeclarations",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ActualFlag = c.Boolean(nullable: false),
                        RentPaidPerMonth = c.Single(nullable: false),
                        RentHouseAddress = c.String(maxLength: 200),
                        LandLordName = c.String(maxLength: 50),
                        LandLordPan = c.String(maxLength: 10),
                        PrevCompSalary = c.Single(nullable: false),
                        PrevCompTds = c.Single(nullable: false),
                        TotalPpfAmt = c.Single(nullable: false),
                        BankDepositAcNo = c.String(maxLength: 20),
                        BankDepositAmount = c.Single(nullable: false),
                        TotalInsurancePremium = c.Single(nullable: false),
                        TotalNscAmount = c.Single(nullable: false),
                        TotalMutualFund = c.Single(nullable: false),
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
                        NationalPensionScheme = c.Single(nullable: false),
                        PropertyAddress = c.String(maxLength: 200),
                        PropertyStatus = c.String(maxLength: 1),
                        LoanBank = c.String(maxLength: 20),
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
                    })
                .PrimaryKey(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag});

            CreateTable(
                    "dbo.TaxDetailsInsurances",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ActualFlag = c.Boolean(nullable: false),
                        PolicyNo = c.String(nullable: false, maxLength: 20),
                        PolicyMode = c.String(),
                        SumInsured = c.Single(nullable: false),
                        PremiumAmount = c.Single(nullable: false),
                        PremiumPaidDate = c.DateTime(),
                        PremiumDueDate = c.DateTime(),
                        AnnualPremiumAmount = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag, t.PolicyNo})
                .ForeignKey("dbo.TaxDeclarations", t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag})
                .Index(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag});

            CreateTable(
                    "dbo.TaxDetailsMutualFunds",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ActualFlag = c.Boolean(nullable: false),
                        MutualFundName = c.String(nullable: false, maxLength: 50),
                        MutualFundAmount = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag, t.MutualFundName})
                .ForeignKey("dbo.TaxDeclarations", t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag})
                .Index(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag});

            CreateTable(
                    "dbo.TaxDetailsNscs",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ActualFlag = c.Boolean(nullable: false),
                        NscNumber = c.String(nullable: false, maxLength: 20),
                        NscPurchaseDate = c.DateTime(),
                        NscAmount = c.Single(nullable: false),
                        NscInterestAmount = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag, t.NscNumber})
                .ForeignKey("dbo.TaxDeclarations", t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag})
                .Index(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag});

            CreateTable(
                    "dbo.TaxDetailsPpfs",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ActualFlag = c.Boolean(nullable: false),
                        PpfAcNo = c.String(nullable: false, maxLength: 20),
                        PpfDepositeDate = c.DateTime(),
                        PpfAmt = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag, t.PpfAcNo})
                .ForeignKey("dbo.TaxDeclarations", t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag})
                .Index(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag});
        }

        public override void Down()
        {
            DropForeignKey("dbo.TaxDetailsPpfs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"}, "dbo.TaxDeclarations");
            DropForeignKey("dbo.TaxDetailsNscs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"}, "dbo.TaxDeclarations");
            DropForeignKey("dbo.TaxDetailsMutualFunds", new[] {"YearMonth", "EmpUnqId", "ActualFlag"},
                "dbo.TaxDeclarations");
            DropForeignKey("dbo.TaxDetailsInsurances", new[] {"YearMonth", "EmpUnqId", "ActualFlag"},
                "dbo.TaxDeclarations");
            DropIndex("dbo.TaxDetailsPpfs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsNscs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsMutualFunds", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsInsurances", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropTable("dbo.TaxDetailsPpfs");
            DropTable("dbo.TaxDetailsNscs");
            DropTable("dbo.TaxDetailsMutualFunds");
            DropTable("dbo.TaxDetailsInsurances");
            DropTable("dbo.TaxDeclarations");
        }
    }
}