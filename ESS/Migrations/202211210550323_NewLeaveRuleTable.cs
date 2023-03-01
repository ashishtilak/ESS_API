namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewLeaveRuleTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LeaveChecks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LeaveTaken = c.String(maxLength: 2),
                        LeaveCheck = c.String(maxLength: 2),
                        IsAllowed = c.Boolean(nullable: false),
                        DaysAllowed = c.Single(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LeaveChecks");
        }
    }
}
