namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddFlagsInEmpModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "IsGpReleaser", c => c.Boolean(nullable: false));
            AddColumn("dbo.Employees", "IsSecUser", c => c.Boolean(nullable: false));
            AddColumn("dbo.Employees", "IsAdmin", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Employees", "IsAdmin");
            DropColumn("dbo.Employees", "IsSecUser");
            DropColumn("dbo.Employees", "IsGpReleaser");
        }
    }
}