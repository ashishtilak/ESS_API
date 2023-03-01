namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NewGpReleaseStrategy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.GpReleaseStrategies",
                    c => new
                    {
                        ReleaseGroupCode = c.String(nullable: false, maxLength: 2),
                        GpReleaseStrategy = c.String(nullable: false, maxLength: 15),
                        CompCode = c.String(maxLength: 2),
                        WrkGrp = c.String(maxLength: 10),
                        UnitCode = c.String(maxLength: 3),
                        DeptCode = c.String(nullable: false, maxLength: 3),
                        StatCode = c.String(nullable: false, maxLength: 3),
                        GpReleaseStrategyName = c.String(maxLength: 100),
                        Active = c.Boolean(nullable: false),
                        UpdDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        UpdUser = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => new {t.ReleaseGroupCode, t.GpReleaseStrategy})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Departments", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.Stations", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode})
                .ForeignKey("dbo.Units", t => new {t.CompCode, t.WrkGrp, t.UnitCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.ReleaseGroupCode)
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode});

            CreateTable(
                    "dbo.GpReleaseStrategyLevels",
                    c => new
                    {
                        ReleaseGroupCode = c.String(nullable: false, maxLength: 2),
                        GpReleaseStrategy = c.String(nullable: false, maxLength: 15),
                        GpReleaseStrategyLevel = c.Int(nullable: false),
                        ReleaseCode = c.String(nullable: false, maxLength: 20),
                        IsFinalRelease = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new {t.ReleaseGroupCode, t.GpReleaseStrategy, t.GpReleaseStrategyLevel})
                .ForeignKey("dbo.GpReleaseStrategies", t => new {t.ReleaseGroupCode, t.GpReleaseStrategy})
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .Index(t => new {t.ReleaseGroupCode, t.GpReleaseStrategy});
        }

        public override void Down()
        {
            DropForeignKey("dbo.GpReleaseStrategyLevels", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.GpReleaseStrategyLevels", new[] {"ReleaseGroupCode", "GpReleaseStrategy"},
                "dbo.GpReleaseStrategies");
            DropForeignKey("dbo.GpReleaseStrategies", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.GpReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.GpReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"},
                "dbo.Stations");
            DropForeignKey("dbo.GpReleaseStrategies", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.GpReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"},
                "dbo.Departments");
            DropForeignKey("dbo.GpReleaseStrategies", "CompCode", "dbo.Companies");
            DropIndex("dbo.GpReleaseStrategyLevels", new[] {"ReleaseGroupCode", "GpReleaseStrategy"});
            DropIndex("dbo.GpReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            DropIndex("dbo.GpReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            DropIndex("dbo.GpReleaseStrategies", new[] {"CompCode"});
            DropIndex("dbo.GpReleaseStrategies", new[] {"ReleaseGroupCode"});
            DropTable("dbo.GpReleaseStrategyLevels");
            DropTable("dbo.GpReleaseStrategies");
        }
    }
}