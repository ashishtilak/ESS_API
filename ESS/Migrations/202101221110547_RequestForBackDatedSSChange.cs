namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RequestForBackDatedSSChange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.RequestDetails",
                    c => new
                    {
                        RequestId = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        Sr = c.Int(nullable: false),
                        FromDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        ToDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        ShiftCode = c.String(maxLength: 2),
                        Reason = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => new {t.RequestId, t.EmpUnqId, t.Sr})
                .ForeignKey("dbo.Shifts", t => t.ShiftCode)
                .ForeignKey("dbo.Requests", t => new {t.RequestId, t.EmpUnqId})
                .Index(t => new {t.RequestId, t.EmpUnqId})
                .Index(t => t.ShiftCode);

            CreateTable(
                    "dbo.RequestReleases",
                    c => new
                    {
                        RequestId = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ReleaseStrategy = c.String(nullable: false, maxLength: 15),
                        ReleaseStrategyLevel = c.Int(nullable: false),
                        ReleaseGroupCode = c.String(nullable: false, maxLength: 2),
                        ReleaseCode = c.String(nullable: false, maxLength: 20),
                        ReleaseStatusCode = c.String(nullable: false, maxLength: 1),
                        ReleaseDate = c.DateTime(),
                        ReleaseAuth = c.String(maxLength: 10),
                        IsFinalRelease = c.Boolean(nullable: false),
                        Remarks = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => new {t.RequestId, t.EmpUnqId, t.ReleaseStrategy, t.ReleaseStrategyLevel})
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .ForeignKey("dbo.Requests", t => new {t.RequestId, t.EmpUnqId})
                .Index(t => new {t.RequestId, t.EmpUnqId})
                .Index(t => t.EmpUnqId)
                .Index(t => t.ReleaseGroupCode)
                .Index(t => t.ReleaseStatusCode);

            CreateTable(
                    "dbo.Requests",
                    c => new
                    {
                        RequestId = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        RequestDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        Remarks = c.String(maxLength: 255),
                        ReleaseGroupCode = c.String(maxLength: 2),
                        ReleaseStrategy = c.String(maxLength: 15),
                        ReleaseStatusCode = c.String(maxLength: 1),
                        AddDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        AddUser = c.String(maxLength: 10),
                        IsPosted = c.String(maxLength: 1),
                        PostUser = c.String(maxLength: 10),
                        PostedDt = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new {t.RequestId, t.EmpUnqId})
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .ForeignKey("dbo.ReleaseStrategies", t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .Index(t => t.EmpUnqId)
                .Index(t => t.ReleaseGroupCode)
                .Index(t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .Index(t => t.ReleaseStatusCode);
        }

        public override void Down()
        {
            DropForeignKey("dbo.RequestReleases", new[] {"RequestId", "EmpUnqId"}, "dbo.Requests");
            DropForeignKey("dbo.RequestDetails", new[] {"RequestId", "EmpUnqId"}, "dbo.Requests");
            DropForeignKey("dbo.Requests", new[] {"ReleaseGroupCode", "ReleaseStrategy"}, "dbo.ReleaseStrategies");
            DropForeignKey("dbo.Requests", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.Requests", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.Requests", "EmpUnqId", "dbo.Employees");
            DropForeignKey("dbo.RequestReleases", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.RequestReleases", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.RequestReleases", "EmpUnqId", "dbo.Employees");
            DropForeignKey("dbo.RequestDetails", "ShiftCode", "dbo.Shifts");
            DropIndex("dbo.Requests", new[] {"ReleaseStatusCode"});
            DropIndex("dbo.Requests", new[] {"ReleaseGroupCode", "ReleaseStrategy"});
            DropIndex("dbo.Requests", new[] {"ReleaseGroupCode"});
            DropIndex("dbo.Requests", new[] {"EmpUnqId"});
            DropIndex("dbo.RequestReleases", new[] {"ReleaseStatusCode"});
            DropIndex("dbo.RequestReleases", new[] {"ReleaseGroupCode"});
            DropIndex("dbo.RequestReleases", new[] {"EmpUnqId"});
            DropIndex("dbo.RequestReleases", new[] {"RequestId", "EmpUnqId"});
            DropIndex("dbo.RequestDetails", new[] {"ShiftCode"});
            DropIndex("dbo.RequestDetails", new[] {"RequestId", "EmpUnqId"});
            DropTable("dbo.Requests");
            DropTable("dbo.RequestReleases");
            DropTable("dbo.RequestDetails");
        }
    }
}