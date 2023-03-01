namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedSection : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Sections",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        UnitCode = c.String(nullable: false, maxLength: 3),
                        DeptCode = c.String(nullable: false, maxLength: 3),
                        StatCode = c.String(nullable: false, maxLength: 3),
                        SecCode = c.String(nullable: false, maxLength: 3),
                        SecName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode, t.SecCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Departments", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .ForeignKey("dbo.Stations", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode})
                .ForeignKey("dbo.Units", t => new {t.CompCode, t.WrkGrp, t.UnitCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode});

            AlterColumn("dbo.LeaveApplicationDetails", "TotalDays", c => c.Single(nullable: false));
        }

        public override void Down()
        {
            DropForeignKey("dbo.Sections", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.Sections", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.Sections", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"},
                "dbo.Stations");
            DropForeignKey("dbo.Sections", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"}, "dbo.Departments");
            DropForeignKey("dbo.Sections", "CompCode", "dbo.Companies");
            DropIndex("dbo.Sections", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            DropIndex("dbo.Sections", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            DropIndex("dbo.Sections", new[] {"CompCode"});
            AlterColumn("dbo.LeaveApplicationDetails", "TotalDays", c => c.Int(nullable: false));
            DropTable("dbo.Sections");
        }
    }
}