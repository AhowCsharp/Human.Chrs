using Microsoft.Extensions.Logging;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.IRepository;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain.Helper;
using Human.Chrs.Domain.CommonModels;

namespace Human.Chrs.Domain
{
    public class StaffDomain
    {
        private readonly ILogger<StaffDomain> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyRuleRepository _companyRuleRepository;
        private readonly ICheckRecordsRepository _checkRecordsRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IOverTimeLogRepository _overTimeLogRepository;
        private readonly IVacationLogRepository _vacationLogRepository;
        private readonly UserService _userService;
        private readonly GeocodingService _geocodingService;

        public StaffDomain(
            ILogger<StaffDomain> logger,
            IAdminRepository adminRepository,
            ICompanyRuleRepository companyRuleRepository,
            ICheckRecordsRepository checkRecordsRepository,
            IStaffRepository staffRepository,
            ICompanyRepository companyRepository,
            IVacationLogRepository vacationLogRepository,
            IOverTimeLogRepository overTimeLogRepository,
            GeocodingService geocodingService,
            UserService userService)
        {
            _logger = logger;
            _adminRepository = adminRepository;
            _companyRepository = companyRepository;
            _staffRepository = staffRepository;
            _companyRepository = companyRepository;
            _companyRuleRepository = companyRuleRepository;
            _vacationLogRepository = vacationLogRepository;
            _checkRecordsRepository = checkRecordsRepository;
            _overTimeLogRepository = overTimeLogRepository;
            _geocodingService = geocodingService;
            _userService = userService;
        }

        public async Task<IEnumerable<VacationLogDTO>> GetVacationLogListAsync()
        {
            var user = _userService.GetCurrentUser();
            var data = await _vacationLogRepository.GetTop5VacationLogsAsync(user.Id, user.CompanyId);
            return data;
        }

        public async Task<CommonResult<IEnumerable<VacationLogDTO>>> InsertNewStaffAsync(VacationLogDTO dto)
        {
            var result = new CommonResult<IEnumerable<VacationLogDTO>>();
            var user = _userService.GetCurrentUser();
            var exist = await _staffRepository.VerifyExistStaffAsync(user.Id, user.CompanyId);
            if (!exist)
            {
                result.AddError("沒找到對應的員工");
                return result;
            }
            dto.StaffId = user.Id;
            dto.CompanyId = user.CompanyId;
            await _vacationLogRepository.InsertAsync(dto);

            result.Data = await _vacationLogRepository.GetTop5VacationLogsAsync(user.Id, user.CompanyId); ;

            return result;
        }

        public async Task<(double,double)> GetStaffViewInfoAsync()
        {
            var result = new CommonResult<Object>();
            var user = _userService.GetCurrentUser();
            var exist = await _staffRepository.VerifyExistStaffAsync(user.Id, user.CompanyId);
            if (!exist)
            {
                result.AddError("沒找到對應的員工");
                //return result;
            }
            var company = await _companyRepository.GetAsync(user.CompanyId);
            if (company == null)
            {
                result.AddError("沒找到對應的公司");
                //return result;
            }
            var xx = await _geocodingService.GetCoordinates(company.Address);
            return xx;
        }
    }
}