namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeBankNameLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Banks", "BankName", c => c.String(maxLength: 150));
            AlterColumn("dbo.TaxDeclarations", "LoanBank", c => c.String(maxLength: 150));
        }

        public override void Down()
        {
            AlterColumn("dbo.TaxDeclarations", "LoanBank", c => c.String(maxLength: 20));
            AlterColumn("dbo.Banks", "BankName", c => c.String(maxLength: 100));
        }
    }
}