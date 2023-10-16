using Microsoft.Extensions.Logging;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.IRepository;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain.Helper;
using Human.Chrs.Domain.CommonModels;
using System.Collections.Generic;
using System.Collections;
using Microsoft.AspNetCore.Components.Forms;
using NLog.Fluent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Human.Chrs.Domain.SeedWork;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing.Printing;
using NPOI.XSSF.UserModel;
using NPOI.OpenXmlFormats.Vml.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Util;
using System.Globalization;
using NPOI.SS.Formula.Functions;

namespace Human.Chrs.Domain
{
    public class SuperDomain
    {
        private readonly ILogger<SuperDomain> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyRuleRepository _companyRuleRepository;
        private readonly ICheckRecordsRepository _checkRecordsRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IVacationLogRepository _vacationLogRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IOverTimeLogRepository _overTimeLogRepository;
        private readonly IPersonalDetailRepository _personalDetailRepository;
        private readonly ISalarySettingRepository _salarySettingRepository;
        private readonly IIncomeLogsRepository _incomeLogsRepository;
        private readonly IEventLogsRepository _eventLogsRepository;
        private readonly IAmendCheckRecordRepository _amendCheckRecordRepository;
        private readonly IMeetLogRepository _meetLogRepository;
        private readonly IAdminNotificationLogsRepository _adminNotificationLogsRepository;
        private readonly IAdminReadLogsRepository _adminReadLogsRepository;
        private readonly IContractTypeRepository _contractTypeRepository;
        private readonly UserService _userService;
        private readonly GeocodingService _geocodingService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public SuperDomain(
            ILogger<SuperDomain> logger,
            IAdminRepository adminRepository,
            ICompanyRuleRepository companyRuleRepository,
            ICheckRecordsRepository checkRecordsRepository,
            IVacationLogRepository vacationLogRepository,
            IDepartmentRepository departmentRepository,
            IStaffRepository staffRepository,
            ICompanyRepository companyRepository,
            IOverTimeLogRepository overTimeLogRepository,
            ISalarySettingRepository salarySettingRepository,
            IPersonalDetailRepository personalDetailRepository,
            IIncomeLogsRepository incomeLogsRepository,
            IEventLogsRepository eventLogsRepository,
            IAmendCheckRecordRepository amendCheckRecordRepository,
            IAdminNotificationLogsRepository adminNotificationLogsRepository,
            IAdminReadLogsRepository adminReadLogsRepository,
            IMeetLogRepository meetLogRepository,
            IContractTypeRepository contractTypeRepository,
            UserService userService,
            GeocodingService geocodingService,
             IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _adminRepository = adminRepository;
            _companyRepository = companyRepository;
            _staffRepository = staffRepository;
            _companyRepository = companyRepository;
            _vacationLogRepository = vacationLogRepository;
            _salarySettingRepository = salarySettingRepository;
            _departmentRepository = departmentRepository;
            _companyRuleRepository = companyRuleRepository;
            _checkRecordsRepository = checkRecordsRepository;
            _overTimeLogRepository = overTimeLogRepository;
            _incomeLogsRepository = incomeLogsRepository;
            _personalDetailRepository = personalDetailRepository;
            _eventLogsRepository = eventLogsRepository;
            _amendCheckRecordRepository = amendCheckRecordRepository;
            _adminNotificationLogsRepository = adminNotificationLogsRepository;
            _adminReadLogsRepository = adminReadLogsRepository;
            _contractTypeRepository = contractTypeRepository;
            _meetLogRepository = meetLogRepository;
            _userService = userService;
            _geocodingService = geocodingService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<CommonResult> CreateOrEditCompanyAsync(CompanyDTO dto)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            if (dto.id == 0)
            {
                try
                {
                    var coordinates = await _geocodingService.GetCoordinates(dto.Address);
                    dto.Longitude = coordinates.Longitude;
                    dto.Latitude = coordinates.Latitude;
                    await _companyRepository.InsertAsync(dto);
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    return result;
                }
            }
            else
            {
                await _companyRepository.UpdateAsync(dto);
            }
            return result;
        }

        public async Task<CommonResult> CreateOrEditAdminAsync(AdminDTO dto)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            if (dto.id == 0)
            {
                try
                {
                    dto.Password = CryptHelper.SaltHashPlus(dto.Password);
                    await _adminRepository.InsertAsync(dto);
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    return result;
                }
            }
            else
            {
                await _adminRepository.UpdateAsync(dto);
            }
            return result;
        }

        public async Task<CommonResult<IEnumerable<CompanyDTO>>> GetAllCompanyAsync()
        {
            var result = new CommonResult<IEnumerable<CompanyDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            var allComs = await _companyRepository.AllCompanyAsync();
            result.Data = allComs;
            return result;
        }


        public async Task<CommonResult<IEnumerable<ContractTypeDTO>>> GetAllContractTypeAsync()
        {
            var result = new CommonResult<IEnumerable<ContractTypeDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            var types = await _contractTypeRepository.AllContractTypeAsync();
            result.Data = types;
            return result;
        }
    }
}