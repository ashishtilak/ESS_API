namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class FinLockInTaxDeclaration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaxDeclarations", "FinLock", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxDeclarations", "FinLock");
        }
    }
}