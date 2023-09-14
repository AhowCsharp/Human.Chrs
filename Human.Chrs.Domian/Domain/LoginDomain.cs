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
                var useridSalted = CryptHelper.SaltHashPlus(adminUser.Id.ToString());
                loginUserInfo.Auth = adminUser.Auth;
                loginUserInfo.StaffNo = adminUser.Id.ToString();
                loginUserInfo.StaffAccount = adminUser.Account;
                loginUserInfo.StaffName = adminUser.UserName;
                loginUserInfo.CompanyId = adminUser.CompanyId;
                loginUserInfo.UserId = $"{adminUser.Id},{useridSalted}";
                loginUserInfo.AdminToken = adminUser.AdminToken;
                companyId = adminUser.CompanyId;
            }
            else if (staff != null)
            {
                var useridSalted = CryptHelper.SaltHashPlus(staff.Id.ToString());
                loginUserInfo.Auth = staff.Auth;
                loginUserInfo.StaffNo = staff.StaffNo;
                loginUserInfo.StaffAccount = staff.StaffAccount;
                loginUserInfo.StaffName = staff.StaffName;
                loginUserInfo.CompanyId = staff.CompanyId;
                loginUserInfo.UserId = $"{staff.Id},{useridSalted}";
                companyId = staff.CompanyId;
            }
            var company = await _companyRepository.GetAsync(companyId);
            if (company == null)
            {
                result.AddError("公司尚未註冊");
                return result;
            }
            if (DateTimeHelper.TaipeiNow < company.ContractStartDate || DateTimeHelper.TaipeiNow > company.ContractEndDate)
            {
                result.AddError("貴司權限已過期");
                return result;
            }

            result.Data = loginUserInfo;

            // 假設CommonResult有一個Data屬性用於儲存結果
            return result;
        }

        public async Task<CommonResult<LoginDTO>> GetUserWithSaltHashAsync(int companyId, string userId)
        {
            var result = new CommonResult<LoginDTO>();
            var user_datas = userId.Split(",");

            if (!CryptHelper.VerifySaltHashPlus(user_datas[1], user_datas[0]))
            {
                return null;
            }
            LoginDTO? loginUserInfo = new LoginDTO();
            StaffDTO? staff = await _staffRepository.GetUsingStaffAsync(Convert.ToInt32(user_datas[0]), companyId);
            AdminDTO? adminUser = staff == null ? await _adminRepository.GetAvailableAdminAsync(Convert.ToInt32(user_datas[0]), companyId) : null;

            if (adminUser == null && staff == null)
            {
                result.AddError("驗證錯誤 admin 及 staff都尚未授權");
                return result;
            }
            else if (adminUser != null)
            {
                loginUserInfo.Auth = adminUser.Auth;
                loginUserInfo.StaffAccount = adminUser.Account;
                loginUserInfo.StaffName = adminUser.UserName;
                loginUserInfo.CompanyId = adminUser.CompanyId;
                loginUserInfo.DepartmentId = adminUser.DepartmentId;
                loginUserInfo.StaffNo = adminUser.StaffNo;
                loginUserInfo.AdminToken = adminUser.AdminToken;
                loginUserInfo.Id = adminUser.Id;
            }
            else if (staff != null)
            {
                loginUserInfo.Auth = staff.Auth;
                loginUserInfo.StaffNo = staff.StaffNo;
                loginUserInfo.StaffAccount = staff.StaffAccount;
                loginUserInfo.StaffName = staff.StaffName;
                loginUserInfo.DepartmentId = staff.DepartmentId;
                loginUserInfo.CompanyId = staff.CompanyId;
                loginUserInfo.Id = staff.Id;
            }
            result.Data = loginUserInfo;

            return result;
        }
    }
}