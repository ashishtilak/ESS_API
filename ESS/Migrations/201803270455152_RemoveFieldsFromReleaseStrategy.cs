namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveFieldsFromReleaseStrategy : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReleaseStrategies", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"},
                "dbo.Departments");
            DropForeignKey("dbo.ReleaseStrategies",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"}, "dbo.Sections");
            DropForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"},
                "dbo.Stations");
            DropForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropIndex("dbo.ReleaseStrategies", new[] {"CompCode"});
            DropIndex("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            DropIndex("dbo.ReleaseStrategies",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            DropColumn("dbo.ReleaseStrategies", "CompCode");
            DropColumn("dbo.ReleaseStrategies", "WrkGrp");
            DropColumn("dbo.ReleaseStrategies", "UnitCode");
            DropColumn("dbo.ReleaseStrategies", "DeptCode");
            DropColumn("dbo.ReleaseStrategies", "StatCode");
            DropColumn("dbo.ReleaseStrategies", "SecCode");
        }

        public override void Down()
        {
            AddColumn("dbo.ReleaseStrategies", "SecCode", c => c.String(maxLength: 3));
            AddColumn("dbo.ReleaseStrategies", "StatCode", c => c.String(maxLength: 3));
            AddColumn("dbo.ReleaseStrategies", "DeptCode", c => c.String(maxLength: 3));
            AddColumn("dbo.ReleaseStrategies", "UnitCode", c => c.String(maxLength: 3));
            AddColumn("dbo.ReleaseStrategies", "WrkGrp", c => c.String(maxLength: 10));
            AddColumn("dbo.ReleaseStrategies", "CompCode", c => c.String(maxLength: 2));
            CreateIndex("dbo.ReleaseStrategies",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            CreateIndex("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            CreateIndex("dbo.ReleaseStrategies", "CompCode");
            AddForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups",
                new[] {"CompCode", "WrkGrp"});
            AddForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units",
                new[] {"CompCode", "WrkGrp", "UnitCode"});
            AddForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"},
                "dbo.Stations", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            AddForeignKey("dbo.ReleaseStrategies",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"}, "dbo.Sections",
                new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            AddForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"},
                "dbo.Departments", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            AddForeignKey("dbo.ReleaseStrategies", "CompCode", "dbo.Companies", "CompCode");
        }
    }
}