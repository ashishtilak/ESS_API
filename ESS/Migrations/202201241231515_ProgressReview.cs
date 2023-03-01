namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProgressReview : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GradeReviews",
                c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        GradeCode = c.String(nullable: false, maxLength: 3),
                        ReviewQtr = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CompCode, t.WrkGrp, t.GradeCode })
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Grades", t => new { t.CompCode, t.WrkGrp, t.GradeCode })
                .ForeignKey("dbo.WorkGroups", t => new { t.CompCode, t.WrkGrp })
                .Index(t => t.CompCode)
                .Index(t => new { t.CompCode, t.WrkGrp, t.GradeCode });
            
            CreateTable(
                "dbo.ReviewDetails",
                c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ReviewQtrNo = c.Int(nullable: false),
                        IsConfirmation = c.Boolean(nullable: false),
                        ReviewDate = c.DateTime(nullable: false, storeType: "date"),
                        PeriodFrom = c.DateTime(nullable: false, storeType: "date"),
                        PeriodTo = c.DateTime(nullable: false, storeType: "date"),
                        Assignments = c.String(maxLength: 255),
                        Strength = c.String(maxLength: 255),
                        Improvements = c.String(maxLength: 255),
                        Suggestions = c.String(maxLength: 255),
                        Rating = c.String(maxLength: 1),
                        Remarks = c.String(maxLength: 100),
                        Recommendation = c.String(maxLength: 1),
                        AddDt = c.DateTime(),
                        AddReleaseCode = c.String(maxLength: 20),
                        AddUser = c.String(maxLength: 10),
                        AddReleaseStatusCode = c.String(maxLength: 1),
                        ReleaseGroupCode = c.String(maxLength: 2),
                        ReleaseStrategy = c.String(maxLength: 15),
                        ReleaseCode = c.String(maxLength: 20),
                        ReleaseDate = c.DateTime(),
                        ReleaseStatusCode = c.String(maxLength: 1),
                        HodRemarks = c.String(maxLength: 100),
                        HrUser = c.String(maxLength: 10),
                        HrReleaseDate = c.DateTime(),
                        HrReleaseStatusCode = c.String(maxLength: 1),
                        HrRemarks = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => new { t.EmpUnqId, t.ReviewQtrNo })
                .ForeignKey("dbo.ReleaseStatus", t => t.AddReleaseStatusCode)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.ReleaseStatus", t => t.HrReleaseStatusCode)
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .Index(t => t.EmpUnqId)
                .Index(t => t.AddReleaseStatusCode)
                .Index(t => t.ReleaseGroupCode)
                .Index(t => t.ReleaseStatusCode)
                .Index(t => t.HrReleaseStatusCode);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ReviewQtr = c.Int(nullable: false),
                        JoinDt = c.DateTime(nullable: false),
                        ConfirmationStatus = c.String(maxLength: 1),
                        AddDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        AddUser = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.EmpUnqId)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .Index(t => t.EmpUnqId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "EmpUnqId", "dbo.Employees");
            DropForeignKey("dbo.ReviewDetails", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.ReviewDetails", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.ReviewDetails", "HrReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.ReviewDetails", "EmpUnqId", "dbo.Employees");
            DropForeignKey("dbo.ReviewDetails", "AddReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.GradeReviews", new[] { "CompCode", "WrkGrp" }, "dbo.WorkGroups");
            DropForeignKey("dbo.GradeReviews", new[] { "CompCode", "WrkGrp", "GradeCode" }, "dbo.Grades");
            DropForeignKey("dbo.GradeReviews", "CompCode", "dbo.Companies");
            DropIndex("dbo.Reviews", new[] { "EmpUnqId" });
            DropIndex("dbo.ReviewDetails", new[] { "HrReleaseStatusCode" });
            DropIndex("dbo.ReviewDetails", new[] { "ReleaseStatusCode" });
            DropIndex("dbo.ReviewDetails", new[] { "ReleaseGroupCode" });
            DropIndex("dbo.ReviewDetails", new[] { "AddReleaseStatusCode" });
            DropIndex("dbo.ReviewDetails", new[] { "EmpUnqId" });
            DropIndex("dbo.GradeReviews", new[] { "CompCode", "WrkGrp", "GradeCode" });
            DropIndex("dbo.GradeReviews", new[] { "CompCode" });
            DropTable("dbo.Reviews");
            DropTable("dbo.ReviewDetails");
            DropTable("dbo.GradeReviews");
        }
    }
}
