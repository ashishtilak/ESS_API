namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NoDuesUnitHeadRelease : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.NoDuesUnitHeads",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        UnitCode = c.String(nullable: false, maxLength: 3),
                        DeptCode = c.String(nullable: false, maxLength: 3),
                        DeptLoB = c.String(maxLength: 2),
                        UnitHead = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Departments", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .ForeignKey("dbo.Units", t => new {t.CompCode, t.WrkGrp, t.UnitCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode});

            AddColumn("dbo.NoDuesMasters", "UnitHead", c => c.String(maxLength: 10));
            AddColumn("dbo.NoDuesMasters", "UhApprovalFlag", c => c.Boolean());
            AddColumn("dbo.NoDuesMasters", "UhApprovalDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.NoDuesMasters", "UhApprovedBy", c => c.String(maxLength: 10));
        }

        public override void Down()
        {
            DropForeignKey("dbo.NoDuesUnitHeads", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.NoDuesUnitHeads", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.NoDuesUnitHeads", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"},
                "dbo.Departments");
            DropForeignKey("dbo.NoDuesUnitHeads", "CompCode", "dbo.Companies");
            DropIndex("dbo.NoDuesUnitHeads", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            DropIndex("dbo.NoDuesUnitHeads", new[] {"CompCode"});
            DropColumn("dbo.NoDuesMasters", "UhApprovedBy");
            DropColumn("dbo.NoDuesMasters", "UhApprovalDate");
            DropColumn("dbo.NoDuesMasters", "UhApprovalFlag");
            DropColumn("dbo.NoDuesMasters", "UnitHead");
            DropTable("dbo.NoDuesUnitHeads");
        }
    }
}