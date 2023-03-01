namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ShiftScheduleTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Shifts",
                    c => new
                    {
                        ShiftCode = c.String(nullable: false, maxLength: 2),
                        ShiftDesc = c.String(maxLength: 50),
                        ShiftStart = c.Time(precision: 7),
                        ShiftEnd = c.Time(precision: 7),
                    })
                .PrimaryKey(t => t.ShiftCode);

            CreateTable(
                    "dbo.ShiftScheduleDetails",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        ScheduleId = c.Int(nullable: false),
                        ShiftDay = c.Int(nullable: false),
                        ShiftCode = c.String(maxLength: 2),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.ScheduleId, t.ShiftDay})
                .ForeignKey("dbo.Shifts", t => t.ShiftCode)
                .ForeignKey("dbo.ShiftSchedules", t => new {t.YearMonth, t.ScheduleId})
                .Index(t => new {t.YearMonth, t.ScheduleId})
                .Index(t => t.ShiftCode);

            CreateTable(
                    "dbo.ShiftSchedules",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        ScheduleId = c.Int(nullable: false),
                        EmpUnqId = c.String(maxLength: 10),
                        CompCode = c.String(maxLength: 2),
                        WrkGrp = c.String(maxLength: 10),
                        UnitCode = c.String(maxLength: 3),
                        DeptCode = c.String(maxLength: 3),
                        StatCode = c.String(maxLength: 3),
                        ReleaseGroupCode = c.String(maxLength: 2),
                        ReleaseStrategy = c.String(maxLength: 15),
                        ReleaseStatusCode = c.String(maxLength: 1),
                        ReleaseDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        ReleaseUser = c.String(maxLength: 10),
                        AddDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        AddUser = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.ScheduleId})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Departments", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .ForeignKey("dbo.ReleaseStrategies", t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .ForeignKey("dbo.Stations", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode})
                .ForeignKey("dbo.Units", t => new {t.CompCode, t.WrkGrp, t.UnitCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.EmpUnqId)
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode})
                .Index(t => t.ReleaseGroupCode)
                .Index(t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .Index(t => t.ReleaseStatusCode);
        }

        public override void Down()
        {
            DropForeignKey("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId"}, "dbo.ShiftSchedules");
            DropForeignKey("dbo.ShiftSchedules", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.ShiftSchedules", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.ShiftSchedules", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"},
                "dbo.Stations");
            DropForeignKey("dbo.ShiftSchedules", new[] {"ReleaseGroupCode", "ReleaseStrategy"},
                "dbo.ReleaseStrategies");
            DropForeignKey("dbo.ShiftSchedules", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.ShiftSchedules", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.ShiftSchedules", "EmpUnqId", "dbo.Employees");
            DropForeignKey("dbo.ShiftSchedules", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"},
                "dbo.Departments");
            DropForeignKey("dbo.ShiftSchedules", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.ShiftScheduleDetails", "ShiftCode", "dbo.Shifts");
            DropIndex("dbo.ShiftSchedules", new[] {"ReleaseStatusCode"});
            DropIndex("dbo.ShiftSchedules", new[] {"ReleaseGroupCode", "ReleaseStrategy"});
            DropIndex("dbo.ShiftSchedules", new[] {"ReleaseGroupCode"});
            DropIndex("dbo.ShiftSchedules", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            DropIndex("dbo.ShiftSchedules", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            DropIndex("dbo.ShiftSchedules", new[] {"CompCode"});
            DropIndex("dbo.ShiftSchedules", new[] {"EmpUnqId"});
            DropIndex("dbo.ShiftScheduleDetails", new[] {"ShiftCode"});
            DropIndex("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId"});
            DropTable("dbo.ShiftSchedules");
            DropTable("dbo.ShiftScheduleDetails");
            DropTable("dbo.Shifts");
        }
    }
}