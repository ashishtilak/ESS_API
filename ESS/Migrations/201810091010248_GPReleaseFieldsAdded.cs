namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class GPReleaseFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GatePasses", "ReleaseGroupCode", c => c.String(maxLength: 2));
            AddColumn("dbo.GatePasses", "ReleaseStrategy", c => c.String(maxLength: 15));
            AddColumn("dbo.GatePasses", "ReleaseStatusCode", c => c.String(maxLength: 1));
            AlterColumn("dbo.GatePasses", "EmpUnqId", c => c.String(maxLength: 10));
            AlterColumn("dbo.GatePasses", "PlaceOfVisit", c => c.String(maxLength: 100));
            AlterColumn("dbo.GatePasses", "Reason", c => c.String(maxLength: 100));
            AlterColumn("dbo.GatePasses", "AddUser", c => c.String(maxLength: 10));
            AlterColumn("dbo.GatePasses", "GateOutUser", c => c.String(maxLength: 10));
            AlterColumn("dbo.GatePasses", "GateInUser", c => c.String(maxLength: 10));
            CreateIndex("dbo.GatePasses", "ReleaseGroupCode");
            CreateIndex("dbo.GatePasses", new[] {"ReleaseGroupCode", "ReleaseStrategy"});
            CreateIndex("dbo.GatePasses", "ReleaseStatusCode");
            AddForeignKey("dbo.GatePasses", "ReleaseGroupCode", "dbo.ReleaseGroups", "ReleaseGroupCode");
            AddForeignKey("dbo.GatePasses", "ReleaseStatusCode", "dbo.ReleaseStatus", "ReleaseStatusCode");
            AddForeignKey("dbo.GatePasses", new[] {"ReleaseGroupCode", "ReleaseStrategy"}, "dbo.ReleaseStrategies",
                new[] {"ReleaseGroupCode", "ReleaseStrategy"});
        }

        public override void Down()
        {
            DropForeignKey("dbo.GatePasses", new[] {"ReleaseGroupCode", "ReleaseStrategy"}, "dbo.ReleaseStrategies");
            DropForeignKey("dbo.GatePasses", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.GatePasses", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropIndex("dbo.GatePasses", new[] {"ReleaseStatusCode"});
            DropIndex("dbo.GatePasses", new[] {"ReleaseGroupCode", "ReleaseStrategy"});
            DropIndex("dbo.GatePasses", new[] {"ReleaseGroupCode"});
            AlterColumn("dbo.GatePasses", "GateInUser", c => c.String());
            AlterColumn("dbo.GatePasses", "GateOutUser", c => c.String());
            AlterColumn("dbo.GatePasses", "AddUser", c => c.String());
            AlterColumn("dbo.GatePasses", "Reason", c => c.String());
            AlterColumn("dbo.GatePasses", "PlaceOfVisit", c => c.String());
            AlterColumn("dbo.GatePasses", "EmpUnqId", c => c.String());
            DropColumn("dbo.GatePasses", "ReleaseStatusCode");
            DropColumn("dbo.GatePasses", "ReleaseStrategy");
            DropColumn("dbo.GatePasses", "ReleaseGroupCode");
        }
    }
}