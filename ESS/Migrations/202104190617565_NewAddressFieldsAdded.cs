namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NewAddressFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmpAddresses", "HrCity", c => c.String(maxLength: 50));
            AddColumn("dbo.EmpAddresses", "HrSociety", c => c.String(maxLength: 50));
            AddColumn("dbo.EmpAddresses", "HrRemarks", c => c.String(maxLength: 50));
        }

        public override void Down()
        {
            DropColumn("dbo.EmpAddresses", "HrRemarks");
            DropColumn("dbo.EmpAddresses", "HrSociety");
            DropColumn("dbo.EmpAddresses", "HrCity");
        }
    }
}