namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CoffDateAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveApplicationDetails", "CoMode", c => c.String(maxLength: 1));
            AddColumn("dbo.LeaveApplicationDetails", "CoDate1", c => c.DateTime());
            AddColumn("dbo.LeaveApplicationDetails", "CoDate2", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.LeaveApplicationDetails", "CoDate2");
            DropColumn("dbo.LeaveApplicationDetails", "CoDate1");
            DropColumn("dbo.LeaveApplicationDetails", "CoMode");
        }
    }
}