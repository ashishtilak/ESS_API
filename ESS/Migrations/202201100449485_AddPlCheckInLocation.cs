namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlCheckInLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "PlCheck", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Locations", "PlCheck");
        }
    }
}
