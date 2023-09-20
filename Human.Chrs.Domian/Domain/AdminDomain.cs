using Microsoft.Extensions.Logging;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.IRepository;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain.Helper;
using Human.Chrs.Domain.CommonModels;
using System.Collections.Generic;
using System.Collections;

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
                    data = await _personalDetailRepository.InsertAsync(dto);
                }
                else
                {
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
            result.Data = await _vacationLogRepository.UpdateAsync(vacationLog);
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
                            staff.PrenatalCheckUpHours = (quotient * 8) - remainingHours;
                        }

                        break;
                }
                await _staffRepository.UpdateAsync(staff);
            }

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

            double total1Hours = staffVacationlogs
            .Where(x => x.VacationType == 1 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.SickDays = (int)(total1Hours / 8);
            salaryView.SickHours = (int)(total1Hours % 8);

            double total2Hours = staffVacationlogs
            .Where(x => x.VacationType == 2 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.ThingDays = (int)(total2Hours / 8);
            salaryView.ThingHours = (int)(total2Hours % 8);

            double total3Hours = staffVacationlogs
            .Where(x => x.VacationType == 3 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.ChildbirthDays = (int)(total3Hours / 8);
            salaryView.ChildbirthHours = (int)(total3Hours % 8);

            double total4Hours = staffVacationlogs
            .Where(x => x.VacationType == 4 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.DeathDays = (int)(total4Hours / 8);
            salaryView.DeathHours = (int)(total4Hours % 8);

            double total5Hours = staffVacationlogs
            .Where(x => x.VacationType == 5 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.MarryDays = (int)(total5Hours / 8);
            salaryView.MarryHours = (int)(total5Hours % 8);

            double total6Hours = staffVacationlogs
            .Where(x => x.VacationType == 6 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.WorkthingDays = (int)(total6Hours / 8);
            salaryView.WorkthingHours = (int)(total6Hours % 8);

            double total7Hours = staffVacationlogs
            .Where(x => x.VacationType == 7 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.WorkhurtDays = (int)(total7Hours / 8);
            salaryView.WorkhurtHours = (int)(total7Hours % 8);

            double total8Hours = staffVacationlogs
            .Where(x => x.VacationType == 8 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.MenstruationDays = (int)(total8Hours / 8);
            salaryView.MenstruationHours = (int)(total8Hours % 8);

            double total9Hours = staffVacationlogs
            .Where(x => x.VacationType == 9 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.TackeCareBabyDays = (int)(total9Hours / 8);
            salaryView.TackeCareBabyHours = (int)(total9Hours % 8);

            double total10Hours = staffVacationlogs
            .Where(x => x.VacationType == 10 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.TocolysisDays = (int)(total10Hours / 8);
            salaryView.TocolysisHours = (int)(total10Hours % 8);

            double total11Hours = staffVacationlogs
            .Where(x => x.VacationType == 11 && x.IsPass == 1)
            .Sum(x => x.Hours);
            salaryView.PrenatalCheckUpDays = (int)(total11Hours / 8);
            salaryView.PrenatalCheckUpHours = (int)(total11Hours % 8);

            salaryView.OverTimeHours = (await _overTimeLogRepository.GetOverTimeLogOfPeriodAsync(staffId, user.CompanyId, firstDayOfLastMonth, lastDayOfLastMonth)).Sum(x => x.OverHours);


            return result;
        }
    }
}