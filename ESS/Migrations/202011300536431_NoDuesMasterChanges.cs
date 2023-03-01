namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NoDuesMasterChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NoDuesDeptLists", "DeptName", c => c.String(maxLength: 20));
            AlterColumn("dbo.NoDuesMasters", "NoticePeriod", c => c.String(maxLength: 50));
            AlterColumn("dbo.NoDuesMasters", "NoticePeriodUnit", c => c.String(maxLength: 10));
        }

        public override void Down()
        {
            AlterColumn("dbo.NoDuesMasters", "NoticePeriodUnit", c => c.String(maxLength: 3));
            AlterColumn("dbo.NoDuesMasters", "NoticePeriod", c => c.Int());
            DropColumn("dbo.NoDuesDeptLists", "DeptName");
        }
    }
}