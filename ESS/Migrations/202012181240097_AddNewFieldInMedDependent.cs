namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddNewFieldInMedDependent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MedDependents", "DelReleaseGroupCode", c => c.String(maxLength: 2));
            AddColumn("dbo.MedDependents", "DelReleaseStrategy", c => c.String(maxLength: 15));
            AddColumn("dbo.MedDependents", "DelReleaseStatusCode", c => c.String(maxLength: 1));
            AddColumn("dbo.MedDependents", "DelReleaseDt", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.MedDependents", "DelReleaseUser", c => c.String(maxLength: 10));
            AddColumn("dbo.MedDependents", "IsChanged", c => c.Boolean(nullable: false));
            CreateIndex("dbo.MedDependents", "DelReleaseGroupCode");
            CreateIndex("dbo.MedDependents", new[] {"DelReleaseGroupCode", "DelReleaseStrategy"});
            CreateIndex("dbo.MedDependents", "DelReleaseStatusCode");
            AddForeignKey("dbo.MedDependents", "DelReleaseGroupCode", "dbo.ReleaseGroups", "ReleaseGroupCode");
            AddForeignKey("dbo.MedDependents", "DelReleaseStatusCode", "dbo.ReleaseStatus", "ReleaseStatusCode");
            AddForeignKey("dbo.MedDependents", new[] {"DelReleaseGroupCode", "DelReleaseStrategy"},
                "dbo.ReleaseStrategies", new[] {"ReleaseGroupCode", "ReleaseStrategy"});
        }

        public override void Down()
        {
            DropForeignKey("dbo.MedDependents", new[] {"DelReleaseGroupCode", "DelReleaseStrategy"},
                "dbo.ReleaseStrategies");
            DropForeignKey("dbo.MedDependents", "DelReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.MedDependents", "DelReleaseGroupCode", "dbo.ReleaseGroups");
            DropIndex("dbo.MedDependents", new[] {"DelReleaseStatusCode"});
            DropIndex("dbo.MedDependents", new[] {"DelReleaseGroupCode", "DelReleaseStrategy"});
            DropIndex("dbo.MedDependents", new[] {"DelReleaseGroupCode"});
            DropColumn("dbo.MedDependents", "IsChanged");
            DropColumn("dbo.MedDependents", "DelReleaseUser");
            DropColumn("dbo.MedDependents", "DelReleaseDt");
            DropColumn("dbo.MedDependents", "DelReleaseStatusCode");
            DropColumn("dbo.MedDependents", "DelReleaseStrategy");
            DropColumn("dbo.MedDependents", "DelReleaseGroupCode");
        }
    }
}