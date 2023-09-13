using Microsoft.AspNetCore.Mvc;
using Human.Chrs.Domain;
using Human.Chrs.Infra.Attribute;
using Human.Chrs.Domain.DTO;

namespace LineTag.Admin.ApiControllers
{
    /// <response code="401">登入失敗、驗證失敗</response>
    [Route("login")]
    [ApiController]
    public class CheckInAndOutController : BaseController
    {
        private readonly ILogger<CheckInAndOutController> _logger;
        private readonly CheckInAndOutDomain _checkdomain;

        public CheckInAndOutController(
            ILogger<CheckInAndOutController> logger,
            CheckInAndOutDomain checkdomain)
        {
            _logger = logger;
            _checkdomain = checkdomain;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifySignIn(LoginRequest loginRequest)
        {
            try
            {
                var result = await _logindomain.LoginVerifyAsync(loginRequest.Account, loginRequest.Password);

                if (result.Success)
                {
                    return Ok(result.Data);
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
    }
}