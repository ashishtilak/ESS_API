namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddReimbAndReimbConvTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.ReimbConvs",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        ReimbId = c.Int(nullable: false),
                        Sr = c.Int(nullable: false),
                        ReimbType = c.String(maxLength: 3),
                        EmpUnqId = c.String(maxLength: 10),
                        ConvDate = c.DateTime(nullable: false),
                        VehicleNo = c.String(maxLength: 15),
                        Particulars = c.String(maxLength: 100),
                        MeterFrom = c.Int(nullable: false),
                        Distance = c.Int(nullable: false),
                        MeterTo = c.Int(nullable: false),
                        Rate = c.Single(nullable: false),
                        Amount = c.Single(nullable: false),
                        Remarks = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.ReimbId, t.Sr})
                .ForeignKey("dbo.Reimbursements", t => new {t.YearMonth, t.ReimbId})
                .Index(t => new {t.YearMonth, t.ReimbId});

            CreateTable(
                    "dbo.Reimbursements",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        ReimbId = c.Int(nullable: false),
                        ReimbType = c.String(maxLength: 3),
                        EmpUnqId = c.String(maxLength: 10),
                        ReimbDate = c.DateTime(nullable: false),
                        PeriodFrom = c.Int(nullable: false),
                        PeriodTo = c.Int(nullable: false),
                        InvoiceNo = c.String(maxLength: 20),
                        AmountClaimed = c.Single(nullable: false),
                        AmountReleased = c.Single(nullable: false),
                        AmountReleaseRemarks = c.String(maxLength: 50),
                        AddUser = c.String(maxLength: 10),
                        AddDateTime = c.DateTime(nullable: false),
                        ReleaseGroupCode = c.String(maxLength: 2),
                        ReleaseStrategy = c.String(maxLength: 15),
                        ReleaseStatusCode = c.String(maxLength: 1),
                        Remarks = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.ReimbId})
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .ForeignKey("dbo.ReleaseStrategies", t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .Index(t => t.ReleaseGroupCode)
                .Index(t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .Index(t => t.ReleaseStatusCode);

            AddColumn("dbo.ApplReleaseStatus", "Reimbursements_YearMonth", c => c.Int());
            AddColumn("dbo.ApplReleaseStatus", "Reimbursements_ReimbId", c => c.Int());
            CreateIndex("dbo.ApplReleaseStatus", new[] {"Reimbursements_YearMonth", "Reimbursements_ReimbId"});
            AddForeignKey("dbo.ApplReleaseStatus", new[] {"Reimbursements_YearMonth", "Reimbursements_ReimbId"},
                "dbo.Reimbursements", new[] {"YearMonth", "ReimbId"});
        }

        public override void Down()
        {
            DropForeignKey("dbo.Reimbursements", new[] {"ReleaseGroupCode", "ReleaseStrategy"},
                "dbo.ReleaseStrategies");
            DropForeignKey("dbo.Reimbursements", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.Reimbursements", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.ReimbConvs", new[] {"YearMonth", "ReimbId"}, "dbo.Reimbursements");
            DropForeignKey("dbo.ApplReleaseStatus", new[] {"Reimbursements_YearMonth", "Reimbursements_ReimbId"},
                "dbo.Reimbursements");
            DropIndex("dbo.Reimbursements", new[] {"ReleaseStatusCode"});
            DropIndex("dbo.Reimbursements", new[] {"ReleaseGroupCode", "ReleaseStrategy"});
            DropIndex("dbo.Reimbursements", new[] {"ReleaseGroupCode"});
            DropIndex("dbo.ReimbConvs", new[] {"YearMonth", "ReimbId"});
            DropIndex("dbo.ApplReleaseStatus", new[] {"Reimbursements_YearMonth", "Reimbursements_ReimbId"});
            DropColumn("dbo.ApplReleaseStatus", "Reimbursements_ReimbId");
            DropColumn("dbo.ApplReleaseStatus", "Reimbursements_YearMonth");
            DropTable("dbo.Reimbursements");
            DropTable("dbo.ReimbConvs");
        }
    }
}