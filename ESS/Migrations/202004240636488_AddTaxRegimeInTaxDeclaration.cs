namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddTaxRegimeInTaxDeclaration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaxDeclarationHistories", "TaxRegime", c => c.String(maxLength: 1));
            AddColumn("dbo.TaxDeclarations", "TaxRegime", c => c.String(maxLength: 1));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxDeclarations", "TaxRegime");
            DropColumn("dbo.TaxDeclarationHistories", "TaxRegime");
        }
    }
}