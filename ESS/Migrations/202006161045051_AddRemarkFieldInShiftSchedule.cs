namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddRemarkFieldInShiftSchedule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShiftSchedules", "Remarks", c => c.String(maxLength: 255));
        }

        public override void Down()
        {
            DropColumn("dbo.ShiftSchedules", "Remarks");
        }
    }
}