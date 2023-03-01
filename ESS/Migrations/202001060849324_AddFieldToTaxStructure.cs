namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddFieldToTaxStructure : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaxDeclarationHistories", "DisableDependent", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarationHistories", "MedicalExpenditure", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarationHistories", "MunicipalTax", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "DisableDependent", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "MedicalExpenditure", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "MunicipalTax", c => c.Single(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxDeclarations", "MunicipalTax");
            DropColumn("dbo.TaxDeclarations", "MedicalExpenditure");
            DropColumn("dbo.TaxDeclarations", "DisableDependent");
            DropColumn("dbo.TaxDeclarationHistories", "MunicipalTax");
            DropColumn("dbo.TaxDeclarationHistories", "MedicalExpenditure");
            DropColumn("dbo.TaxDeclarationHistories", "DisableDependent");
        }
    }
}