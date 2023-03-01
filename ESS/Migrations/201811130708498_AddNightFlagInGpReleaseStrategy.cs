namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddNightFlagInGpReleaseStrategy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GpReleaseStrategies", "NightFlag", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.GpReleaseStrategies", "NightFlag");
        }
    }
}