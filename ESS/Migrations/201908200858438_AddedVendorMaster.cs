namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedVendorMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Vendors",
                    c => new
                    {
                        VendorCode = c.String(nullable: false, maxLength: 10),
                        VendorName = c.String(maxLength: 100),
                        VendorAddress1 = c.String(maxLength: 255),
                        VendorAddress2 = c.String(maxLength: 255),
                        VendorAddress3 = c.String(maxLength: 255),
                        UpdDt = c.DateTime(),
                        UpdUser = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.VendorCode);
        }

        public override void Down()
        {
            DropTable("dbo.Vendors");
        }
    }
}