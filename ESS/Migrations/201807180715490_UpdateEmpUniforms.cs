namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateEmpUniforms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmpUniforms", "TrouserSize", c => c.Int(nullable: false));
            AddColumn("dbo.EmpUniforms", "ShirtSize", c => c.Int(nullable: false));
            AddColumn("dbo.EmpUniforms", "UpdUser", c => c.String());
            AddColumn("dbo.EmpUniforms", "UpdTime", c => c.DateTime(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.EmpUniforms", "UpdTime");
            DropColumn("dbo.EmpUniforms", "UpdUser");
            DropColumn("dbo.EmpUniforms", "ShirtSize");
            DropColumn("dbo.EmpUniforms", "TrouserSize");
        }
    }
}