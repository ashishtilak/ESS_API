namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddCompositePKinTaxTables : DbMigration
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
            AlterColumn("dbo.TaxDetailsBankDeposits", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsInsurances", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsMutualFunds", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsNscs", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsPpfs", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsSukanyas", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.TaxDetailsUlips", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AddPrimaryKey("dbo.TaxDetailsBankDeposits", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "Id"});
            AddPrimaryKey("dbo.TaxDetailsInsurances", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "Id"});
            AddPrimaryKey("dbo.TaxDetailsMutualFunds", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "Id"});
            AddPrimaryKey("dbo.TaxDetailsNscs", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "Id"});
            AddPrimaryKey("dbo.TaxDetailsPpfs", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "Id"});
            AddPrimaryKey("dbo.TaxDetailsSukanyas", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "Id"});
            AddPrimaryKey("dbo.TaxDetailsUlips", new[] {"YearMonth", "EmpUnqId", "ActualFlag", "Id"});
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
            AlterColumn("dbo.TaxDetailsUlips", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsSukanyas", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsPpfs", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsNscs", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsMutualFunds", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsInsurances", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.TaxDetailsBankDeposits", "EmpUnqId", c => c.String(maxLength: 10));
            AddPrimaryKey("dbo.TaxDetailsUlips", "Id");
            AddPrimaryKey("dbo.TaxDetailsSukanyas", "Id");
            AddPrimaryKey("dbo.TaxDetailsPpfs", "Id");
            AddPrimaryKey("dbo.TaxDetailsNscs", "Id");
            AddPrimaryKey("dbo.TaxDetailsMutualFunds", "Id");
            AddPrimaryKey("dbo.TaxDetailsInsurances", "Id");
            AddPrimaryKey("dbo.TaxDetailsBankDeposits", "Id");
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