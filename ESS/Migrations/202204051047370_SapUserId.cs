namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SapUserId : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SapUserIds",
                c => new
                    {
                        SapUserId = c.String(nullable: false, maxLength: 12),
                        EmpUnqId = c.String(maxLength: 10),
                        LineOfBusiness = c.String(maxLength: 2),
                        UserName = c.String(maxLength: 50),
                        DeptName = c.String(maxLength: 50),
                        IsCommon = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ValidTo = c.DateTime(),
                        Remarks = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.SapUserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SapUserIds");
        }
    }
}
