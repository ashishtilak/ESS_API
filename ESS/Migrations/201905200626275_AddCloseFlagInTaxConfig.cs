namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddCloseFlagInTaxConfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaxConfigs", "CloseFlag", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxConfigs", "CloseFlag");
        }
    }
}