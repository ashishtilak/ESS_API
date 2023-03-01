namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleReq : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VehicleReqs",
                c => new
                    {
                        ReqId = c.Int(nullable: false, identity: true),
                        EmpUnqId = c.String(maxLength: 10),
                        ReqDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        BookingDate = c.DateTime(nullable: false, storeType: "date"),
                        BookingSlot = c.Int(nullable: false),
                        BookingStatus = c.Boolean(nullable: false),
                        PickupTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        PickupLocation = c.String(maxLength: 50),
                        DropLocation = c.String(maxLength: 50),
                        NumberOfPass = c.Int(nullable: false),
                        Remarks = c.String(maxLength: 50),
                        AddDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        AddUser = c.String(maxLength: 10),
                        ReleaseGroupCode = c.String(maxLength: 2),
                        ReleaseStrategy = c.String(maxLength: 15),
                        ReleaseCode = c.String(maxLength: 20),
                        ReleaseStatusCode = c.String(maxLength: 1),
                        ReleaseRemarks = c.String(maxLength: 255),
                        ReleaseAuth = c.String(maxLength: 10),
                        ReleaseDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        AdminReleaseStatusCode = c.String(maxLength: 1),
                        AdminUser = c.String(maxLength: 10),
                        AdminReleaseDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        AdminReleaseRemarks = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.ReqId)
                .ForeignKey("dbo.ReleaseStatus", t => t.AdminReleaseStatusCode)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .ForeignKey("dbo.ReleaseStrategies", t => new { t.ReleaseGroupCode, t.ReleaseStrategy })
                .Index(t => t.EmpUnqId)
                .Index(t => t.ReleaseGroupCode)
                .Index(t => new { t.ReleaseGroupCode, t.ReleaseStrategy })
                .Index(t => t.ReleaseStatusCode)
                .Index(t => t.AdminReleaseStatusCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VehicleReqs", new[] { "ReleaseGroupCode", "ReleaseStrategy" }, "dbo.ReleaseStrategies");
            DropForeignKey("dbo.VehicleReqs", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.VehicleReqs", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.VehicleReqs", "EmpUnqId", "dbo.Employees");
            DropForeignKey("dbo.VehicleReqs", "AdminReleaseStatusCode", "dbo.ReleaseStatus");
            DropIndex("dbo.VehicleReqs", new[] { "AdminReleaseStatusCode" });
            DropIndex("dbo.VehicleReqs", new[] { "ReleaseStatusCode" });
            DropIndex("dbo.VehicleReqs", new[] { "ReleaseGroupCode", "ReleaseStrategy" });
            DropIndex("dbo.VehicleReqs", new[] { "ReleaseGroupCode" });
            DropIndex("dbo.VehicleReqs", new[] { "EmpUnqId" });
            DropTable("dbo.VehicleReqs");
        }
    }
}
