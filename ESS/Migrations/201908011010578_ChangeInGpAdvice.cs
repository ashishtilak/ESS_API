namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeInGpAdvice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GpAdvices", "ReleaseGroupCode", c => c.String(maxLength: 2));
            AddColumn("dbo.GpAdvices", "GaReleaseStrategy", c => c.String(maxLength: 15));
            AddColumn("dbo.GpAdvices", "ReleaseStatusCode", c => c.String(maxLength: 1));
            CreateIndex("dbo.GpAdvices", "ReleaseGroupCode");
            CreateIndex("dbo.GpAdvices", new[] {"ReleaseGroupCode", "GaReleaseStrategy"});
            CreateIndex("dbo.GpAdvices", "ReleaseStatusCode");
            AddForeignKey("dbo.GpAdvices", "ReleaseGroupCode", "dbo.ReleaseGroups", "ReleaseGroupCode");
            AddForeignKey("dbo.GpAdvices", "ReleaseStatusCode", "dbo.ReleaseStatus", "ReleaseStatusCode");
            AddForeignKey("dbo.GpAdvices", new[] {"ReleaseGroupCode", "GaReleaseStrategy"}, "dbo.GaReleaseStrategies",
                new[] {"ReleaseGroupCode", "GaReleaseStrategy"});
        }

        public override void Down()
        {
            DropForeignKey("dbo.GpAdvices", new[] {"ReleaseGroupCode", "GaReleaseStrategy"}, "dbo.GaReleaseStrategies");
            DropForeignKey("dbo.GpAdvices", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.GpAdvices", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropIndex("dbo.GpAdvices", new[] {"ReleaseStatusCode"});
            DropIndex("dbo.GpAdvices", new[] {"ReleaseGroupCode", "GaReleaseStrategy"});
            DropIndex("dbo.GpAdvices", new[] {"ReleaseGroupCode"});
            DropColumn("dbo.GpAdvices", "ReleaseStatusCode");
            DropColumn("dbo.GpAdvices", "GaReleaseStrategy");
            DropColumn("dbo.GpAdvices", "ReleaseGroupCode");
        }
    }
}