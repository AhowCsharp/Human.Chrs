using Microsoft.Extensions.Logging;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.IRepository;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain.Helper;
using Human.Chrs.Domain.CommonModels;
using Human.Chrs.Enum;
using System;
using System.ComponentModel.Design;

namespace Human.Chrs.Domain
{
    public class ScheduleDomain
    {
        private readonly ILogger<ScheduleDomain> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyRuleRepository _companyRuleRepository;
        private readonly ICheckRecordsRepository _checkRecordsRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IOverTimeLogRepository _overTimeLogRepository;
        private readonly IIncomeLogsRepository _incomeLogsRepository;
        private readonly IVacationLogRepository _vacationLogRepository;
        private readonly IEventLogsRepository _eventLogsRepository;
        private readonly IPersonalDetailRepository _personalDetailRepository;
        private readonly IReadLogsRepository _readLogsRepository;
        private readonly IAdminReadLogsRepository _adminReadLogsRepository;
        private readonly IAdminNotificationLogsRepository _adminNotificationLogsRepository;
        private readonly INotificationLogsRepository _notificationLogsRepository;
        private readonly CheckInAndOutDomain _checkInAndOutDomain;
        private readonly UserService _userService;
        private readonly GeocodingService _geocodingService;

        public ScheduleDomain(
            ILogger<ScheduleDomain> logger,
            IAdminRepository adminRepository,
            ICompanyRuleRepository companyRuleRepository,
            ICheckRecordsRepository checkRecordsRepository,
            IIncomeLogsRepository incomeLogsRepository,
            IEventLogsRepository eventLogsRepository,
            IStaffRepository staffRepository,
            IPersonalDetailRepository personalDetailRepository,
            ICompanyRepository companyRepository,
            IVacationLogRepository vacationLogRepository,
            IOverTimeLogRepository overTimeLogRepository,
            IAdminNotificationLogsRepository adminNotificationLogsRepository,
            IAdminReadLogsRepository adminReadLogsRepository,
            INotificationLogsRepository notificationLogsRepository,
            IReadLogsRepository readLogsRepository,
            CheckInAndOutDomain checkInAndOutDomain,
            GeocodingService geocodingService,
            UserService userService)
        {
            _logger = logger;
            _adminRepository = adminRepository;
            _companyRepository = companyRepository;
            _eventLogsRepository = eventLogsRepository;
            _personalDetailRepository = personalDetailRepository;
            _incomeLogsRepository = incomeLogsRepository;
            _staffRepository = staffRepository;
            _companyRepository = companyRepository;
            _companyRuleRepository = companyRuleRepository;
            _vacationLogRepository = vacationLogRepository;
            _checkRecordsRepository = checkRecordsRepository;
            _overTimeLogRepository = overTimeLogRepository;
            _adminNotificationLogsRepository = adminNotificationLogsRepository;
            _adminReadLogsRepository = adminReadLogsRepository;
            _notificationLogsRepository = notificationLogsRepository;
            _readLogsRepository = readLogsRepository;
            _checkInAndOutDomain = checkInAndOutDomain;
            _geocodingService = geocodingService;
            _userService = userService;
        }

        public async Task<bool> UpdateStaffInfoAsync()
        {
            bool isSuccessFul = await _staffRepository.UpdateWorkDaysAndFindStaffAsync();
            return isSuccessFul;
        }

        public async Task DeleteNotificationAsync()
        {
            await _notificationLogsRepository.DeletePersonalNotificationBeforeSevenDaysAsync();
        }
    }
}