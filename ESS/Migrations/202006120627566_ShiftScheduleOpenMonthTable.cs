namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ShiftScheduleOpenMonthTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.SsOpenMonths",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        PostingEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.YearMonth);
        }

        public override void Down()
        {
            DropTable("dbo.SsOpenMonths");
        }
    }
}