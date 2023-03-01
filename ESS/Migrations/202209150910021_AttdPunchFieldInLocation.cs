namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttdPunchFieldInLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "AttdPunchLimitDays", c => c.Int(nullable: false));
            AddColumn("dbo.Locations", "AttdPunchMonthLimit", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Locations", "AttdPunchMonthLimit");
            DropColumn("dbo.Locations", "AttdPunchLimitDays");
        }
    }
}
