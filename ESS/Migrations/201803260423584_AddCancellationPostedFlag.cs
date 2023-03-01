namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddCancellationPostedFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveApplicationDetails", "IsCancellationPosted", c => c.Boolean());
        }

        public override void Down()
        {
            DropColumn("dbo.LeaveApplicationDetails", "IsCancellationPosted");
        }
    }
}