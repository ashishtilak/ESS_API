namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialNoDuesConfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.NoDuesCreators",
                    c => new
                    {
                        Dept = c.String(nullable: false, maxLength: 3),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => new {t.Dept, t.EmpUnqId});

            CreateTable(
                    "dbo.NoDuesMasters",
                    c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        JoinDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ResignDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RelieveDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        NoDuesStartDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        AddUser = c.String(maxLength: 10),
                        AddDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ClosedFlag = c.Boolean(nullable: false),
                        DeptParticulars = c.String(maxLength: 50),
                        DeptRemarks = c.String(maxLength: 200),
                        DeptAmount = c.Single(),
                        DeptNoDuesFlag = c.Boolean(nullable: false),
                        DeptApprovalFlag = c.Boolean(nullable: false),
                        DeptAddUser = c.String(maxLength: 10),
                        DeptAddDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        NoticePeriod = c.Int(),
                        NoticePeriodUnit = c.String(maxLength: 3),
                        LastWorkingDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        ModeOfLeaving = c.String(maxLength: 20),
                        ExitInterviewFlag = c.Boolean(),
                        HrAddUser = c.String(maxLength: 10),
                        HrAddDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        HrApprovalFlag = c.Boolean(),
                        HrApprovalDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        HrApprovedBy = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.EmpUnqId)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .Index(t => t.EmpUnqId);

            CreateTable(
                    "dbo.NoDuesReleaseStatus",
                    c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ReleaseGroupCode = c.String(nullable: false, maxLength: 2),
                        ReleaseStrategy = c.String(nullable: false, maxLength: 15),
                        ReleaseStrategyLevel = c.Int(nullable: false),
                        ReleaseCode = c.String(nullable: false, maxLength: 20),
                        ReleaseStatusCode = c.String(nullable: false, maxLength: 1),
                        ReleaseDate = c.DateTime(),
                        ReleaseAuth = c.String(maxLength: 10),
                        IsFinalRelease = c.Boolean(nullable: false),
                        Remarks = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => new {t.EmpUnqId, t.ReleaseGroupCode, t.ReleaseStrategy, t.ReleaseStrategyLevel})
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .ForeignKey("dbo.NoDuesMasters", t => t.EmpUnqId)
                .Index(t => t.EmpUnqId)
                .Index(t => t.ReleaseGroupCode)
                .Index(t => t.ReleaseStatusCode);

            CreateTable(
                    "dbo.NoDuesReleasers",
                    c => new
                    {
                        Dept = c.String(nullable: false, maxLength: 3),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => new {t.Dept, t.EmpUnqId});

            CreateTable(
                    "dbo.NoDuesStatus",
                    c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        Hod = c.Boolean(nullable: false),
                        Finance = c.Boolean(nullable: false),
                        Stores = c.Boolean(nullable: false),
                        Admin = c.Boolean(nullable: false),
                        Cafeteria = c.Boolean(nullable: false),
                        Hr = c.Boolean(nullable: false),
                        PrgHr = c.Boolean(nullable: false),
                        Township = c.Boolean(nullable: false),
                        EandI = c.Boolean(nullable: false),
                        It = c.Boolean(nullable: false),
                        Security = c.Boolean(nullable: false),
                        Safety = c.Boolean(nullable: false),
                        Ohc = c.Boolean(nullable: false),
                        School = c.Boolean(nullable: false),
                        Er = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EmpUnqId);

            AddColumn("dbo.Employees", "JoinDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }

        public override void Down()
        {
            DropForeignKey("dbo.NoDuesReleaseStatus", "EmpUnqId", "dbo.NoDuesMasters");
            DropForeignKey("dbo.NoDuesReleaseStatus", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.NoDuesReleaseStatus", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.NoDuesMasters", "EmpUnqId", "dbo.Employees");
            DropIndex("dbo.NoDuesReleaseStatus", new[] {"ReleaseStatusCode"});
            DropIndex("dbo.NoDuesReleaseStatus", new[] {"ReleaseGroupCode"});
            DropIndex("dbo.NoDuesReleaseStatus", new[] {"EmpUnqId"});
            DropIndex("dbo.NoDuesMasters", new[] {"EmpUnqId"});
            DropColumn("dbo.Employees", "JoinDate");
            DropTable("dbo.NoDuesStatus");
            DropTable("dbo.NoDuesReleasers");
            DropTable("dbo.NoDuesReleaseStatus");
            DropTable("dbo.NoDuesMasters");
            DropTable("dbo.NoDuesCreators");
        }
    }
}