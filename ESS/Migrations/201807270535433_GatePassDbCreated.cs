namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class GatePassDbCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.GatePasses",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GatePassDate = c.DateTime(nullable: false),
                        GatePassNo = c.Int(nullable: false),
                        GatePassItem = c.Int(nullable: false),
                        EmpUnqId = c.String(),
                        PlaceOfVisit = c.String(),
                        Reason = c.String(),
                        AddUser = c.String(),
                        AddDateTime = c.DateTime(nullable: false),
                        GateOutDateTime = c.DateTime(),
                        GateOutUser = c.String(),
                        GateOutIp = c.String(),
                        GateInDateTime = c.DateTime(),
                        GateInUser = c.String(),
                        GateInIp = c.String(),
                    })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.GatePasses");
        }
    }
}