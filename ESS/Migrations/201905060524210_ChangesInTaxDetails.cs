namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangesInTaxDetails : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TaxDetailsInsurances", "PolicyMode");
        }

        public override void Down()
        {
            AddColumn("dbo.TaxDetailsInsurances", "PolicyMode", c => c.String());
        }
    }
}