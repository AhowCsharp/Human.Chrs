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

        public async Task<CommonResult<IEnumerable<StaffDTO>>> InsertNewStaffAsync(StaffDTO dto)
        {
            var result = new CommonResult<IEnumerable<StaffDTO>>();
            var user = _userService.GetCurrentUser();
            var verifyAdminToken = await _adminRepository.VerifyAdminTokenAsync(user);
            if (verifyAdminToken)
            {
                dto.ResignationDate = DateTimeHelper.TaipeiNow;
                dto.Status = 1;
                dto.SpecialRestDays = 0;
                dto.SickDays = 0;
                dto.ThingDays = 0;
                dto.ChildbirthDays = 0;
                dto.DeathDays = 0;
                dto.MarryDays = 0;
                dto.SpecialRestHours = 0;
                dto.SickHours = 0;
                dto.ThingHours = 0;
                dto.ChildbirthHours = 0;
                dto.DeathHours = 0;
                dto.PersonalDetailId = null;
                //dto.WorkDay = 0;
                //dto.RestDay = 0;
                dto.CreateDate = DateTimeHelper.TaipeiNow;
                dto.Creator = user.StaffName;
                dto.EditDate = DateTimeHelper.TaipeiNow;
                dto.Editor = user.StaffName;

                await _staffRepository.InsertAsync(dto);
            }
            else
            {
                result.AddError("操作者沒有權杖");

                return result;
            }

            var data = await _staffRepository.GetAllStaffAsync(user.Id, user.CompanyId);
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

            var data = await _staffRepository.GetAllStaffAsync(user.Id, user.CompanyId);

            return data;
        }
    }
}