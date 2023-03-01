namespace ESS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.ApplReleaseStatus",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        ReleaseGroupCode = c.String(nullable: false, maxLength: 2),
                        ApplicationId = c.Int(nullable: false),
                        ReleaseStrategy = c.String(nullable: false, maxLength: 15),
                        ReleaseStrategyLevel = c.Int(nullable: false),
                        ReleaseCode = c.String(nullable: false, maxLength: 10),
                        ReleaseStatusCode = c.String(nullable: false, maxLength: 1),
                        ReleaseDate = c.DateTime(),
                        ReleaseAuth = c.String(maxLength: 10),
                        IsFinalRelease = c.Boolean(nullable: false),
                        Remarks = c.String(maxLength: 255),
                        LeaveApplications_YearMonth = c.Int(),
                        LeaveApplications_LeaveAppId = c.Int(),
                    })
                .PrimaryKey(t => new
                    {t.YearMonth, t.ReleaseGroupCode, t.ApplicationId, t.ReleaseStrategy, t.ReleaseStrategyLevel})
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .ForeignKey("dbo.LeaveApplications",
                    t => new {t.LeaveApplications_YearMonth, t.LeaveApplications_LeaveAppId})
                .Index(t => t.ReleaseGroupCode)
                .Index(t => t.ReleaseStatusCode)
                .Index(t => new {t.LeaveApplications_YearMonth, t.LeaveApplications_LeaveAppId});

            CreateTable(
                    "dbo.ReleaseGroups",
                    c => new
                    {
                        ReleaseGroupCode = c.String(nullable: false, maxLength: 2),
                        ReleaseGroupDesc = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ReleaseGroupCode);

            CreateTable(
                    "dbo.ReleaseStatus",
                    c => new
                    {
                        ReleaseStatusCode = c.String(nullable: false, maxLength: 1),
                        ReleaseStatusDesc = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.ReleaseStatusCode);

            CreateTable(
                    "dbo.Categories",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        CatCode = c.String(nullable: false, maxLength: 3),
                        CatName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.CatCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp});

            CreateTable(
                    "dbo.Companies",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        CompName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.CompCode);

            CreateTable(
                    "dbo.WorkGroups",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        WrkGrpDesc = c.String(maxLength: 50),
                        AddDt = c.DateTime(),
                        AddUser = c.String(maxLength: 8),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .Index(t => t.CompCode);

            CreateTable(
                    "dbo.Contractors",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        UnitCode = c.String(nullable: false, maxLength: 3),
                        ContCode = c.String(nullable: false, maxLength: 3),
                        ContName = c.String(maxLength: 150),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.ContCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Units", t => new {t.CompCode, t.WrkGrp, t.UnitCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode});

            CreateTable(
                    "dbo.Units",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        UnitCode = c.String(nullable: false, maxLength: 3),
                        UnitName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.UnitCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp});

            CreateTable(
                    "dbo.Departments",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        UnitCode = c.String(nullable: false, maxLength: 3),
                        DeptCode = c.String(nullable: false, maxLength: 3),
                        DeptName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp});

            CreateTable(
                    "dbo.Designations",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        DesgCode = c.String(nullable: false, maxLength: 3),
                        DesgName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.DesgCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp});

            CreateTable(
                    "dbo.Employees",
                    c => new
                    {
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        CompCode = c.String(maxLength: 2),
                        WrkGrp = c.String(maxLength: 10),
                        EmpTypeCode = c.String(maxLength: 3),
                        UnitCode = c.String(maxLength: 3),
                        DeptCode = c.String(maxLength: 3),
                        StatCode = c.String(maxLength: 3),
                        CatCode = c.String(maxLength: 3),
                        DesgCode = c.String(maxLength: 3),
                        GradeCode = c.String(maxLength: 3),
                        EmpName = c.String(maxLength: 50),
                        FatherName = c.String(maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        IsReleaser = c.Boolean(nullable: false),
                        IsHrUser = c.Boolean(nullable: false),
                        IsHod = c.Boolean(nullable: false),
                        Pass = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.EmpUnqId)
                .ForeignKey("dbo.Categories", t => new {t.CompCode, t.WrkGrp, t.CatCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Departments", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .ForeignKey("dbo.Designations", t => new {t.CompCode, t.WrkGrp, t.DesgCode})
                .ForeignKey("dbo.EmpTypes", t => new {t.CompCode, t.WrkGrp, t.EmpTypeCode})
                .ForeignKey("dbo.Grades", t => new {t.CompCode, t.WrkGrp, t.GradeCode})
                .ForeignKey("dbo.Stations", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode})
                .ForeignKey("dbo.Units", t => new {t.CompCode, t.WrkGrp, t.UnitCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => new {t.CompCode, t.WrkGrp, t.CatCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.DesgCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.EmpTypeCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.GradeCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode});

            CreateTable(
                    "dbo.EmpTypes",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        EmpTypeCode = c.String(nullable: false, maxLength: 3),
                        EmpTypeName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.EmpTypeCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp});

            CreateTable(
                    "dbo.Grades",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        GradeCode = c.String(nullable: false, maxLength: 3),
                        GradeName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.GradeCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp});

            CreateTable(
                    "dbo.Stations",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        UnitCode = c.String(nullable: false, maxLength: 3),
                        DeptCode = c.String(nullable: false, maxLength: 3),
                        StatCode = c.String(nullable: false, maxLength: 3),
                        StatName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Departments", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .ForeignKey("dbo.Units", t => new {t.CompCode, t.WrkGrp, t.UnitCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode});

            CreateTable(
                    "dbo.LeaveApplicationDetails",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        LeaveAppId = c.Int(nullable: false),
                        LeaveAppItem = c.Int(nullable: false),
                        CompCode = c.String(maxLength: 2),
                        WrkGrp = c.String(maxLength: 10),
                        LeaveTypeCode = c.String(maxLength: 2),
                        FromDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        ToDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        HalfDayFlag = c.Boolean(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        IsPosted = c.String(),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.LeaveAppId, t.LeaveAppItem})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.LeaveApplications", t => new {t.YearMonth, t.LeaveAppId})
                .ForeignKey("dbo.LeaveTypes", t => new {t.CompCode, t.WrkGrp, t.LeaveTypeCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => new {t.YearMonth, t.LeaveAppId})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp, t.LeaveTypeCode});

            CreateTable(
                    "dbo.LeaveApplications",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        LeaveAppId = c.Int(nullable: false),
                        EmpUnqId = c.String(maxLength: 10),
                        CompCode = c.String(maxLength: 2),
                        WrkGrp = c.String(maxLength: 10),
                        UnitCode = c.String(maxLength: 3),
                        DeptCode = c.String(maxLength: 3),
                        StatCode = c.String(maxLength: 3),
                        CatCode = c.String(maxLength: 3),
                        IsHod = c.Boolean(nullable: false),
                        ReleaseGroupCode = c.String(maxLength: 2),
                        ReleaseStrategy = c.String(maxLength: 15),
                        ReleaseStatusCode = c.String(maxLength: 1),
                        AddDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        AddUser = c.String(maxLength: 10),
                        UpdDt = c.DateTime(precision: 7, storeType: "datetime2"),
                        UpdUser = c.String(maxLength: 10),
                        Remarks = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => new {t.YearMonth, t.LeaveAppId})
                .ForeignKey("dbo.Categories", t => new {t.CompCode, t.WrkGrp, t.CatCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Departments", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStatus", t => t.ReleaseStatusCode)
                .ForeignKey("dbo.ReleaseStrategies", t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .ForeignKey("dbo.Stations", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode})
                .ForeignKey("dbo.Units", t => new {t.CompCode, t.WrkGrp, t.UnitCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.EmpUnqId)
                .Index(t => new {t.CompCode, t.WrkGrp, t.CatCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode})
                .Index(t => t.ReleaseGroupCode)
                .Index(t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .Index(t => t.ReleaseStatusCode);

            CreateTable(
                    "dbo.ReleaseStrategies",
                    c => new
                    {
                        ReleaseGroupCode = c.String(nullable: false, maxLength: 2),
                        ReleaseStrategy = c.String(nullable: false, maxLength: 15),
                        ReleaseStrategyName = c.String(maxLength: 100),
                        CompCode = c.String(maxLength: 2),
                        WrkGrp = c.String(maxLength: 10),
                        UnitCode = c.String(maxLength: 3),
                        DeptCode = c.String(maxLength: 3),
                        StatCode = c.String(maxLength: 3),
                        CatCode = c.String(maxLength: 3),
                        IsHod = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .ForeignKey("dbo.Categories", t => new {t.CompCode, t.WrkGrp, t.CatCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Departments", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.Stations", t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode})
                .ForeignKey("dbo.Units", t => new {t.CompCode, t.WrkGrp, t.UnitCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.ReleaseGroupCode)
                .Index(t => new {t.CompCode, t.WrkGrp, t.CatCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode})
                .Index(t => new {t.CompCode, t.WrkGrp, t.UnitCode, t.DeptCode, t.StatCode});

            CreateTable(
                    "dbo.LeaveTypes",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        LeaveTypeCode = c.String(nullable: false, maxLength: 2),
                        LeaveTypeName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.LeaveTypeCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp});

            CreateTable(
                    "dbo.LeaveBalances",
                    c => new
                    {
                        CompCode = c.String(nullable: false, maxLength: 2),
                        WrkGrp = c.String(nullable: false, maxLength: 10),
                        YearMonth = c.Int(nullable: false),
                        EmpUnqId = c.String(nullable: false, maxLength: 10),
                        LeaveTypeCode = c.String(nullable: false, maxLength: 2),
                        Opening = c.Single(nullable: false),
                        Availed = c.Single(nullable: false),
                        Balance = c.Single(nullable: false),
                        Encashed = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new {t.CompCode, t.WrkGrp, t.YearMonth, t.EmpUnqId, t.LeaveTypeCode})
                .ForeignKey("dbo.Companies", t => t.CompCode)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .ForeignKey("dbo.LeaveTypes", t => new {t.CompCode, t.WrkGrp, t.LeaveTypeCode})
                .ForeignKey("dbo.WorkGroups", t => new {t.CompCode, t.WrkGrp})
                .Index(t => t.CompCode)
                .Index(t => new {t.CompCode, t.WrkGrp, t.LeaveTypeCode})
                .Index(t => t.EmpUnqId);

            CreateTable(
                    "dbo.OpenMonths",
                    c => new
                    {
                        YearMonth = c.Int(nullable: false),
                        OpenYear = c.Int(nullable: false),
                        PrevMonth = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.YearMonth);

            CreateTable(
                    "dbo.ReleaseAuths",
                    c => new
                    {
                        ReleaseCode = c.String(nullable: false, maxLength: 10),
                        EmpUnqId = c.String(maxLength: 10),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ReleaseCode)
                .ForeignKey("dbo.Employees", t => t.EmpUnqId)
                .Index(t => t.EmpUnqId);

            CreateTable(
                    "dbo.ReleaseStrategyLevels",
                    c => new
                    {
                        ReleaseGroupCode = c.String(nullable: false, maxLength: 2),
                        ReleaseStrategy = c.String(nullable: false, maxLength: 15),
                        ReleaseStrategyLevel = c.Int(nullable: false),
                        ReleaseCode = c.String(nullable: false, maxLength: 10),
                        IsFinalRelease = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new {t.ReleaseGroupCode, t.ReleaseStrategy, t.ReleaseStrategyLevel})
                .ForeignKey("dbo.ReleaseGroups", t => t.ReleaseGroupCode)
                .ForeignKey("dbo.ReleaseStrategies", t => new {t.ReleaseGroupCode, t.ReleaseStrategy})
                .Index(t => t.ReleaseGroupCode)
                .Index(t => new {t.ReleaseGroupCode, t.ReleaseStrategy});

            CreateTable(
                    "dbo.AspNetRoles",
                    c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                    "dbo.AspNetUserRoles",
                    c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new {t.UserId, t.RoleId})
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                    "dbo.AspNetUsers",
                    c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                    "dbo.AspNetUserClaims",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);

            CreateTable(
                    "dbo.AspNetUserLogins",
                    c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new {t.LoginProvider, t.ProviderKey, t.UserId})
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ReleaseStrategyLevels", new[] {"ReleaseGroupCode", "ReleaseStrategy"},
                "dbo.ReleaseStrategies");
            DropForeignKey("dbo.ReleaseStrategyLevels", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.ReleaseAuths", "EmpUnqId", "dbo.Employees");
            DropForeignKey("dbo.LeaveBalances", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.LeaveBalances", new[] {"CompCode", "WrkGrp", "LeaveTypeCode"}, "dbo.LeaveTypes");
            DropForeignKey("dbo.LeaveBalances", "EmpUnqId", "dbo.Employees");
            DropForeignKey("dbo.LeaveBalances", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.LeaveApplicationDetails", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.LeaveApplicationDetails", new[] {"CompCode", "WrkGrp", "LeaveTypeCode"},
                "dbo.LeaveTypes");
            DropForeignKey("dbo.LeaveTypes", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.LeaveTypes", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.LeaveApplications", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.LeaveApplications", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.LeaveApplications", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"},
                "dbo.Stations");
            DropForeignKey("dbo.LeaveApplications", new[] {"ReleaseGroupCode", "ReleaseStrategy"},
                "dbo.ReleaseStrategies");
            DropForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"},
                "dbo.Stations");
            DropForeignKey("dbo.ReleaseStrategies", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"},
                "dbo.Departments");
            DropForeignKey("dbo.ReleaseStrategies", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "CatCode"}, "dbo.Categories");
            DropForeignKey("dbo.LeaveApplications", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.LeaveApplications", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropForeignKey("dbo.LeaveApplicationDetails", new[] {"YearMonth", "LeaveAppId"}, "dbo.LeaveApplications");
            DropForeignKey("dbo.LeaveApplications", "EmpUnqId", "dbo.Employees");
            DropForeignKey("dbo.LeaveApplications", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"},
                "dbo.Departments");
            DropForeignKey("dbo.LeaveApplications", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.LeaveApplications", new[] {"CompCode", "WrkGrp", "CatCode"}, "dbo.Categories");
            DropForeignKey("dbo.ApplReleaseStatus",
                new[] {"LeaveApplications_YearMonth", "LeaveApplications_LeaveAppId"}, "dbo.LeaveApplications");
            DropForeignKey("dbo.LeaveApplicationDetails", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"},
                "dbo.Stations");
            DropForeignKey("dbo.Stations", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.Stations", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.Stations", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"}, "dbo.Departments");
            DropForeignKey("dbo.Stations", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp", "GradeCode"}, "dbo.Grades");
            DropForeignKey("dbo.Grades", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.Grades", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp", "EmpTypeCode"}, "dbo.EmpTypes");
            DropForeignKey("dbo.EmpTypes", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.EmpTypes", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp", "DesgCode"}, "dbo.Designations");
            DropForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"}, "dbo.Departments");
            DropForeignKey("dbo.Employees", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.Employees", new[] {"CompCode", "WrkGrp", "CatCode"}, "dbo.Categories");
            DropForeignKey("dbo.Designations", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.Designations", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.Departments", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.Departments", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.Contractors", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.Contractors", new[] {"CompCode", "WrkGrp", "UnitCode"}, "dbo.Units");
            DropForeignKey("dbo.Units", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.Units", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.Contractors", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.Categories", new[] {"CompCode", "WrkGrp"}, "dbo.WorkGroups");
            DropForeignKey("dbo.WorkGroups", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.Categories", "CompCode", "dbo.Companies");
            DropForeignKey("dbo.ApplReleaseStatus", "ReleaseStatusCode", "dbo.ReleaseStatus");
            DropForeignKey("dbo.ApplReleaseStatus", "ReleaseGroupCode", "dbo.ReleaseGroups");
            DropIndex("dbo.AspNetUserLogins", new[] {"UserId"});
            DropIndex("dbo.AspNetUserClaims", new[] {"UserId"});
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] {"RoleId"});
            DropIndex("dbo.AspNetUserRoles", new[] {"UserId"});
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ReleaseStrategyLevels", new[] {"ReleaseGroupCode", "ReleaseStrategy"});
            DropIndex("dbo.ReleaseStrategyLevels", new[] {"ReleaseGroupCode"});
            DropIndex("dbo.ReleaseAuths", new[] {"EmpUnqId"});
            DropIndex("dbo.LeaveBalances", new[] {"EmpUnqId"});
            DropIndex("dbo.LeaveBalances", new[] {"CompCode", "WrkGrp", "LeaveTypeCode"});
            DropIndex("dbo.LeaveBalances", new[] {"CompCode"});
            DropIndex("dbo.LeaveTypes", new[] {"CompCode", "WrkGrp"});
            DropIndex("dbo.LeaveTypes", new[] {"CompCode"});
            DropIndex("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            DropIndex("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            DropIndex("dbo.ReleaseStrategies", new[] {"CompCode", "WrkGrp", "CatCode"});
            DropIndex("dbo.ReleaseStrategies", new[] {"ReleaseGroupCode"});
            DropIndex("dbo.LeaveApplications", new[] {"ReleaseStatusCode"});
            DropIndex("dbo.LeaveApplications", new[] {"ReleaseGroupCode", "ReleaseStrategy"});
            DropIndex("dbo.LeaveApplications", new[] {"ReleaseGroupCode"});
            DropIndex("dbo.LeaveApplications", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            DropIndex("dbo.LeaveApplications", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            DropIndex("dbo.LeaveApplications", new[] {"CompCode", "WrkGrp", "CatCode"});
            DropIndex("dbo.LeaveApplications", new[] {"EmpUnqId"});
            DropIndex("dbo.LeaveApplicationDetails", new[] {"CompCode", "WrkGrp", "LeaveTypeCode"});
            DropIndex("dbo.LeaveApplicationDetails", new[] {"CompCode"});
            DropIndex("dbo.LeaveApplicationDetails", new[] {"YearMonth", "LeaveAppId"});
            DropIndex("dbo.Stations", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            DropIndex("dbo.Stations", new[] {"CompCode"});
            DropIndex("dbo.Grades", new[] {"CompCode", "WrkGrp"});
            DropIndex("dbo.Grades", new[] {"CompCode"});
            DropIndex("dbo.EmpTypes", new[] {"CompCode", "WrkGrp"});
            DropIndex("dbo.EmpTypes", new[] {"CompCode"});
            DropIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode", "StatCode"});
            DropIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "GradeCode"});
            DropIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "EmpTypeCode"});
            DropIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "DesgCode"});
            DropIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "UnitCode", "DeptCode"});
            DropIndex("dbo.Employees", new[] {"CompCode", "WrkGrp", "CatCode"});
            DropIndex("dbo.Designations", new[] {"CompCode", "WrkGrp"});
            DropIndex("dbo.Designations", new[] {"CompCode"});
            DropIndex("dbo.Departments", new[] {"CompCode", "WrkGrp"});
            DropIndex("dbo.Departments", new[] {"CompCode"});
            DropIndex("dbo.Units", new[] {"CompCode", "WrkGrp"});
            DropIndex("dbo.Units", new[] {"CompCode"});
            DropIndex("dbo.Contractors", new[] {"CompCode", "WrkGrp", "UnitCode"});
            DropIndex("dbo.Contractors", new[] {"CompCode"});
            DropIndex("dbo.WorkGroups", new[] {"CompCode"});
            DropIndex("dbo.Categories", new[] {"CompCode", "WrkGrp"});
            DropIndex("dbo.Categories", new[] {"CompCode"});
            DropIndex("dbo.ApplReleaseStatus", new[] {"LeaveApplications_YearMonth", "LeaveApplications_LeaveAppId"});
            DropIndex("dbo.ApplReleaseStatus", new[] {"ReleaseStatusCode"});
            DropIndex("dbo.ApplReleaseStatus", new[] {"ReleaseGroupCode"});
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ReleaseStrategyLevels");
            DropTable("dbo.ReleaseAuths");
            DropTable("dbo.OpenMonths");
            DropTable("dbo.LeaveBalances");
            DropTable("dbo.LeaveTypes");
            DropTable("dbo.ReleaseStrategies");
            DropTable("dbo.LeaveApplications");
            DropTable("dbo.LeaveApplicationDetails");
            DropTable("dbo.Stations");
            DropTable("dbo.Grades");
            DropTable("dbo.EmpTypes");
            DropTable("dbo.Employees");
            DropTable("dbo.Designations");
            DropTable("dbo.Departments");
            DropTable("dbo.Units");
            DropTable("dbo.Contractors");
            DropTable("dbo.WorkGroups");
            DropTable("dbo.Companies");
            DropTable("dbo.Categories");
            DropTable("dbo.ReleaseStatus");
            DropTable("dbo.ReleaseGroups");
            DropTable("dbo.ApplReleaseStatus");
        }
    }
}