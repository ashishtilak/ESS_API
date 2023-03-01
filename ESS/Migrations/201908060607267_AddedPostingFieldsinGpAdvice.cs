namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedPostingFieldsinGpAdvice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GpAdvices", "PostedUser", c => c.String(maxLength: 10));
            AddColumn("dbo.GpAdvices", "PostedDt", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.GpAdvices", "PostedDt");
            DropColumn("dbo.GpAdvices", "PostedUser");
        }
    }
}