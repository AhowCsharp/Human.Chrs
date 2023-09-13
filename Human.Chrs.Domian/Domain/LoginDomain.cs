using Microsoft.Extensions.Logging;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.IRepository;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain.Helper;
using Human.Chrs.Domain.CommonModels;

namespace Human.Chrs.Domain
{
    public class LoginDomain
    {
        private readonly ILogger<LoginDomain> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly UserService _userService;

        public LoginDomain(
            ILogger<LoginDomain> logger,
            IAdminRepository adminRepository,
            IStaffRepository staffRepository,
            ICompanyRepository companyRepository,
            UserService userService)
        {
            _logger = logger;
            _adminRepository = adminRepository;
            _staffRepository = staffRepository;
            _companyRepository = companyRepository;
            _userService = userService;
        }

        public async Task<CommonResult<LoginDTO>> LoginVerifyAsync(string account, string password)
        {
            var result = new CommonResult<LoginDTO>();
            int companyId = 0;
            LoginDTO? loginUserInfo = new LoginDTO();
            StaffDTO? staff = await _staffRepository.VerifyLoginStaffAsync(account, password);
            AdminDTO? adminUser = staff == null ? await _adminRepository.VerifyLoginAdminAsync(account, password) : null;

            if (adminUser == null && staff == null)
            {
                result.AddError("登入帳號密碼錯誤");
                return result;
            }
            else if (adminUser != null)
            {
                loginUserInfo.Auth = adminUser.Permissions;
                loginUserInfo.StaffNo = adminUser.Id.ToString();
                loginUserInfo.StaffAccount = adminUser.Account;
                loginUserInfo.StaffName = adminUser.UserName;
                loginUserInfo.CompanyId = adminUser.CompanyId;
                loginUserInfo.Id = adminUser.Id;
                companyId = adminUser.CompanyId;
            }
            else if (staff != null)
            {
                loginUserInfo.Auth = staff.Auth;
                loginUserInfo.StaffNo = staff.StaffNo;
                loginUserInfo.StaffAccount = staff.StaffAccount;
                loginUserInfo.StaffName = staff.StaffName;
                loginUserInfo.CompanyId = staff.CompanyId;
                loginUserInfo.Id = staff.Id;
                companyId = staff.CompanyId;
            }
            var company = await _companyRepository.GetAsync(companyId);
            if (company == null)
            {
                result.AddError("公司尚未註冊");
                return result;
            }
            if(DateTimeHelper.TaipeiNow < company.ContractStartDate || DateTimeHelper.TaipeiNow > company.ContractEndDate)
            {
                result.AddError("貴司權限已過期");
                return result;
            }

            result.Data = loginUserInfo;
            
            // 假設CommonResult有一個Data屬性用於儲存結果
            return result;
        }

        public async Task<AdminDTO> GetAdminWithSaltHashAsync(int companyId, string account)
        {
            var account_datas = account.Split(",");

            if (!CryptHelper.VerifySaltHash(account_datas[1], account_datas[0]))
            {
                return null;
            }

            return await _adminRepository.GetAvailableAdminAsync(companyId, account_datas[0]);
        }
    }
}