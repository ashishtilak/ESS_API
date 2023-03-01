namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressProof : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddressProofs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmpUnqId = c.String(maxLength: 10),
                        ApplicationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Purpose = c.String(maxLength: 50),
                        AddDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Proof = c.String(maxLength: 50),
                        HrReleaseStatusCode = c.String(maxLength: 1),
                        HrRemarks = c.String(maxLength: 255),
                        HrUser = c.String(maxLength: 10),
                        HrReleaseDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.ReleaseStatus", t => t.HrReleaseStatusCode)
                .Index(t => t.EmpUnqId)
                .Index(t => t.HrReleaseStatusCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AddressProofs", "HrReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.AddressProofs", "EmpUnqId", "dbo.Employees");
            DropIndex("dbo.AddressProofs", new[] { "HrReleaseStatusCode" });
            DropIndex("dbo.AddressProofs", new[] { "EmpUnqId" });
            DropTable("dbo.AddressProofs");
        }
    }
}
