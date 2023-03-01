namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedAttdFieldsInGatePass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GatePasses", "AttdGpOutTime", c => c.DateTime());
            AddColumn("dbo.GatePasses", "AttdGpInTime", c => c.DateTime());
            AddColumn("dbo.GatePasses", "AttdGpFlag", c => c.String(maxLength: 10));
        }

        public override void Down()
        {
            DropColumn("dbo.GatePasses", "AttdGpFlag");
            DropColumn("dbo.GatePasses", "AttdGpInTime");
            DropColumn("dbo.GatePasses", "AttdGpOutTime");
        }
    }
}