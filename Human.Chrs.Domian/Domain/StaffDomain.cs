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
        private readonly IEventLogsRepository _eventLogsRepository;
        private readonly CheckInAndOutDomain _checkInAndOutDomain;
        private readonly UserService _userService;
        private readonly GeocodingService _geocodingService;

        public StaffDomain(
            ILogger<StaffDomain> logger,
            IAdminRepository adminRepository,
            ICompanyRuleRepository companyRuleRepository,
            ICheckRecordsRepository checkRecordsRepository,
            IEventLogsRepository eventLogsRepository,
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
            _eventLogsRepository = eventLogsRepository;
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
            staffView.VacationLogDTOs = (await _vacationLogRepository.GetTop5VacationLogsAsync(user.Id, user.CompanyId)).ToList();
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
            foreach (var item in staffView.VacationLogDTOs)
            {
                switch (item.VacationType)
                {
                    case 0:

                        item.VacationTypeName = "特休";
                        break;

                    case 1:

                        item.VacationTypeName = "病假";
                        break;

                    case 2:

                        item.VacationTypeName = "事假";
                        break;

                    case 3:

                        item.VacationTypeName = "生育假或育嬰假";
                        break;

                    case 4:

                        item.VacationTypeName = "喪假";
                        break;

                    case 5:

                        item.VacationTypeName = "婚假";
                        break;

                    default:
                        item.VacationTypeName = "未知假別";
                        break;
                }
            }

            result.Data = staffView;
            return result;
        }

        public async Task<CommonResult<bool>> ApplyVacationAsync(int type, DateTime startDate, DateTime endDate, int hours, string reason)
        {
            var result = new CommonResult<bool>();
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
            var repeat = await _vacationLogRepository.VerifyVacationLogsAsync(user.Id, user.CompanyId, startDate, endDate);

            bool canApply = false;
            var staff = await _staffRepository.GetAsync(user.Id);
            var vacation = new VacationLogDTO();
            vacation.ApplyDate = DateTimeHelper.TaipeiNow;
            vacation.CompanyId = user.CompanyId;
            vacation.StaffId = user.Id;
            vacation.VacationType = type;
            vacation.ActualStartDate = startDate;
            vacation.ActualEndDate = endDate;
            vacation.Hours = hours;
            vacation.Reason = reason;
            //SpecialRest, //特休
            //SickDays, //病假
            //ThingDays, //事假
            //ChildbirthDays, //生育假
            //DeathDays, //喪假
            //MarryDays, //婚假
            switch (type)
            {
                case 0:
                    if ((staff.SpecialRestDays * 8 + staff.SpecialRestHours) > hours && !repeat)
                    {
                        canApply = true;
                    }
                    break;

                case 1:
                    if ((staff.SickDays * 8 + staff.SickHours) > hours && !repeat)
                    {
                        canApply = true;
                    }
                    break;

                case 2:
                    if ((staff.ThingDays * 8 + staff.ThingHours) > hours && !repeat)
                    {
                        canApply = true;
                    }
                    break;

                case 3:
                    if ((staff.ChildbirthDays * 8 + staff.ChildbirthHours) > hours && !repeat)
                    {
                        canApply = true;
                    }
                    break;

                case 4:
                    if ((staff.DeathDays * 8 + staff.DeathHours) > hours && !repeat)
                    {
                        canApply = true;
                    }
                    break;

                case 5:
                    if ((staff.MarryDays * 8 + staff.MarryHours) > hours && !repeat)
                    {
                        canApply = true;
                    }
                    break;

                default:
                    canApply = false;
                    break;
            }

            if (!canApply)
            {
                result.AddError("您已超過該假別可請求的時數");
                return result;
            }
            await _vacationLogRepository.InsertAsync(vacation);

            return result;
        }

        public async Task<CommonResult<bool>> EventAddAsync(DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime, string title, string detail, int level)
        {
            var result = new CommonResult<bool>();
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

            if (endDate < startDate)
            {
                result.AddError("時間格式不正確");
                return result;
            }
            var dto = new EventLogsDTO();
            dto.StartDate = startDate;
            dto.EndDate = endDate;
            dto.EndTime = endTime;
            dto.Title = title;
            dto.StartTime = startTime;
            dto.Detail = detail;
            dto.StaffId = user.Id;
            dto.CompanyId = user.CompanyId;
            dto.LevelStatus = level;
            await _eventLogsRepository.InsertAsync(dto);

            return result;
        }

        public async Task<CommonResult<List<EventDTO>>> EventsGetAsync()
        {
            var result = new CommonResult<List<EventDTO>>();
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
            var eventsDTO = new List<EventDTO>();
            var events = await _eventLogsRepository.GetAllEventLogsAsync(user.Id, user.CompanyId);
            foreach (var item in events)
            {
                var newEvent = new EventDTO();
                if (item.StartDate == item.EndDate)
                {
                    DateTime newDateTime = new DateTime(
                        item.StartDate.Year,
                        item.StartDate.Month,
                        item.StartDate.Day,
                        item.StartTime.Hours,
                        item.StartTime.Minutes,
                        item.StartTime.Seconds
                    );
                    DateTime newEndDateTime = new DateTime(
                    item.EndDate.Year,
                    item.EndDate.Month,
                    item.EndDate.Day,
                    item.EndTime.Hours,
                    item.EndTime.Minutes,
                    item.EndTime.Seconds
                    );
                    newEvent.AllDay = false;
                    newEvent.Start = newDateTime;
                    newEvent.End = newEndDateTime;
                    newEvent.Title = item.Title;
                    newEvent.Detail = item.Detail;
                    newEvent.LevelStatus = item.LevelStatus;
                    eventsDTO.Add(newEvent);
                }
                else
                {
                    newEvent.AllDay = false;
                    newEvent.Start = item.StartDate;
                    newEvent.End = item.EndDate;
                    newEvent.Title = item.Title;
                    newEvent.Detail = item.Detail;
                    newEvent.LevelStatus = item.LevelStatus;
                    eventsDTO.Add(newEvent);
                }
            }
            result.Data = eventsDTO;
            return result;
        }

        public async Task<CommonResult<IEnumerable<CheckRecordsDTO>>> GetCheckListAsync(DateTime? startDate, DateTime? endDate)
        {
            var result = new CommonResult<IEnumerable<CheckRecordsDTO>>();
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
            if (startDate == null)
            {
                startDate = new DateTime(DateTimeHelper.TaipeiNow.Year, DateTimeHelper.TaipeiNow.Month, 1);
            }
            if (endDate == null)
            {
                endDate = new DateTime(DateTimeHelper.TaipeiNow.Year, DateTimeHelper.TaipeiNow.Month, DateTime.DaysInMonth(DateTimeHelper.TaipeiNow.Year, DateTimeHelper.TaipeiNow.Month));
            }

            var data = await _checkRecordsRepository.GetCheckRecordListAsync(user.Id, user.CompanyId, startDate.Value, endDate.Value);
            result.Data = data;
            return result;
        }
    }
}