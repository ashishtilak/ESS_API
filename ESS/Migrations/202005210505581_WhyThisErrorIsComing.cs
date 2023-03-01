namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class WhyThisErrorIsComing : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmpAddresses", "HrUser", c => c.Boolean(nullable: false));
            AddColumn("dbo.EmpAddresses", "HrVerificationDate", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.EmpAddresses", "HrVerificationDate");
            DropColumn("dbo.EmpAddresses", "HrUser");
        }
    }
}