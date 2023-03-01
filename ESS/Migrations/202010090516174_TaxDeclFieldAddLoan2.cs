namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class TaxDeclFieldAddLoan2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaxDeclarationHistories", "HouseLoanPrincipal2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "HouseLoanPrincipal2", c => c.Single(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxDeclarations", "HouseLoanPrincipal2");
            DropColumn("dbo.TaxDeclarationHistories", "HouseLoanPrincipal2");
        }
    }
}