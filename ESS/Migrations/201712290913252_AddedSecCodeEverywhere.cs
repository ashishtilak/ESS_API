namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedSecCodeEverywhere : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            DropIndex("dbo.LeaveApplications", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            DropIndex("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            AddColumn("dbo.Employees", "SecCode", c => c.String(maxLength: 3));
            AddColumn("dbo.LeaveApplications", "SecCode", c => c.String(maxLength: 3));
            AddColumn("dbo.ReleaseStrategies", "SecCode", c => c.String(maxLength: 3));
            CreateIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            CreateIndex("dbo.LeaveApplications",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            CreateIndex("dbo.ReleaseStrategies",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            AddForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"},
                "dbo.Sections", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            AddForeignKey("dbo.ReleaseStrategies",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"}, "dbo.Sections",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            AddForeignKey("dbo.LeaveApplications",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"}, "dbo.Sections",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
        }

        public override void Down()
        {
            DropForeignKey("dbo.LeaveApplications",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"}, "dbo.Sections");
            DropForeignKey("dbo.ReleaseStrategies",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"}, "dbo.Sections");
            DropForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"},
                "dbo.Sections");
            DropIndex("dbo.ReleaseStrategies",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            DropIndex("dbo.LeaveApplications",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            DropIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            DropColumn("dbo.ReleaseStrategies", "SecCode");
            DropColumn("dbo.LeaveApplications", "SecCode");
            DropColumn("dbo.Employees", "SecCode");
            CreateIndex("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            CreateIndex("dbo.LeaveApplications", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            CreateIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
        }
    }
}