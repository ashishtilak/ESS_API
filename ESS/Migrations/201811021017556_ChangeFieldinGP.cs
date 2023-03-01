namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeFieldinGP : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.GatePasses", name: "ReleaseStrategy", newName: "GpReleaseStrategy");
            RenameIndex(table: "dbo.GatePasses", name: "IX_ReleaseGroupCode_ReleaseStrategy",
                newName: "IX_ReleaseGroupCode_GpReleaseStrategy");
        }

        public override void Down()
        {
            RenameIndex(table: "dbo.GatePasses", name: "IX_ReleaseGroupCode_GpReleaseStrategy",
                newName: "IX_ReleaseGroupCode_ReleaseStrategy");
            RenameColumn(table: "dbo.GatePasses", name: "GpReleaseStrategy", newName: "ReleaseStrategy");
        }
    }
}