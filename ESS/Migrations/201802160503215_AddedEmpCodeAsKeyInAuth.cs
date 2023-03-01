namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedEmpCodeAsKeyInAuth : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ReleaseAuths", new[] {"EmpUnqId"});
            DropPrimaryKey("dbo.ReleaseAuths");
            AlterColumn("dbo.ReleaseAuths", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AddPrimaryKey("dbo.ReleaseAuths", new[] {"ReleaseCode", "EmpUnqId"});
            CreateIndex("dbo.ReleaseAuths", "EmpUnqId");
        }

        public override void Down()
        {
            DropIndex("dbo.ReleaseAuths", new[] {"EmpUnqId"});
            DropPrimaryKey("dbo.ReleaseAuths");
            AlterColumn("dbo.ReleaseAuths", "EmpUnqId", c => c.String(maxLength: 10));
            AddPrimaryKey("dbo.ReleaseAuths", "ReleaseCode");
            CreateIndex("dbo.ReleaseAuths", "EmpUnqId");
        }
    }
}