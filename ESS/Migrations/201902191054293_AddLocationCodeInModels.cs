namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddLocationCodeInModels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "Location", c => c.String(maxLength: 5));
            AddColumn("dbo.Companies", "Location", c => c.String(maxLength: 5));
            AddColumn("dbo.WorkGroups", "Location", c => c.String(maxLength: 5));
            AddColumn("dbo.Units", "Location", c => c.String(maxLength: 5));
            AddColumn("dbo.Departments", "Location", c => c.String(maxLength: 5));
            AddColumn("dbo.Designations", "Location", c => c.String(maxLength: 5));
            AddColumn("dbo.EmpTypes", "Location", c => c.String(maxLength: 5));
            AddColumn("dbo.Grades", "Location", c => c.String(maxLength: 5));
            AddColumn("dbo.Stations", "Location", c => c.String(maxLength: 5));
            AddColumn("dbo.Sections", "Location", c => c.String(maxLength: 5));
        }

        public override void Down()
        {
            DropColumn("dbo.Sections", "Location");
            DropColumn("dbo.Stations", "Location");
            DropColumn("dbo.Grades", "Location");
            DropColumn("dbo.EmpTypes", "Location");
            DropColumn("dbo.Designations", "Location");
            DropColumn("dbo.Departments", "Location");
            DropColumn("dbo.Units", "Location");
            DropColumn("dbo.WorkGroups", "Location");
            DropColumn("dbo.Companies", "Location");
            DropColumn("dbo.Categories", "Location");
        }
    }
}