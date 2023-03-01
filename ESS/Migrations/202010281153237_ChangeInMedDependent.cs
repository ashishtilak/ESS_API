namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeInMedDependent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MedDependents", "Relation", c => c.String(maxLength: 10));
            //AddColumn("dbo.MedDependents", "BirthCertificateNo", c => c.String(maxLength: 20));
            DropColumn("dbo.MedDependents", "Rleation");
            //DropColumn("dbo.MedDependents", "BirthCertiicateNo");
        }

        public override void Down()
        {
            //AddColumn("dbo.MedDependents", "BirthCertiicateNo", c => c.String(maxLength: 20));
            AddColumn("dbo.MedDependents", "Rleation", c => c.String(maxLength: 10));
            //DropColumn("dbo.MedDependents", "BirthCertificateNo");
            DropColumn("dbo.MedDependents", "Relation");
        }
    }
}