namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemovedCatFromRelease : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "CatCode"}, "dbo.Categories");
            DropIndex("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "CatCode"});
            CreateIndex("dbo.ReleaseStrategies", "CompCode");
            DropColumn("dbo.ReleaseStrategies", "CatCode");
        }

        public override void Down()
        {
            AddColumn("dbo.ReleaseStrategies", "CatCode", c => c.String(maxLength: 3));
            DropIndex("dbo.ReleaseStrategies", new[] {"CompCode"});
            CreateIndex("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "CatCode"});
            AddForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "CatCode"}, "dbo.Categories",
                new[] {"CompCode", "WrkGrp", "CatCode"});
        }
    }
}