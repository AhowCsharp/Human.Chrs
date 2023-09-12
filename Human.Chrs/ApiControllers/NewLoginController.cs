using Microsoft.AspNetCore.Mvc;

namespace LineTag.Admin.ApiControllers
{
    /// <response code="401">登入失敗、驗證失敗</response>
    [Route("login")]
    [ApiController]
    public class NewLoginController : BaseController
    {
        private readonly ILogger<NewLoginController> _logger;
        private readonly NewLoginDomain _logindomain;
        private readonly LineTagAdminConfig _config;

        public NewLoginController(
            ILogger<NewLoginController> logger,
            NewLoginDomain logindomain,
            LineTagAdminConfig config)
        {
            _logger = logger;
            _logindomain = logindomain;
            _config = config;
        }

        /// <summary>
        /// 驗證登入資訊
        /// </summary>
        /// <param name="loginRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("verify")]
        [ApTokenAuth]
        [ProducesResponseType(typeof(NewLoginViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifySignIn(NewLoginCodeViewModel loginRequest)
        {
            try
            {
                var result = await _logindomain.LineLoginVerifyAsync(loginRequest.Code);

                if (result.Success)
                {
                    return Ok(result.Data.ToViewModel());
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(VerifySignIn));

                return ServerError500();
            }
        }

        /// <summary>
        /// 取得登入用網址
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("url")]
        [ApTokenAuth]
        [ProducesResponseType(typeof(NewLoginViewModel), StatusCodes.Status200OK)]
        public IActionResult GetLineLoginUrl()
        {
            try
            {
                var result = _logindomain.GetLineLoginUrl();

                if (result.Success)
                {
                    var response = new NewLoginUrlViewModel()
                    {
                        LoginUrl = result.Data
                    };

                    return Ok(response);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetLineLoginUrl));

                return ServerError500();
            }
        }

        /// <summary>
        /// 驗證登入資訊
        /// </summary>
        /// <param name="oaLoginRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("oaverify")]
        [ApTokenAuth]
        [OfficialAccountAuth]
        [UserAuth(AllowSkipLineUserId = true)]
        [ProducesResponseType(typeof(NewLoginViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> OaUserVerifySignIn(NewLoginCodeViewModel oaLoginRequest)
        {
            try
            {
                var result = await _logindomain.LineOaLoginVerifyAsync(oaLoginRequest.Code, oaLoginRequest.OfficialAccountId.Value);

                if (result.Success)
                {
                    return Ok(result.Data.ToViewModel());
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(OaUserVerifySignIn));

                return ServerError500();
            }
        }

        #region 開發用API

        /// <summary>
        /// 開發用網址 直接登入 For Chase
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="403">無此權限</response>
        /// <returns></returns>
        // TODO 這邊上線前要拔除，應考慮環境問題
        [HttpGet]
        [Route("devsignin")]
        [ApTokenAuth]
        [ProducesResponseType(typeof(NewLoginViewModel), StatusCodes.Status200OK)]
        public IActionResult DevSignIn()
        {
            var userId = "Ubea9851d31b7a9868304a7eddfae07ad";
            var useridSalted = CryptHelper.SaltHash(userId);

            var response = new NewLoginViewModel()
            {
                LineUserId = $"{userId},{useridSalted}",
                OfficialAccountId = 4
            };

            return Ok(response);
        }

        #endregion 開發用API
    }
}