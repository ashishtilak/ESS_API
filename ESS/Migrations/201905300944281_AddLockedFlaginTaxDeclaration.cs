namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddLockedFlaginTaxDeclaration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaxDeclarations", "LockEntry", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxDeclarations", "LockEntry");
        }
    }
}