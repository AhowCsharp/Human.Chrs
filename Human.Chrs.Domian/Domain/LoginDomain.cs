using Microsoft.Extensions.Logging;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.IRepository;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain.Helper;

namespace Human.Chrs.Domain
{
    public class LoginDomain
    {
        private readonly ILogger<LoginDomain> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly UserService _userService;

        public LoginDomain(
            ILogger<LoginDomain> logger,
            IAdminRepository adminRepository,
            ICompanyRepository companyRepository,
            UserService userService)
        {
            _logger = logger;
            _adminRepository = adminRepository;
            _companyRepository = companyRepository;
            _userService = userService;
        }

        //public async Task<NewCommonResult<VerifySignInDTO>> LoginVerifyAsync(string code)
        //{
        //    var result = new NewCommonResult<VerifySignInDTO>();

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

        public async Task<AdminDTO> GetAdminWithSaltHashAsync(int officialAccountId, string lineUserId)
        {
            var lineUserId_datas = lineUserId.Split(",");

            if (!CryptHelper.VerifySaltHash(lineUserId_datas[1], lineUserId_datas[0]))
            {
                return null;
            }

            return await _adminRepository.GetAvailableAdminAsync(officialAccountId, lineUserId_datas[0]);
        }
    }
}