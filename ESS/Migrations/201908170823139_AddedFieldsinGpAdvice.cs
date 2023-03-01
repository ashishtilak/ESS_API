namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedFieldsinGpAdvice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GpAdvices", "Requisitioner", c => c.String(maxLength: 10));
            AddColumn("dbo.GpAdvices", "SapGpNumber", c => c.String(maxLength: 10));
            AddColumn("dbo.GpAdvices", "PoTerms", c => c.String(maxLength: 255));
        }

        public override void Down()
        {
            DropColumn("dbo.GpAdvices", "PoTerms");
            DropColumn("dbo.GpAdvices", "SapGpNumber");
            DropColumn("dbo.GpAdvices", "Requisitioner");
        }
    }
}