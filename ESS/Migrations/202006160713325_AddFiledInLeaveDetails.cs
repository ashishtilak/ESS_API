namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddFiledInLeaveDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveApplicationDetails", "AdditionalRemarks", c => c.String(maxLength: 255));
        }

        public override void Down()
        {
            DropColumn("dbo.LeaveApplicationDetails", "AdditionalRemarks");
        }
    }
}