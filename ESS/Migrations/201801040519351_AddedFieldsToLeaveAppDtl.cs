namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedFieldsToLeaveAppDtl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveApplicationDetails", "Remarks", c => c.String());
            AddColumn("dbo.LeaveApplicationDetails", "PlaceOfVisit", c => c.String());
            AddColumn("dbo.LeaveApplicationDetails", "ContactAddress", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.LeaveApplicationDetails", "ContactAddress");
            DropColumn("dbo.LeaveApplicationDetails", "PlaceOfVisit");
            DropColumn("dbo.LeaveApplicationDetails", "Remarks");
        }
    }
}