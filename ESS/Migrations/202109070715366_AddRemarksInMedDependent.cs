namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemarksInMedDependent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MedDependents", "Remarks", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MedDependents", "Remarks");
        }
    }
}
