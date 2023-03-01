namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class IsSubmittedAddedInTaxDecl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaxDeclarationHistories", "IsSubmitted", c => c.Boolean(nullable: false));
            AddColumn("dbo.TaxDeclarations", "IsSubmitted", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxDeclarations", "IsSubmitted");
            DropColumn("dbo.TaxDeclarationHistories", "IsSubmitted");
        }
    }
}