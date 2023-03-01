namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddGatePassAdvice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.GpAdviceDetails",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        GpAdviceNo = c.Int(nullable: false),
                        GpAdviceItem = c.Int(nullable: false),
                        MaterialCode = c.String(maxLength: 20),
                        MaterialDesc = c.String(maxLength: 100),
                        MaterialQty = c.Single(nullable: false),
                        ApproxValue = c.Single(nullable: false),
                        HsnCode = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.GpAdviceNo, t.GpAdviceItem})
                .ForeignKey("dbo.GpAdvices", t => new {t.YearMonth, t.GpAdviceNo})
                .Index(t => new {t.YearMonth, t.GpAdviceNo});

            CreateTable(
                    "dbo.GpAdvices",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        GpAdviceNo = c.Int(nullable: false),
                        GpAdviceDate = c.DateTime(nullable: false),
                        EmpUnqId = c.String(maxLength: 10),
                        UnitCode = c.String(maxLength: 3),
                        DeptCode = c.String(maxLength: 3),
                        StatCode = c.String(maxLength: 3),
                        GpAdviceType = c.String(maxLength: 1),
                        Purpose = c.String(maxLength: 255),
                        WorkOrderNo = c.String(maxLength: 20),
                        VendorCode = c.String(maxLength: 10),
                        VendorName = c.String(maxLength: 100),
                        VendorAddress1 = c.String(maxLength: 255),
                        VendorAddress2 = c.String(maxLength: 255),
                        VendorAddress3 = c.String(maxLength: 255),
                        ApproxDateOfReturn = c.DateTime(nullable: false),
                        ModeOfTransport = c.String(maxLength: 20),
                        TransporterName = c.String(maxLength: 100),
                        GpAdviceStatus = c.String(maxLength: 1),
                        AddDt = c.DateTime(nullable: false),
                        AddUser = c.String(maxLength: 10),
                        UpdDt = c.DateTime(),
                        UpdUser = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.GpAdviceNo});
        }

        public override void Down()
        {
            DropForeignKey("dbo.GpAdviceDetails", new[] {"YearMonth", "GpAdviceNo"}, "dbo.GpAdvices");
            DropIndex("dbo.GpAdviceDetails", new[] {"YearMonth", "GpAdviceNo"});
            DropTable("dbo.GpAdvices");
            DropTable("dbo.GpAdviceDetails");
        }
    }
}