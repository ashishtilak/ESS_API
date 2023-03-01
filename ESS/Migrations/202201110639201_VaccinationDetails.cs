namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VaccinationDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Vaccinations",
                c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        FirstDoseDate = c.DateTime(),
                        SecondDoseDate = c.DateTime(),
                        ThirdDoseDate = c.DateTime(),
                        UpdateDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.EmpUnqId)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .Index(t => t.EmpUnqId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vaccinations", "EmpUnqId", "dbo.Employees");
            DropIndex("dbo.Vaccinations", new[] { "EmpUnqId" });
            DropTable("dbo.Vaccinations");
        }
    }
}
