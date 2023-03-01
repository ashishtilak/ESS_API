namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddYearMonthToGatePass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GatePasses", "YearMonth", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.GatePasses", "YearMonth");
        }
    }
}