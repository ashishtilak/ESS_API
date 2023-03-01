namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttdPunchFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MissedPunches", "PunchType", c => c.String(maxLength: 1));
            AddColumn("dbo.MissedPunches", "InOutFlag", c => c.String(maxLength: 1));
            AddColumn("dbo.MissedPunches", "AttdPunchTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MissedPunches", "AttdPunchTime");
            DropColumn("dbo.MissedPunches", "InOutFlag");
            DropColumn("dbo.MissedPunches", "PunchType");
        }
    }
}
