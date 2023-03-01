namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedCounterInEmpAddress : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.EmpAddresses");
            AddColumn("dbo.EmpAddresses", "Counter", c => c.Int(nullable: false));
            AddColumn("dbo.EmpAddresses", "UpdDt", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.EmpAddresses", new[] {"EmpUnqId", "Counter"});
        }

        public override void Down()
        {
            DropPrimaryKey("dbo.EmpAddresses");
            DropColumn("dbo.EmpAddresses", "UpdDt");
            DropColumn("dbo.EmpAddresses", "Counter");
            AddPrimaryKey("dbo.EmpAddresses", "EmpUnqId");
        }
    }
}