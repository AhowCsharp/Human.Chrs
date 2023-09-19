using Microsoft.Extensions.Logging;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.IRepository;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain.Helper;
using Human.Chrs.Domain.CommonModels;
using System.Collections.Generic;

namespace Human.Chrs.Domain
{
    public class AdminDomain
    {
        private readonly ILogger<AdminDomain> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyRuleRepository _companyRuleRepository;
        private readonly ICheckRecordsRepository _checkRecordsRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IOverTimeLogRepository _overTimeLogRepository;
        private readonly IPersonalDetailRepository _personalDetailRepository;
        private readonly UserService _userService;

        public AdminDomain(
            ILogger<AdminDomain> logger,
            IAdminRepository adminRepository,
            ICompanyRuleRepository companyRuleRepository,
            ICheckRecordsRepository checkRecordsRepository,
            IStaffRepository staffRepository,
            ICompanyRepository companyRepository,
            IOverTimeLogRepository overTimeLogRepository,
            IPersonalDetailRepository personalDetailRepository,
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
            _personalDetailRepository = personalDetailRepository;
            _userService = userService;
        }//async Task<CurrentUser>

        public async Task<CommonResult<IEnumerable<StaffDTO>>> InsertNewStaffAsync(StaffDTO newStaff)
        {
            var result = new CommonResult<IEnumerable<StaffDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            int staffId;
            if (verifyAdminToken)
            {
                newStaff.ResignationDate = DateTimeHelper.TaipeiNow;
                newStaff.Status = 1;
                newStaff.SpecialRestDays = 0;   //特休
                newStaff.SickDays = 30;     // 病假
                newStaff.ThingDays = 14; // 事假
                newStaff.DeathDays = 8; //喪假
                newStaff.MarryDays = 8; //婚假
                newStaff.MenstruationDays = newStaff.Gender == "女性" ? 1 : 0;  // 生理假 每月一天
                newStaff.ChildbirthDays = newStaff.Gender == "女性" ? 56 : 0; // 產假
                newStaff.TocolysisDays = newStaff.Gender == "女性" ? 7 : 0; //安胎假
                newStaff.PrenatalCheckUpDays = 7;  // 陪產檢 陪產假  產檢假
                newStaff.TackeCareBabyDays = 365 * 2; //留職停薪育嬰假

                newStaff.SpecialRestHours = 0;
                newStaff.SickHours = 0;
                newStaff.ThingHours = 0;
                newStaff.ChildbirthHours = 0;
                newStaff.DeathHours = 0;
                newStaff.MenstruationHours = 0;
                newStaff.ChildbirthHours = 0;
                newStaff.TocolysisHours = 0;
                newStaff.PrenatalCheckUpHours = 0;
                newStaff.TackeCareBabyHours = 0;

                newStaff.PersonalDetailId = null;

                newStaff.CreateDate = DateTimeHelper.TaipeiNow;
                newStaff.Creator = user.StaffName;
                newStaff.EditDate = DateTimeHelper.TaipeiNow;
                newStaff.Editor = user.StaffName;

                staffId = (await _staffRepository.InsertAsync(newStaff)).id;
            }
            else
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            var data = await _staffRepository.GetAllStaffAsync(user.CompanyId);
            result.Data = data;

            return result;
        }

        public async Task<CommonResult<PersonalDetailDTO>> RecordStaffDetailAsync(PersonalDetailDTO dto)
        {
            var result = new CommonResult<PersonalDetailDTO>();
            var user = _userService.GetCurrentUser();
            var data = new PersonalDetailDTO();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (verifyAdminToken)
            {
                dto.IdentityNo = CryptHelper.SaltHashPlus(dto.IdentityNo);
                data = await _personalDetailRepository.InsertAsync(dto);
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

            var data = await _staffRepository.GetAllStaffAsync(user.CompanyId);

            return data;
        }
    }
}