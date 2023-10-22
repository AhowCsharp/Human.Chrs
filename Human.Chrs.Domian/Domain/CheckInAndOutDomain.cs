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
            var dateNow = DateTimeHelper.TaipeiNow;
            var start = dateNow.Date; // 設置為那天的 00:00:00
            var end = dateNow.Date.AddSeconds(86399); // 設置為那天的 23:59:59
            var partimeRule = await _companyRuleRepository.GetParttimeRuleAsync(user.CompanyId, user.DepartmentId, user.Id, start, end);
            if (partimeRule != null)
            {
                var checkLog = await _checkRecordsRepository.GetCheckRecordAsync(user.CompanyId, user.Id);
                if (checkLog == null)
                {
                    record.CompanyId = user.CompanyId;
                    record.StaffId = user.Id;
                    record.IsCheckInOutLocation = DistanceHelper.CalculateDistance(partimeRule.Latitude.Value, partimeRule.Longitude.Value, latitude, longitude) > 200 ? 1 : 0;
                    record.CheckInTime = DateTimeHelper.TaipeiNow;
                    record.CheckInMemo = memo;
                    record.IsCheckInLate = DateTimeHelper.TaipeiNow.TimeOfDay > partimeRule.CheckInEndTime ? 1 : 0;
                    if (record.IsCheckInLate == 1)
                    {
                        exceededMinutes = (int)(DateTimeHelper.TaipeiNow.TimeOfDay - partimeRule.CheckInEndTime).TotalMinutes;
                    }
                    record.CheckInLateTimes = exceededMinutes;
                    await _checkRecordsRepository.InsertAsync(record);
                }
                else if (checkLog.CheckOutTime == null)
                {
                    checkLog.IsCheckOutOutLocation = DistanceHelper.CalculateDistance(partimeRule.Latitude.Value, partimeRule.Longitude.Value, latitude, longitude) > 200 ? 1 : 0;
                    checkLog.CheckOutTime = DateTimeHelper.TaipeiNow;
                    checkLog.CheckOutMemo = memo;
                    checkLog.IsCheckOutEarly = DateTimeHelper.TaipeiNow.TimeOfDay < partimeRule.CheckOutStartTime ? 1 : 0;
                    checkLog.IsCheckOutEarly = DateTimeHelper.TaipeiNow.TimeOfDay < checkLog.CheckInTime.Value.TimeOfDay.Add(TimeSpan.FromHours(partimeRule.NeedWorkMinute / 60)) ? 1 : 0;
                    if (checkLog.IsCheckOutEarly == 1)
                    {
                        exceededMinutes = (int)(checkLog.CheckInTime.Value.TimeOfDay.Add(TimeSpan.FromHours(partimeRule.NeedWorkMinute / 60)) - DateTimeHelper.TaipeiNow.TimeOfDay).TotalMinutes;
                    }
                    checkLog.CheckOutEarlyTimes = exceededMinutes;
                    await _checkRecordsRepository.UpdateAsync(checkLog);
                }
                else
                {
                    result.AddError("今天已打過卡");
                    return result;
                }

                return result;
            }




            var rule = await _companyRuleRepository.GetCompanyRuleAsync(user.CompanyId, user.DepartmentId);
            if (rule == null || !rule.Latitude.HasValue ||  !rule.Longitude.HasValue || string.IsNullOrEmpty(rule.WorkAddress))
            {
                result.AddError("該公司尚未設置規則");
                return result;
            }

            var fullTimecheckLog = await _checkRecordsRepository.GetCheckRecordAsync(user.CompanyId, user.Id);
            if (fullTimecheckLog == null)
            {
                record.CompanyId = user.CompanyId;
                record.StaffId = user.Id;
                record.IsCheckInOutLocation = DistanceHelper.CalculateDistance(rule.Latitude.Value, rule.Longitude.Value, latitude, longitude) > 200 ? 1 : 0;
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
            else if (fullTimecheckLog.CheckOutTime == null)
            {
                fullTimecheckLog.IsCheckOutOutLocation = DistanceHelper.CalculateDistance(rule.Latitude.Value, rule.Longitude.Value, latitude, longitude) > 200 ? 1 : 0;
                fullTimecheckLog.CheckOutTime = DateTimeHelper.TaipeiNow;
                fullTimecheckLog.CheckOutMemo = memo;
                fullTimecheckLog.IsCheckOutEarly = DateTimeHelper.TaipeiNow.TimeOfDay < rule.CheckOutStartTime ? 1 : 0;
                fullTimecheckLog.IsCheckOutEarly = DateTimeHelper.TaipeiNow.TimeOfDay < fullTimecheckLog.CheckInTime.Value.TimeOfDay.Add(TimeSpan.FromHours(rule.NeedWorkMinute / 60)) ? 1 : 0;
                if (fullTimecheckLog.IsCheckOutEarly == 1)
                {
                    exceededMinutes = (int)(fullTimecheckLog.CheckInTime.Value.TimeOfDay.Add(TimeSpan.FromHours(rule.NeedWorkMinute / 60)) - DateTimeHelper.TaipeiNow.TimeOfDay).TotalMinutes;
                }
                fullTimecheckLog.CheckOutEarlyTimes = exceededMinutes;
                await _checkRecordsRepository.UpdateAsync(fullTimecheckLog);
            }
            else
            {
                result.AddError("今天已打過卡");
                return result;
            }

            return result;
        }

        public async Task<CommonResult> InsertOverTimeAsync(DateTime chooseDate, int hours, string reason)
        {
            var result = new CommonResult();
            var overTime = new OverTimeLogDTO();
            var user = _userService.GetCurrentUser();
            var exsit = await _overTimeLogRepository.GetOverTimeLogAsync(user.Id, user.CompanyId);
            if (exsit != null)
            {
                result.AddError("今日已報過加班");
                return result;
            }
            overTime.StaffId = user.Id;
            overTime.CompanyId = user.CompanyId;
            overTime.OverHours = hours;
            overTime.IsValidate = 0;
            overTime.OvertimeDate = DateTimeHelper.TaipeiNow;
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
            var user = _userService.GetCurrentUser();
            var rule = await _companyRuleRepository.GetCompanyRuleAsync(user.CompanyId, user.DepartmentId);
            if (rule == null || !rule.Latitude.HasValue || rule.Longitude.HasValue || string.IsNullOrEmpty(rule.WorkAddress))
            {
                result.AddError("該公司尚未設置規則");
                return result;
            }
            double distance = DistanceHelper.CalculateDistance(rule.Latitude.Value, rule.Longitude.Value, latitude, longitude);
            bool overDistance = distance > 200;
            result.Data = overDistance;

            return result;
        }

        public CommonResult<bool> CheckDistanceAsync(CompanyRuleDTO rule, double longitude, double latitude)
        {
            var result = new CommonResult<bool>();
            if (!rule.Longitude.HasValue || string.IsNullOrEmpty(rule.WorkAddress))
            {
                result.AddError("該公司尚未設置規則");
                return result;
            }
            double distance = DistanceHelper.CalculateDistance(rule.Latitude.Value, rule.Longitude.Value, latitude, longitude);
            bool overDistance = distance > 200;
            result.Data = overDistance;

            return result;
        }
    }
}