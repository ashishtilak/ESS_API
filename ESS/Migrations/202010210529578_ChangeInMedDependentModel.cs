namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeInMedDependentModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MedDependents", "Pan", c => c.String(maxLength: 10));
            AddColumn("dbo.MedDependents", "Aadhar", c => c.String(maxLength: 12));
            AddColumn("dbo.MedDependents", "BirthCertificateNo", c => c.String(maxLength: 20));
            DropColumn("dbo.MedDependents", "ProofType");
            DropColumn("dbo.MedDependents", "ProofNo");
        }

        public override void Down()
        {
            AddColumn("dbo.MedDependents", "ProofNo", c => c.String(maxLength: 20));
            AddColumn("dbo.MedDependents", "ProofType", c => c.String(maxLength: 1));
            DropColumn("dbo.MedDependents", "BirthCertificateNo");
            DropColumn("dbo.MedDependents", "Aadhar");
            DropColumn("dbo.MedDependents", "Pan");
        }
    }
}