namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeRelCodeLenInAPpRel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ApplReleaseStatus", "ReleaseCode", c => c.String(nullable: false, maxLength: 20));
        }

        public override void Down()
        {
            AlterColumn("dbo.ApplReleaseStatus", "ReleaseCode", c => c.String(nullable: false, maxLength: 10));
        }
    }
}