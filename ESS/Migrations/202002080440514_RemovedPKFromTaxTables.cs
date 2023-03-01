namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemovedPKFromTaxTables : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TaxDetailsBankDeposits", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsInsurances", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsMutualFunds", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsNscs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsPpfs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsSukanyas", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsUlips", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropPrimaryKey("dbo.TaxDetailsBankDeposits");
            DropPrimaryKey("dbo.TaxDetailsInsurances");
            DropPrimaryKey("dbo.TaxDetailsMutualFunds");
            DropPrimaryKey("dbo.TaxDetailsNscs");
            DropPrimaryKey("dbo.TaxDetailsPpfs");
            DropPrimaryKey("dbo.TaxDetailsSukanyas");
            DropPrimaryKey("dbo.TaxDetailsUlips");
            AddColumn("dbo.TaxDetailsBankDeposits", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.TaxDetailsInsurances", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.TaxDetailsMutualFunds", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.TaxDetailsNscs", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.TaxDetailsPpfs", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.TaxDetailsSukanyas", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.TaxDetailsUlips", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.TaxDetailsBankDeposits", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsBankDeposits", "BankAccountNo", c => c.String(maxLength: 20));
            AlterColumn("dbo.TaxDetailsInsurances", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsInsurances", "PolicyNo", c => c.String(maxLength: 20));
            AlterColumn("dbo.TaxDetailsMutualFunds", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsMutualFunds", "MutualFundName", c => c.String(maxLength: 50));
            AlterColumn("dbo.TaxDetailsNscs", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsNscs", "NscNumber", c => c.String(maxLength: 20));
            AlterColumn("dbo.TaxDetailsPpfs", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsPpfs", "PpfAcNo", c => c.String(maxLength: 20));
            AlterColumn("dbo.TaxDetailsSukanyas", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsSukanyas", "SukanyaName", c => c.String(maxLength: 50));
            AlterColumn("dbo.TaxDetailsUlips", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsUlips", "UlipNo", c => c.String(maxLength: 50));
            AddPrimaryKey("dbo.TaxDetailsBankDeposits", "Id");
            AddPrimaryKey("dbo.TaxDetailsInsurances", "Id");
            AddPrimaryKey("dbo.TaxDetailsMutualFunds", "Id");
            AddPrimaryKey("dbo.TaxDetailsNscs", "Id");
            AddPrimaryKey("dbo.TaxDetailsPpfs", "Id");
            AddPrimaryKey("dbo.TaxDetailsSukanyas", "Id");
            AddPrimaryKey("dbo.TaxDetailsUlips", "Id");
            CreateIndex("dbo.TaxDetailsBankDeposits", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsInsurances", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsMutualFunds", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsNscs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsPpfs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsSukanyas", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsUlips", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
        }

        public override void Down()
        {
            DropIndex("dbo.TaxDetailsUlips", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsSukanyas", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsPpfs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsNscs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsMutualFunds", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsInsurances", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropIndex("dbo.TaxDetailsBankDeposits", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropPrimaryKey("dbo.TaxDetailsUlips");
            DropPrimaryKey("dbo.TaxDetailsSukanyas");
            DropPrimaryKey("dbo.TaxDetailsPpfs");
            DropPrimaryKey("dbo.TaxDetailsNscs");
            DropPrimaryKey("dbo.TaxDetailsMutualFunds");
            DropPrimaryKey("dbo.TaxDetailsInsurances");
            DropPrimaryKey("dbo.TaxDetailsBankDeposits");
            AlterColumn("dbo.TaxDetailsUlips", "UlipNo", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.TaxDetailsUlips", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsSukanyas", "SukanyaName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.TaxDetailsSukanyas", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsPpfs", "PpfAcNo", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.TaxDetailsPpfs", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsNscs", "NscNumber", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.TaxDetailsNscs", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsMutualFunds", "MutualFundName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.TaxDetailsMutualFunds", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsInsurances", "PolicyNo", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.TaxDetailsInsurances", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsBankDeposits", "BankAccountNo", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.TaxDetailsBankDeposits", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            DropColumn("dbo.TaxDetailsUlips", "Id");
            DropColumn("dbo.TaxDetailsSukanyas", "Id");
            DropColumn("dbo.TaxDetailsPpfs", "Id");
            DropColumn("dbo.TaxDetailsNscs", "Id");
            DropColumn("dbo.TaxDetailsMutualFunds", "Id");
            DropColumn("dbo.TaxDetailsInsurances", "Id");
            DropColumn("dbo.TaxDetailsBankDeposits", "Id");
            AddPrimaryKey("dbo.TaxDetailsUlips", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "UlipNo"});
            AddPrimaryKey("dbo.TaxDetailsSukanyas", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "SukanyaName"});
            AddPrimaryKey("dbo.TaxDetailsPpfs", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "PpfAcNo"});
            AddPrimaryKey("dbo.TaxDetailsNscs", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "NscNumber"});
            AddPrimaryKey("dbo.TaxDetailsMutualFunds", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "MutualFundName"});
            AddPrimaryKey("dbo.TaxDetailsInsurances", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "PolicyNo"});
            AddPrimaryKey("dbo.TaxDetailsBankDeposits", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "BankAccountNo"});
            CreateIndex("dbo.TaxDetailsUlips", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsSukanyas", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsPpfs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsNscs", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsMutualFunds", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsInsurances", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            CreateIndex("dbo.TaxDetailsBankDeposits", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
        }
    }
}