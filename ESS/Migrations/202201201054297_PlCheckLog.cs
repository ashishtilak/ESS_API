namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlCheckLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlCheckLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UpdateDate = c.DateTime(nullable: false),
                        EmpUnqId = c.String(maxLength: 10),
                        OldValue = c.Boolean(nullable: false),
                        NewValue = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PlCheckLogs");
        }
    }
}
