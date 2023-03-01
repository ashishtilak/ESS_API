namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddPerEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmpAddresses", "PreEmail", c => c.String(maxLength: 70));
        }

        public override void Down()
        {
            DropColumn("dbo.EmpAddresses", "PreEmail");
        }
    }
}