namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedEmpUniform : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.EmpUniforms",
                    c => new
                    {
                        Year = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => new {t.Year, t.EmpUnqId})
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .Index(t => t.EmpUnqId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.EmpUniforms", "EmpUnqId", "dbo.Employees");
            DropIndex("dbo.EmpUniforms", new[] {"EmpUnqId"});
            DropTable("dbo.EmpUniforms");
        }
    }
}