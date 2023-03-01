namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangesInEmpSeparation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmpSeparations", "FurtherReleaseRequired", c => c.Boolean(nullable: false));
            AddColumn("dbo.EmpSeparations", "FurtherReleaser", c => c.String(maxLength: 10));
            AddColumn("dbo.EmpSeparations", "FurtherReleaseStatusCode", c => c.String(maxLength: 1));
            AddColumn("dbo.EmpSeparations", "FurtherReleaseDate",
                c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.EmpSeparations", "HrUser", c => c.String(maxLength: 10));
            AddColumn("dbo.EmpSeparations", "HrStatusDate",
                c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            CreateIndex("dbo.EmpSeparations", "FurtherReleaseStatusCode");
            AddForeignKey("dbo.EmpSeparations", "FurtherReleaseStatusCode", "dbo.ReleaseStatus", "ReleaseStatusCode");
        }

        public override void Down()
        {
            DropForeignKey("dbo.EmpSeparations", "FurtherReleaseStatusCode", "dbo.ReleaseStatus");
            DropIndex("dbo.EmpSeparations", new[] {"FurtherReleaseStatusCode"});
            DropColumn("dbo.EmpSeparations", "HrStatusDate");
            DropColumn("dbo.EmpSeparations", "HrUser");
            DropColumn("dbo.EmpSeparations", "FurtherReleaseDate");
            DropColumn("dbo.EmpSeparations", "FurtherReleaseStatusCode");
            DropColumn("dbo.EmpSeparations", "FurtherReleaser");
            DropColumn("dbo.EmpSeparations", "FurtherReleaseRequired");
        }
    }
}