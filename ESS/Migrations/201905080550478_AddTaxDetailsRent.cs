namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddTaxDetailsRent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TaxDetailsRents",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        ActualFlag = c.Boolean(nullable: false),
                        EmpUnqIdYear = c.String(nullable: false, maxLength: 15),
                        April = c.Int(nullable: false),
                        May = c.Int(nullable: false),
                        June = c.Int(nullable: false),
                        July = c.Int(nullable: false),
                        August = c.Int(nullable: false),
                        September = c.Int(nullable: false),
                        October = c.Int(nullable: false),
                        November = c.Int(nullable: false),
                        December = c.Int(nullable: false),
                        January = c.Int(nullable: false),
                        February = c.Int(nullable: false),
                        March = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag, t.EmpUnqIdYear})
                .ForeignKey("dbo.TaxDeclarations", t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag})
                .Index(t => new {t.YearMonth, t.EmpUnqId, t.ActualFlag});

            AddColumn("dbo.TaxDeclarations", "TotalRentPaid", c => c.Single(nullable: false));
            DropColumn("dbo.TaxDeclarations", "RentPaidPerMonth");
        }

        public override void Down()
        {
            AddColumn("dbo.TaxDeclarations", "RentPaidPerMonth", c => c.Single(nullable: false));
            DropForeignKey("dbo.TaxDetailsRents", new[] {"YearMonth", "EmpUnqId", "ActualFlag"}, "dbo.TaxDeclarations");
            DropIndex("dbo.TaxDetailsRents", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
            DropColumn("dbo.TaxDeclarations", "TotalRentPaid");
            DropTable("dbo.TaxDetailsRents");
        }
    }
}