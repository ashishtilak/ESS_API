namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeInLeaveRules : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveRules", "Location", c => c.String(maxLength: 5));
            AddColumn("dbo.LeaveRules", "LeaveTypeCode", c => c.String(maxLength: 2));
            AddColumn("dbo.LeaveRules", "LeaveAllowed", c => c.Boolean(nullable: false));
            AddColumn("dbo.LeaveRules", "DaysAllowed", c => c.Single(nullable: false));
            AddColumn("dbo.LeaveRules", "Active", c => c.Boolean(nullable: false));
            AlterColumn("dbo.LeaveRules", "LeaveRule", c => c.String(maxLength: 50));
            DropColumn("dbo.LeaveRules", "AllowedCl");
        }

        public override void Down()
        {
            AddColumn("dbo.LeaveRules", "AllowedCl", c => c.Single(nullable: false));
            AlterColumn("dbo.LeaveRules", "LeaveRule", c => c.String());
            DropColumn("dbo.LeaveRules", "Active");
            DropColumn("dbo.LeaveRules", "DaysAllowed");
            DropColumn("dbo.LeaveRules", "LeaveAllowed");
            DropColumn("dbo.LeaveRules", "LeaveTypeCode");
            DropColumn("dbo.LeaveRules", "Location");
        }
    }
}