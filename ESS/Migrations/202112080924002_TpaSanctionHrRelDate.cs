namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TpaSanctionHrRelDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TpaSanctions", "HrReleaseDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TpaSanctions", "HrReleaseDate");
        }
    }
}
