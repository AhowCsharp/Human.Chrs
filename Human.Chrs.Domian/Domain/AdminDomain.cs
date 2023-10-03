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
        private readonly UserService _userService;

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
            UserService userService)
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
            _userService = userService;
        }//async Task<CurrentUser>

        public async Task<CommonResult<IEnumerable<StaffDTO>>> CreateOrEditStaffAsync(StaffDTO newStaff)
        {
            var result = new CommonResult<IEnumerable<StaffDTO>>();
            var user = _userService.GetCurrentUser();
            bool isCreate = newStaff.id == 0;
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            int staffId;
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");

                return result;
                //newStaff.ResignationDate = DateTimeHelper.TaipeiNow;
                //newStaff.Status = 1;
                //newStaff.SpecialRestDays = 0;   //特休
                //newStaff.SickDays = 30;     // 病假
                //newStaff.ThingDays = 14; // 事假
                //newStaff.DeathDays = 8; //喪假
                //newStaff.MarryDays = 8; //婚假
                //newStaff.MenstruationDays = newStaff.Gender == "女性" ? 1 : 0;  // 生理假 每月一天
                //newStaff.ChildbirthDays = newStaff.Gender == "女性" ? 56 : 0; // 產假
                //newStaff.TocolysisDays = newStaff.Gender == "女性" ? 7 : 0; //安胎假
                //newStaff.PrenatalCheckUpDays = 7;  // 陪產檢 陪產假  產檢假
                //newStaff.TackeCareBabyDays = 365 * 2; //留職停薪育嬰假

                //newStaff.SpecialRestHours = 0;
                //newStaff.SickHours = 0;
                //newStaff.ThingHours = 0;
                //newStaff.ChildbirthHours = 0;
                //newStaff.DeathHours = 0;
                //newStaff.MenstruationHours = 0;
                //newStaff.ChildbirthHours = 0;
                //newStaff.TocolysisHours = 0;
                //newStaff.PrenatalCheckUpHours = 0;
                //newStaff.TackeCareBabyHours = 0;

                //newStaff.PersonalDetailId = null;
            }
            if (isCreate)
            {
                newStaff.CreateDate = DateTimeHelper.TaipeiNow;
                newStaff.Creator = user.StaffName;
                newStaff.EditDate = DateTimeHelper.TaipeiNow;
                newStaff.Editor = user.StaffName;
                await _staffRepository.InsertAsync(newStaff);
            }
            else
            {
                newStaff.EditDate = DateTimeHelper.TaipeiNow;
                newStaff.Editor = user.StaffName;
                await _staffRepository.UpdateAsync(newStaff);
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
                if (isCreate)
                {
                    dto.EditDate = DateTimeHelper.TaipeiNow;
                    dto.CreateDate = DateTimeHelper.TaipeiNow;
                    dto.Creator = user.StaffName;
                    dto.Editor = user.StaffName;
                    result.Data = await _salarySettingRepository.InsertAsync(dto);
                }
                else
                {
                    dto.EditDate = DateTimeHelper.TaipeiNow;
                    dto.Editor = user.StaffName;
                    result.Data = await _salarySettingRepository.UpdateAsync(dto);
                }
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

        public async Task<IEnumerable<DepartmentDTO>> GetDepartmentsOfCompanyAsync()
        {
            var user = _userService.GetCurrentUser();

            var departments = await _departmentRepository.GetDepartmentsOfCompanyAsync(user.CompanyId);

            return departments;
        }

        public async Task<IEnumerable<CompanyRuleDTO>> GetRulesOfCompanyAsync()
        {
            var user = _userService.GetCurrentUser();
            var departments = await _companyRuleRepository.GetCompanyRulesAsync(user.CompanyId);

            return departments;
        }

        public async Task<CommonResult<bool>> CreateOrUpdateRuleAsync(CompanyRuleDTO request)
        {
            var result = new CommonResult<bool>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            bool isCreate = request.id == 0;
            try
            {
                if (isCreate)
                {
                    await _companyRuleRepository.InsertAsync(request);
                }
                else
                {
                    await _companyRuleRepository.UpdateAsync(request);
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

        public async Task<CommonResult<AdminDTO>> CreateOrEditAdminAsync(AdminDTO adminDTO)
        {
            var result = new CommonResult<AdminDTO>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (!verifyAdminToken)
            {
                result.AddError("操作者沒有權杖");
                return result;
            }
            if (adminDTO.id == 0)
            {
                result.Data = await _adminRepository.InsertAsync(adminDTO);
            }
            else
            {
                result.Data = await _adminRepository.UpdateAsync(adminDTO);
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
            }
            result.Data = overtimeList;
            return result;
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
            var salaryView = new SalaryViewDTO();

            DateTime today = DateTimeHelper.TaipeiNow;
            DateTime firstDayOfLastMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
            DateTime lastDayOfLastMonth = new DateTime(today.Year, today.Month, 1).AddDays(-1);

            var staffCheckRecords = await _checkRecordsRepository.GetCheckRecordListAsync(user.CompanyId, staffId, firstDayOfLastMonth, lastDayOfLastMonth);
            var staffVacationlogs = await _vacationLogRepository.GetPeriodVacationLogsAsync(staffId, user.CompanyId, firstDayOfLastMonth, lastDayOfLastMonth);
            bool vacationNotAlreadyChecked = staffVacationlogs.Any(x => x.IsPass == 0);
            if (vacationNotAlreadyChecked)
            {
                result.AddError("請將該員工的請假全都審核完畢");
                return result;
            }

            var setting = await _salarySettingRepository.GetSalarySettingAsync(staffId, user.CompanyId);
            var staff = await _staffRepository.GetUsingStaffAsync(staffId, user.CompanyId);
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

            var perHourSalary = (salaryView.SalarySetting.BasicSalary + salaryView.SalarySetting.FullCheckInMoney) / 30 / 8;

            salaryView.OverTimeHours = (await _overTimeLogRepository.GetOverTimeLogOfPeriodAsync(staffId, user.CompanyId, firstDayOfLastMonth, lastDayOfLastMonth)).Sum(x => x.OverHours);
            salaryView.TotalSalaryNoOvertime = salaryView.SalarySetting.BasicSalary + salaryView.SalarySetting.FullCheckInMoney - salaryView.TotalSickHours * perHourSalary / 2 -
                                               salaryView.TotalThingHours * perHourSalary - salaryView.TotalMenstruationHours * perHourSalary / 2;
            salaryView.PerHourSalary = perHourSalary;

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
    }
}