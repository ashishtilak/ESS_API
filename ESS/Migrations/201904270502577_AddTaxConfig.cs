namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddTaxConfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TaxConfigs",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YearMonth = c.Int(nullable: false),
                        ActualFlag = c.Boolean(nullable: false),
                        StartDt = c.DateTime(nullable: false),
                        EndDt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.TaxConfigs");
        }
    }
}