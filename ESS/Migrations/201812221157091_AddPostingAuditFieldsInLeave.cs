namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddPostingAuditFieldsInLeave : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveApplicationDetails", "PostUser", c => c.String());
            AddColumn("dbo.LeaveApplicationDetails", "PostedDt", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.LeaveApplications", "ClientIp", c => c.String(maxLength: 15));
        }

        public override void Down()
        {
            DropColumn("dbo.LeaveApplications", "ClientIp");
            DropColumn("dbo.LeaveApplicationDetails", "PostedDt");
            DropColumn("dbo.LeaveApplicationDetails", "PostUser");
        }
    }
}