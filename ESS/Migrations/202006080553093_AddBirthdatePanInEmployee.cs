namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddBirthdatePanInEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "BirthDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Employees", "Pan", c => c.String(maxLength: 10));
        }

        public override void Down()
        {
            DropColumn("dbo.Employees", "Pan");
            DropColumn("dbo.Employees", "BirthDate");
        }
    }
}