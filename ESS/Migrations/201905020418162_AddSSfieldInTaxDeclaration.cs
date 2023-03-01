namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddSSfieldInTaxDeclaration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaxDeclarations", "NotifiedMutualFund2", c => c.Single(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxDeclarations", "NotifiedMutualFund2");
        }
    }
}