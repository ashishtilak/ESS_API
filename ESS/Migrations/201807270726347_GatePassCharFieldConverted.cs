namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class GatePassCharFieldConverted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GatePasses", "Mode", c => c.String(maxLength: 1));
            AddColumn("dbo.GatePasses", "GatePassStatus", c => c.String(maxLength: 1));
        }

        public override void Down()
        {
            DropColumn("dbo.GatePasses", "GatePassStatus");
            DropColumn("dbo.GatePasses", "Mode");
        }
    }
}