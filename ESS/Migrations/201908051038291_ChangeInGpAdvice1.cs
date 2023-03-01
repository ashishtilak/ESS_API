namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeInGpAdvice1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GpAdvices", "Remarks", c => c.String(maxLength: 100));
        }

        public override void Down()
        {
            DropColumn("dbo.GpAdvices", "Remarks");
        }
    }
}