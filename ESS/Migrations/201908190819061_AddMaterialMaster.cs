namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddMaterialMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Materials",
                    c => new
                    {
                        MaterialCode = c.String(nullable: false, maxLength: 16),
                        MaterialDesc = c.String(maxLength: 50),
                        Uom = c.String(maxLength: 5),
                    })
                .PrimaryKey(t => t.MaterialCode);
        }

        public override void Down()
        {
            DropTable("dbo.Materials");
        }
    }
}