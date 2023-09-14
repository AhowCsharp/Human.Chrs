using Microsoft.Extensions.Logging;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.IRepository;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain.Helper;
using Human.Chrs.Domain.CommonModels;

namespace Human.Chrs.Domain
{
    public class CheckInAndOutDomain
    {
        private readonly ILogger<CheckInAndOutDomain> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyRuleRepository _companyRuleRepository;
        private readonly ICheckRecordsRepository _checkRecordsRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IOverTimeLogRepository _overTimeLogRepository;
        private readonly UserService _userService;

        public CheckInAndOutDomain(
            ILogger<CheckInAndOutDomain> logger,
            IAdminRepository adminRepository,
            ICompanyRuleRepository companyRuleRepository,
            ICheckRecordsRepository checkRecordsRepository,
            IStaffRepository staffRepository,
            ICompanyRepository companyRepository,
            IOverTimeLogRepository overTimeLogRepository,
            UserService userService)
        {
            _logger = logger;
            _adminRepository = adminRepository;
            _companyRepository = companyRepository;
            _staffRepository = staffRepository;
            _companyRepository = companyRepository;
            _companyRuleRepository = companyRuleRepository;
            _checkRecordsRepository = checkRecordsRepository;
            _overTimeLogRepository = overTimeLogRepository;
            _userService = userService;
        }

        public async Task<CommonResult> CheckInOutAsync(double longitude, double latitude, string memo)
        {
            var result = new CommonResult();
            var record = new CheckRecordsDTO();
            var user = _userService.GetCurrentUser();
            int exceededMinutes = 0;
            var company = await _companyRepository.GetAsync(user.CompanyId);
            if (company == null)
            {
                result.AddError("找不到該公司");
                return result;
            }
            var rule = await _companyRuleRepository.GetCompanyRuleAsync(user.CompanyId, user.DepartmentId.Value);
            if (rule == null)
            {
                result.AddError("該公司尚未設置規則");
                return result;
            }
            var checkLog = await _checkRecordsRepository.GetCheckRecordAsync(user.CompanyId, user.Id);
            if (checkLog == null)
            {
                record.CompanyId = user.CompanyId;
                record.StaffId = user.Id;
                record.IsCheckInOutLocation = DistanceHelper.CalculateDistance(company.Latitude, company.Longitude, latitude, longitude) > 200 ? 1 : 0;
                record.CheckInTime = DateTimeHelper.TaipeiNow;
                record.CheckInMemo = memo;
                record.IsCheckInLate = DateTimeHelper.TaipeiNow.TimeOfDay > rule.CheckInEndTime ? 1 : 0;
                if (record.IsCheckInLate == 1)
                {
                    exceededMinutes = (int)(DateTimeHelper.TaipeiNow.TimeOfDay - rule.CheckInEndTime).TotalMinutes;
                }
                record.CheckInLateTimes = exceededMinutes;
                await _checkRecordsRepository.InsertAsync(record);
            }
            else
            {
                checkLog.IsCheckOutOutLocation = DistanceHelper.CalculateDistance(company.Latitude, company.Longitude, latitude, longitude) > 200 ? 1 : 0;
                checkLog.CheckOutTime = DateTimeHelper.TaipeiNow;
                checkLog.CheckOutMemo = memo;
                checkLog.IsCheckOutEarly = DateTimeHelper.TaipeiNow.TimeOfDay < rule.CheckOutStartTime ? 1 : 0;
                if (checkLog.IsCheckOutEarly == 1)
                {
                    exceededMinutes = (int)(rule.CheckOutStartTime - DateTimeHelper.TaipeiNow.TimeOfDay).TotalMinutes;
                }
                checkLog.CheckOutEarlyTimes = exceededMinutes;
                await _checkRecordsRepository.UpdateAsync(checkLog);
            }

            return result;
        }

        public async Task<CommonResult> InsertOverTimeAsync(int staffId, int companyId, int hours, string reason)
        {
            var result = new CommonResult();
            var overTime = new OverTimeLogDTO();
            var exsit = await _overTimeLogRepository.GetOverTimeLogAsync(staffId, companyId);
            if (exsit != null)
            {
                result.AddError("今日已報過加班");
                return result;
            }
            overTime.StaffId = staffId;
            overTime.CompanyId = companyId;
            overTime.OverHours = hours;
            overTime.IsValidate = 0;
            overTime.OvertimeDate = DateTimeHelper.TaipeiNow.Date;
            overTime.Reason = reason;
            await _overTimeLogRepository.InsertAsync(overTime);
            return result;
        }

        public async Task<CommonResult<bool>> CheckDistanceAsync(int companyId, double longitude, double latitude)
        {
            var result = new CommonResult<bool>();
            var company = await _companyRepository.GetAsync(companyId);
            if (company == null)
            {
                result.AddError("找不到該公司");
                return result;
            }
            double distance = DistanceHelper.CalculateDistance(company.Latitude, company.Longitude, latitude, longitude);
            bool overDistance = distance > 200;
            result.Data = overDistance;

            return result;
        }
    }
}