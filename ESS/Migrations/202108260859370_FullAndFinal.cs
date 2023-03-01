namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FullAndFinal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FullAndFinals",
                c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        RelieveDate = c.DateTime(),
                        DocumentNo = c.String(maxLength: 50),
                        RecoveryAmount = c.Single(),
                        CashDeposited = c.String(maxLength: 50),
                        DepositDate = c.DateTime(storeType: "date"),
                        Remarks = c.String(maxLength: 255),
                        GratuityFlag = c.Boolean(nullable: false),
                        AddUser = c.String(maxLength: 10),
                        AddDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.EmpUnqId)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .Index(t => t.EmpUnqId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FullAndFinals", "EmpUnqId", "dbo.Employees");
            DropIndex("dbo.FullAndFinals", new[] { "EmpUnqId" });
            DropTable("dbo.FullAndFinals");
        }
    }
}
