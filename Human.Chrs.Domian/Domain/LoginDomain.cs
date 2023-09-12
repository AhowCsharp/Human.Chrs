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
            }
            else if (staff != null)
            {
                loginUserInfo.Auth = staff.Auth;
                loginUserInfo.StaffNo = staff.StaffNo;
                loginUserInfo.StaffAccount = staff.StaffAccount;
                loginUserInfo.StaffName = staff.StaffName;
                loginUserInfo.CompanyId = staff.CompanyId;
                loginUserInfo.Id = staff.Id;
            }

            result.Data = loginUserInfo;  // 假設CommonResult有一個Data屬性用於儲存結果
            return result;
        }

        //public async Task<CommonResult<VerifySignInDTO>> LoginVerifyAsync(string code)

        //{
        //    var result = new CommonResult<VerifySignInDTO>();

        //    LineLoginToken lineLoginToken = null;

        //    try
        //    {
        //        lineLoginToken = await _adminRepository.GetLoginTokenAsync(code, _config.LoginUrl);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        result.AddError("登入資訊Code錯誤");
        //    }

        //    if (lineLoginToken != null)
        //    {
        //        var profile = await _adminRepository.GetLoginProfileAsync(lineLoginToken.id_token);
        //        if (string.IsNullOrEmpty(profile.sub))
        //        {
        //            result.AddError("id_token資料錯誤");
        //        }

        //        var aos = await _adminRepository.GetAvailableAdminListAsync(profile.sub);

        //        if (!aos.Any())
        //        {
        //            result.AddError("使用者帳號不存在");
        //        }

        //        if (!result.Success)
        //        {
        //            return result;
        //        }

        //        foreach (var dto in aos.ToList())
        //        {
        //            dto.Name = profile.name;
        //            dto.PictureUrl = profile.picture;
        //        }

        //        await _adminRepository.UpdateByLineProfileAsync(aos);

        //        var admin = aos.OrderByDescending(x => x.LastUseDate).FirstOrDefault();

        //        var useridSalted = CryptHelper.SaltHash(admin.UserId);

        //        var response = new VerifySignInDTO()
        //        {
        //            // 加鹽雜湊UserId
        //            LineUserId = $"{admin.UserId},{useridSalted}",
        //            OfficialAccountId = admin.OfficialAccountId
        //        };

        //        result.Data = response;
        //    }

        //    return result;
        //}

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