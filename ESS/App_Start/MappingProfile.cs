using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using ESS.Controllers;
using ESS.Dto;
using ESS.Models;

namespace ESS.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employees, EmployeeDto>();
            CreateMap<EmployeeDto, Employees>();

            CreateMap<Company, CompanyDto>();
            CreateMap<WorkGroups, WorkGroupDto>();
            CreateMap<Units, UnitDto>();
            CreateMap<Departments, DepartmentDto>();
            CreateMap<Stations, StationDto>();
            CreateMap<Sections, SectionDto>();
            CreateMap<Categories, CategoryDto>();
            CreateMap<EmpTypes, EmpTypeDto>();
            CreateMap<Grades, GradeDto>();
            CreateMap<Designations, DesignationDto>();
            CreateMap<ReleaseGroups, ReleaseGroupDto>();
            CreateMap<ReleaseStrategies, ReleaseStrategyDto>();
            CreateMap<ReleaseStrategyLevels, ReleaseStrategyLevelDto>();

            CreateMap<LeaveTypes, LeaveTypeDto>();

            CreateMap<LeaveBalance, LeaveBalanceDto>();
            CreateMap<LeaveBalanceDto, LeaveBalance>();

            CreateMap<LeaveApplications, LeaveApplicationDto>();
            CreateMap<LeaveApplicationDto, LeaveApplications>();

            CreateMap<LeaveApplicationDetails, LeaveApplicationDetailDto>();
            CreateMap<LeaveApplicationDetailDto, LeaveApplicationDetails>();

            CreateMap<ApplReleaseStatus, ApplReleaseStatusDto>();

            CreateMap<EmpUniform, EmpUniformDto>();

            CreateMap<GatePass, GatePassDto>();

            CreateMap<EmpAddress, EmpAddressDto>();

            CreateMap<GpReleaseStrategies, GpReleaseStrategyDto>();
            CreateMap<GpReleaseStrategyLevels, GpReleaseStrategyLevelDto>();

            CreateMap<TaxDeclarations, TaxDeclarationDto>();
            CreateMap<TaxDeclarationDto, TaxDeclarations>();

            CreateMap<TaxDeclarationHistory, TaxDeclarationDto>();
            CreateMap<TaxDeclarationDto, TaxDeclarationHistory>();

            CreateMap<TaxDetailsInsurance, TaxDetailsInsuranceDto>();
            CreateMap<TaxDetailsInsuranceDto, TaxDetailsInsurance>();

            CreateMap<TaxDetailsMutualFunds, TaxDetailsMutualFundsDto>();
            CreateMap<TaxDetailsMutualFundsDto, TaxDetailsMutualFunds>();

            CreateMap<TaxDetailsNsc, TaxDetailsNscDto>();
            CreateMap<TaxDetailsNscDto, TaxDetailsNsc>();

            CreateMap<TaxDetailsPpf, TaxDetailsPpfDto>();
            CreateMap<TaxDetailsPpfDto, TaxDetailsPpf>();

            CreateMap<Banks, BankDto>();

            CreateMap<TaxDetailsBankDeposit, TaxDetailsBankDepositDto>();
            CreateMap<TaxDetailsBankDepositDto, TaxDetailsBankDeposit>();

            CreateMap<TaxDetailsUlip, TaxDetailsUlipDto>();
            CreateMap<TaxDetailsUlipDto, TaxDetailsUlip>();

            CreateMap<TaxDetailsSukanya, TaxDetailsSukanyaDto>();
            CreateMap<TaxDetailsSukanyaDto, TaxDetailsSukanya>();

            CreateMap<TaxDetailsRent, TaxDetailsRentDto>();
            CreateMap<TaxDetailsRentDto, TaxDetailsRent>();

            CreateMap<RoleAuth, RoleAuthDto>();

            CreateMap<GpAdvices, GpAdviceDto>();
            CreateMap<GpAdviceDto, GpAdvices>();
            CreateMap<GpAdviceDetails, GpAdviceDetailsDto>();
            CreateMap<GpAdviceDetailsDto, GpAdviceDetails>();
            CreateMap<GaReleaseStrategies, GaReleaseStrategyDto>();
            CreateMap<GaReleaseStrategyDto, GaReleaseStrategies>();
            CreateMap<GaReleaseStrategyLevels, GaReleaseStrategyLevelDto>();
            CreateMap<GaReleaseStrategyLevelDto, GaReleaseStrategyLevels>();

            CreateMap<Materials, MaterialDto>();
            CreateMap<MaterialDto, Materials>();

            CreateMap<Vendors, VendorDto>();
            CreateMap<VendorDto, Vendors>();

            CreateMap<Shifts, ShiftDto>();
            CreateMap<ShiftDto, Shifts>();

            CreateMap<ShiftSchedules, ShiftScheduleDto>();
            CreateMap<ShiftScheduleDto, ShiftSchedules>();
            CreateMap<ShiftScheduleDetails, ShiftScheduleDetailDto>();
            CreateMap<ShiftScheduleDetailDto, ShiftScheduleDetails>();

            CreateMap<Reimbursements, ReimbursementDto>();
            CreateMap<ReimbursementDto, Reimbursements>();

            CreateMap<ReimbConv, ReimbConvDto>();
            CreateMap<ReimbConvDto, ReimbConv>();

            CreateMap<MedDependent, MedDependentDto>();
            CreateMap<MedDependentDto, MedDependent>();

            CreateMap<MedEmpUhid, MedEmpUhidDto>();
            CreateMap<MedEmpUhidDto, MedEmpUhid>();

            CreateMap<MedicalFitness, MedicalFitnessDto>();
            CreateMap<MedicalFitnessDto, MedicalFitness>();

            CreateMap<NoDuesMaster, NoDuesMasterDto>();
            CreateMap<NoDuesMasterDto, NoDuesMaster>();

            CreateMap<NoDuesReleaseStatus, NoDuesReleaseStatusDto>();
            CreateMap<NoDuesReleaseStatusDto, NoDuesReleaseStatus>();

            CreateMap<NoDuesDept, NoDuesDeptDto>();
            CreateMap<NoDuesDeptDto, NoDuesDept>();

            CreateMap<NoDuesDeptDetails, NoDuesDeptDetailsDto>();
            CreateMap<NoDuesDeptDetailsDto, NoDuesDeptDetails>();

            CreateMap<NoDuesStatus, NoDuesStatusDto>();
            CreateMap<NoDuesStatusDto, NoDuesStatus>();

            CreateMap<Requests, RequestDto>();
            CreateMap<RequestDto, Requests>();
            CreateMap<RequestDetails, RequestDetailsDto>();
            CreateMap<RequestDetailsDto, RequestDetails>();
            CreateMap<RequestRelease, RequestReleaseDto>();
            CreateMap<RequestReleaseDto, RequestRelease>();

            CreateMap<EmpSeparation, EmpSeparationDto>();
            CreateMap<EmpSeparationDto, EmpSeparation>();

            CreateMap<MissedPunch, MissedPunchDto>();
            CreateMap<MissedPunchDto, MissedPunch>();
            CreateMap<MissedPunchReleaseStatus, MissedPunchReleaseStatusDto>();
            CreateMap<MissedPunchReleaseStatusDto, MissedPunchReleaseStatus>();

            CreateMap<MedIntimation, MedIntimationDto>();
            CreateMap<MedIntimationDto, MedIntimation>();

            CreateMap<FullAndFinal, FullAndFinalDto>();
            CreateMap<FullAndFinalDto, FullAndFinal>();

            CreateMap<TpaSanction, TpaSanctionDto>();
            CreateMap<TpaSanctionDto, TpaSanction>();
            CreateMap<TpaRelease, TpaReleaseDto>();
            CreateMap<TpaReleaseDto, TpaRelease>();

            CreateMap<Vaccination, VaccinationDto>();
            CreateMap<VaccinationDto, Vaccination>();

            CreateMap<Reviews, ReviewDto>();
            CreateMap<ReviewDto, Reviews>();
            CreateMap<ReviewDetails, ReviewDetailDto>();
            CreateMap<ReviewDetailDto, ReviewDetails>();

            CreateMap<SapUserIds, SapUserIdDto>();
            CreateMap<SapUserIdDto, SapUserIds>();

            CreateMap<AddressProof, AddressProofDto>();
            CreateMap<AddressProofDto, AddressProof>();

            CreateMap<VehicleReq, VehicleReqDto>();
            CreateMap<VehicleReqDto, VehicleReq>();
        }
    }
}