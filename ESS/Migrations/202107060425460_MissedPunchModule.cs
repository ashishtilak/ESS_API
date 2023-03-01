namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MissedPunchModule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MissedPunches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddDate = c.DateTime(nullable: false),
                        EmpUnqId = c.String(maxLength: 10),
                        Reason = c.String(maxLength: 50),
                        Remarks = c.String(maxLength: 255),
                        ReleaseStrategy = c.String(maxLength: 15),
                        ReleaseStatusCode = c.String(maxLength: 1),
                        InTime = c.DateTime(),
                        InTimeUser = c.String(maxLength: 10),
                        OutTime = c.DateTime(),
                        OutTimeUser = c.String(maxLength: 10),
                        PostingFlag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .Index(t => t.EmpUnqId)
                .Index(t => t.ReleaseStatusCode);
            
            CreateTable(
                "dbo.MissedPunchReleaseStatus",
                c => new
                    {
                        ApplicationId = c.Int(nullable: false),
                        ReleaseStrategy = c.String(nullable: false, maxLength: 15),
                        ReleaseStrategyLevel = c.Int(nullable: false),
                        ReleaseCode = c.String(nullable: false, maxLength: 20),
                        ReleaseStatusCode = c.String(nullable: false, maxLength: 1),
                        ReleaseDate = c.DateTime(),
                        ReleaseAuth = c.String(maxLength: 10),
                        IsFinalRelease = c.Boolean(nullable: false),
                        Remarks = c.String(maxLength: 255),
                        MissedPunch_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.ApplicationId, t.ReleaseStrategy, t.ReleaseStrategyLevel })
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .ForeignKey("dbo.MissedPunches", t => t.MissedPunch_Id)
                .Index(t => t.ReleaseStatusCode)
                .Index(t => t.MissedPunch_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MissedPunches", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.MissedPunchReleaseStatus", "MissedPunch_Id", "dbo.MissedPunches");
            DropForeignKey("dbo.MissedPunchReleaseStatus", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.MissedPunches", "EmpUnqId", "dbo.Employees");
            DropIndex("dbo.MissedPunchReleaseStatus", new[] { "MissedPunch_Id" });
            DropIndex("dbo.MissedPunchReleaseStatus", new[] { "ReleaseStatusCode" });
            DropIndex("dbo.MissedPunches", new[] { "ReleaseStatusCode" });
            DropIndex("dbo.MissedPunches", new[] { "EmpUnqId" });
            DropTable("dbo.MissedPunchReleaseStatus");
            DropTable("dbo.MissedPunches");
        }
    }
}
