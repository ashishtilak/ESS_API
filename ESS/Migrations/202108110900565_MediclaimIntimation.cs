namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediclaimIntimation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MedIntimations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IntimationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EmpUnqId = c.String(maxLength: 10),
                        InsuredMobileNo = c.String(maxLength: 15),
                        PatientName = c.String(maxLength: 50),
                        Relation = c.String(maxLength: 10),
                        IntimatorName = c.String(maxLength: 50),
                        IntimatorMobileNo = c.String(maxLength: 15),
                        AdmissionDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DoctorName = c.String(maxLength: 50),
                        DoctorRegistrationNumber = c.String(maxLength: 50),
                        Diagnosis = c.String(maxLength: 255),
                        HospitalName = c.String(maxLength: 50),
                        HospitalRegistrationNumber = c.String(maxLength: 50),
                        HospitalAddress = c.String(maxLength: 255),
                        Pin = c.String(maxLength: 6),
                        AddUser = c.String(maxLength: 10),
                        HrUser = c.String(maxLength: 10),
                        HrApproveDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        HrRemarks = c.String(maxLength: 50),
                        ReleaseStatusCode = c.String(maxLength: 1),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .Index(t => t.EmpUnqId)
                .Index(t => t.ReleaseStatusCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MedIntimations", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.MedIntimations", "EmpUnqId", "dbo.Employees");
            DropIndex("dbo.MedIntimations", new[] { "ReleaseStatusCode" });
            DropIndex("dbo.MedIntimations", new[] { "EmpUnqId" });
            DropTable("dbo.MedIntimations");
        }
    }
}
