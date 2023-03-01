namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddGaReleaserFlaginEmpMast : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "IsGaReleaser", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Employees", "IsGaReleaser");
        }
    }
}