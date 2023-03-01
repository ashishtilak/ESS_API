namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class EmpSeparation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.EmpSeparations",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmpUnqId = c.String(maxLength: 10),
                        ApplicationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Mode = c.String(maxLength: 1),
                        Reason = c.String(maxLength: 50),
                        ReasonOther = c.String(maxLength: 50),
                        RelieveDate = c.DateTime(nullable: false, storeType: "date"),
                        ResignText = c.String(maxLength: 1500),
                        ReleaseStatusCode = c.String(maxLength: 1),
                        StatusHr = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .Index(t => t.EmpUnqId)
                .Index(t => t.ReleaseStatusCode);
        }

        public override void Down()
        {
            DropForeignKey("dbo.EmpSeparations", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.EmpSeparations", "EmpUnqId", "dbo.Employees");
            DropIndex("dbo.EmpSeparations", new[] {"ReleaseStatusCode"});
            DropIndex("dbo.EmpSeparations", new[] {"EmpUnqId"});
            DropTable("dbo.EmpSeparations");
        }
    }
}