namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeLengthOfRelCodeInAuth : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ReleaseAuths");
            AlterColumn("dbo.ReleaseAuths", "ReleaseCode", c => c.String(nullable: false, maxLength: 20));
            AddPrimaryKey("dbo.ReleaseAuths", "ReleaseCode");
        }

        public override void Down()
        {
            DropPrimaryKey("dbo.ReleaseAuths");
            AlterColumn("dbo.ReleaseAuths", "ReleaseCode", c => c.String(nullable: false, maxLength: 10));
            AddPrimaryKey("dbo.ReleaseAuths", "ReleaseCode");
        }
    }
}