namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class OtFlagAddedInEmp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "OtFlag", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Employees", "OtFlag");
        }
    }
}