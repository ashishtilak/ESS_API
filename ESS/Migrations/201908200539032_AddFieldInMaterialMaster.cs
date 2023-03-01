namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddFieldInMaterialMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Materials", "UpdDt", c => c.DateTime());
            AddColumn("dbo.Materials", "UpdUser", c => c.String(maxLength: 10));
        }

        public override void Down()
        {
            DropColumn("dbo.Materials", "UpdUser");
            DropColumn("dbo.Materials", "UpdDt");
        }
    }
}