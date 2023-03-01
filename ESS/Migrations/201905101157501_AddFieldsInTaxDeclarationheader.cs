namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddFieldsInTaxDeclarationheader : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaxDeclarations", "SevereDisability", c => c.Single(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxDeclarations", "SevereDisability");
        }
    }
}