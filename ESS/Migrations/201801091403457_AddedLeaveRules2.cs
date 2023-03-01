namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedLeaveRules2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveRules", "AllowedCl", c => c.Single(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.LeaveRules", "AllowedCl");
        }
    }
}