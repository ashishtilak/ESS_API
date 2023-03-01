namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class LocationModel_EmpModelChange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Locations",
                    c => new
                    {
                        Location = c.String(nullable: false, maxLength: 5),
                        RemoteConnection = c.String(),
                        AttendanceServerApi = c.String(),
                        MailAddress = c.String(),
                        SmtpClient = c.String(),
                        PortalAddress = c.String(),
                    })
                .PrimaryKey(t => t.Location);

            AddColumn("dbo.Employees", "Location", c => c.String(maxLength: 5));
        }

        public override void Down()
        {
            DropColumn("dbo.Employees", "Location");
            DropTable("dbo.Locations");
        }
    }
}