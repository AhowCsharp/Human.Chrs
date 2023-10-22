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
using System.ComponentModel.Design;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace Human.Chrs.Domain
{
    public class AdminDomain
    {
        private readonly ILogger<AdminDomain> _logger;
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
        private readonly UserService _userService;
        private readonly GeocodingService _geocodingService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IContractTypeRepository _contractTypeRepository;

        public AdminDomain(
            ILogger<AdminDomain> logger,
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
            UserService userService,
            GeocodingService geocodingService,
            IWebHostEnvironment hostEnvironment,
            IContractTypeRepository contractTypeRepository)
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
            _meetLogRepository = meetLogRepository;
            _userService = userService;
            _geocodingService = geocodingService;
            _hostEnvironment = hostEnvironment;
            _contractTypeRepository = contractTypeRepository;
        }

        public async Task<CommonResult> SwitchReadStatusAsync(int notificationId)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            var dto = new AdminReadLogsDTO
            {
                AdminId = user.Id,
                CompanyId = user.CompanyId,
                AdminNotificationId = notificationId,
                ReadDate = DateTimeHelper.TaipeiNow,
            };
            var notification = await _adminNotificationLogsRepository.GetAsync(notificationId);

            if (notification != null)
            {
                notification.ReadAdminIds = string.IsNullOrEmpty(notification.ReadAdminIds)
                    ? user.Id.ToString()
                    : notification.ReadAdminIds + "," + user.Id.ToString();
                await _adminNotificationLogsRepository.UpdateAsync(notification);
            }
            await _adminReadLogsRepository.InsertAsync(dto);
            return result;
        }

        public async Task<CommonResult> SwitchAllReadStatusAsync(List<int> notificationIds)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            var dtosToInsert = new List<AdminReadLogsDTO>();
            //var notificationsToUpdate = new List<AdminNotificationLogsDTO>();

            foreach (var notificationId in notificationIds)
            {
                var dto = new AdminReadLogsDTO
                {
                    AdminId = user.Id,
                    CompanyId = user.CompanyId,
                    AdminNotificationId = notificationId,
                    ReadDate = DateTimeHelper.TaipeiNow,
                };

                var notification = await _adminNotificationLogsRepository.GetAsync(notificationId);
                if (notification != null)
                {
                    notification.ReadAdminIds = string.IsNullOrEmpty(notification.ReadAdminIds)
                        ? user.Id.ToString()
                        : notification.ReadAdminIds + "," + user.Id.ToString();

                    await _adminNotificationLogsRepository.UpdateAsync(notification);
                }

                dtosToInsert.Add(dto);
            }
            // Batch update and insert

            await _adminReadLogsRepository.InsertRangeAsync(dtosToInsert);

            return result;
        }

        public async Task<CommonResult<List<EventDTO>>> EventsGetAsync()
        {
            var result = new CommonResult<List<EventDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            var eventsDTO = new List<EventDTO>();
            var events = await _eventLogsRepository.GetCompanyPartimeEventLogsAsync(user.CompanyId);
            foreach (var eventDTO in events)
            {
                var staff = await _staffRepository.GetAsync(eventDTO.StaffId);
                var newEvent = new EventDTO();
                if (eventDTO.StartDate.Date == eventDTO.EndDate.Date)
                {
                    DateTime newDateTime = new DateTime(
                        eventDTO.StartDate.Year,
                        eventDTO.StartDate.Month,
                        eventDTO.StartDate.Day,
                        eventDTO.StartTime.Hours,
                        eventDTO.StartTime.Minutes,
                        eventDTO.StartTime.Seconds
                    );
                    DateTime newEndDateTime = new DateTime(
                    eventDTO.EndDate.Year,
                    eventDTO.EndDate.Month,
                    eventDTO.EndDate.Day,
                    eventDTO.EndTime.Hours,
                    eventDTO.EndTime.Minutes,
                    eventDTO.EndTime.Seconds
                    );
                    newEvent.AllDay = false;
                    newEvent.Start = newDateTime;
                    newEvent.End = newEndDateTime;
                    newEvent.Title = eventDTO.Title;
                    newEvent.Detail = eventDTO.Detail;
                    newEvent.LevelStatus = eventDTO.LevelStatus;
                    newEvent.StaffId = staff.id;
                    newEvent.StaffName = staff.StaffName;
                    newEvent.id = eventDTO.id;
                    eventsDTO.Add(newEvent);
                }
                else
                {
                    newEvent.AllDay = false;
                    newEvent.Start = eventDTO.StartDate;
                    newEvent.End = eventDTO.EndDate;
                    newEvent.Title = eventDTO.Title;
                    newEvent.Detail = eventDTO.Detail;
                    newEvent.LevelStatus = eventDTO.LevelStatus;
                    newEvent.id = eventDTO.id;
                    newEvent.StaffId = staff.id;
                    newEvent.StaffName = staff.StaffName;
                    eventsDTO.Add(newEvent);
                }
            }
            result.Data = eventsDTO;
            return result;
        }

        public async Task<CommonResult<List<EventDTO>>> MeetEventsGetAsync()
        {
            var result = new CommonResult<List<EventDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            var eventsDTO = new List<EventDTO>();
            var events = await _meetLogRepository.GetMeetEventLogsAsync(user.CompanyId);
            foreach (var eventDTO in events)
            {
                var staff = await _staffRepository.GetAsync(eventDTO.StaffId.Value);
                var newEvent = new EventDTO();
                if (eventDTO.StartDate.Date == eventDTO.EndDate.Date)
                {
                    DateTime newDateTime = new DateTime(
                        eventDTO.StartDate.Year,
                        eventDTO.StartDate.Month,
                        eventDTO.StartDate.Day,
                        eventDTO.StartTime.Hours,
                        eventDTO.StartTime.Minutes,
                        eventDTO.StartTime.Seconds
                    );
                    DateTime newEndDateTime = new DateTime(
                    eventDTO.StartDate.Year,
                    eventDTO.StartDate.Month,
                    eventDTO.StartDate.Day,
                    eventDTO.EndTime.HasValue ? eventDTO.EndTime.Value.Hours : eventDTO.StartTime.Hours,
                    eventDTO.EndTime.HasValue ? eventDTO.EndTime.Value.Minutes : eventDTO.StartTime.Minutes,
                    eventDTO.EndTime.HasValue ? eventDTO.EndTime.Value.Seconds : eventDTO.StartTime.Seconds
                    );
                    newEvent.AllDay = false;
                    newEvent.Start = newDateTime;
                    newEvent.End = newEndDateTime;
                    newEvent.Title = eventDTO.Title;
                    newEvent.Detail = eventDTO.Detail;
                    newEvent.LevelStatus = 4;
                    newEvent.StaffId = staff != null ? staff.id : 0;
                    newEvent.StaffName = staff != null ? staff.StaffName : String.Empty;
                    newEvent.id = eventDTO.id;
                    newEvent.MeetId = eventDTO.id;
                    eventsDTO.Add(newEvent);
                }
                else
                {
                    newEvent.AllDay = false;
                    newEvent.Start = eventDTO.StartDate;
                    newEvent.End = eventDTO.EndDate;
                    newEvent.Title = eventDTO.Title;
                    newEvent.Detail = eventDTO.Detail;
                    newEvent.LevelStatus = 4;
                    newEvent.id = eventDTO.id;
                    newEvent.StaffId = staff != null ? staff.id : 0;
                    newEvent.StaffName = staff != null ? staff.StaffName : String.Empty;
                    newEvent.MeetId = eventDTO.id;
                    eventsDTO.Add(newEvent);
                }
            }
            result.Data = eventsDTO;
            return result;
        }

        public async Task<CommonResult<List<EventDTO>>> DeleteMeetAsync(int id)
        {
            var result = new CommonResult<List<EventDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            await _eventLogsRepository.DeleteAllEventsWithMeetIdAsync(id);

            await _meetLogRepository.DeleteAsync(id);
            var events = (await MeetEventsGetAsync()).Data;
            result.Data = events;

            return result;
        }

        public async Task<CommonResult<List<AdminDTO>>> DeleteAdminAsync(int id)
        {
            var result = new CommonResult<List<AdminDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            await _adminRepository.DeleteAsync(id);

            var admins = await GetAllAdminsAsync();
            result.Data = admins.Data.ToList();
            return result;
        }

        public async Task<CommonResult<List<AdminDTO>>> SwitchAdminAsync(int id)
        {
            var result = new CommonResult<List<AdminDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            var admin = await _adminRepository.GetAsync(id);
            admin.Status = !admin.Status;
            await _adminRepository.UpdateAsync(admin);
            var admins = await GetAllAdminsAsync();
            result.Data = admins.Data.ToList();
            return result;
        }

        public async Task<CommonResult> ParttimeWorkAdd(int staffId, DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime, string title, string detail, int level)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            if (endDate < startDate)
            {
                result.AddError("時間格式不正確");
                return result;
            }

            var staff = await _staffRepository.GetUsingStaffAsync(staffId, user.CompanyId);
            var department = await _departmentRepository.GetAsync(staff.DepartmentId);
            try
            {
                var map = await _geocodingService.GetCoordinates(detail);
                var dto = new EventLogsDTO();
                dto.StartDate = startDate;
                dto.EndDate = endDate;
                dto.EndTime = endTime;
                dto.Title = title;
                dto.StartTime = startTime;
                dto.Detail = detail;
                dto.StaffId = staffId;
                dto.CompanyId = user.CompanyId;
                dto.LevelStatus = level;
                await _eventLogsRepository.InsertAsync(dto);

                DateTime currentDay = startDate;
                while (currentDay <= endDate)
                {
                    var parttimeRule = new CompanyRuleDTO
                    {
                        CompanyId = user.CompanyId,
                        DepartmentId = staff.DepartmentId,
                        CheckInStartTime = startTime,
                        CheckInEndTime = startTime,
                        CheckOutStartTime = endTime,
                        CheckOutEndTime = endTime,
                        DepartmentName = department.DepartmentName,
                        NeedWorkMinute = (int)(endTime - startTime).TotalMinutes, // 這裡要修正，您可能想要用 endTime - startTime
                        CreateDate = DateTimeHelper.TaipeiNow,
                        Creator = user.StaffName,
                        EditDate = DateTimeHelper.TaipeiNow,
                        Editor = user.StaffName,
                        WorkAddress = detail,
                        Latitude = map.Latitude,
                        Longitude = map.Longitude,
                        ParttimeStaffId = staff.id,
                        ParttimeDate = currentDay
                    };
                    await _companyRuleRepository.InsertAsync(parttimeRule);

                    currentDay = currentDay.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                result.AddError(ex.Message);
                return result;
            }
            return result;
        }

        public async Task<CommonResult> MeetAdd(EventLogsDTO dto)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            if (dto.EndTime < dto.StartTime)
            {
                result.AddError("時間格式不正確");
                return result;
            }

            try
            {
                var meetDTO = new MeetLogDTO();
                meetDTO.Creator = user.StaffName;
                meetDTO.CreateDate = DateTimeHelper.TaipeiNow;

                if (dto.MeetType == 1)
                {
                    meetDTO.CompanyId = user.CompanyId;
                    meetDTO.StaffId = 0;
                    meetDTO.DepartmentId = 0;
                    meetDTO.Title = dto.Title;
                    meetDTO.Detail = dto.Detail;
                    meetDTO.StartDate = dto.StartDate;
                    meetDTO.EndDate = dto.EndDate;
                    meetDTO.StartTime = dto.StartTime;
                    meetDTO.EndTime = dto.EndTime;
                    meetDTO = await _meetLogRepository.InsertAsync(meetDTO);

                    var allstaffId = (await _staffRepository.GetAllStaffAsync(user.CompanyId)).Select(x => x.id).ToList();
                    var events = new List<EventLogsDTO>();
                    foreach (var staffId in allstaffId)
                    {
                        var realEventDTO = new EventLogsDTO();
                        realEventDTO.StaffId = staffId;
                        realEventDTO.CompanyId = user.CompanyId;
                        realEventDTO.MeetId = meetDTO.id;

                        realEventDTO.Title = dto.Title;
                        realEventDTO.Detail = dto.Detail;
                        realEventDTO.StartDate = dto.StartDate;
                        realEventDTO.EndDate = dto.EndDate;
                        realEventDTO.StartTime = dto.StartTime;
                        realEventDTO.EndTime = dto.EndTime;
                        realEventDTO.LevelStatus = dto.LevelStatus;
                        events.Add(realEventDTO);
                    }
                    await _eventLogsRepository.AddManyEventLogsAsync(events);
                }
                else if (dto.MeetType == 2)
                {
                    meetDTO.CompanyId = user.CompanyId;
                    meetDTO.StaffId = 0;
                    meetDTO.DepartmentId = dto.DepartmentId.Value;
                    meetDTO.Title = dto.Title;
                    meetDTO.Detail = dto.Detail;
                    meetDTO.StartDate = dto.StartDate;
                    meetDTO.EndDate = dto.EndDate;
                    meetDTO.StartTime = dto.StartTime;
                    meetDTO.EndTime = dto.EndTime;
                    meetDTO = await _meetLogRepository.InsertAsync(meetDTO);

                    var allstaffId = (await _staffRepository.GetDepartmentStaffAsync(user.CompanyId, dto.DepartmentId.Value)).Select(x => x.id).ToList();
                    var events = new List<EventLogsDTO>();
                    foreach (var staffId in allstaffId)
                    {
                        var realEventDTO = new EventLogsDTO();
                        realEventDTO.StaffId = staffId;
                        realEventDTO.CompanyId = user.CompanyId;
                        realEventDTO.MeetId = meetDTO.id;

                        realEventDTO.Title = dto.Title;
                        realEventDTO.Detail = dto.Detail;
                        realEventDTO.StartDate = dto.StartDate;
                        realEventDTO.EndDate = dto.EndDate;
                        realEventDTO.StartTime = dto.StartTime;
                        realEventDTO.EndTime = dto.EndTime;
                        realEventDTO.LevelStatus = dto.LevelStatus;
                        events.Add(realEventDTO);
                    }
                    await _eventLogsRepository.AddManyEventLogsAsync(events);
                }
                else if (dto.MeetType == 3)
                {
                    meetDTO.CompanyId = user.CompanyId;
                    meetDTO.StaffId = dto.StaffId;
                    meetDTO.DepartmentId = 0;
                    meetDTO.Title = dto.Title;
                    meetDTO.Detail = dto.Detail;
                    meetDTO.StartDate = dto.StartDate;
                    meetDTO.EndDate = dto.EndDate;
                    meetDTO.StartTime = dto.StartTime;
                    meetDTO.EndTime = dto.EndTime;
                    meetDTO = await _meetLogRepository.InsertAsync(meetDTO);

                    var staff = await _staffRepository.GetAsync(dto.StaffId);

                    dto.StaffId = staff.id;
                    dto.CompanyId = user.CompanyId;
                    dto.MeetId = meetDTO.id;

                    await _eventLogsRepository.InsertAsync(dto);
                }

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(ex.Message);
                return result;
            }
        }

        public async Task<CommonResult<IEnumerable<StaffDTO>>> CreateOrEditStaffAsync(StaffDTO newStaff)
        {
            var result = new CommonResult<IEnumerable<StaffDTO>>();
            var user = _userService.GetCurrentUser();
            bool isCreate = newStaff.id == 0;
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            if (isCreate)
            {
                var staffCount = await _staffRepository.StaffCountAsync(user.CompanyId);
                var company = await _companyRepository.GetAsync(user.CompanyId);
                var contract = await _contractTypeRepository.GetContractTypeAsync(company.ContractType.Value);
                if ((staffCount + 1) > contract.StaffLimit)
                {
                    result.AddError("超出合約員工人數限制");

                    return result;
                }
                var existAccount = await _staffRepository.VerifyAccountAsync(newStaff.StaffAccount);
                var existAdminAccount = await _adminRepository.VerifyAdminAccountAsync(newStaff.StaffAccount);
                var existEmail = await _staffRepository.VerifyEmailAsync(newStaff.Email);
                if (existEmail)
                {
                    result.AddError("信箱已註冊過");

                    return result;
                }
                if (existAccount || existAdminAccount)
                {
                    result.AddError("帳號重複");

                    return result;
                }
                var department = await _departmentRepository.GetAsync(newStaff.DepartmentId);
                newStaff.WorkLocation = (await _companyRuleRepository.GetCompanyRuleAsync(newStaff.CompanyId, newStaff.DepartmentId)).WorkAddress;
                newStaff.CreateDate = DateTimeHelper.TaipeiNow;
                newStaff.Creator = user.StaffName;
                newStaff.EditDate = DateTimeHelper.TaipeiNow;
                newStaff.Editor = user.StaffName;
                newStaff.Department = department.DepartmentName;
                await _staffRepository.InsertAsync(newStaff);
            }
            else
            {
                var oldData = await _staffRepository.GetAsync(newStaff.id);
                var existEmail = await _staffRepository.VerifyEmailAsync(newStaff.Email, newStaff.id);
                if (existEmail)
                {
                    result.AddError("信箱已註冊過");

                    return result;
                }

                if (newStaff.StaffAccount == oldData.StaffAccount)
                {
                    newStaff.WorkLocation = (await _companyRuleRepository.GetCompanyRuleAsync(newStaff.CompanyId, newStaff.DepartmentId)).WorkAddress;
                    newStaff.EditDate = DateTimeHelper.TaipeiNow;
                    newStaff.Editor = user.StaffName;
                    var department = await _departmentRepository.GetAsync(newStaff.DepartmentId);
                    newStaff.Department = department.DepartmentName;
                    await _staffRepository.UpdateAsync(newStaff);
                }
                else
                {
                    var existAccount = await _staffRepository.VerifyAccountAsync(newStaff.StaffAccount);
                    var existAdminAccount = await _adminRepository.VerifyAdminAccountAsync(newStaff.StaffAccount);
                    if (existAccount || existAdminAccount)
                    {
                        result.AddError("帳號重複");

                        return result;
                    }
                    newStaff.WorkLocation = (await _companyRuleRepository.GetCompanyRuleAsync(newStaff.CompanyId, newStaff.DepartmentId)).WorkAddress;
                    newStaff.EditDate = DateTimeHelper.TaipeiNow;
                    newStaff.Editor = user.StaffName;
                    await _staffRepository.UpdateAsync(newStaff);
                }
            }

            var data = await _staffRepository.GetAllStaffAsync(user.CompanyId);
            result.Data = data;

            return result;
        }

        public async Task<CommonResult<PersonalDetailDTO>> RecordStaffDetailAsync(PersonalDetailDTO dto)
        {
            var result = new CommonResult<PersonalDetailDTO>();
            bool isCreate = dto.id == 0;
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            PersonalDetailDTO? data;
            if (verifyAdminToken)
            {
                if (isCreate)
                {
                    dto.IdentityNo = CryptHelper.SaltHashPlus(dto.IdentityNo);
                    dto.CompanyId = user.CompanyId;
                    data = await _personalDetailRepository.InsertAsync(dto);
                    var staff = await _staffRepository.GetUsingStaffAsync(dto.StaffId, dto.CompanyId);
                    staff.PersonalDetailId = data.id;
                    await _staffRepository.UpdateAsync(staff);
                }
                else
                {
                    dto.CompanyId = user.CompanyId;
                    data = await _personalDetailRepository.UpdateAsync(dto);
                }
            }
            else
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            result.Data = data;

            return result;
        }

        public async Task<CommonResult<SalarySettingDTO>> CreateOrEditSalarySetting(SalarySettingDTO dto)
        {
            var result = new CommonResult<SalarySettingDTO>();
            bool isCreate = dto.id == 0;
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (verifyAdminToken)
            {
                var staff = await _staffRepository.GetUsingStaffAsync(dto.StaffId, user.CompanyId);
                if (staff.EmploymentTypeId != 1)
                {
                    result.AddError("該員工並非全職雇用類別");

                    return result;
                }
                if (isCreate)
                {
                    var exist = await _salarySettingRepository.GetSalarySettingAsync(dto.StaffId, dto.CompanyId);
                    if (exist != null)
                    {
                        result.AddError("該員工已有薪資設定");

                        return result;
                    }
                    dto.EditDate = DateTimeHelper.TaipeiNow;
                    dto.CreateDate = DateTimeHelper.TaipeiNow;
                    dto.Creator = user.StaffName;
                    dto.Editor = user.StaffName;
                    result.Data = await _salarySettingRepository.InsertAsync(dto);
                }
                else
                {
                    var oldData = await _salarySettingRepository.GetAsync(dto.id);
                    oldData.BasicSalary = dto.BasicSalary;
                    oldData.FullCheckInMoney = dto.FullCheckInMoney;
                    oldData.OtherPercent = dto.OtherPercent;
                    oldData.EditDate = DateTimeHelper.TaipeiNow;
                    oldData.Editor = user.StaffName;
                    oldData.FoodSuportMoney = dto.FoodSuportMoney;
                    result.Data = await _salarySettingRepository.UpdateAsync(oldData);
                }
            }
            else
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            return result;
        }

        public async Task<CommonResult> DeleteSalarySettingAsunc(int id)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (verifyAdminToken)
            {
                var dto = await _salarySettingRepository.GetAsync(id);
                var staff = await _staffRepository.GetUsingStaffAsync(dto.StaffId, dto.CompanyId);
                if (staff != null)
                {
                    result.AddError("請先更改該員工狀態");

                    return result;
                }
                await _salarySettingRepository.DeleteAsync(id);
            }
            else
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            return result;
        }

        public async Task<CommonResult> DeleteDepartmentAsunc(int departmentId, int otherDepartmentId)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            var departments = await _departmentRepository.GetDepartmentsOfCompanyAsync(user.CompanyId);
            if (departments.Count() == 1)
            {
                result.AddError("不可以刪除所有部門");

                return result;
            }

            var rule = await _companyRuleRepository.GetCompanyRuleAsync(user.CompanyId, departmentId);
            await _companyRuleRepository.DeleteAsync(rule.id);

            var newDepartment = await _departmentRepository.GetAsync(otherDepartmentId);
            var departmentStaffs = await _staffRepository.GetDepartmentStaffAsync(user.CompanyId, departmentId);
            foreach (var staff in departmentStaffs)
            {
                staff.DepartmentId = newDepartment.id;
                staff.Department = newDepartment.DepartmentName;
                await _staffRepository.UpdateAsync(staff);
            }

            await _departmentRepository.DeleteAsync(departmentId);
            return result;
        }

        public async Task<CommonResult> RemovestaffAsync(int id)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (verifyAdminToken)
            {
                var staff = await _staffRepository.GetAsync(id);
                if (staff == null)
                {
                    result.AddError("出事了 找不到該員工");

                    return result;
                }
                var eventLogs = await _eventLogsRepository.GetAllEventLogsAsync(staff.id, staff.CompanyId);
                await _eventLogsRepository.RemoveEventLogsAsync(eventLogs, user.CompanyId);
                await _staffRepository.DeleteAsync(id);
            }
            else
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            return result;
        }

        public async Task<CommonResult<string>> UploadAvatarAsync(IFormFile avatar)
        {
            var result = new CommonResult<string>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            var admin = await _adminRepository.GetAvailableAdminAsync(user.Id, user.CompanyId);

            if (!string.IsNullOrEmpty(admin.AvatarUrl))
            {
                string[] parts = admin.AvatarUrl.Split('/');
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
            admin.AvatarUrl = url;
            await _adminRepository.UpdateAsync(admin);
            result.Data = url;
            return result;
        }

        public async Task<CommonResult<AdminDTO>> GetAdminDetailAsync()
        {
            var result = new CommonResult<AdminDTO>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (verifyAdminToken)
            {
                var admin = await _adminRepository.GetAvailableAdminAsync(user.Id, user.CompanyId);
                if (admin == null)
                {
                    result.AddError("找不到該管理者 出事了啊北");

                    return result;
                }
                admin.CompanyName = (await _companyRepository.GetAsync(user.CompanyId)).CompanyName;
                result.Data = admin;
            }
            else
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            return result;
        }

        public async Task<CommonResult<IEnumerable<SalarySettingDTO>>> GetAllSalarySettingAsync()
        {
            var result = new CommonResult<IEnumerable<SalarySettingDTO>>();

            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (verifyAdminToken)
            {
                var settings = (await _salarySettingRepository.GetAllSalarySettingAsync(user.CompanyId)).ToList();
                foreach (var setting in settings)
                {
                    var staff = await _staffRepository.GetUsingStaffAsync(setting.StaffId, setting.CompanyId);
                    if (staff == null)
                    {
                        result.AddError("系統錯誤 找不到該員工");
                        return result;
                    }
                    setting.StaffName = staff.StaffName;
                    setting.StaffNo = staff.StaffNo;
                }
                result.Data = settings;
            }
            else
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            return result;
        }

        public async Task<IEnumerable<StaffDTO>> GetAllStaffAsync()
        {
            var user = _userService.GetCurrentUser();

            var staffs = await _staffRepository.GetAllStaffAsync(user.CompanyId);

            return staffs;
        }

        public async Task<CommonResult<IEnumerable<AdminDTO>>> GetAllAdminsAsync()
        {
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            var result = new CommonResult<IEnumerable<AdminDTO>>();
            if (verifyAdminToken)
            {
                var admins = (await _adminRepository.GetAllAdminsAsync(user.CompanyId)).ToList();
                foreach (var admin in admins)
                {
                    admin.CompanyName = (await _companyRepository.GetAsync(admin.CompanyId)).CompanyName;
                }
                result.Data = admins;
            }
            else
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            return result;
        }

        public async Task<IEnumerable<DepartmentDTO>> GetDepartmentsOfCompanyAsync()
        {
            var user = _userService.GetCurrentUser();

            var departments = (await _departmentRepository.GetDepartmentsOfCompanyAsync(user.CompanyId)).ToList();
            var com = await _companyRepository.GetAsync(user.CompanyId);
            foreach (var item in departments)
            {
                item.CompanyName = com.CompanyName;
            }
            return departments;
        }

        public async Task<CommonResult<IEnumerable<DepartmentDTO>>> CreateDepartmentAsync(string departmentName)
        {
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            var result = new CommonResult<IEnumerable<DepartmentDTO>>();
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            var newDp = new DepartmentDTO
            {
                CompanyId = user.CompanyId,
                CompanyRuleId = 0,
                DepartmentName = departmentName,
            };
            await _departmentRepository.InsertAsync(newDp);
            result.Data = await GetDepartmentsOfCompanyAsync();
            return result;
        }

        public async Task<CommonResult<IEnumerable<StaffDTO>>> GetPartTimeStaffsAsync(int month)
        {
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            var result = new CommonResult<IEnumerable<StaffDTO>>();
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            int year = DateTimeHelper.TaipeiNow.Year;  // 当前年份
            DateTime start = new DateTime(year, month, 1); // 该月的第一天
            DateTime end = start.AddMonths(1).AddDays(-1); // 该月的最后一天

            var parttimes = (await _staffRepository.GetAllParttimeStaffAsync(user.CompanyId)).ToList();
            List<StaffDTO> staffsToRemove = new List<StaffDTO>();

            foreach (var staff in parttimes)
            {
                int totalMinutes = 0;
                var checks = await _checkRecordsRepository.GetCheckRecordListAsync(staff.id, user.CompanyId, start, end);
                foreach (var check in checks)
                {
                    if (!check.CheckOutTime.HasValue || !check.CheckInTime.HasValue)
                    {
                        break; // 若其中一个没有值，立即退出循环
                    }

                    TimeSpan difference = check.CheckOutTime.Value - check.CheckInTime.Value;
                    int workminutes = (int)difference.TotalMinutes;
                    totalMinutes += workminutes;
                }
                int hours = totalMinutes / 60;  // 使用整数除法得到小时数
                int minutes = totalMinutes % 60; // 使用模数运算符得到余数分钟
                int overtimemoney = 0;
                var overtimesList = (await _overTimeLogRepository.GetOverTimeLogOfPeriodAfterValidateAsync(staff.id, user.CompanyId, start, end)).ToList();
                foreach (var log in overtimesList)
                {
                    var overFirstStepMoney = 0;
                    var overSecStepStepMoney = 0;
                    if (log.OverHours <= 2)
                    {
                        overFirstStepMoney = (int)Math.Round(log.OverHours * staff.ParttimeMoney.Value * 1.33);
                        overtimemoney += overFirstStepMoney;
                    }
                    else if (log.OverHours > 2)
                    {
                        overFirstStepMoney = (int)Math.Round(2 * staff.ParttimeMoney.Value * 1.33);
                        overtimemoney += overFirstStepMoney;
                        overSecStepStepMoney = (int)Math.Round((log.OverHours - 2) * staff.ParttimeMoney.Value * 1.66);
                        overtimemoney += overSecStepStepMoney;
                    }
                }
                var overtimesHours = overtimesList.Sum(x => x.OverHours);
                if (hours == 0 && minutes == 0)
                {
                    staffsToRemove.Add(staff);
                    continue;
                }
                staff.TotalPartimeHours = hours;
                staff.TotalPartimeMinutes = minutes;
                staff.ParttimeOverTimeHours = overtimesHours;
                staff.ParttimeOverTimeTotalMony = overtimemoney;
            }

            foreach (var staff in staffsToRemove)
            {
                parttimes.Remove(staff);
            }
            result.Data = parttimes;
            return result;
        }

        public async Task<CommonResult<IEnumerable<AmendCheckRecordDTO>>> GetGetAmendrecordsAsync(DateTime start, DateTime end)
        {
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            var result = new CommonResult<IEnumerable<AmendCheckRecordDTO>>();
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            var applications = await _amendCheckRecordRepository.GetAllAmendCheckRecordAsync(user.CompanyId, start, end);

            result.Data = applications;
            return result;
        }

        public async Task<CommonResult<IEnumerable<DepartmentDTO>>> UpdateDepartmentAsync(IEnumerable<DepartmentDTO> dtos)
        {
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            var result = new CommonResult<IEnumerable<DepartmentDTO>>();
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
            }
            foreach (var dto in dtos.ToList())
            {
                var rule = await _companyRuleRepository.GetCompanyRuleAsync(user.CompanyId, dto.id);
                var staffs = await _staffRepository.GetAllStaffAsync(user.CompanyId);
                foreach (var staff in staffs)
                {
                    if (staff.DepartmentId == dto.id && staff.Department != dto.DepartmentName)
                    {
                        staff.Department = dto.DepartmentName;
                        await _staffRepository.UpdateAsync(staff);
                    }
                }
                if (rule.DepartmentName != dto.DepartmentName)
                {
                    rule.DepartmentName = dto.DepartmentName;
                    await _companyRuleRepository.UpdateAsync(rule);
                }
                await _departmentRepository.UpdateAsync(dto);
            }

            result.Data = await GetDepartmentsOfCompanyAsync();
            return result;
        }

        public async Task<IEnumerable<CompanyRuleDTO>> GetRulesOfCompanyAsync()
        {
            var user = _userService.GetCurrentUser();
            var departments = await _companyRuleRepository.GetCompanyRulesAsync(user.CompanyId);

            return departments;
        }

        public async Task<CommonResult<bool>> UpdateRulesAsync(IEnumerable<CompanyRuleDTO> requests)
        {
            var result = new CommonResult<bool>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            try
            {
                foreach (var re in requests)
                {
                    if (string.IsNullOrEmpty(re.WorkAddress))
                    {
                        result.AddError("請輸入正確地址");
                        return result;
                    }

                    try
                    {
                        var coordinate = await _geocodingService.GetCoordinates(re.WorkAddress);
                        re.Latitude = coordinate.Latitude;
                        re.Longitude = coordinate.Longitude;
                    }
                    catch (Exception ex)
                    {
                        result.AddError(ex.Message);
                        return result;
                    }

                    re.EditDate = DateTimeHelper.TaipeiNow;
                    re.Editor = user.StaffName;
                    re.DepartmentName = (await _departmentRepository.GetAsync(re.DepartmentId)).DepartmentName;

                    var newDTO = await _companyRuleRepository.UpdateAsync(re);

                    var department = await _departmentRepository.GetAsync(newDTO.DepartmentId);
                    if (department == null)
                    {
                        result.AddError("找不到該部門");
                        return result;
                    }
                    department.CompanyRuleId = newDTO.id;
                    await _departmentRepository.UpdateAsync(department);
                }

                result.Data = true;
            }
            catch (Exception ex)
            {
                result.AddError(ex.ToString());
                result.Data = false;
            }
            return result;
        }

        public async Task<CommonResult> CreateRuleAsync(CompanyRuleDTO dto)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            try
            {
                var exist = await _companyRuleRepository.GetCompanyRuleAsync(dto.CompanyId, dto.DepartmentId);
                if (exist != null)
                {
                    result.AddError("該部門已有規定 不得重複新增");
                    return result;
                }

                if (string.IsNullOrEmpty(dto.WorkAddress))
                {
                    result.AddError("請輸入正確地址");
                    return result;
                }

                try
                {
                    var coordinate = await _geocodingService.GetCoordinates(dto.WorkAddress);
                    dto.Latitude = coordinate.Latitude;
                    dto.Longitude = coordinate.Longitude;
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    return result;
                }
                dto.Creator = user.StaffName;
                dto.NeedWorkMinute = dto.NeedWorkMinute * 60;
                dto.CreateDate = DateTimeHelper.TaipeiNow;
                dto.DepartmentName = (await _departmentRepository.GetAsync(dto.DepartmentId)).DepartmentName;

                dto = await _companyRuleRepository.InsertAsync(dto);

                var department = await _departmentRepository.GetAsync(dto.DepartmentId);
                if (department == null)
                {
                    result.AddError("找不到該部門");
                    return result;
                }
                department.CompanyRuleId = dto.id;
                await _departmentRepository.UpdateAsync(department);
            }
            catch (Exception ex)
            {
                result.AddError(ex.ToString());
            }
            return result;
        }

        public async Task<CommonResult<CheckRecordsViewDTO>> GetStaffAttendanceAsync(int staffId, DateTime start, DateTime end)
        {
            var result = new CommonResult<CheckRecordsViewDTO>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            var checkRecordView = new CheckRecordsViewDTO();
            var staffCheckRecords = await _checkRecordsRepository.GetCheckRecordListAsync(user.CompanyId, staffId, start, end);
            var vacationLogs = await _vacationLogRepository.GetPeriodVacationLogsAsync(staffId, user.CompanyId, start, end);
            int totalCheckInLateDays = staffCheckRecords.Select(x => x.IsCheckInLate == 1).Count();
            int totalCheckInLateMinutes = staffCheckRecords.Where(x => x.IsCheckInLate == 1).Sum(x => x.CheckInLateTimes ?? 0);
            int totalCheckOutEarlyDays = staffCheckRecords.Select(x => x.IsCheckOutEarly == 1).Count();
            int totalCheckOutEarlyMinutes = staffCheckRecords.Where(x => x.IsCheckOutEarly == 1).Sum(x => x.CheckOutEarlyTimes ?? 0);
            int outLocationCheckInDays = staffCheckRecords.Select(x => x.IsCheckInOutLocation).Count();
            int outLocationCheckOutDays = staffCheckRecords.Select(x => x.IsCheckOutOutLocation).Count();

            checkRecordView.CheckRecords = staffCheckRecords;
            checkRecordView.VacationLogs = vacationLogs;
            checkRecordView.TotalCheckInLateDays = totalCheckInLateDays;
            checkRecordView.TotalCheckInLateMinutes = totalCheckInLateMinutes;
            checkRecordView.TotalCheckOutEarlyDays = totalCheckOutEarlyDays;
            checkRecordView.TotalCheckOutEarlyMinutes = totalCheckOutEarlyMinutes;
            checkRecordView.OutLocationCheckInDays = outLocationCheckInDays;
            checkRecordView.OutLocationCheckOutDays = outLocationCheckOutDays;
            result.Data = checkRecordView;
            return result;
        }

        public async Task<CommonResult<List<AdminDTO>>> CreateOrEditAdminAsync(AdminDTO adminDTO)
        {
            var result = new CommonResult<List<AdminDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }

            if (adminDTO.id == 0)
            {
                var adminCount = await _adminRepository.GetAllAdminsCountAsync(user.CompanyId);
                var company = await _companyRepository.GetAsync(user.CompanyId);
                var contract = await _contractTypeRepository.GetContractTypeAsync(company.ContractType.Value);
                if ((adminCount + 1) > contract.AdminLimit)
                {
                    result.AddError("超出合約管理人數限制");

                    return result;
                }
                var isExist = await _adminRepository.VerifyAdminAccountAsync(adminDTO.Account, adminDTO.id);
                if (isExist)
                {
                    result.AddError("該帳號已註冊過 請換帳號");
                    return result;
                }
                adminDTO.Password = CryptHelper.SaltHashPlus(adminDTO.Password);
                await _adminRepository.InsertAsync(adminDTO);
                var admins = await GetAllAdminsAsync();
                result.Data = admins.Data.ToList();
            }
            else
            {
                var isExist = await _adminRepository.VerifyAdminAccountAsync(adminDTO.Account, adminDTO.id);
                if (isExist)
                {
                    result.AddError("該帳號已註冊過 請換帳號");
                    return result;
                }
                if (user.Id != adminDTO.id)
                {
                    var editAdmin = await _adminRepository.GetAsync(adminDTO.id);
                    adminDTO.Password = editAdmin.Password;
                    adminDTO.AdminToken = editAdmin.AdminToken;
                    await _adminRepository.UpdateAsync(adminDTO);
                    var admins = await GetAllAdminsAsync();
                    result.Data = admins.Data.ToList();
                }
                else
                {
                    var admins = await GetAllAdminsAsync();
                    if (!string.IsNullOrEmpty(adminDTO.PrePassword))
                    {
                        var ad = await _adminRepository.GetAsync(user.Id);
                        adminDTO.AdminToken = ad.AdminToken;
                        if (CryptHelper.VerifySaltHashPlus(ad.Password, adminDTO.PrePassword))
                        {
                            ad.Password = CryptHelper.SaltHashPlus(adminDTO.Password);
                            await _adminRepository.UpdateAsync(adminDTO);
                            result.Data = admins.Data.ToList();
                        }
                        else
                        {
                            result.AddError("先前密碼驗證錯誤");
                            return result;
                        }
                    }
                    await _adminRepository.UpdateAsync(adminDTO);
                    result.Data = admins.Data.ToList();
                }
            }
            return result;
        }

        public async Task<CommonResult<IEnumerable<VacationLogDTO>>> GetAllVacationApplicationAsync(DateTime start, DateTime end)
        {
            var result = new CommonResult<IEnumerable<VacationLogDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            var allVacations = (await _vacationLogRepository.GetPeriodVacationLogsAsync(user.Id, start, end)).ToList();
            foreach (var log in allVacations)
            {
                var staff = await _staffRepository.GetUsingStaffAsync(log.StaffId, log.CompanyId);
                if (staff == null)
                {
                    result.AddError("找不到該員工 系統錯誤");
                    return result;
                }
                log.StaffName = staff.StaffName;
                log.StaffNo = staff.StaffNo;
                switch (log.VacationType)
                {
                    case 0:
                        log.VacationTypeName = "特休";
                        break;

                    case 1:
                        log.VacationTypeName = "病假";
                        break;

                    case 2:
                        log.VacationTypeName = "事假";
                        break;

                    case 3:
                        log.VacationTypeName = "產假";
                        break;

                    case 4:
                        log.VacationTypeName = "喪假";
                        break;

                    case 5:
                        log.VacationTypeName = "婚假";
                        break;

                    case 6:
                        log.VacationTypeName = "公假";
                        break;

                    case 7:
                        log.VacationTypeName = "工傷病假";
                        break;

                    case 8:
                        log.VacationTypeName = "生理假";
                        break;

                    case 9:
                        log.VacationTypeName = "育嬰留職停薪假";
                        break;

                    case 10:
                        log.VacationTypeName = "安胎假";
                        break;

                    case 11:
                        log.VacationTypeName = "產檢假";
                        break;

                    default:
                        result.AddError("出事了 假別出現錯誤");
                        return result;
                }
            }
            result.Data = allVacations;
            return result;
        }

        public async Task<CommonResult<IEnumerable<OverTimeLogDTO>>> GetAllOvertimeAsync(DateTime start, DateTime end)
        {
            var result = new CommonResult<IEnumerable<OverTimeLogDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }

            var overtimeList = (await _overTimeLogRepository.GetOverTimeLogOfPeriodAsync(user.CompanyId, start, end)).ToList();

            foreach (var overtime in overtimeList)
            {
                var staff = await _staffRepository.GetUsingStaffAsync(overtime.StaffId, overtime.CompanyId);
                if (staff == null)
                {
                    result.AddError("找不到該員工 系統錯誤");
                    return result;
                }
                overtime.StaffName = staff.StaffName;
                overtime.OvertimeDate = overtime.OvertimeDate;
            }
            result.Data = overtimeList;
            return result;
        }

        public async Task<CommonResult<IEnumerable<StaffDTO>>> GetAllDaySalaryStaffAsync()
        {
            var result = new CommonResult<IEnumerable<StaffDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            var staffs = await _staffRepository.GetAllDaySalaryStaffAsync(user.CompanyId);

            result.Data = staffs;
            return result;
        }

        public async Task<CommonResult<DayStaffDTO>> GetDayStaffSalaryView(int staffId, int month)
        {
            var result = new CommonResult<DayStaffDTO>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            int year = DateTimeHelper.TaipeiNow.Year;
            DateTime start = new DateTime(year, month, 1);
            DateTime end = start.AddMonths(1).AddDays(-1);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }

            var staff = await _staffRepository.GetAsync(staffId);
            if (staff.CompanyId != user.CompanyId)
            {
                result.AddError("操作者沒有該公司讀取權杖");
                return result;
            }
            var records = (await _checkRecordsRepository.GetCheckRecordListAsync(staffId, user.CompanyId, start, end)).ToList();
            int workDays = 0;
            int lateOrEarlyDays = 0;
            int outLocationDays = 0;
            foreach (var record in records)
            {
                if (record.CheckInTime.HasValue && record.CheckOutTime.HasValue)
                {
                    if (record.IsCheckInLate == 1 || record.IsCheckOutEarly == 1)
                    {
                        lateOrEarlyDays += 1;
                    }
                    if (record.IsCheckInOutLocation == 1 || record.IsCheckOutOutLocation == 1)
                    {
                        outLocationDays += 1;
                    }
                    workDays += 1;
                }
            }
            int overtimemoney = 0;
            staff.ParttimeMoney = staff.DaySalary.Value / 8;
            var overtimesList = (await _overTimeLogRepository.GetOverTimeLogOfPeriodAfterValidateAsync(staff.id, user.CompanyId, start, end)).ToList();
            foreach (var log in overtimesList)
            {
                var overFirstStepMoney = 0;
                var overSecStepStepMoney = 0;
                if (log.OverHours <= 2)
                {
                    overFirstStepMoney = (int)Math.Round(log.OverHours * staff.ParttimeMoney.Value * 1.33);
                    overtimemoney += overFirstStepMoney;
                }
                else if (log.OverHours > 2)
                {
                    overFirstStepMoney = (int)Math.Round(2 * staff.ParttimeMoney.Value * 1.33);
                    overtimemoney += overFirstStepMoney;
                    overSecStepStepMoney = (int)Math.Round((log.OverHours - 2) * staff.ParttimeMoney.Value * 1.66);
                    overtimemoney += overSecStepStepMoney;
                }
            }
            var overtimesHours = overtimesList.Sum(x => x.OverHours);

            var dayStaffSalaryView = new DayStaffDTO
            {
                StaffId = staffId,
                StaffNo = staff.StaffNo,
                CompanyId = staff.CompanyId,
                Department = staff.Department,
                LevelPosition = staff.LevelPosition,
                WorkLocation = staff.WorkLocation,
                Status = staff.Status,
                StaffName = staff.StaffName,
                DaySalary = staff.DaySalary,
                WorkDays = $"工作日:{workDays} 天",
                LateOrEarlyDays = $"遲到早退:{lateOrEarlyDays} 天",
                OutLocationDays = $"定位外打卡:{outLocationDays} 天",
                OverTimeHours = overtimesHours,
                OverTimeSalary = overtimemoney,
                Month = $"{month}月份",
                TotalDaysSalary = workDays * (staff.DaySalary ?? 0)
            };

            result.Data = dayStaffSalaryView;
            return result;
        }

        public async Task<(List<CheckRecordsDTO> list, StaffDTO? staff)> GetExcelDatasAsync(int staffId, int month)
        {
            // 1. Validation and User Verification
            var user = _userService.GetCurrentUser();
            // 2. Fetch Data
            int year = DateTimeHelper.TaipeiNow.Year;
            DateTime start = new DateTime(year, month, 1);
            DateTime end = start.AddMonths(1).AddDays(-1);
            var records = (await _checkRecordsRepository.GetCheckRecordListAsync(staffId, user.CompanyId, start, end)).ToList();
            var staff = await _staffRepository.GetUsingStaffAsync(staffId, user.CompanyId);

            return (records, staff);
        }

        public async Task<List<IncomeLogsDTO>> GetSalaryExcelDatasAsync(int month)
        {
            // 1. Validation and User Verification
            var user = _userService.GetCurrentUser();
            // 2. Fetch Data
            int year = DateTimeHelper.TaipeiNow.Year;
            DateTime start = new DateTime(year, month, 1);
            DateTime end = start.AddMonths(1).AddDays(-1);
            var records = (await _incomeLogsRepository.GetCompanyIncomeLogsAsync(user.CompanyId, start, end)).ToList();
            foreach (var record in records)
            {
                var staff = await _staffRepository.GetUsingStaffAsync(record.StaffId, user.CompanyId);
                if (staff != null)
                {
                    record.StaffName = staff.StaffName;
                }
            }

            return records;
        }

        public async Task<CommonResult<VacationLogDTO>> VerifyVacationsAsync(int vacationId, bool isPass)
        {
            var result = new CommonResult<VacationLogDTO>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            var vacationLog = await _vacationLogRepository.GetAsync(vacationId);
            if (vacationLog == null)
            {
                result.AddError("找不到該請假紀錄");
                return result;
            }
            vacationLog.IsPass = isPass ? 1 : -1;
            vacationLog.ApproverName = user.StaffName;
            vacationLog.ApproverId = user.Id;
            vacationLog.AuditDate = DateTimeHelper.TaipeiNow;
            if (isPass)
            {
                var staff = await _staffRepository.GetAsync(vacationLog.StaffId);
                switch (vacationLog.VacationType)
                {
                    case 0: //特休
                        if (staff.SpecialRestHours >= vacationLog.Hours)
                        {
                            staff.SpecialRestHours -= vacationLog.Hours;
                        }
                        else
                        {
                            int remainingHours = vacationLog.Hours - staff.SpecialRestHours;
                            int quotient = (int)Math.Ceiling(remainingHours / 8.0);
                            staff.SpecialRestDays -= quotient;
                            if (staff.SpecialRestDays < 0)
                            {
                                vacationLog.IsPass = -1;
                                vacationLog.ApproverName = "系統自動拒絕";
                                result.AddError("該假別已超過該員工請假額度");
                            }
                            staff.SpecialRestHours = (quotient * 8) - remainingHours;
                        }

                        break;

                    case 1://病假
                        if (staff.SickHours >= vacationLog.Hours)
                        {
                            staff.SickHours -= vacationLog.Hours;
                        }
                        else
                        {
                            int remainingHours = vacationLog.Hours - staff.SickHours;
                            int quotient = (int)Math.Ceiling(remainingHours / 8.0);
                            staff.SickDays -= quotient;
                            if (staff.SickDays < 0)
                            {
                                vacationLog.IsPass = -1;
                                vacationLog.ApproverName = "系統自動拒絕";
                                result.AddError("該假別已超過該員工請假額度");
                            }
                            staff.SickHours = (quotient * 8) - remainingHours;
                        }

                        break;

                    case 2: //事假

                        if (staff.ThingHours >= vacationLog.Hours)
                        {
                            staff.ThingHours -= vacationLog.Hours;
                        }
                        else
                        {
                            int remainingHours = vacationLog.Hours - staff.ThingHours;
                            int quotient = (int)Math.Ceiling(remainingHours / 8.0);
                            staff.ThingDays -= quotient;
                            if (staff.ThingDays < 0)
                            {
                                vacationLog.IsPass = -1;
                                vacationLog.ApproverName = "系統自動拒絕";
                                result.AddError("該假別已超過該員工請假額度");
                            }
                            staff.ThingHours = (quotient * 8) - remainingHours;
                        }
                        break;

                    case 3://產假

                        if (staff.ChildbirthHours >= vacationLog.Hours)
                        {
                            staff.ChildbirthHours -= vacationLog.Hours;
                        }
                        else
                        {
                            int remainingHours = vacationLog.Hours - staff.ChildbirthHours;
                            int quotient = (int)Math.Ceiling(remainingHours / 8.0);
                            staff.ChildbirthDays -= quotient;
                            if (staff.ChildbirthDays < 0)
                            {
                                vacationLog.IsPass = -1;
                                vacationLog.ApproverName = "系統自動拒絕";
                                result.AddError("該假別已超過該員工請假額度");
                            }
                            staff.ChildbirthHours = (quotient * 8) - remainingHours;
                        }
                        break;

                    case 4://喪假

                        if (staff.DeathHours >= vacationLog.Hours)
                        {
                            staff.DeathHours -= vacationLog.Hours;
                        }
                        else
                        {
                            int remainingHours = vacationLog.Hours - staff.DeathHours;
                            int quotient = (int)Math.Ceiling(remainingHours / 8.0);
                            staff.DeathDays -= quotient;
                            if (staff.DeathDays < 0)
                            {
                                vacationLog.IsPass = -1;
                                vacationLog.ApproverName = "系統自動拒絕";
                                result.AddError("該假別已超過該員工請假額度");
                            }
                            staff.DeathHours = (quotient * 8) - remainingHours;
                        }
                        break;

                    case 5://婚假

                        if (staff.MarryHours >= vacationLog.Hours)
                        {
                            staff.MarryHours -= vacationLog.Hours;
                        }
                        else
                        {
                            int remainingHours = vacationLog.Hours - staff.MarryHours;
                            int quotient = (int)Math.Ceiling(remainingHours / 8.0);
                            staff.MarryDays -= quotient;
                            if (staff.MarryDays < 0)
                            {
                                vacationLog.IsPass = -1;
                                vacationLog.ApproverName = "系統自動拒絕";
                                result.AddError("該假別已超過該員工請假額度");
                            }
                            staff.MarryHours = (quotient * 8) - remainingHours;
                        }
                        break;

                    case 6://公假
                        break;

                    case 7://工傷病假
                        break;

                    case 8://生理假

                        if (staff.MenstruationHours >= vacationLog.Hours)
                        {
                            staff.MenstruationHours -= vacationLog.Hours;
                        }
                        else
                        {
                            int remainingHours = vacationLog.Hours - staff.MenstruationHours;
                            int quotient = (int)Math.Ceiling(remainingHours / 8.0);
                            staff.MenstruationDays -= quotient;
                            if (staff.MenstruationDays < 0)
                            {
                                vacationLog.IsPass = -1;
                                vacationLog.ApproverName = "系統自動拒絕";
                                result.AddError("該假別已超過該員工請假額度");
                            }
                            staff.MenstruationHours = (quotient * 8) - remainingHours;
                        }
                        break;

                    case 9://育嬰留職停薪假

                        if (staff.TackeCareBabyHours >= vacationLog.Hours)
                        {
                            staff.TackeCareBabyHours -= vacationLog.Hours;
                        }
                        else
                        {
                            int remainingHours = vacationLog.Hours - staff.TackeCareBabyHours;
                            int quotient = (int)Math.Ceiling(remainingHours / 8.0);
                            staff.TackeCareBabyDays -= quotient;
                            if (staff.TackeCareBabyDays < 0)
                            {
                                vacationLog.IsPass = -1;
                                vacationLog.ApproverName = "系統自動拒絕";
                                result.AddError("該假別已超過該員工請假額度");
                            }
                            staff.TackeCareBabyHours = (quotient * 8) - remainingHours;
                        }
                        break;

                    case 10://安胎

                        if (staff.TocolysisHours >= vacationLog.Hours)
                        {
                            staff.TocolysisHours -= vacationLog.Hours;
                        }
                        else
                        {
                            int remainingHours = vacationLog.Hours - staff.TocolysisHours;
                            int quotient = (int)Math.Ceiling(remainingHours / 8.0);
                            staff.TocolysisDays -= quotient;
                            if (staff.TocolysisDays < 0)
                            {
                                vacationLog.IsPass = -1;
                                vacationLog.ApproverName = "系統自動拒絕";
                                result.AddError("該假別已超過該員工請假額度");
                            }
                            staff.TocolysisHours = (quotient * 8) - remainingHours;
                        }
                        break;

                    case 11://產檢
                        if (staff.PrenatalCheckUpHours >= vacationLog.Hours)
                        {
                            staff.PrenatalCheckUpHours -= vacationLog.Hours;
                        }
                        else
                        {
                            int remainingHours = vacationLog.Hours - staff.PrenatalCheckUpHours;
                            int quotient = (int)Math.Ceiling(remainingHours / 8.0);
                            staff.PrenatalCheckUpDays -= quotient;
                            if (staff.PrenatalCheckUpDays < 0)
                            {
                                vacationLog.IsPass = -1;
                                vacationLog.ApproverName = "系統自動拒絕";
                                result.AddError("該假別已超過該員工請假額度");
                            }
                            staff.PrenatalCheckUpHours = (quotient * 8) - remainingHours;
                        }

                        break;
                }
                if (result.Success)
                {
                    await _staffRepository.UpdateAsync(staff);
                }
            }
            result.Data = await _vacationLogRepository.UpdateAsync(vacationLog);
            return result;
        }

        public async Task<CommonResult<SalaryViewDTO>> GetStaffSalaryViewAsync(int staffId)
        {
            var result = new CommonResult<SalaryViewDTO>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            var staff = await _staffRepository.GetUsingStaffAsync(staffId, user.CompanyId);
            if (staff.CompanyId != user.CompanyId)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }

            var salaryView = new SalaryViewDTO();

            DateTime today = DateTimeHelper.TaipeiNow;
            DateTime start = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
            DateTime end = new DateTime(today.Year, today.Month, 1).AddDays(-1);

            var staffCheckRecords = await _checkRecordsRepository.GetCheckRecordListAsync(user.CompanyId, staffId, start, end);
            var staffVacationlogs = await _vacationLogRepository.GetPeriodVacationLogsAsync(staffId, user.CompanyId, start, end);
            bool vacationNotAlreadyChecked = staffVacationlogs.Any(x => x.IsPass == 0);
            if (vacationNotAlreadyChecked)
            {
                result.AddError("請將該員工的請假全都審核完畢");
                return result;
            }

            var setting = await _salarySettingRepository.GetSalarySettingAsync(staffId, user.CompanyId);

            setting.StaffName = staff.StaffName;
            setting.StaffNo = staff.StaffNo;
            salaryView.SalarySetting = setting;

            salaryView.TotalCheckInLateDays = staffCheckRecords.Select(x => x.IsCheckInLate == 1).Count();
            salaryView.TotalCheckInLateMinutes = staffCheckRecords.Where(x => x.IsCheckInLate == 1).Sum(x => x.CheckInLateTimes ?? 0);
            salaryView.TotalCheckOutEarlyDays = staffCheckRecords.Select(x => x.IsCheckOutEarly == 1).Count();
            salaryView.TotalCheckOutEarlyMinutes = staffCheckRecords.Where(x => x.IsCheckOutEarly == 1).Sum(x => x.CheckOutEarlyTimes ?? 0);
            salaryView.OutLocationCheckInDays = staffCheckRecords.Select(x => x.IsCheckInOutLocation).Count();
            salaryView.OutLocationCheckOutDays = staffCheckRecords.Select(x => x.IsCheckOutOutLocation).Count();

            double total0Hours = staffVacationlogs
            .Where(x => x.VacationType == 0 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.SpecialRestDays = (int)(total0Hours / 8);
            salaryView.SpecialRestHours = (int)(total0Hours % 8);
            salaryView.TotalSpecialRestHours = (int)total0Hours;

            double total1Hours = staffVacationlogs
            .Where(x => x.VacationType == 1 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.SickDays = (int)(total1Hours / 8);
            salaryView.SickHours = (int)(total1Hours % 8);
            salaryView.TotalSickHours = (int)total1Hours;

            double total2Hours = staffVacationlogs
            .Where(x => x.VacationType == 2 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.ThingDays = (int)(total2Hours / 8);
            salaryView.ThingHours = (int)(total2Hours % 8);
            salaryView.TotalThingHours = (int)total2Hours;

            double total3Hours = staffVacationlogs
            .Where(x => x.VacationType == 3 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.ChildbirthDays = (int)(total3Hours / 8);
            salaryView.ChildbirthHours = (int)(total3Hours % 8);
            salaryView.TotalChildbirthHours = (int)total3Hours;

            double total4Hours = staffVacationlogs
            .Where(x => x.VacationType == 4 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.DeathDays = (int)(total4Hours / 8);
            salaryView.DeathHours = (int)(total4Hours % 8);
            salaryView.TotalDeathHours = (int)total4Hours;

            double total5Hours = staffVacationlogs
            .Where(x => x.VacationType == 5 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.MarryDays = (int)(total5Hours / 8);
            salaryView.MarryHours = (int)(total5Hours % 8);
            salaryView.TotalMarryHours = (int)total5Hours;

            double total6Hours = staffVacationlogs
            .Where(x => x.VacationType == 6 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.WorkthingDays = (int)(total6Hours / 8);
            salaryView.WorkthingHours = (int)(total6Hours % 8);
            salaryView.TotalWorkthingHours = (int)total6Hours;

            double total7Hours = staffVacationlogs
            .Where(x => x.VacationType == 7 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.WorkhurtDays = (int)(total7Hours / 8);
            salaryView.WorkhurtHours = (int)(total7Hours % 8);
            salaryView.TotalWorkhurtHours = (int)total7Hours;

            double total8Hours = staffVacationlogs
            .Where(x => x.VacationType == 8 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.MenstruationDays = (int)(total8Hours / 8);
            salaryView.MenstruationHours = (int)(total8Hours % 8);
            salaryView.TotalMenstruationHours = (int)total8Hours;

            double total9Hours = staffVacationlogs
            .Where(x => x.VacationType == 9 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.TackeCareBabyDays = (int)(total9Hours / 8);
            salaryView.TackeCareBabyHours = (int)(total9Hours % 8);
            salaryView.TotalTackeCareBabyHours = (int)total9Hours;

            double total10Hours = staffVacationlogs
            .Where(x => x.VacationType == 10 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.TocolysisDays = (int)(total10Hours / 8);
            salaryView.TocolysisHours = (int)(total10Hours % 8);
            salaryView.TotalTocolysisHours = (int)total10Hours;

            double total11Hours = staffVacationlogs
            .Where(x => x.VacationType == 11 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.PrenatalCheckUpDays = (int)(total11Hours / 8);
            salaryView.PrenatalCheckUpHours = (int)(total11Hours % 8);
            salaryView.TotalPrenatalCheckUpHours = (int)total11Hours;

            // 首先，将int值转换为decimal，确保计算过程中的精度
            decimal basicSalary = salaryView.SalarySetting.BasicSalary;
            decimal fullCheckInMoney = salaryView.SalarySetting.FullCheckInMoney;

            // 执行计算。由于我们已经使用了decimal类型，除法和乘法都将保持精度。
            decimal tempValue = (basicSalary + fullCheckInMoney) / 30.0m / 8.0m;

            // 这里，我们可以根据需要进行四舍五入。假设我们想要保留两位小数。
            decimal roundedValue = Math.Round(tempValue, 3); // 如果你希望结果更接近实际数值，可以增加小数位数

            // 如果你需要将结果作为整数返回，可以进行转换。但是请注意，这可能会导致一些精度损失。
            decimal perHourSalary = roundedValue;

            salaryView.TotalSalaryNoOvertime = salaryView.SalarySetting.BasicSalary + salaryView.SalarySetting.FullCheckInMoney - salaryView.TotalSickHours * perHourSalary / 2 -
                                               salaryView.TotalThingHours * perHourSalary - salaryView.TotalMenstruationHours * perHourSalary / 2;
            salaryView.PerHourSalary = perHourSalary;

            decimal overtimemoney = 0;
            var overtimesList = (await _overTimeLogRepository.GetOverTimeLogOfPeriodAfterValidateAsync(staff.id, user.CompanyId, start, end)).ToList();
            foreach (var log in overtimesList)
            {
                decimal overFirstStepMoney = 0;
                decimal overSecStepStepMoney = 0;
                if (log.OverHours <= 2)
                {
                    decimal overHoursDecimal = Convert.ToDecimal(log.OverHours);

                    // 确保常数也是 decimal 类型
                    decimal multiplier = 1.33m;

                    // 现在，所有的值都是 decimal 类型，你可以进行计算
                    overFirstStepMoney = overHoursDecimal * perHourSalary * multiplier;
                    overtimemoney += overFirstStepMoney;
                }
                else if (log.OverHours > 2)
                {
                    overFirstStepMoney = 2 * perHourSalary * 1.33m;
                    overtimemoney += overFirstStepMoney;
                    overSecStepStepMoney = (log.OverHours - 2) * perHourSalary * 1.66m;
                    overtimemoney += overSecStepStepMoney;
                }
            }
            salaryView.OverTimeHours = overtimesList.Sum(x => x.OverHours);
            decimal roundedOvertimeMoney = Math.Round(overtimemoney, MidpointRounding.AwayFromZero);
            salaryView.OverTimeMoney = Convert.ToInt32(roundedOvertimeMoney);
            salaryView.FoodSuportMoney = setting.FoodSuportMoney.HasValue ? setting.FoodSuportMoney.Value : 0;
            result.Data = salaryView;
            return result;
        }

        public async Task<CommonResult> ReadyToPayMoneyAsync(IncomeLogsDTO dto, bool isChange)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            dto.IssueDate = DateTimeHelper.TaipeiNow;
            dto.CompanyId = user.CompanyId;

            if (await _incomeLogsRepository.IsRepeatPayAsync(dto.StaffId, user.CompanyId, dto.SalaryOfMonth))
            {
                result.AddError("該月已發放薪資");
                return result;
            }

            if (isChange)
            {
                DateTime firstDayOfLastMonth = dto.IssueDate.AddMonths(-1);
                firstDayOfLastMonth = new DateTime(firstDayOfLastMonth.Year, firstDayOfLastMonth.Month, 1);
                DateTime lastDayOfLastMonth = dto.IssueDate.AddDays(-dto.IssueDate.Day);
                var staff = await _staffRepository.GetUsingStaffAsync(dto.StaffId, user.CompanyId);
                var lastMonthOverTime = (await _overTimeLogRepository.GetOverTimeLogOfPeriodAfterValidateAsync(dto.StaffId, user.CompanyId, firstDayOfLastMonth, lastDayOfLastMonth)).Sum(x => x.OverHours);
                if (staff == null)
                {
                    result.AddError("系統錯誤 找不到該名員工");
                    return result;
                }
                if (staff.OverTimeHours < lastMonthOverTime)
                {
                    result.AddError("該員工已無加班時數");
                    return result;
                }
                staff.OverTimeHours -= lastMonthOverTime;
                await _staffRepository.UpdateAsync(staff);
                await _incomeLogsRepository.InsertAsync(dto);
            }
            else
            {
                await _incomeLogsRepository.InsertAsync(dto);
            }

            return result;
        }

        public async Task<CommonResult> VerifyOvertimeApplyAsync(int id, bool isPass)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            var overtimeLog = await _overTimeLogRepository.GetAsync(id);
            if (overtimeLog == null)
            {
                result.AddError("找不到該申請紀錄 系統錯誤");
                return result;
            }
            overtimeLog.Inspector = user.StaffName;
            overtimeLog.InspectorId = user.Id;
            overtimeLog.ValidateDate = DateTimeHelper.TaipeiNow;
            overtimeLog.IsValidate = isPass ? 1 : -1;
            await _overTimeLogRepository.UpdateAsync(overtimeLog);

            return result;
        }

        public async Task<CommonResult> VerifyAmendrecordAsync(int id, bool isPass)
        {
            var result = new CommonResult();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            var amendrecord = await _amendCheckRecordRepository.GetAsync(id);
            if (amendrecord == null)
            {
                result.AddError("找不到該申請紀錄 系統錯誤");
                return result;
            }

            if (isPass)
            {
                var staff = await _staffRepository.GetUsingStaffAsync(amendrecord.StaffId, amendrecord.CompanyId);
                var rule = await _companyRuleRepository.GetCompanyRuleAsync(staff.CompanyId, staff.DepartmentId);

                DateTime endOfDay = amendrecord.CheckDate.Date.AddDays(1).AddTicks(-1);
                var existRecord = await _checkRecordsRepository.GetCheckRecordPeriodAsync(amendrecord.StaffId, amendrecord.CompanyId, amendrecord.CheckDate, endOfDay);

                if (existRecord == null)
                {
                    if (amendrecord.CheckType == 0)
                    {
                        var record = new CheckRecordsDTO
                        {
                            CompanyId = amendrecord.CompanyId,
                            StaffId = amendrecord.StaffId,
                            CheckInTime = amendrecord.CheckTime,
                            CheckOutTime = null,
                            CheckInMemo = amendrecord.Reason,
                            CheckOutMemo = String.Empty,
                            IsCheckInOutLocation = 0,
                            IsCheckOutOutLocation = 0,
                            IsCheckOutEarly = 0,
                            CheckOutEarlyTimes = 0
                        };
                        if (amendrecord.CheckTime.TimeOfDay <= rule.CheckInEndTime)
                        {
                            record.IsCheckInLate = 0;
                            record.CheckInLateTimes = 0;
                        }
                        else
                        {
                            int differenceInMinutes = (int)Math.Round((amendrecord.CheckTime.TimeOfDay - rule.CheckInEndTime).TotalMinutes);
                            record.IsCheckInLate = 1;
                            record.CheckInLateTimes = differenceInMinutes;
                        }
                        await _checkRecordsRepository.InsertAsync(record);
                    }
                    else
                    {
                        var record = new CheckRecordsDTO
                        {
                            CompanyId = amendrecord.CompanyId,
                            StaffId = amendrecord.StaffId,
                            CheckInTime = null,
                            CheckOutTime = amendrecord.CheckTime,
                            CheckInMemo = String.Empty,
                            CheckOutMemo = amendrecord.Reason,
                            IsCheckInOutLocation = 0,
                            IsCheckOutOutLocation = 0,
                            IsCheckInLate = 0,
                            CheckInLateTimes = 0,
                        };
                        if (amendrecord.CheckTime.TimeOfDay > rule.CheckOutStartTime)
                        {
                            record.IsCheckOutEarly = 0;
                            record.CheckOutEarlyTimes = 0;
                        }
                        else
                        {
                            int differenceInMinutes = (int)Math.Round((rule.CheckInEndTime - amendrecord.CheckTime.TimeOfDay).TotalMinutes);
                            record.IsCheckOutEarly = 1;
                            record.CheckOutEarlyTimes = differenceInMinutes;
                        }
                        await _checkRecordsRepository.InsertAsync(record);
                    }
                }
                else
                {
                    if (amendrecord.CheckType == 0)
                    {
                        existRecord.CheckInTime = amendrecord.CheckTime;
                        if (amendrecord.CheckTime.TimeOfDay <= rule.CheckInEndTime)
                        {
                            existRecord.IsCheckInLate = 0;
                            existRecord.CheckInLateTimes = 0;
                        }
                        else
                        {
                            int differenceInMinutes = (int)Math.Round((amendrecord.CheckTime.TimeOfDay - rule.CheckInEndTime).TotalMinutes);
                            existRecord.IsCheckInLate = 1;
                            existRecord.CheckInLateTimes = differenceInMinutes;
                        }
                        await _checkRecordsRepository.UpdateAsync(existRecord);
                    }
                    else
                    {
                        existRecord.CheckOutTime = amendrecord.CheckTime;
                        if (amendrecord.CheckTime.TimeOfDay > rule.CheckOutStartTime)
                        {
                            existRecord.IsCheckOutEarly = 0;
                            existRecord.CheckOutEarlyTimes = 0;
                        }
                        else
                        {
                            int differenceInMinutes = (int)Math.Round((rule.CheckInEndTime - amendrecord.CheckTime.TimeOfDay).TotalMinutes);
                            existRecord.IsCheckOutEarly = 1;
                            existRecord.CheckOutEarlyTimes = differenceInMinutes;
                        }
                        await _checkRecordsRepository.UpdateAsync(existRecord);
                    }
                }
            }

            amendrecord.Inspector = user.StaffName;
            amendrecord.ValidateDate = DateTimeHelper.TaipeiNow;
            amendrecord.IsValidate = isPass ? 1 : -1;
            await _amendCheckRecordRepository.UpdateAsync(amendrecord);

            return result;
        }
    }
}