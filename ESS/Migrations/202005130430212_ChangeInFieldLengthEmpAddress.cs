namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeInFieldLengthEmpAddress : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmpAddresses", "HouseNumber", c => c.String(maxLength: 100));
            AlterColumn("dbo.EmpAddresses", "SocietyName", c => c.String(maxLength: 100));
        }

        public override void Down()
        {
            AlterColumn("dbo.EmpAddresses", "SocietyName", c => c.String(maxLength: 50));
            AlterColumn("dbo.EmpAddresses", "HouseNumber", c => c.String(maxLength: 50));
        }
    }
}