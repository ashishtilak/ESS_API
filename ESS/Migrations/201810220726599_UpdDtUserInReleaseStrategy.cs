namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdDtUserInReleaseStrategy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReleaseStrategies", "UpdDt", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.ReleaseStrategies", "UpdUser", c => c.String(maxLength: 10));
        }

        public override void Down()
        {
            DropColumn("dbo.ReleaseStrategies", "UpdUser");
            DropColumn("dbo.ReleaseStrategies", "UpdDt");
        }
    }
}