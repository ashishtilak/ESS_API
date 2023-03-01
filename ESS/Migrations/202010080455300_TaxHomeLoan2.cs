namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class TaxHomeLoan2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaxDeclarationHistories", "PropertyAddress2", c => c.String(maxLength: 200));
            AddColumn("dbo.TaxDeclarationHistories", "PropertyStatus2", c => c.String(maxLength: 1));
            AddColumn("dbo.TaxDeclarationHistories", "LoanBank2", c => c.String(maxLength: 150));
            AddColumn("dbo.TaxDeclarationHistories", "LoanBankPan2", c => c.String(maxLength: 10));
            AddColumn("dbo.TaxDeclarationHistories", "LoanAmount2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarationHistories", "LoanDate2", c => c.DateTime());
            AddColumn("dbo.TaxDeclarationHistories", "Purpose2", c => c.String(maxLength: 1));
            AddColumn("dbo.TaxDeclarationHistories", "ConstructionCompDate2", c => c.DateTime());
            AddColumn("dbo.TaxDeclarationHistories", "PossessionDate2", c => c.DateTime());
            AddColumn("dbo.TaxDeclarationHistories", "Ownership2", c => c.String(maxLength: 1));
            AddColumn("dbo.TaxDeclarationHistories", "JointOwnerName2", c => c.String(maxLength: 50));
            AddColumn("dbo.TaxDeclarationHistories", "JointOwnerRelation2", c => c.String(maxLength: 20));
            AddColumn("dbo.TaxDeclarationHistories", "JointOwnerShare2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarationHistories", "RentalIncomePerMonth2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarationHistories", "MunicipalTax2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarationHistories", "InterestOnLoan2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarationHistories", "InterestPreConstruction2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "PropertyAddress2", c => c.String(maxLength: 200));
            AddColumn("dbo.TaxDeclarations", "PropertyStatus2", c => c.String(maxLength: 1));
            AddColumn("dbo.TaxDeclarations", "LoanBank2", c => c.String(maxLength: 150));
            AddColumn("dbo.TaxDeclarations", "LoanBankPan2", c => c.String(maxLength: 10));
            AddColumn("dbo.TaxDeclarations", "LoanAmount2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "LoanDate2", c => c.DateTime());
            AddColumn("dbo.TaxDeclarations", "Purpose2", c => c.String(maxLength: 1));
            AddColumn("dbo.TaxDeclarations", "ConstructionCompDate2", c => c.DateTime());
            AddColumn("dbo.TaxDeclarations", "PossessionDate2", c => c.DateTime());
            AddColumn("dbo.TaxDeclarations", "Ownership2", c => c.String(maxLength: 1));
            AddColumn("dbo.TaxDeclarations", "JointOwnerName2", c => c.String(maxLength: 50));
            AddColumn("dbo.TaxDeclarations", "JointOwnerRelation2", c => c.String(maxLength: 20));
            AddColumn("dbo.TaxDeclarations", "JointOwnerShare2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "RentalIncomePerMonth2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "MunicipalTax2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "InterestOnLoan2", c => c.Single(nullable: false));
            AddColumn("dbo.TaxDeclarations", "InterestPreConstruction2", c => c.Single(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TaxDeclarations", "InterestPreConstruction2");
            DropColumn("dbo.TaxDeclarations", "InterestOnLoan2");
            DropColumn("dbo.TaxDeclarations", "MunicipalTax2");
            DropColumn("dbo.TaxDeclarations", "RentalIncomePerMonth2");
            DropColumn("dbo.TaxDeclarations", "JointOwnerShare2");
            DropColumn("dbo.TaxDeclarations", "JointOwnerRelation2");
            DropColumn("dbo.TaxDeclarations", "JointOwnerName2");
            DropColumn("dbo.TaxDeclarations", "Ownership2");
            DropColumn("dbo.TaxDeclarations", "PossessionDate2");
            DropColumn("dbo.TaxDeclarations", "ConstructionCompDate2");
            DropColumn("dbo.TaxDeclarations", "Purpose2");
            DropColumn("dbo.TaxDeclarations", "LoanDate2");
            DropColumn("dbo.TaxDeclarations", "LoanAmount2");
            DropColumn("dbo.TaxDeclarations", "LoanBankPan2");
            DropColumn("dbo.TaxDeclarations", "LoanBank2");
            DropColumn("dbo.TaxDeclarations", "PropertyStatus2");
            DropColumn("dbo.TaxDeclarations", "PropertyAddress2");
            DropColumn("dbo.TaxDeclarationHistories", "InterestPreConstruction2");
            DropColumn("dbo.TaxDeclarationHistories", "InterestOnLoan2");
            DropColumn("dbo.TaxDeclarationHistories", "MunicipalTax2");
            DropColumn("dbo.TaxDeclarationHistories", "RentalIncomePerMonth2");
            DropColumn("dbo.TaxDeclarationHistories", "JointOwnerShare2");
            DropColumn("dbo.TaxDeclarationHistories", "JointOwnerRelation2");
            DropColumn("dbo.TaxDeclarationHistories", "JointOwnerName2");
            DropColumn("dbo.TaxDeclarationHistories", "Ownership2");
            DropColumn("dbo.TaxDeclarationHistories", "PossessionDate2");
            DropColumn("dbo.TaxDeclarationHistories", "ConstructionCompDate2");
            DropColumn("dbo.TaxDeclarationHistories", "Purpose2");
            DropColumn("dbo.TaxDeclarationHistories", "LoanDate2");
            DropColumn("dbo.TaxDeclarationHistories", "LoanAmount2");
            DropColumn("dbo.TaxDeclarationHistories", "LoanBankPan2");
            DropColumn("dbo.TaxDeclarationHistories", "LoanBank2");
            DropColumn("dbo.TaxDeclarationHistories", "PropertyStatus2");
            DropColumn("dbo.TaxDeclarationHistories", "PropertyAddress2");
        }
    }
}