namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddGaReleaseStrategy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.GaReleaseStrategies",
                    c => new
                    {
                        ReleaseGroupCode = c.String(nullable: false, maxLength: 2),
                        GaReleaseStrategy = c.String(nullable: false, maxLength: 15),
                        Active = c.Boolean(nullable: false),
                        CompCode = c.String(maxLength: 2),
                        DeptCode = c.String(nullable: false, maxLength: 3),
                        GaReleaseStrategyName = c.String(maxLength: 100),
                        StatCode = c.String(nullable: false, maxLength: 3),
                        UnitCode = c.String(maxLength: 3),
                        UpdDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        UpdUser = c.String(maxLength: 10),
                        WrkGrp = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => new {t.ReleaseGroupCode, t.GaReleaseStrategy})
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
                    "dbo.GaReleaseStrategyLevels",
                    c => new
                    {
                        ReleaseGroupCode = c.String(nullable: false, maxLength: 2),
                        GaReleaseStrategy = c.String(nullable: false, maxLength: 15),
                        GaReleaseStrategyLevel = c.Int(nullable: false),
                        ReleaseCode = c.String(nullable: false, maxLength: 20),
                        IsFinalRelease = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new {t.ReleaseGroupCode, t.GaReleaseStrategy, t.GaReleaseStrategyLevel})
                .ForeignKey("dbo.GaReleaseStrategies", t => new {t.ReleaseGroupCode, t.GaReleaseStrategy})
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .Index(t => new {t.ReleaseGroupCode, t.GaReleaseStrategy});
        }

        public override void Down()
        {
            DropForeignKey("dbo.GaReleaseStrategyLevels", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.GaReleaseStrategyLevels", new[] {"ReleaseGroupCode", "GaReleaseStrategy"},
                "dbo.GaReleaseStrategies");
            DropForeignKey("dbo.GaReleaseStrategies", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.GaReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.GaReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"},
                "dbo.Stations");
            DropForeignKey("dbo.GaReleaseStrategies", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.GaReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"},
                "dbo.Departments");
            DropForeignKey("dbo.GaReleaseStrategies", "CompCode", "dbo.Companies");
            DropIndex("dbo.GaReleaseStrategyLevels", new[] {"ReleaseGroupCode", "GaReleaseStrategy"});
            DropIndex("dbo.GaReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            DropIndex("dbo.GaReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            DropIndex("dbo.GaReleaseStrategies", new[] {"CompCode"});
            DropIndex("dbo.GaReleaseStrategies", new[] {"ReleaseGroupCode"});
            DropTable("dbo.GaReleaseStrategyLevels");
            DropTable("dbo.GaReleaseStrategies");
        }
    }
}