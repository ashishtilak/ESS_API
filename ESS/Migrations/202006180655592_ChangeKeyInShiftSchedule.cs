namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ChangeKeyInShiftSchedule : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId"}, "dbo.ShiftSchedules");
            DropIndex("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId"});
            DropIndex("dbo.ShiftSchedules", new[] {"EmpUnqId"});
            DropPrimaryKey("dbo.ShiftScheduleDetails");
            DropPrimaryKey("dbo.ShiftSchedules");
            AddColumn("dbo.ShiftScheduleDetails", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.ShiftSchedules", "EmpUnqId", c => c.String(nullable: false, maxLength: 10));
            AddPrimaryKey("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId", "EmpUnqId", "ShiftDay"});
            AddPrimaryKey("dbo.ShiftSchedules", new[] {"YearMonth", "ScheduleId", "EmpUnqId"});
            CreateIndex("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId", "EmpUnqId"});
            CreateIndex("dbo.ShiftSchedules", "EmpUnqId");
            AddForeignKey("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId", "EmpUnqId"},
                "dbo.ShiftSchedules", new[] {"YearMonth", "ScheduleId", "EmpUnqId"});
        }

        public override void Down()
        {
            DropForeignKey("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId", "EmpUnqId"},
                "dbo.ShiftSchedules");
            DropIndex("dbo.ShiftSchedules", new[] {"EmpUnqId"});
            DropIndex("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId", "EmpUnqId"});
            DropPrimaryKey("dbo.ShiftSchedules");
            DropPrimaryKey("dbo.ShiftScheduleDetails");
            AlterColumn("dbo.ShiftSchedules", "EmpUnqId", c => c.String(maxLength: 10));
            DropColumn("dbo.ShiftScheduleDetails", "EmpUnqId");
            AddPrimaryKey("dbo.ShiftSchedules", new[] {"YearMonth", "ScheduleId"});
            AddPrimaryKey("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId", "ShiftDay"});
            CreateIndex("dbo.ShiftSchedules", "EmpUnqId");
            CreateIndex("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId"});
            AddForeignKey("dbo.ShiftScheduleDetails", new[] {"YearMonth", "ScheduleId"}, "dbo.ShiftSchedules",
                new[] {"YearMonth", "ScheduleId"});
        }
    }
}