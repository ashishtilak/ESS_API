namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class test : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GpAdvices", "ApproxDateOfReturn", c => c.DateTime());
        }

        public override void Down()
        {
            AlterColumn("dbo.GpAdvices", "ApproxDateOfReturn", c => c.DateTime(nullable: false));
        }
    }
}