namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedCancellationInLeaveAppDtl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveApplicationDetails", "Cancelled", c => c.Boolean());
            AddColumn("dbo.LeaveApplicationDetails", "ParentId", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.LeaveApplicationDetails", "ParentId");
            DropColumn("dbo.LeaveApplicationDetails", "Cancelled");
        }
    }
}