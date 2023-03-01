namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class FieldChangesInEmpAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmpAddresses", "HouseNumber", c => c.String(maxLength: 50));
            AddColumn("dbo.EmpAddresses", "SocietyName", c => c.String(maxLength: 50));
            AddColumn("dbo.EmpAddresses", "AreaName", c => c.String(maxLength: 50));
            AddColumn("dbo.EmpAddresses", "LandMark", c => c.String(maxLength: 50));
            AddColumn("dbo.EmpAddresses", "Tehsil", c => c.String(maxLength: 50));
            AddColumn("dbo.EmpAddresses", "PoliceStation", c => c.String(maxLength: 50));
            AddColumn("dbo.EmpAddresses", "HrVerified", c => c.Boolean(nullable: false));
            DropColumn("dbo.EmpAddresses", "PreAdd1");
            DropColumn("dbo.EmpAddresses", "PreAdd2");
            DropColumn("dbo.EmpAddresses", "PreAdd3");
            DropColumn("dbo.EmpAddresses", "PreAdd4");
        }

        public override void Down()
        {
            AddColumn("dbo.EmpAddresses", "PreAdd4", c => c.String(maxLength: 50));
            AddColumn("dbo.EmpAddresses", "PreAdd3", c => c.String(maxLength: 50));
            AddColumn("dbo.EmpAddresses", "PreAdd2", c => c.String(maxLength: 100));
            AddColumn("dbo.EmpAddresses", "PreAdd1", c => c.String(maxLength: 100));
            DropColumn("dbo.EmpAddresses", "HrVerified");
            DropColumn("dbo.EmpAddresses", "PoliceStation");
            DropColumn("dbo.EmpAddresses", "Tehsil");
            DropColumn("dbo.EmpAddresses", "LandMark");
            DropColumn("dbo.EmpAddresses", "AreaName");
            DropColumn("dbo.EmpAddresses", "SocietyName");
            DropColumn("dbo.EmpAddresses", "HouseNumber");
        }
    }
}