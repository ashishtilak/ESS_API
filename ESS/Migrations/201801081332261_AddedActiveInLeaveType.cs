namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedActiveInLeaveType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveTypes", "Active", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.LeaveTypes", "Active");
        }
    }
}