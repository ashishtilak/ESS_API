namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedEmployeePresentAddress : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.EmpAddresses",
                    c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        PreAdd1 = c.String(maxLength: 100),
                        PreAdd2 = c.String(maxLength: 100),
                        PreAdd3 = c.String(maxLength: 50),
                        PreAdd4 = c.String(maxLength: 50),
                        PreDistrict = c.String(maxLength: 50),
                        PreCity = c.String(maxLength: 50),
                        PreState = c.String(maxLength: 50),
                        PrePin = c.String(maxLength: 6),
                        PrePhone = c.String(maxLength: 20),
                        PreResPhone = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.EmpUnqId);
        }

        public override void Down()
        {
            DropTable("dbo.EmpAddresses");
        }
    }
}