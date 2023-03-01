using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<WorkGroups> WorkGroups { get; set; }
        public DbSet<Units> Units { get; set; }
        public DbSet<Designations> Designations { get; set; }
        public DbSet<Grades> Grades { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<Stations> Stations { get; set; }
        public DbSet<Sections> Sections { get; set; }
        public DbSet<Contractors> Contractors { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<EmpTypes> EmpTypes { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<LeaveTypes> LeaveTypes { get; set; }
        public DbSet<LeaveBalance> LeaveBalance { get; set; }
        public DbSet<ReleaseStatus> ReleaseStatus { get; set; }
        public DbSet<ReleaseGroups> ReleaseGroups { get; set; }
        public DbSet<ReleaseStrategies> ReleaseStrategy { get; set; }
        public DbSet<ReleaseStrategyLevels> ReleaseStrategyLevels { get; set; }
        public DbSet<ReleaseAuth> ReleaseAuth { get; set; }
        public DbSet<LeaveApplications> LeaveApplications { get; set; }
        public DbSet<LeaveApplicationDetails> LeaveApplicationDetails { get; set; }
        public DbSet<ApplReleaseStatus> ApplReleaseStatus { get; set; }
        public DbSet<OpenMonth> OpenMonth { get; set; }
        public DbSet<LeaveRules> LeaveRules { get; set; }
        public DbSet<EmpUniform> EmpUniform { get; set; }
        public DbSet<GatePass> GatePass { get; set; }
        public DbSet<EmpAddress> EmpAddress { get; set; }
        public DbSet<GpReleaseStrategies> GpReleaseStrategy { get; set; }
        public DbSet<GpReleaseStrategyLevels> GpReleaseStrategyLevels { get; set; }
        public DbSet<Locations> Location { get; set; }
        public DbSet<TaxDeclarations> TaxDeclarations { get; set; }
        public DbSet<TaxDetailsInsurance> TaxDetailsInsurances { get; set; }
        public DbSet<TaxDetailsMutualFunds> TaxDetailsMutualFundz { get; set; }
        public DbSet<TaxDetailsNsc> TaxDetailsNscs { get; set; }
        public DbSet<TaxDetailsPpf> TaxDetailsPpfs { get; set; }
        public DbSet<TaxConfig> TaxConfig { get; set; }
        public DbSet<Banks> Banks { get; set; }
        public DbSet<TaxDetailsUlip> TaxDetailsUlips { get; set; }
        public DbSet<TaxDetailsSukanya> TaxDetailsSukanyas { get; set; }
        public DbSet<TaxDetailsBankDeposit> TaxDetailsBankDeposits { get; set; }
        public DbSet<TaxDetailsRent> TaxDetailRents { get; set; }
        public DbSet<TaxDeclarationHistory> TaxDeclarationHistories { get; set; }
        public DbSet<Roles> Role { get; set; }
        public DbSet<RoleAuth> RoleAuths { get; set; }
        public DbSet<RoleUsers> RoleUser { get; set; }
        public DbSet<GpAdvices> GpAdvices { get; set; }
        public DbSet<GpAdviceDetails> GpAdviceDetails { get; set; }
        public DbSet<GaReleaseStrategies> GaReleaseStrategies { get; set; }
        public DbSet<GaReleaseStrategyLevels> GaReleaseStrategyLevels { get; set; }
        public DbSet<Materials> Materials { get; set; }
        public DbSet<Vendors> Vendors { get; set; }
        public DbSet<Shifts> Shifts { get; set; }
        public DbSet<ShiftSchedules> ShiftSchedules { get; set; }
        public DbSet<ShiftScheduleDetails> ShiftScheduleDetails { get; set; }
        public DbSet<SsOpenMonth> SsOpenMonth { get; set; }
        public DbSet<Reimbursements> Reimbursement { get; set; }
        public DbSet<ReimbConv> ReimbConvs { get; set; }
        public DbSet<MedPolicy> MedPolicies { get; set; }
        public DbSet<MedDependent> MedDependents { get; set; }
        public DbSet<MedEmpUhid> MedEmpUhids { get; set; }
        public DbSet<MedicalFitness> MedicalFitness { get; set; }
        public DbSet<MedIntimation> MedIntimations { get; set; }

        public DbSet<NoDuesMaster> NoDuesMaster { get; set; }
        public DbSet<NoDuesStatus> NoDuesStatus { get; set; }
        public DbSet<NoDuesCreators> NoDuesCreator { get; set; }
        public DbSet<NoDuesReleasers> NoDuesReleaser { get; set; }
        public DbSet<NoDuesReleaseStatus> NoDuesReleaseStatus { get; set; }
        public DbSet<NoDuesDeptList> NoDuesDeptList { get; set; }
        public DbSet<NoDuesDept> NoDuesDept { get; set; }
        public DbSet<NoDuesDeptDetails> NoDuesDeptDetails { get; set; }
        public DbSet<NoDuesUnitHead> NoDuesUnitHead { get; set; }

        public DbSet<Requests> Requests { get; set; }
        public DbSet<RequestDetails> RequestDetails { get; set; }
        public DbSet<RequestRelease> RequestReleases { get; set; }

        public DbSet<EmpSeparation> EmpSeparations { get; set; }

        public DbSet<MissedPunch> MissedPunches { get; set; }
        public DbSet<MissedPunchReleaseStatus> MissedPunchReleaseStatus { get; set; }

        public DbSet<FullAndFinal> FullAndFinals { get; set; }

        public DbSet<TpaSanction> TpaSanctions { get; set; }
        public DbSet<TpaRelease> TpaReleases { get; set; }

        public DbSet<Vaccination> Vaccinations { get; set; }

        public DbSet<PlCheckLog> PlCheckLogs { get; set; }

        public DbSet<GradeReviews> GradeReviews { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<ReviewDetails> ReviewDetails { get; set; }

        public DbSet<SapUserIds> SapUserIds { get; set; }

        public DbSet<AddressProof> AddressProofs { get; set; }

        public DbSet<VehicleReq> VehicleReqs { get; set; }

        // NEW TABLE FOR NEW LOGIC FOR LEAVE RULE CHECKS
        public DbSet<LeaveChecks> LeaveChecks { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }


        //Changes for Auditing... 
        public override int SaveChanges()
        {
            List<string> listChanges = new List<string>();
            //List<string> listTable = new List<string>();

            //var objectStateManager = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager;
            //IEnumerable<ObjectStateEntry> changes =
            //    objectStateManager.GetObjectStateEntries(EntityState.Modified | EntityState.Unchanged);


            //foreach (var stateEntry in changes)
            //{
            //    var modifiedProperties = stateEntry.GetModifiedProperties();

            //    foreach (var property in modifiedProperties)
            //    {
            //        if (Convert.ToString(stateEntry.OriginalValues[property]) !=
            //            Convert.ToString(stateEntry.CurrentValues[property]))
            //        {
            //            listTable.Add(stateEntry.EntityKey.EntitySetName);
            //            listChanges.Add(property + " From " + Convert.ToString(stateEntry.OriginalValues[property]) +
            //                            " to " + Convert.ToString(stateEntry.CurrentValues[property]));
            //        }
            //    }
            //}


            var changes = ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged);

            foreach (var change in changes)
            {
                //Here you can have name of models that needs history

                if (change.Entity.GetType().Name != "Employees") continue;

                if (change.State != EntityState.Modified) continue;

                var originalVal = change.OriginalValues;
                var currVal = change.CurrentValues;

                listChanges.AddRange(
                    from propertyName in originalVal.PropertyNames
                    let original = originalVal[propertyName] == null ? "" : originalVal[propertyName]
                    let current = currVal[propertyName] == null ? "" : currVal[propertyName]
                    where original != current
                    select "Changed " + propertyName + " From " + original.ToString() + " to " + current.ToString()
                );
            }

            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
                //return 0;
            }
        }


        ////End Changes for Auditing
    }
}