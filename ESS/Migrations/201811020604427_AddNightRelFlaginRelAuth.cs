namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddNightRelFlaginRelAuth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReleaseAuths", "IsGpNightReleaser", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.ReleaseAuths", "IsGpNightReleaser");
        }
    }
}