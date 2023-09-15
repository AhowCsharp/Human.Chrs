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
        private readonly CheckInAndOutDomain _checkInAndOutDomain;
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
            CheckInAndOutDomain checkInAndOutDomain,
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
            _checkInAndOutDomain = checkInAndOutDomain;
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

        public async Task<CommonResult<StaffViewDTO>> GetStaffViewInfoAsync(double longitude, double latitude)
        {
            var result = new CommonResult<StaffViewDTO>();
            var staffView = new StaffViewDTO();
            var user = _userService.GetCurrentUser();
            var exist = await _staffRepository.VerifyExistStaffAsync(user.Id, user.CompanyId);
            if (!exist)
            {
                result.AddError("沒找到對應的員工");
                return result;
            }
            var company = await _companyRepository.GetAsync(user.CompanyId);
            if (company == null)
            {
                result.AddError("沒找到對應的公司");
                return result;
            }

            if (company.Latitude == 0 || company.Longitude == 0)
            {
                var location = await _geocodingService.GetCoordinates(company.Address);
                company.Latitude = location.Latitude;
                company.Longitude = location.Longitude;
                await _companyRepository.UpdateAsync(company);
            }
            var checkRecord = await _checkRecordsRepository.GetCheckRecordAsync(user.CompanyId, user.Id);
            var rule = await _companyRuleRepository.GetCompanyRuleAsync(user.CompanyId, user.DepartmentId);
            if (rule == null)
            {
                result.AddError("未找到貴司登記的上班規定");
                return result;
            }
            staffView.IsOverLocation = (_checkInAndOutDomain.CheckDistanceAsync(company, longitude, latitude)).Data;
            staffView.CheckInRange = rule.CheckInStartTime.ToString(@"hh\:mm") + "~" + rule.CheckInEndTime.ToString(@"hh\:mm");
            staffView.CheckOutRange = rule.CheckOutStartTime.ToString(@"hh\:mm") + "~" + rule.CheckOutEndTime.ToString(@"hh\:mm");
            staffView.AfternoonRange = rule.AfternoonTime;
            staffView.CompanyName = company.CompanyName;
            staffView.VacationLogDTOs = await _vacationLogRepository.GetTop5VacationLogsAsync(user.Id, user.CompanyId);
            if (checkRecord != null)
            {
                if (checkRecord.CheckInTime != null && checkRecord.CheckOutTime == null)
                {
                    staffView.IsCheckIn = true;
                    staffView.ChekinTime = checkRecord.CheckInTime;
                    staffView.IsCheckOut = false;
                }
                else if (checkRecord.CheckInTime != null && checkRecord.CheckOutTime != null)
                {
                    staffView.IsCheckIn = true;
                    staffView.ChekinTime = checkRecord.CheckInTime;
                    staffView.IsCheckOut = true;
                    staffView.ChekOutTime = checkRecord.CheckOutTime;
                }
            }
            else 
            {
                staffView.IsCheckIn = false;
                staffView.IsCheckOut = false;
            }
            staffView.CheckInStartHour = int.Parse(staffView.CheckInRange.Split('~', ':')[0]);
            staffView.CheckInStartMinute = int.Parse(staffView.CheckInRange.Split('~', ':')[1]);
            staffView.CheckInEndHour = int.Parse(staffView.CheckInRange.Split('~', ':')[2]);
            staffView.CheckInEndMinute = int.Parse(staffView.CheckInRange.Split('~', ':')[3]);
            staffView.CheckOutStartHour = int.Parse(staffView.CheckOutRange.Split('~', ':')[0]);
            staffView.CheckOutStartMinute = int.Parse(staffView.CheckOutRange.Split('~', ':')[1]);
            staffView.CheckOutEndHour = int.Parse(staffView.CheckOutRange.Split('~', ':')[2]);
            staffView.CheckOutEndMinute = int.Parse(staffView.CheckOutRange.Split('~', ':')[3]);

            result.Data = staffView;
            return result;
        }
    }
}