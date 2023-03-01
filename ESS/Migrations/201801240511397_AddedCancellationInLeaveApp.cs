namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedCancellationInLeaveApp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveApplications", "Cancelled", c => c.Boolean());
            AddColumn("dbo.LeaveApplications", "ParentId", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.LeaveApplications", "ParentId");
            DropColumn("dbo.LeaveApplications", "Cancelled");
        }
    }
}