namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddBanks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Banks",
                    c => new
                    {
                        BankId = c.Int(nullable: false, identity: true),
                        BankName = c.String(maxLength: 100),
                        BankPan = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.BankId);
        }

        public override void Down()
        {
            DropTable("dbo.Banks");
        }
    }
}