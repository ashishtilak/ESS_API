namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddCompAccInEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "CompanyAcc", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Employees", "CompanyAcc");
        }
    }
}