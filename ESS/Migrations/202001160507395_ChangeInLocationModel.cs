namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeInLocationModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "PaySlipFolder", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Locations", "PaySlipFolder");
        }
    }
}