using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Hosting;

namespace ESS.Controllers.Api
{
    public class ReleaseStrategyController : ApiController
    {
        private ApplicationDbContext _context;

        public ReleaseStrategyController()
        {
            _context = new ApplicationDbContext();
        }


        [HttpGet]
        public IHttpActionResult GetReleaseStrategy(string releaseGroup, string empUnqId)
        {
            if (releaseGroup == ReleaseGroups.LeaveApplication ||
                releaseGroup == ReleaseGroups.OutStationDuty ||
                releaseGroup == ReleaseGroups.CompOff ||
                releaseGroup == ReleaseGroups.ShiftSchedule ||
                releaseGroup == ReleaseGroups.NoDues)
            {
                var releaseStrDto = _context.ReleaseStrategy
                    .Where(r =>
                        r.ReleaseStrategy == empUnqId &&
                        r.ReleaseGroupCode == releaseGroup
                    ).ToList()
                    .Select(Mapper.Map<ReleaseStrategies, ReleaseStrategyDto>)
                    .FirstOrDefault();


                if (releaseStrDto == null)
                    return BadRequest("Invalid release strategy/not defined.");


                var relStrLvl = _context.ReleaseStrategyLevels
                    .Where(r =>
                        r.ReleaseGroupCode == releaseStrDto.ReleaseGroupCode &&
                        r.ReleaseStrategy == releaseStrDto.ReleaseStrategy
                    ).ToList()
                    .Select(Mapper.Map<ReleaseStrategyLevels, ReleaseStrategyLevelDto>)
                    .ToList();
                foreach (var levelDto in relStrLvl)
                {
                    var relCode = levelDto.ReleaseCode;
                    var releser = _context.ReleaseAuth
                        .FirstOrDefault(r => r.ReleaseCode == relCode && r.Active);

                    if (releser == null)
                        continue;

                    var emp = _context.Employees
                        .Select(e => new EmployeeDto
                        {
                            EmpUnqId = e.EmpUnqId,
                            EmpName = e.EmpName
                        })
                        .Single(e => e.EmpUnqId == releser.EmpUnqId);

                    levelDto.EmpUnqId = emp.EmpUnqId;
                    levelDto.EmpName = emp.EmpName;

                    releaseStrDto.ReleaseStrategyLevels.Add(levelDto);
                }

                return Ok(releaseStrDto);
            }

            if (releaseGroup == ReleaseGroups.GatePass)
            {
                //Get emp details like compcode, wrkgrp....

                var gpEmp = _context.Employees
                    .SingleOrDefault(e => e.EmpUnqId == empUnqId);

                if (gpEmp == null)
                    return BadRequest("Invalid employee");

                var gpReleaseStrDto = _context.GpReleaseStrategy
                    .Where(r =>
                        r.ReleaseGroupCode == releaseGroup &&
                        r.CompCode == gpEmp.CompCode &&
                        r.WrkGrp == gpEmp.WrkGrp &&
                        r.UnitCode == gpEmp.UnitCode &&
                        r.DeptCode == gpEmp.DeptCode &&
                        r.StatCode == gpEmp.StatCode &&
                        r.Active
                    )
                    .Select(Mapper.Map<GpReleaseStrategies, GpReleaseStrategyDto>)
                    .FirstOrDefault();


                var relStrLvl = new List<GpReleaseStrategyLevelDto>();

                if (gpReleaseStrDto != null)
                {
                    relStrLvl = _context.GpReleaseStrategyLevels
                        .Where(r =>
                            r.ReleaseGroupCode == gpReleaseStrDto.ReleaseGroupCode &&
                            r.GpReleaseStrategy == gpReleaseStrDto.GpReleaseStrategy
                        )
                        .Select(Mapper.Map<GpReleaseStrategyLevels, GpReleaseStrategyLevelDto>)
                        .ToList();
                }

                // DAY release strategy

                var gpReleaseStrDayDto = _context.GpReleaseStrategy
                    .Where(r =>
                        r.ReleaseGroupCode == releaseGroup &&
                        r.GpReleaseStrategy == empUnqId &&
                        r.Active
                    )
                    .Select(Mapper.Map<GpReleaseStrategies, GpReleaseStrategyDto>)
                    .FirstOrDefault();


                if (gpReleaseStrDto == null && gpReleaseStrDayDto == null)
                    return BadRequest("Invalid release strategy/not defined.");


                var relStrLvlDay = _context.GpReleaseStrategyLevels
                    .Where(r =>
                        r.ReleaseGroupCode == gpReleaseStrDayDto.ReleaseGroupCode &&
                        r.GpReleaseStrategy == gpReleaseStrDayDto.GpReleaseStrategy
                    )
                    .Select(Mapper.Map<GpReleaseStrategyLevels, GpReleaseStrategyLevelDto>)
                    .ToList();


                relStrLvl.AddRange(relStrLvlDay);

                int count = 0;

                foreach (var levelDto in relStrLvl)
                {
                    var relCode = levelDto.ReleaseCode;
                    var releser = _context.ReleaseAuth
                        .Where(r => r.ReleaseCode == relCode && r.Active)
                        .ToList();

                    if (releser.Count != 0)
                        count += releser.Count;

                    foreach (var r in releser)
                    {
                        var emp = _context.Employees
                            .Select(e => new EmployeeDto
                            {
                                EmpUnqId = e.EmpUnqId,
                                EmpName = e.EmpName
                            })
                            .Single(e => e.EmpUnqId == r.EmpUnqId);

                        GpReleaseStrategyLevelDto relDto =
                            new GpReleaseStrategyLevelDto
                            {
                                ReleaseGroupCode = levelDto.ReleaseGroupCode,
                                GpReleaseStrategy = levelDto.GpReleaseStrategy,
                                GpReleaseStrategyLevel = levelDto.GpReleaseStrategyLevel,
                                ReleaseCode = levelDto.ReleaseCode,
                                IsFinalRelease = levelDto.IsFinalRelease,
                                EmpUnqId = emp.EmpUnqId,
                                EmpName = emp.EmpName,
                                IsGpNightReleaser = r.IsGpNightReleaser
                            };


                        gpReleaseStrDto.GpReleaseStrategyLevels.Add(relDto);
                    }
                }


                if (count == 0)
                    return BadRequest("No one is authorized to release!");

                return Ok(gpReleaseStrDto);
            }

            if (releaseGroup == ReleaseGroups.GatePassAdvice)
            {
                //For gate pass advice

                var releaseStrDto = _context.GaReleaseStrategies
                    .Where(r =>
                        r.GaReleaseStrategy == empUnqId &&
                        r.ReleaseGroupCode == releaseGroup
                    ).ToList()
                    .Select(Mapper.Map<GaReleaseStrategies, GaReleaseStrategyDto>)
                    .FirstOrDefault();


                if (releaseStrDto == null)
                    return BadRequest("Invalid release strategy/not defined.");


                var relStrLvl = _context.GaReleaseStrategyLevels
                    .Where(r =>
                        r.ReleaseGroupCode == releaseStrDto.ReleaseGroupCode &&
                        r.GaReleaseStrategy == releaseStrDto.GaReleaseStrategy
                    ).ToList()
                    .Select(Mapper.Map<GaReleaseStrategyLevels, GaReleaseStrategyLevelDto>)
                    .ToList();

                foreach (var levelDto in relStrLvl)
                {
                    var relCode = levelDto.ReleaseCode;
                    var releser = _context.ReleaseAuth
                        .FirstOrDefault(r => r.ReleaseCode == relCode && r.Active);

                    if (releser == null)
                        return BadRequest("No one is authorized to release!");

                    var emp = _context.Employees
                        .Select(e => new EmployeeDto
                        {
                            EmpUnqId = e.EmpUnqId,
                            EmpName = e.EmpName
                        })
                        .Single(e => e.EmpUnqId == releser.EmpUnqId);

                    levelDto.EmpUnqId = emp.EmpUnqId;
                    levelDto.EmpName = emp.EmpName;

                    releaseStrDto.GaReleaseStrategyLevels.Add(levelDto);
                }

                return Ok(releaseStrDto);
            }
            else
                return
                    BadRequest(
                        "Release strategy group code not found."); //If other that LA/GP is specified, return error
        }


        [HttpGet]
        public IHttpActionResult GetReleaseStrategy(string empUnqId)
        {
            //get employee details
            var emp = _context.Employees.Single(e => e.EmpUnqId == empUnqId);
            if (emp == null)
                return BadRequest("Invalid employee code.");

            //return if employee is not a releaser
            if (!emp.IsReleaser)
                return BadRequest("Employee is not authorized to release (check flag).");

            //if he's a releaser, get his release code
            //and based on the code, get all his release strategy levels

            var relCode = _context.ReleaseAuth.Where(r => r.EmpUnqId == emp.EmpUnqId).ToList();

            //create blank employee list for output
            List<EmployeeDto> employees = new List<EmployeeDto>();


            //loop through all release codes found (ideally it should be only one)
            foreach (var releaseAuth in relCode)
            {
                //find all release strategies to which this code belongs
                var relStrategyLevel = _context.ReleaseStrategyLevels
                    .Include(r => r.ReleaseStrategies)
                    .Where(r => r.ReleaseCode == releaseAuth.ReleaseCode)
                    .ToList();


                var relStrategy = relStrategyLevel.Select(level => level.ReleaseStrategies).ToList();

                //and for each strategy we found above,
                //search for employee who match the release criteria
                foreach (var strategy in relStrategy)
                {
                    var relEmployee = _context.Employees
                        .Where(
                            e =>
                                //e.CompCode == strategy.CompCode &&
                                //e.WrkGrp == strategy.WrkGrp &&
                                //e.UnitCode == strategy.UnitCode &&
                                //e.DeptCode == strategy.DeptCode &&
                                //e.StatCode == strategy.StatCode &&
                                //e.SecCode == strategy.SecCode &&
                                //e.IsHod == strategy.IsHod &&
                                e.EmpUnqId == strategy.ReleaseStrategy &&
                                strategy.Active
                        )
                        .Select(
                            e => new EmployeeDto
                            {
                                EmpUnqId = e.EmpUnqId,
                                EmpName = e.EmpName,
                                FatherName = e.FatherName,
                                Active = e.Active,
                                Pass = e.Pass,

                                CompCode = e.CompCode,
                                WrkGrp = e.WrkGrp,
                                UnitCode = e.UnitCode,
                                DeptCode = e.DeptCode,
                                StatCode = e.StatCode,
                                //SecCode = e.SecCode,
                                CatCode = e.CatCode,
                                EmpTypeCode = e.EmpTypeCode,
                                GradeCode = e.GradeCode,
                                DesgCode = e.DesgCode,


                                CompName = e.Company.CompName,
                                WrkGrpDesc = e.WorkGroup.WrkGrpDesc,
                                UnitName = e.Units.UnitName,
                                DeptName = e.Departments.DeptName,
                                StatName = e.Stations.StatName,
                                //SecName = e.Sections.SecName,
                                CatName = e.Categories.CatName,
                                EmpTypeName = e.EmpTypes.EmpTypeName,
                                GradeName = e.Grades.GradeName,
                                DesgName = e.Designations.DesgName,

                                IsHod = e.IsHod,
                                IsHrUser = e.IsHrUser,
                                IsReleaser = e.IsReleaser,
                                Email = e.Email,

                                Location = e.Location
                            }
                        )
                        .ToList();

                    //add all above employees to our output list
                    employees.AddRange(relEmployee);
                }
            }

            //if there're any employee, return them
            if (employees.Count == 0)
                return BadRequest("No employee found...");

            return Ok(employees);
        }


        private class RelStrUpload
        {
            public string ReleaseGroupCode { get; set; }
            public string ReleaseStrategy { get; set; }
            public string ReleaseStrategyName { get; set; }
            public bool IsHod { get; set; }
            public bool Active { get; set; }
            public DateTime? UpdDt { get; set; }
            public string UpdUser { get; set; }
            public List<RelStrLvlUpload> ReleaseStrategyLevels;
        }

        private class RelStrLvlUpload
        {
            public string ReleaseGroupCode { get; set; }
            public string ReleaseStrategy { get; set; }
            public int ReleaseStrategyLevel { get; set; }
            public string ReleaseCode { get; set; }
            public string ReleaseCode2 { get; set; }
            public string ReleaseCode3 { get; set; }

            public bool IsFinalRelease { get; set; }
        }

        [HttpPost]
        [ActionName("UploadRelStr")]
        public IHttpActionResult UploadRelStr()
        {
            HttpContext httpContext = HttpContext.Current;

            if (httpContext.Request.Files.Count <= 0) return BadRequest("NO FILES???");

            try
            {
                // get folder in temp
                string folder = HostingEnvironment.MapPath(@"~/App_Data/tmp/");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder ?? throw new InvalidOperationException("Folder not found"));

                for (var i = 0; i < httpContext.Request.Files.Count;)
                {
                    // get posted file from http content
                    HttpPostedFile postedFile = httpContext.Request.Files[i];
                    if (postedFile.ContentLength <= 0) return BadRequest("Passed file is null!");

                    try
                    {
                        // must be a csv file
                        string fileExt = Path.GetExtension(postedFile.FileName);
                        if (fileExt != ".csv") return BadRequest("Invalid file extension.");

                        // save received file in temp for later analysis if required
                        postedFile.SaveAs(
                            HostingEnvironment.MapPath(@"~/App_Data/tmp/RelStr") + "-" +
                            DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv");

                        // open stream
                        using (var reader = new StreamReader(postedFile.InputStream))
                        {
                            // This will just rip off the first Header line off the excel file
                            string[] header = reader.ReadLine()?.Split(',');
                            if (header == null)
                                return BadRequest("Header is null!!");

                            if (header.Length != 6)
                                return BadRequest("Invalid template format. Pl don't change column order");

                            // list of all release strategy records
                            var relStrInputList = new List<RelStrUpload>();

                            // single one
                            var relStrInput = new RelStrUpload();

                            while (!reader.EndOfStream)
                            {
                                // read a line
                                var row = reader.ReadLine()?.Split(',');
                                if (row == null) continue;

                                // read individual columns
                                if (row[0] != ReleaseGroups.LeaveApplication &&
                                    row[0] != ReleaseGroups.OutStationDuty)
                                    return BadRequest("Only LA/OD allowed.");

                                //check if employee is same as previous line 
                                //if not, create new release strategy record

                                if (row[1] != relStrInput.ReleaseStrategy)
                                {
                                    if (relStrInput.ReleaseStrategy != null)
                                        relStrInputList.Add(relStrInput);

                                    relStrInput = new RelStrUpload
                                    {
                                        ReleaseGroupCode = row[0],
                                        ReleaseStrategy = row[1],
                                        ReleaseStrategyName = row[1],
                                        IsHod = false,
                                        Active = true,
                                        UpdDt = DateTime.Now,
                                        UpdUser = "Admin" //TODO: REPLACE WITH ACTUAL USER
                                    };
                                }

                                var relStrLevel = new RelStrLvlUpload
                                {
                                    ReleaseGroupCode = row[0],
                                    ReleaseStrategy = row[1],
                                    ReleaseStrategyLevel = int.Parse(row[2]),
                                    ReleaseCode = row[3],
                                    ReleaseCode2 = row[4],
                                    ReleaseCode3 = row[5]
                                };

                                if (relStrInput.ReleaseStrategyLevels == null)
                                    relStrInput.ReleaseStrategyLevels = new List<RelStrLvlUpload>();

                                relStrInput.ReleaseStrategyLevels.Add(relStrLevel);
                            }

                            relStrInputList.Add(relStrInput);

                            // release strategy list is ready
                            // now validate input

                            if (relStrInputList.Any())
                            {
                                var errors = new List<string>();

                                foreach (RelStrUpload relStr in relStrInputList)
                                    errors.AddRange(ValidateRelStr(relStr));

                                if (errors.Count > 0)
                                    return Content(HttpStatusCode.BadRequest, errors);

                                // VALIDATION OK
                                // TIME TO UPLOAD REL STR

                                try
                                {
                                    var uploaded = UploadRelStr(relStrInputList);
                                    return Ok();
                                }
                                catch (Exception ex)
                                {
                                    return BadRequest("Error: " + ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Error:" + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            return Ok();
        }

        private IEnumerable<string> ValidateRelStr(RelStrUpload relStr)
        {
            var errors = new List<string>();

            // check 1
            // see if employee exists and is active
            Employees emp = _context.Employees.FirstOrDefault(e => e.EmpUnqId == relStr.ReleaseStrategy);
            if (emp == null || emp.Active == false)
            {
                errors.Add("Employee " + relStr.ReleaseStrategy + " not found or is inactive.");
                return errors;
            }
            // check 1 over


            // check 2
            // check for release strategy levels.
            // should be in sequence properly
            // set is final release in last one if not already.
            var i = 1;

            var allReleaseCodes = new List<string>();

            foreach (RelStrLvlUpload levels in relStr.ReleaseStrategyLevels)
            {
                if (levels.ReleaseStrategyLevel != i)
                {
                    errors.Add("Check release strategy levels for " + relStr.ReleaseStrategy);
                    return errors;
                }

                i++;

                if (levels.ReleaseStrategyLevel == relStr.ReleaseStrategyLevels.Max(r => r.ReleaseStrategyLevel))
                    levels.IsFinalRelease = true;

                // check 3
                // check for release code employees 
                // should be available and active

                if (string.IsNullOrEmpty(levels.ReleaseCode))
                {
                    errors.Add("Release code 1 must not be blank.");
                    return errors;
                }

                var rel1 = _context.Employees.FirstOrDefault(e => e.EmpUnqId == levels.ReleaseCode);
                if (rel1 == null || rel1.Active == false)
                    errors.Add("Release one employee not found or inactive.");

                allReleaseCodes.Add(levels.ReleaseCode);

                if (!string.IsNullOrEmpty(levels.ReleaseCode2))
                {
                    var rel2 = _context.Employees.FirstOrDefault(e => e.EmpUnqId == levels.ReleaseCode2);
                    if (rel2 == null || rel2.Active == false)
                        errors.Add("Release2 employee not found or inactive.");

                    allReleaseCodes.Add(levels.ReleaseCode2);
                }

                if (!string.IsNullOrEmpty(levels.ReleaseCode3))
                {
                    var rel3 = _context.Employees.FirstOrDefault(e => e.EmpUnqId == levels.ReleaseCode3);
                    if (rel3 == null || rel3.Active == false)
                        errors.Add("Release3 employee not found or inactive.");

                    allReleaseCodes.Add(levels.ReleaseCode3);
                }

                // check 3 over
            }
            // check 2 over
            // lstNames.GroupBy(n => n).Any(c => c.Count() > 1);

            if (allReleaseCodes.GroupBy(n => n).Any(c => c.Count() > 1))
            {
                errors.Add("Duplicate release codes not allowed. Check upload file.");
            }

            // ADD ANY MORE CHECKS REQUIRED HERE....

            return errors;
        }

        // UPLOAD RELEASE STRATEGY
        // INCOMING DATA SHOULD BE VALIDATED AND ERROR FREE
        private List<RelStrUpload> UploadRelStr(List<RelStrUpload> relStrUploadList)
        {
            try
            {
                // create transaction, as so many updates will take place
                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    // LOOP ON EACH RELEASE STRATEGY HEADER
                    foreach (RelStrUpload strategy in relStrUploadList)
                    {
                        //
                        // 0. CHECK IF RELEASE STRATEGY EXIST, IF NOT CREATE NEW
                        //

                        var existingRelStr = _context.ReleaseStrategy
                            .FirstOrDefault(r =>
                                r.ReleaseGroupCode == strategy.ReleaseGroupCode &&
                                r.ReleaseStrategy == strategy.ReleaseStrategy);
                        // if not existing, create new
                        if (existingRelStr == null)
                        {
                            _context.ReleaseStrategy.Add(new ReleaseStrategies
                            {
                                ReleaseGroupCode = strategy.ReleaseGroupCode,
                                ReleaseStrategy = strategy.ReleaseStrategy,
                                ReleaseStrategyName = strategy.ReleaseStrategyName,
                                IsHod = false,
                                Active = true,
                                UpdDt = DateTime.Now,
                                UpdUser = "Admin"
                            });
                        }
                        else
                        {
                            existingRelStr.Active = true;
                            existingRelStr.UpdDt = DateTime.Now;
                            existingRelStr.UpdUser = "Admin";
                        }

                        _context.SaveChanges();
                        //


                        //
                        // 1. DELETE ALL EXISTING REL STRATEGY LEVELS 
                        //

                        var existingRelStrLevels = _context.ReleaseStrategyLevels
                            .Where(r => r.ReleaseGroupCode == strategy.ReleaseGroupCode &&
                                        r.ReleaseStrategy == strategy.ReleaseStrategy)
                            .ToList();

                        _context.ReleaseStrategyLevels.RemoveRange(existingRelStrLevels);
                        _context.SaveChanges();


                        //
                        // 2. CHECK AVAILABILITY OF RELEASE CODE
                        //

                        foreach (RelStrLvlUpload level in strategy.ReleaseStrategyLevels)
                        {
                            // create array of all release codes
                            var releaseCodes = new List<string>
                            {
                                level.ReleaseCode,
                                level.ReleaseCode2,
                                level.ReleaseCode3
                            };
                            // remove all null values
                            releaseCodes.RemoveAll(string.IsNullOrEmpty);
                            releaseCodes.Sort();

                            string newReleaseCode = string.Join("/", releaseCodes.ToArray());

                            // check if release code is available
                            ReleaseAuth relCode =
                                _context.ReleaseAuth.FirstOrDefault(r => r.ReleaseCode == newReleaseCode);

                            //
                            // 3. IF NOT FOUND, CREATE IN RELEASE AUTH TABLE
                            //
                            if (relCode == null)
                            {
                                _context.ReleaseAuth.Add(new ReleaseAuth
                                {
                                    ReleaseCode = newReleaseCode,
                                    EmpUnqId = level.ReleaseCode,
                                    ValidFrom = DateTime.Now.Date,
                                    ValidTo = new DateTime(2030, 12, 31),
                                    Active = true,
                                    IsGpNightReleaser = false
                                });

                                if (!string.IsNullOrEmpty(level.ReleaseCode2))
                                {
                                    _context.ReleaseAuth.Add(new ReleaseAuth
                                    {
                                        ReleaseCode = newReleaseCode,
                                        EmpUnqId = level.ReleaseCode2,
                                        ValidFrom = DateTime.Now.Date,
                                        ValidTo = new DateTime(2030, 12, 31),
                                        Active = true,
                                        IsGpNightReleaser = false
                                    });
                                }

                                if (!string.IsNullOrEmpty(level.ReleaseCode3))
                                {
                                    _context.ReleaseAuth.Add(new ReleaseAuth
                                    {
                                        ReleaseCode = newReleaseCode,
                                        EmpUnqId = level.ReleaseCode3,
                                        ValidFrom = DateTime.Now.Date,
                                        ValidTo = new DateTime(2030, 12, 31),
                                        Active = true,
                                        IsGpNightReleaser = false
                                    });
                                }

                                _context.SaveChanges();
                            }

                            // CHECK 3 OVER

                            // CHECK 2 OVER

                            //
                            // CHECK 4 check if user is authorized for release role
                            //
                            RoleUsers relRole = _context.RoleUser
                                .FirstOrDefault(e =>
                                    e.EmpUnqId == level.ReleaseCode &&
                                    e.RoleId == 2); //TODO: REMOVE MAGIC NUMBER 2

                            // if not, add authorization role
                            if (relRole == null)
                            {
                                _context.RoleUser.Add(new RoleUsers
                                {
                                    RoleId = 2,
                                    EmpUnqId = level.ReleaseCode,
                                    UpdateUserId = "Admin",
                                    UpdateDate = DateTime.Now
                                });
                                _context.SaveChanges();
                            }


                            // 2nd and 3rd releasers
                            if (!string.IsNullOrEmpty(level.ReleaseCode2))
                            {
                                relRole = _context.RoleUser
                                    .FirstOrDefault(e =>
                                        e.EmpUnqId == level.ReleaseCode2 &&
                                        e.RoleId == 2); //TODO: REMOVE MAGIC NUMBER 2

                                if (relRole == null)
                                {
                                    _context.RoleUser.Add(new RoleUsers
                                    {
                                        RoleId = 2,
                                        EmpUnqId = level.ReleaseCode2,
                                        UpdateUserId = "Admin",
                                        UpdateDate = DateTime.Now
                                    });
                                    _context.SaveChanges();
                                }
                            }

                            if (!string.IsNullOrEmpty(level.ReleaseCode3))
                            {
                                relRole = _context.RoleUser
                                    .FirstOrDefault(e =>
                                        e.EmpUnqId == level.ReleaseCode3 &&
                                        e.RoleId == 2); //TODO: REMOVE MAGIC NUMBER 2

                                // if not, add authorization role
                                if (relRole == null)
                                {
                                    _context.RoleUser.Add(new RoleUsers
                                    {
                                        RoleId = 2,
                                        EmpUnqId = level.ReleaseCode3,
                                        UpdateUserId = "Admin",
                                        UpdateDate = DateTime.Now
                                    });
                                    _context.SaveChanges();
                                }
                            }

                            // CHECK 4 OVER

                            _context.ReleaseStrategyLevels.Add(new ReleaseStrategyLevels
                            {
                                ReleaseGroupCode = level.ReleaseGroupCode,
                                ReleaseStrategy = level.ReleaseStrategy,
                                ReleaseStrategyLevel = level.ReleaseStrategyLevel,
                                ReleaseCode = newReleaseCode,
                                IsFinalRelease = level.IsFinalRelease
                            });

                            _context.SaveChanges();
                        } // RELEASE STRATEGY LEVEL LOOP OVER

                        //
                        // 6 CHECK IN LEAVE APPS FOR PENDING LEAVE APPLICATIONS OF EMPLOYEE
                        //

                        var leaveApp = _context.LeaveApplications
                            .Where(l => l.ReleaseGroupCode == strategy.ReleaseGroupCode &&
                                        l.ReleaseStrategy == strategy.ReleaseStrategy &&
                                        (l.ReleaseStatusCode == ReleaseStatus.NotReleased ||
                                         l.ReleaseStatusCode == ReleaseStatus.PartiallyReleased))
                            .ToList();

                        if (leaveApp.Count > 0)
                        {
                            foreach (LeaveApplications application in leaveApp)
                            {
                                // DELETE ALL APP RELEASE STATUS RECORDS FOR THIS LEAVE APP
                                var appRelList = _context.ApplReleaseStatus
                                    .Where(l => l.ReleaseGroupCode == strategy.ReleaseGroupCode &&
                                                l.ApplicationId == application.LeaveAppId)
                                    .ToList();
                                _context.ApplReleaseStatus.RemoveRange(appRelList);

                                _context.SaveChanges();

                                // CREATE NEW APP RELEASE STATUS RECORDS BASED ON CHANGED REL STRATEGY
                                foreach (var level in strategy.ReleaseStrategyLevels)
                                {
                                    var releaseCodes = new List<string>
                                    {
                                        level.ReleaseCode,
                                        level.ReleaseCode2,
                                        level.ReleaseCode3
                                    };
                                    // remove all null values
                                    releaseCodes.RemoveAll(string.IsNullOrEmpty);
                                    releaseCodes.Sort();

                                    string newReleaseCode = string.Join("/", releaseCodes.ToArray());

                                    _context.ApplReleaseStatus.Add(new ApplReleaseStatus
                                    {
                                        YearMonth = application.YearMonth,
                                        ReleaseGroupCode = application.ReleaseGroupCode,
                                        ApplicationId = application.LeaveAppId,
                                        ReleaseStrategy = strategy.ReleaseStrategy,
                                        ReleaseStrategyLevel = level.ReleaseStrategyLevel,
                                        ReleaseCode = newReleaseCode,
                                        ReleaseStatusCode = level.ReleaseStrategyLevel == 1
                                            ? ReleaseStatus.InRelease
                                            : ReleaseStatus.NotReleased,
                                        ReleaseAuth = level.ReleaseCode,
                                        IsFinalRelease = level.IsFinalRelease,
                                    });

                                    _context.SaveChanges();
                                }

                                application.ReleaseStatusCode = ReleaseStatus.NotReleased;
                                _context.SaveChanges();
                            }
                        }

                        // CHECK 6 OVER
                    } // RELEASE STRATEGY MAIN LOOP OVER

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return relStrUploadList;
        }

        [HttpPost]
        [ActionName("uploadgprel")]
        public IHttpActionResult UploadGpRelStr()
        {
            HttpContext httpContext = HttpContext.Current;

            if (httpContext.Request.Files.Count <= 0) return BadRequest("NO FILES???");

            try
            {
                // get folder in temp
                string folder = HostingEnvironment.MapPath(@"~/App_Data/tmp/");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder ?? throw new InvalidOperationException("Folder not found"));

                for (var i = 0; i < httpContext.Request.Files.Count;)
                {
                    // get posted file from http content
                    HttpPostedFile postedFile = httpContext.Request.Files[i];
                    if (postedFile.ContentLength <= 0) return BadRequest("Passed file is null!");

                    try
                    {
                        // must be a csv file
                        string fileExt = Path.GetExtension(postedFile.FileName);
                        if (fileExt != ".csv") return BadRequest("Invalid file extension.");

                        // save received file in temp for later analysis if required
                        postedFile.SaveAs(
                            HostingEnvironment.MapPath(@"~/App_Data/tmp/RelStr") + "-" +
                            DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv");

                        // open stream
                        using (var reader = new StreamReader(postedFile.InputStream))
                        {
                            // This will just rip off the first Header line off the excel file
                            string[] header = reader.ReadLine()?.Split(',');
                            if (header == null)
                                return BadRequest("Header is null!!");

                            if (header.Length != 6)
                                return BadRequest("Invalid template format. Pl don't change column order");

                            // list of all release strategy records
                            var relStrInputList = new List<RelStrUpload>();

                            // single one
                            var relStrInput = new RelStrUpload();

                            while (!reader.EndOfStream)
                            {
                                // read a line
                                var row = reader.ReadLine()?.Split(',');
                                if (row == null) continue;

                                // read individual columns
                                if (row[0] != ReleaseGroups.GatePass)
                                    return BadRequest("Only GP allowed.");

                                //check if employee is same as previous line 
                                //if not, create new release strategy record

                                if (row[1] != relStrInput.ReleaseStrategy)
                                {
                                    if (relStrInput.ReleaseStrategy != null)
                                        relStrInputList.Add(relStrInput);

                                    relStrInput = new RelStrUpload
                                    {
                                        ReleaseGroupCode = row[0],
                                        ReleaseStrategy = row[1],
                                        ReleaseStrategyName = row[1],
                                        IsHod = false,
                                        Active = true,
                                        UpdDt = DateTime.Now,
                                        UpdUser = "Admin" //TODO: REPLACE WITH ACTUAL USER
                                    };
                                }

                                var relStrLevel = new RelStrLvlUpload
                                {
                                    ReleaseGroupCode = row[0],
                                    ReleaseStrategy = row[1],
                                    ReleaseStrategyLevel = int.Parse(row[2]),
                                    ReleaseCode = row[3],
                                    ReleaseCode2 = row[4],
                                    ReleaseCode3 = row[5]
                                };

                                if (relStrInput.ReleaseStrategyLevels == null)
                                    relStrInput.ReleaseStrategyLevels = new List<RelStrLvlUpload>();

                                relStrInput.ReleaseStrategyLevels.Add(relStrLevel);
                            }

                            relStrInputList.Add(relStrInput);

                            // release strategy list is ready
                            // now validate input

                            if (relStrInputList.Any())
                            {
                                var errors = new List<string>();

                                // should be no duplicate emp list 
                                List<string> dup = relStrInputList
                                    .GroupBy(e => e.ReleaseStrategy)
                                    .Where(g => g.Count() > 1)
                                    .Select(g => g.Key)
                                    .ToList();


                                if (dup.Count > 0)
                                    errors.Add("List contains duplicate records.");
                                else
                                {
                                    foreach (RelStrUpload relStr in relStrInputList)
                                        errors.AddRange(ValidateGpRelStr(relStr));
                                }

                                if (errors.Count > 0)
                                    return Content(HttpStatusCode.BadRequest, errors);

                                // VALIDATION OK
                                // TIME TO UPLOAD REL STR

                                try
                                {
                                    var uploaded = UploadGpRelStr(relStrInputList);
                                    return Ok();
                                }
                                catch (Exception ex)
                                {
                                    return BadRequest("Error: " + ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Error:" + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex);
            }

            return Ok();
        }

        private IEnumerable<string> ValidateGpRelStr(RelStrUpload relStr)
        {
            var errors = new List<string>();

            // check 1
            // see if employee exists and is active
            Employees emp = _context.Employees.FirstOrDefault(e => e.EmpUnqId == relStr.ReleaseStrategy);
            if (emp == null || emp.Active == false)
            {
                errors.Add("Employee " + relStr.ReleaseStrategy + " not found or is inactive.");
                return errors;
            }
            // check 1 over


            // check 2
            // check for release strategy levels.
            // should be in sequence properly
            // set is final release in last one if not already.

            var allReleaseCodes = new List<string>();

            foreach (RelStrLvlUpload levels in relStr.ReleaseStrategyLevels)
            {
                if (levels.ReleaseStrategyLevel != 1)
                {
                    errors.Add("Release strategy level should be single only, check for " + relStr.ReleaseStrategy);
                    return errors;
                }

                // check 3
                // check for release code employees 
                // should be available and active

                if (string.IsNullOrEmpty(levels.ReleaseCode))
                {
                    errors.Add("Release code 1 must not be blank.");
                    return errors;
                }

                var rel1 = _context.Employees.FirstOrDefault(e => e.EmpUnqId == levels.ReleaseCode);
                if (rel1 == null || rel1.Active == false)
                    errors.Add("Release one employee not found or inactive.");

                allReleaseCodes.Add(levels.ReleaseCode);

                if (!string.IsNullOrEmpty(levels.ReleaseCode2))
                {
                    var rel2 = _context.Employees.FirstOrDefault(e => e.EmpUnqId == levels.ReleaseCode2);
                    if (rel2 == null || rel2.Active == false)
                        errors.Add("Release2 employee not found or inactive.");

                    allReleaseCodes.Add(levels.ReleaseCode2);
                }

                if (!string.IsNullOrEmpty(levels.ReleaseCode3))
                {
                    var rel3 = _context.Employees.FirstOrDefault(e => e.EmpUnqId == levels.ReleaseCode3);
                    if (rel3 == null || rel3.Active == false)
                        errors.Add("Release3 employee not found or inactive.");

                    allReleaseCodes.Add(levels.ReleaseCode3);
                }


                // check 3 over
            }
            // check 2 over
            // lstNames.GroupBy(n => n).Any(c => c.Count() > 1);

            if (allReleaseCodes.GroupBy(n => n).Any(c => c.Count() > 1))
            {
                errors.Add("Duplicate release codes not allowed. Check upload file.");
            }

            // ADD ANY MORE CHECKS REQUIRED HERE....

            return errors;
        }

        // UPLOAD GP RELEASE STRATEGY
        private List<RelStrUpload> UploadGpRelStr(List<RelStrUpload> relStrUploadList)
        {
            try
            {
                // create transaction, as so many updates will take place
                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    // LOOP ON EACH RELEASE STRATEGY HEADER
                    foreach (RelStrUpload strategy in relStrUploadList)
                    {
                        //
                        // 0. CHECK IF RELEASE STRATEGY EXIST, IF NOT CREATE NEW
                        //

                        Employees emp = _context.Employees.FirstOrDefault(e => e.EmpUnqId == strategy.ReleaseStrategy);
                        if (emp == null) continue;

                        var existingRelStr = _context.GpReleaseStrategy
                            .FirstOrDefault(r =>
                                r.ReleaseGroupCode == strategy.ReleaseGroupCode &&
                                r.GpReleaseStrategy == strategy.ReleaseStrategy);
                        // if not existing, create new
                        if (existingRelStr == null)
                        {
                            _context.GpReleaseStrategy.Add(new GpReleaseStrategies
                            {
                                ReleaseGroupCode = strategy.ReleaseGroupCode,
                                GpReleaseStrategy = strategy.ReleaseStrategy,
                                GpReleaseStrategyName = strategy.ReleaseStrategyName,
                                CompCode = emp.CompCode,
                                WrkGrp = emp.WrkGrp,
                                UnitCode = emp.UnitCode,
                                DeptCode = emp.DeptCode,
                                StatCode = emp.StatCode,
                                Active = true,
                                UpdDt = DateTime.Now,
                                UpdUser = "Admin"
                            });
                        }
                        else
                        {
                            existingRelStr.CompCode = emp.CompCode;
                            existingRelStr.WrkGrp = emp.WrkGrp;
                            existingRelStr.UnitCode = emp.UnitCode;
                            existingRelStr.DeptCode = emp.DeptCode;
                            existingRelStr.StatCode = emp.StatCode;
                            existingRelStr.Active = true;
                            existingRelStr.UpdDt = DateTime.Now;
                            existingRelStr.UpdUser = "Admin";
                        }

                        _context.SaveChanges();
                        //


                        //
                        // 1. DELETE ALL EXISTING REL STRATEGY LEVELS 
                        //

                        var existingRelStrLevels = _context.GpReleaseStrategyLevels
                            .Where(r => r.ReleaseGroupCode == strategy.ReleaseGroupCode &&
                                        r.GpReleaseStrategy == strategy.ReleaseStrategy)
                            .ToList();

                        _context.GpReleaseStrategyLevels.RemoveRange(existingRelStrLevels);
                        _context.SaveChanges();

                        //
                        // 2. CHECK AVAILABILITY OF RELEASE CODE
                        //


                        // there should be only single level, but we'll have to loop
                        foreach (RelStrLvlUpload level in strategy.ReleaseStrategyLevels)
                        {
                            // create array of all releasers
                            var releaseCodes = new List<string>
                            {
                                level.ReleaseCode,
                                level.ReleaseCode2,
                                level.ReleaseCode3
                            };
                            // remove all null values
                            releaseCodes.RemoveAll(string.IsNullOrEmpty);
                            releaseCodes.Sort();

                            string newReleaseCode = "GP_" + strategy.ReleaseStrategy;

                            foreach (string code in releaseCodes)
                            {
                                // check if release code is available
                                ReleaseAuth relCode =
                                    _context.ReleaseAuth.FirstOrDefault(r => 
                                        r.ReleaseCode == newReleaseCode &&
                                        r.EmpUnqId == code);

                                //
                                // 3. IF NOT FOUND, CREATE IN RELEASE AUTH TABLE
                                //
                                if (relCode != null) continue;

                                _context.ReleaseAuth.Add(new ReleaseAuth
                                {
                                    ReleaseCode = newReleaseCode,
                                    EmpUnqId = code,
                                    ValidFrom = DateTime.Now.Date,
                                    ValidTo = new DateTime(2030, 12, 31),
                                    Active = true,
                                    IsGpNightReleaser = false
                                });
                                _context.SaveChanges();

                                // CHECK 3 OVER
                            }


                            // CHECK 2 OVER

                            //
                            // CHECK 4 check if user is authorized for release role
                            //
                            RoleUsers relRole = _context.RoleUser
                                .FirstOrDefault(e =>
                                    e.EmpUnqId == level.ReleaseCode &&
                                    e.RoleId == 7); //TODO: REMOVE MAGIC NUMBER 2

                            // if not, add authorization role
                            if (relRole == null)
                            {
                                _context.RoleUser.Add(new RoleUsers
                                {
                                    RoleId = 7,
                                    EmpUnqId = level.ReleaseCode,
                                    UpdateUserId = "Admin",
                                    UpdateDate = DateTime.Now
                                });
                                _context.SaveChanges();
                            }


                            // 2nd and 3rd releasers
                            if (!string.IsNullOrEmpty(level.ReleaseCode2))
                            {
                                relRole = _context.RoleUser
                                    .FirstOrDefault(e =>
                                        e.EmpUnqId == level.ReleaseCode2 &&
                                        e.RoleId == 7); //TODO: REMOVE MAGIC NUMBER 2

                                if (relRole == null)
                                {
                                    _context.RoleUser.Add(new RoleUsers
                                    {
                                        RoleId = 7,
                                        EmpUnqId = level.ReleaseCode2,
                                        UpdateUserId = "Admin",
                                        UpdateDate = DateTime.Now
                                    });
                                    _context.SaveChanges();
                                }
                            }

                            if (!string.IsNullOrEmpty(level.ReleaseCode3))
                            {
                                relRole = _context.RoleUser
                                    .FirstOrDefault(e =>
                                        e.EmpUnqId == level.ReleaseCode3 &&
                                        e.RoleId == 7); //TODO: REMOVE MAGIC NUMBER 2

                                // if not, add authorization role
                                if (relRole == null)
                                {
                                    _context.RoleUser.Add(new RoleUsers
                                    {
                                        RoleId = 7,
                                        EmpUnqId = level.ReleaseCode3,
                                        UpdateUserId = "Admin",
                                        UpdateDate = DateTime.Now
                                    });
                                    _context.SaveChanges();
                                }
                            }

                            // CHECK 4 OVER

                            _context.GpReleaseStrategyLevels.Add(new GpReleaseStrategyLevels
                            {
                                ReleaseGroupCode = level.ReleaseGroupCode,
                                GpReleaseStrategy = level.ReleaseStrategy,
                                GpReleaseStrategyLevel = level.ReleaseStrategyLevel,
                                ReleaseCode = newReleaseCode,
                                IsFinalRelease = level.IsFinalRelease
                            });

                            _context.SaveChanges();
                        } // RELEASE STRATEGY LEVEL LOOP OVER
                    } // RELEASE STRATEGY MAIN LOOP OVER

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return relStrUploadList;
        }
    }
}