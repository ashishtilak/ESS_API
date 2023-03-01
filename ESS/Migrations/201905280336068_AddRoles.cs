namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddRoles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Roles",
                    c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.RoleId);

            CreateTable(
                    "dbo.RoleAuths",
                    c => new
                    {
                        RoleId = c.Int(nullable: false),
                        MenuId = c.String(nullable: false, maxLength: 100),
                        MenuName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => new {t.RoleId, t.MenuId});

            CreateTable(
                    "dbo.RoleUsers",
                    c => new
                    {
                        RoleId = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        UpdateUserId = c.String(maxLength: 10),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new {t.RoleId, t.EmpUnqId});
        }

        public override void Down()
        {
            DropTable("dbo.RoleUsers");
            DropTable("dbo.RoleAuths");
            DropTable("dbo.Roles");
        }
    }
}