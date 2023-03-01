namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class MedicalFitness : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.MedicalFitnesses",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TestDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        EmpUnqId = c.String(maxLength: 10),
                        CardBlockedOn = c.DateTime(precision: 7, storeType: "datetime2"),
                        CardBlockedDays = c.Int(nullable: false),
                        CardBlockedReason = c.String(maxLength: 50),
                        FitnessFlag = c.Boolean(nullable: false),
                        Remarks = c.String(maxLength: 50),
                        AddDt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        AddUser = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .Index(t => t.EmpUnqId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.MedicalFitnesses", "EmpUnqId", "dbo.Employees");
            DropIndex("dbo.MedicalFitnesses", new[] {"EmpUnqId"});
            DropTable("dbo.MedicalFitnesses");
        }
    }
}