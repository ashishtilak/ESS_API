namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class LeaveAppDetailFieldChanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LeaveApplicationDetails", "IsPosted", c => c.String(maxLength: 1));
            AlterColumn("dbo.LeaveApplicationDetails", "Remarks", c => c.String(maxLength: 255));
            AlterColumn("dbo.LeaveApplicationDetails", "PlaceOfVisit", c => c.String(maxLength: 255));
            AlterColumn("dbo.LeaveApplicationDetails", "ContactAddress", c => c.String(maxLength: 255));
            AlterColumn("dbo.LeaveApplicationDetails", "PostUser", c => c.String(maxLength: 10));
        }

        public override void Down()
        {
            AlterColumn("dbo.LeaveApplicationDetails", "PostUser", c => c.String());
            AlterColumn("dbo.LeaveApplicationDetails", "ContactAddress", c => c.String());
            AlterColumn("dbo.LeaveApplicationDetails", "PlaceOfVisit", c => c.String());
            AlterColumn("dbo.LeaveApplicationDetails", "Remarks", c => c.String());
            AlterColumn("dbo.LeaveApplicationDetails", "IsPosted", c => c.String());
        }
    }
}