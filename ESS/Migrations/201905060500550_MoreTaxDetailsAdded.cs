namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class MoreTaxDetailsAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TaxDetailsBankDeposits",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ActualFlag = c.Boolean(nullable: false),
                        BankAccountNo = c.String(nullable: false, maxLength: 20),
                        DepositDate = c.DateTime(),
                        DepositAmount = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag, t.BankAccountNo})
                .ForeignKey("dbo.TaxDeclarations", t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag})
                .Index(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag});

            CreateTable(
                    "dbo.TaxDetailsSukanyas",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ActualFlag = c.Boolean(nullable: false),
                        SukanyaName = c.String(nullable: false, maxLength: 50),
                        SukanyaDate = c.DateTime(),
                        SukanyaAmount = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag, t.SukanyaName})
                .ForeignKey("dbo.TaxDeclarations", t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag})
                .Index(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag});

            CreateTable(
                    "dbo.TaxDetailsUlips",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ActualFlag = c.Boolean(nullable: false),
                        UlipNo = c.String(nullable: false, maxLength: 50),
                        UlipDate = c.DateTime(),
                        UlipAmount = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag, t.UlipNo})
                .ForeignKey("dbo.TaxDeclarations", t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag})
                .Index(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag});

            AddColumn("dbo.TaxDeclarations", "TotalBankDepositAmount", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "TotalUlip", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "TotalSukanya", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDetailsInsurances", "PolicyDate", c => c.DateTime());
            AddColumn("dbo.TaxDetailsMutualFunds", "MutualFundDate", c => c.DateTime());
            DropColumn("dbo.TaxDeclarations", "BankDepositAcNo");
            DropColumn("dbo.TaxDeclarations", "BankDepositAmount");
            DropColumn("dbo.TaxDeclarations", "Ulip");
            DropColumn("dbo.TaxDeclarations", "NotifiedMutualFund");
            DropColumn("dbo.TaxDeclarations", "NotifiedMutualFund2");
            DropColumn("dbo.TaxDetailsInsurances", "PremiumAmount");
        }

        public override void Down()
        {
            AddColumn("dbo.TaxDetailsInsurances", "PremiumAmount", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "NotifiedMutualFund2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "NotifiedMutualFund", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "Ulip", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "BankDepositAmount", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "BankDepositAcNo", c => c.String(maxLength: 20));
            DropForeignKey("dbo.TaxDetailsUlips", new[] {"YearMonth", "EmpUnqId", "ActualFlag"}, "dbo.TaxDeclarations");
            DropForeignKey("dbo.TaxDetailsSukanyas", new[] {"YearMonth", "EmpUnqId", "ActualFlag"},
                "dbo.TaxDeclarations");
            DropForeignKey("dbo.TaxDetailsBankDeposits", new[] {"YearMonth", "EmpUnqId", "ActualFlag"},
                "dbo.TaxDeclarations");
            DropIndex("dbo.TaxDetailsUlips", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsSukanyas", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsBankDeposits", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropColumn("dbo.TaxDetailsMutualFunds", "MutualFundDate");
            DropColumn("dbo.TaxDetailsInsurances", "PolicyDate");
            DropColumn("dbo.TaxDeclarations", "TotalSukanya");
            DropColumn("dbo.TaxDeclarations", "TotalUlip");
            DropColumn("dbo.TaxDeclarations", "TotalBankDepositAmount");
            DropTable("dbo.TaxDetailsUlips");
            DropTable("dbo.TaxDetailsSukanyas");
            DropTable("dbo.TaxDetailsBankDeposits");
        }
    }
}