namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NoDuesDeptDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.NoDuesDepts",
                    c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        DeptId = c.String(nullable: false, maxLength: 3),
                        NoDuesFlag = c.Boolean(nullable: false),
                        Remarks = c.String(maxLength: 20),
                        ApprovalFlag = c.Boolean(),
                        ApprovalDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        ApprovedBy = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => new {t.EmpUnqId, t.DeptId})
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .Index(t => t.EmpUnqId);

            CreateTable(
                    "dbo.NoDuesDeptDetails",
                    c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        DeptId = c.String(nullable: false, maxLength: 3),
                        Sr = c.Int(nullable: false),
                        Particulars = c.String(maxLength: 50),
                        Remarks = c.String(maxLength: 200),
                        Amount = c.Single(),
                        AddUser = c.String(maxLength: 10),
                        AddDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new {t.EmpUnqId, t.DeptId, t.Sr})
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .Index(t => t.EmpUnqId);

            CreateTable(
                    "dbo.NoDuesDeptLists",
                    c => new
                    {
                        DeptId = c.String(nullable: false, maxLength: 3),
                        Index = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DeptId);

            AddColumn("dbo.NoDuesStatus", "UnitHead", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropForeignKey("dbo.NoDuesDeptDetails", "EmpUnqId", "dbo.Employees");
            DropForeignKey("dbo.NoDuesDepts", "EmpUnqId", "dbo.Employees");
            DropIndex("dbo.NoDuesDeptDetails", new[] {"EmpUnqId"});
            DropIndex("dbo.NoDuesDepts", new[] {"EmpUnqId"});
            DropColumn("dbo.NoDuesStatus", "UnitHead");
            DropTable("dbo.NoDuesDeptLists");
            DropTable("dbo.NoDuesDeptDetails");
            DropTable("dbo.NoDuesDepts");
        }
    }
}