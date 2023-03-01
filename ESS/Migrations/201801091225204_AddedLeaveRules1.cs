namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedLeaveRules1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.LeaveRules",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LeaveRule = c.String(),
                    })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.LeaveRules");
        }
    }
}