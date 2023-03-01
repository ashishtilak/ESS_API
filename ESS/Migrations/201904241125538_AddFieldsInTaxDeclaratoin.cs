namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddFieldsInTaxDeclaratoin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaxDeclarations", "Ulip", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "NotifiedMutualFund", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "UpdateDate", c => c.DateTime(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxDeclarations", "UpdateDate");
            DropColumn("dbo.TaxDeclarations", "NotifiedMutualFund");
            DropColumn("dbo.TaxDeclarations", "Ulip");
        }
    }
}