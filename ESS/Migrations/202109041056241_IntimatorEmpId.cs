namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IntimatorEmpId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MedIntimations", "IntimatorEmpUnqId", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MedIntimations", "IntimatorEmpUnqId");
        }
    }
}
