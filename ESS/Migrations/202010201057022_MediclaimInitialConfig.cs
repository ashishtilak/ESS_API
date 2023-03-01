namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class MediclaimInitialConfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.MedDependents",
                    c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        DepSr = c.Int(nullable: false),
                        DepName = c.String(maxLength: 50),
                        Rleation = c.String(maxLength: 10),
                        BirthDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Gender = c.String(maxLength: 1),
                        MarriageDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        ProofType = c.String(maxLength: 1),
                        ProofNo = c.String(maxLength: 20),
                        EffectiveDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ReleaseGroupCode = c.String(maxLength: 2),
                        ReleaseStrategy = c.String(maxLength: 15),
                        ReleaseStatusCode = c.String(maxLength: 1),
                        ReleaseDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        ReleaseUser = c.String(maxLength: 10),
                        Active = c.Boolean(nullable: false),
                        AddUser = c.String(maxLength: 10),
                        AddDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new {t.EmpUnqId, t.DepSr})
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .ForeignKey("dbo.ReleaseStrategies", t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .Index(t => t.ReleaseGroupCode)
                .Index(t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .Index(t => t.ReleaseStatusCode);

            CreateTable(
                    "dbo.MedEmpUhids",
                    c => new
                    {
                        PolicyYear = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        DepSr = c.Int(nullable: false),
                        Uhid = c.String(maxLength: 25),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new {t.PolicyYear, t.EmpUnqId, t.DepSr})
                .ForeignKey("dbo.MedDependents", t => new {t.EmpUnqId, t.DepSr})
                .Index(t => new {t.EmpUnqId, t.DepSr});

            CreateTable(
                    "dbo.MedPolicies",
                    c => new
                    {
                        PolicyYear = c.Int(nullable: false),
                        PolicyNumber = c.String(nullable: false, maxLength: 20),
                        InsurerName = c.String(maxLength: 50),
                        PolicyType = c.String(maxLength: 1),
                        CompCode = c.String(maxLength: 2),
                        WrkGrp = c.String(maxLength: 10),
                        UnitCode = c.String(maxLength: 3),
                        TpaName = c.String(maxLength: 50),
                        ContactPerson = c.String(maxLength: 50),
                        ContactNumber = c.String(maxLength: 20),
                        AltContactNumber = c.String(maxLength: 20),
                        ContactEmail = c.String(maxLength: 50),
                        ValidFrom = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ValidTo = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new {t.PolicyYear, t.PolicyNumber})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Units", t => new {t.CompCode, t.WrkGrp, t.UnitCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode});
        }

        public override void Down()
        {
            DropForeignKey("dbo.MedPolicies", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.MedPolicies", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.MedPolicies", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.MedEmpUhids", new[] {"EmpUnqId", "DepSr"}, "dbo.MedDependents");
            DropForeignKey("dbo.MedDependents", new[] {"ReleaseGroupCode", "ReleaseStrategy"}, "dbo.ReleaseStrategies");
            DropForeignKey("dbo.MedDependents", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.MedDependents", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropIndex("dbo.MedPolicies", new[] {"CompCode", "WrkGrp", "UnitCode"});
            DropIndex("dbo.MedPolicies", new[] {"CompCode"});
            DropIndex("dbo.MedEmpUhids", new[] {"EmpUnqId", "DepSr"});
            DropIndex("dbo.MedDependents", new[] {"ReleaseStatusCode"});
            DropIndex("dbo.MedDependents", new[] {"ReleaseGroupCode", "ReleaseStrategy"});
            DropIndex("dbo.MedDependents", new[] {"ReleaseGroupCode"});
            DropTable("dbo.MedPolicies");
            DropTable("dbo.MedEmpUhids");
            DropTable("dbo.MedDependents");
        }
    }
}