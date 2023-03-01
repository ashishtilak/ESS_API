namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TpaSanction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TpaReleases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReleaseGroupCode = c.String(maxLength: 2),
                        TpaSanctionId = c.Int(nullable: false),
                        ReleaseStrategy = c.String(maxLength: 15),
                        ReleaseStrategyLevel = c.Int(nullable: false),
                        ReleaseCode = c.String(maxLength: 20),
                        PreReleaseStatusCode = c.String(maxLength: 1),
                        PreReleaseDate = c.DateTime(),
                        PreReleaseAuth = c.String(maxLength: 10),
                        IsFinalRelease = c.Boolean(nullable: false),
                        PreRemarks = c.String(maxLength: 255),
                        PostReleaseStatusCode = c.String(maxLength: 1),
                        PostReleaseAuth = c.String(maxLength: 10),
                        PostReleaseDate = c.DateTime(),
                        PostRemarks = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReleaseStatus", t => t.PostReleaseStatusCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.PreReleaseStatusCode)
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .Index(t => t.ReleaseGroupCode)
                .Index(t => t.PreReleaseStatusCode)
                .Index(t => t.PostReleaseStatusCode);
            
            CreateTable(
                "dbo.TpaSanctions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmpUnqId = c.String(maxLength: 10),
                        TpaDate = c.DateTime(nullable: false, storeType: "date"),
                        TpaShiftCode = c.String(maxLength: 2),
                        RequiredHours = c.Single(nullable: false),
                        PreJustification = c.String(maxLength: 255),
                        ReleaseGroupCode = c.String(maxLength: 2),
                        ReleaseStrategy = c.String(maxLength: 15),
                        PreReleaseStatusCode = c.String(maxLength: 1),
                        PreRemarks = c.String(maxLength: 255),
                        AddDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        AddUser = c.String(maxLength: 10),
                        ActShiftCode = c.String(maxLength: 2),
                        WrkHours = c.Single(nullable: false),
                        SanctionTpa = c.Single(nullable: false),
                        PostJustification = c.String(maxLength: 255),
                        PostReleaseStatusCode = c.String(maxLength: 1),
                        PostRemarks = c.String(maxLength: 255),
                        HrReleaseStatusCode = c.String(maxLength: 1),
                        HrPostRemarks = c.String(maxLength: 255),
                        HrUser = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.ReleaseStatus", t => t.HrReleaseStatusCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.PostReleaseStatusCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.PreReleaseStatusCode)
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStrategies", t => new { t.ReleaseGroupCode, t.ReleaseStrategy })
                .Index(t => t.EmpUnqId)
                .Index(t => t.ReleaseGroupCode)
                .Index(t => new { t.ReleaseGroupCode, t.ReleaseStrategy })
                .Index(t => t.PreReleaseStatusCode)
                .Index(t => t.PostReleaseStatusCode)
                .Index(t => t.HrReleaseStatusCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TpaSanctions", new[] { "ReleaseGroupCode", "ReleaseStrategy" }, "dbo.ReleaseStrategies");
            DropForeignKey("dbo.TpaSanctions", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.TpaSanctions", "PreReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.TpaSanctions", "PostReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.TpaSanctions", "HrReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.TpaSanctions", "EmpUnqId", "dbo.Employees");
            DropForeignKey("dbo.TpaReleases", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.TpaReleases", "PreReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.TpaReleases", "PostReleaseStatusCode", "dbo.ReleaseStatus");
            DropIndex("dbo.TpaSanctions", new[] { "HrReleaseStatusCode" });
            DropIndex("dbo.TpaSanctions", new[] { "PostReleaseStatusCode" });
            DropIndex("dbo.TpaSanctions", new[] { "PreReleaseStatusCode" });
            DropIndex("dbo.TpaSanctions", new[] { "ReleaseGroupCode", "ReleaseStrategy" });
            DropIndex("dbo.TpaSanctions", new[] { "ReleaseGroupCode" });
            DropIndex("dbo.TpaSanctions", new[] { "EmpUnqId" });
            DropIndex("dbo.TpaReleases", new[] { "PostReleaseStatusCode" });
            DropIndex("dbo.TpaReleases", new[] { "PreReleaseStatusCode" });
            DropIndex("dbo.TpaReleases", new[] { "ReleaseGroupCode" });
            DropTable("dbo.TpaSanctions");
            DropTable("dbo.TpaReleases");
        }
    }
}
