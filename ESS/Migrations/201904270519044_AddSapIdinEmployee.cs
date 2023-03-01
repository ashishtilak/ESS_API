namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddSapIdinEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "SapId", c => c.String(maxLength: 12));
        }

        public override void Down()
        {
            DropColumn("dbo.Employees", "SapId");
        }
    }
}