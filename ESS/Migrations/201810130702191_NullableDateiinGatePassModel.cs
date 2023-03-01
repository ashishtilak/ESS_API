namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NullableDateiinGatePassModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GatePasses", "AttdUpdate", c => c.DateTime());
        }

        public override void Down()
        {
            AlterColumn("dbo.GatePasses", "AttdUpdate", c => c.DateTime(nullable: false));
        }
    }
}