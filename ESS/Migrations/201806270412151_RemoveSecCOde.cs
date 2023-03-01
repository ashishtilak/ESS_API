namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveSecCOde : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"},
                "dbo.Sections");
            DropIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            CreateIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            DropColumn("dbo.Employees", "SecCode");
        }

        public override void Down()
        {
            AddColumn("dbo.Employees", "SecCode", c => c.String(maxLength: 3));
            DropIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            CreateIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
            AddForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"},
                "dbo.Sections", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode", "SecCode"});
        }
    }
}