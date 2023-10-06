using Microsoft.Extensions.Logging;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.IRepository;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain.Helper;
using Human.Chrs.Domain.CommonModels;
using Human.Chrs.Enum;
using System;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;

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
        private readonly IIncomeLogsRepository _incomeLogsRepository;
        private readonly IVacationLogRepository _vacationLogRepository;
        private readonly IEventLogsRepository _eventLogsRepository;
        private readonly IPersonalDetailRepository _personalDetailRepository;
        private readonly IAmendCheckRecordRepository _amendCheckRecordRepository;
        private readonly CheckInAndOutDomain _checkInAndOutDomain;
        private readonly UserService _userService;
        private readonly GeocodingService _geocodingService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public StaffDomain(
            ILogger<StaffDomain> logger,
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
            CheckInAndOutDomain checkInAndOutDomain,
            GeocodingService geocodingService,
            UserService userService,
            IAmendCheckRecordRepository amendCheckRecordRepository,
            IWebHostEnvironment hostEnvironment)
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
            _checkInAndOutDomain = checkInAndOutDomain;
            _amendCheckRecordRepository = amendCheckRecordRepository;
            _geocodingService = geocodingService;
            _userService = userService;
            _hostEnvironment = hostEnvironment;
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
            staffView.IsOverLocation = (_checkInAndOutDomain.CheckDistanceAsync(rule, longitude, latitude)).Data;
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
            var staffDetail = await _personalDetailRepository.GetStaffDetailInfoAsync(user.Id, user.CompanyId);
            if (staffDetail == null)
            {
                result.AddError("員工個人詳細資料未填寫完整");
                return result;
            }
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
            switch (type)
            {
                case 0: //特休
                    if ((staff.SpecialRestDays * 8 + staff.SpecialRestHours) > hours && !repeat)
                    {
                        canApply = true;
                    }
                    break;

                case 1://病假
                    if ((staff.SickDays * 8 + staff.SickHours) > hours && !repeat)
                    {
                        canApply = true;
                    }
                    break;

                case 2: //事假
                    if ((staff.ThingDays * 8 + staff.ThingHours) > hours && !repeat)
                    {
                        canApply = true;
                    }
                    break;

                case 3://產假
                    if ((staff.ChildbirthDays * 8 + staff.ChildbirthHours) > hours && !repeat && staffDetail.Gender == "女性")
                    {
                        canApply = true;
                    }
                    break;

                case 4://喪假
                    if ((staff.DeathDays * 8 + staff.DeathHours) > hours && !repeat)
                    {
                        canApply = true;
                    }
                    break;

                case 5://婚假
                    if ((staff.MarryDays * 8 + staff.MarryHours) > hours && !repeat)
                    {
                        canApply = true;
                    }
                    break;

                case 6://公假
                    canApply = true;
                    break;

                case 7://工傷病假
                    canApply = true;
                    break;

                case 8://生理假
                    if ((staff.MenstruationDays * 8 + staff.MenstruationHours) > hours && !repeat && staffDetail.Gender == "女性")
                    {
                        canApply = true;
                    }
                    break;

                case 9://育嬰留職停薪假
                    if ((staff.TackeCareBabyDays * 8 + staff.TackeCareBabyHours) > hours && !repeat && staffDetail.Gender == "女性")
                    {
                        canApply = true;
                    }
                    break;

                case 10://安胎
                    if ((staff.TocolysisDays * 8 + staff.TocolysisHours) > hours && !repeat && staffDetail.Gender == "女性")
                    {
                        canApply = true;
                    }
                    break;

                case 11://產檢
                    if ((staff.PrenatalCheckUpDays * 8 + staff.PrenatalCheckUpHours) > hours && !repeat)
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

        public async Task<CommonResult<PersonalDetailDTO?>> GetStaffDetailAsync(int? staffId)
        {
            var result = new CommonResult<PersonalDetailDTO?>();
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
            if (staffId.HasValue)
            {
                var detail = await _personalDetailRepository.GetStaffDetailInfoAsync(staffId.Value, user.CompanyId);
                result.Data = detail;
            }
            else
            {
                var staff = await _staffRepository.GetUsingStaffAsync(user.Id, user.CompanyId);
                var detail = await _personalDetailRepository.GetStaffDetailInfoAsync(user.Id, user.CompanyId);
                detail.WorkLocation = staff.WorkLocation;
                detail.LevelPosition = staff.LevelPosition;
                detail.Department = staff.Department;
                detail.PhoneNumber = staff.StaffPhoneNumber;
                detail.EntryDate = staff.EntryDate;
                detail.Email = staff.Email;
                detail.CompanyName = company.CompanyName;
                result.Data = detail;
            }

            return result;
        }

        public async Task<CommonResult<List<EventDTO>>> DeleteEventAsync(int id)
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
            await _eventLogsRepository.DeleteAsync(id);
            var events = (await EventsGetAsync()).Data;
            result.Data = events;

            return result;
        }

        public async Task<CommonResult<IEnumerable<AmendCheckRecordDTO>>> GetAmendCheckRecordAsync()
        {
            var result = new CommonResult<IEnumerable<AmendCheckRecordDTO>>();
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
            result.Data = await _amendCheckRecordRepository.GetTop6AmendCheckRecordAsync(user.Id, user.CompanyId);

            return result;
        }

        public async Task<CommonResult> ApplyAmendCheckRecordAsync(DateTime checkdate, DateTime checktime, string reason, int checkType)
        {
            var result = new CommonResult();
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
            var dto = new AmendCheckRecordDTO
            {
                StaffId = user.Id,
                CompanyId = user.CompanyId,
                CheckDate = checkdate,
                CheckTime = checktime,
                CheckType = checkType,
                Reason = reason,
                IsValidate = 0,
                Applicant = user.StaffName,
                ApplicationDate = DateTimeHelper.TaipeiNow,
            };
            await _amendCheckRecordRepository.InsertAsync(dto);

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
                    newEvent.id = item.id;
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
                    newEvent.id = item.id;
                    eventsDTO.Add(newEvent);
                }
            }
            result.Data = eventsDTO;
            return result;
        }

        public async Task<CommonResult<IEnumerable<CheckRecordsDTO>>> GetCheckListAsync(DateTime? startDate, DateTime? endDate, int staffId)
        {
            var result = new CommonResult<IEnumerable<CheckRecordsDTO>>();
            var user = _userService.GetCurrentUser();

            var exist = await _staffRepository.VerifyExistStaffAsync(staffId, user.CompanyId);
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

            var data = await _checkRecordsRepository.GetCheckRecordListAsync(staffId, user.CompanyId, startDate.Value, endDate.Value);
            result.Data = data;
            return result;
        }

        public async Task<CommonResult<IEnumerable<CheckRecordsDTO>>> GetPersonalChecksAsync(int month)
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
            // Assuming the year is the current year.
            int year = DateTimeHelper.TaipeiNow.Year;

            // Get the first day of the month.
            var startDate = new DateTime(year, month, 1);

            // Get the first day of the next month, then subtract one day.
            var endDate = new DateTime(year, month % 12 + 1, 1).AddDays(-1);
            var data = await _checkRecordsRepository.GetCheckRecordListAsync(user.Id, user.CompanyId, startDate, endDate);
            result.Data = data;
            return result;
        }

        public async Task<CommonResult<IEnumerable<OverTimeLogDTO>>> GetovertimeListAsync(int? staffId, DateTime startDate, DateTime endDate)
        {
            var result = new CommonResult<IEnumerable<OverTimeLogDTO>>();
            var user = _userService.GetCurrentUser();
            if (staffId.HasValue)
            {
                var exist = await _staffRepository.VerifyExistStaffAsync(staffId.Value, user.CompanyId);
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
                var staff = await _staffRepository.GetUsingStaffAsync(staffId.Value, user.CompanyId);
                var data = (await _overTimeLogRepository.GetOverTimeLogOfPeriodAfterValidateAsync(staffId.Value, user.CompanyId, startDate, endDate)).ToList();
                foreach (var item in data)
                {
                    item.StaffName = staff.StaffName;
                }
                result.Data = data;
                return result;
            }
            else
            {
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
                var staff = await _staffRepository.GetUsingStaffAsync(user.Id, user.CompanyId);
                var data = (await _overTimeLogRepository.GetOverTimeLogOfPeriodAfterValidateAsync(user.Id, user.CompanyId, startDate, endDate)).ToList();
                foreach (var item in data)
                {
                    item.StaffName = staff.StaffName;
                }
                result.Data = data;
                return result;
            }
        }

        public async Task<CommonResult<IEnumerable<OverTimeLogDTO>>> GetThisMonthOvertimeListAsync(int month)
        {
            var result = new CommonResult<IEnumerable<OverTimeLogDTO>>();
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
            var staff = await _staffRepository.GetUsingStaffAsync(user.Id, user.CompanyId);
            // Assuming the year is the current year.
            int year = DateTimeHelper.TaipeiNow.Year;

            // Get the first day of the month.
            var startDate = new DateTime(year, month, 1);

            // Get the first day of the next month, then subtract one day.
            var endDate = new DateTime(year, month % 12 + 1, 1).AddDays(-1);
            var data = (await _overTimeLogRepository.GetOverTimeLogOfPeriodAsync(user.Id, user.CompanyId, startDate, endDate)).ToList();
            foreach (var item in data)
            {
                item.StaffName = staff.StaffName;
            }
            result.Data = data;
            return result;
        }

        public async Task<CommonResult<IEnumerable<IncomeLogsDTO>>> GetIncomeLogsAsync()
        {
            var result = new CommonResult<IEnumerable<IncomeLogsDTO>>();
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

            var data = await _incomeLogsRepository.GetIncomeLogsAsync(user.Id, user.CompanyId);
            result.Data = data;
            return result;
        }

        public async Task<CommonResult<IncomeLogsDTO>> GetSalaryDetailAsync(int id)
        {
            var result = new CommonResult<IncomeLogsDTO>();
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

            var data = await _incomeLogsRepository.GetAsync(id);
            result.Data = data;
            return result;
        }

        public async Task<CommonResult<string>> UploadAvatarAsync(IFormFile avatar)
        {
            var result = new CommonResult<string>();
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

            var staff = await _staffRepository.GetUsingStaffAsync(user.Id, user.CompanyId);

            if (!string.IsNullOrEmpty(staff.AvatarUrl))
            {
                string[] parts = staff.AvatarUrl.Split('/');
                if (parts.Length >= 3)
                {
                    // 取得不包括擴展名的檔名
                    string existFileNameWithoutExtension = Path.GetFileNameWithoutExtension(parts[2]);
                    // 取得完整擴展名
                    string extensionOld = Path.GetExtension(parts[2]);

                    // 確定在Avatar資料夾的完整路徑
                    var existingAvatarPath = Path.Combine(_hostEnvironment.WebRootPath, "Avatar", $"{existFileNameWithoutExtension}{extensionOld}");

                    if (File.Exists(existingAvatarPath))
                    {
                        try
                        {
                            File.Delete(existingAvatarPath);
                        }
                        catch (Exception ex)
                        {
                            //// 如果出現問題，如文件訪問問題，捕獲異常
                            //result.AddError(ex.Message);
                            //return result;
                        }
                    }
                }
            }

            // 1. Generate UUID and create the save path
            string extension;

            switch (avatar.ContentType)
            {
                case "image/jpeg":
                    extension = ".jpg";
                    break;

                case "image/png":
                    extension = ".png";
                    break;

                case "image/gif":
                    extension = ".gif";
                    break;

                case "image/bmp":
                    extension = ".bmp";
                    break;

                default:
                    result.AddError("不支持的文件格式");
                    return result;
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var folderPath = Path.Combine(_hostEnvironment.WebRootPath, "Avatar");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var savePath = Path.Combine(_hostEnvironment.WebRootPath, "Avatar", fileName); // _hostEnvironment 需要注入IHostEnvironment

            // 2. Save the uploaded file to the specified path
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await avatar.CopyToAsync(fileStream);
            }

            // 3. Save the URL to the database
            var url = $"/avatar/{fileName}";
            staff.AvatarUrl = url; // 假設您的Staff模型中有一個名為AvatarUrl的屬性
            await _staffRepository.UpdateAsync(staff);
            result.Data = url;
            return result;
        }
    }
}