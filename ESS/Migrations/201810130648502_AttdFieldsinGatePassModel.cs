namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AttdFieldsinGatePassModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GatePasses", "GpRemarks", c => c.String(maxLength: 100));
            AddColumn("dbo.GatePasses", "AttdUpdate", c => c.DateTime(nullable: false));
            AddColumn("dbo.GatePasses", "AttdFlag", c => c.String(maxLength: 10));
        }

        public override void Down()
        {
            DropColumn("dbo.GatePasses", "AttdFlag");
            DropColumn("dbo.GatePasses", "AttdUpdate");
            DropColumn("dbo.GatePasses", "GpRemarks");
        }
    }
}