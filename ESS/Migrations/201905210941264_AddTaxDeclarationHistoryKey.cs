namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddTaxDeclarationHistoryKey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TaxDeclarationHistories");
            AddColumn("dbo.TaxDeclarationHistories", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.TaxDeclarationHistories", "EmpUnqId", c => c.String(maxLength: 10));
            AddPrimaryKey("dbo.TaxDeclarationHistories", "Id");
        }

        public override void Down()
        {
            DropPrimaryKey("dbo.TaxDeclarationHistories");
            AlterColumn("dbo.TaxDeclarationHistories", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            DropColumn("dbo.TaxDeclarationHistories", "Id");
            AddPrimaryKey("dbo.TaxDeclarationHistories", new[] {"YearMonth", "EmpUnqId", "ActualFlag"});
        }
    }
}