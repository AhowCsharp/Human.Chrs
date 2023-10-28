using Microsoft.AspNetCore.Mvc;
using Human.Chrs.Domain;
using Human.Chrs.Infra.Attribute;
using Human.Chrs.ViewModel.Request;
using SendGrid;
using SendGrid.Helpers.Mail;
using Human.Chrs.Domain.Helper;

namespace Human.Chrs.Admin.ApiControllers
{
    /// <response code="401">登入失敗、驗證失敗</response>
    [Route("login")]
    [ApiController]
    public class LoginController : BaseController
    {
        private readonly ILogger<LoginController> _logger;
        private readonly LoginDomain _logindomain;
        private readonly SendGridClient _sendGridClient;

        public LoginController(
            ILogger<LoginController> logger,
            LoginDomain logindomain,
            SendGridClient sendGridClient)
        {
            _logger = logger;
            _logindomain = logindomain;
            _sendGridClient = sendGridClient;
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

        /// <summary>
        /// 驗證單一設備資訊
        /// </summary>
        /// <param name="request">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("deviceid")]
        [ApTokenAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyDeviceId(DeviceIdRequest request)
        {
            try
            {
                var result = await _logindomain.DeviceIdVerifyAsync(request.Account, request.Password, request.DeviceId);

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

        /// <summary>
        /// 初次綁定設備
        /// </summary>
        /// <param name="request">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPut]
        [Route("deviceid")]
        [ApTokenAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterDeviceId(DeviceIdRequest request)
        {
            try
            {
                var result = await _logindomain.RegisterDeviceIdAsync(request.Account, request.Password, request.DeviceId);

                if (result.Success)
                {
                    return Ok(result);
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

        [HttpPost]
        [Route("send")]
        [ApTokenAuth]
        public async Task<IActionResult> SendForgetPasswordEmail(ResetPasswordRequest request)
        {
            var result = await _logindomain.GetForgetPasswordStaffAsync(request.Account, request.Email);
            if (result.Success)
            {
                var newPw = RandomHelper.GenerateRandomPassword(10);
                var from = new EmailAddress("dorei.marketing.studio@gmail.com", "䒳芮多媒體科技工作室");
                var to = new EmailAddress(result.Data.Email, result.Data.Name);
                var subject = "密碼重置 - Reset Password";

                var htmlContent = $@"
                <html>
                    <head>
                        <style>
                            .email-content {{
                                font-family: Arial, sans-serif;
                                padding: 20px;
                                border: 1px solid #e1e1e1;
                                max-width: 600px;
                                margin: 20px auto;
                            }}
                            .header {{
                                background-color: #f2f2f2;
                                padding: 10px;
                                text-align: center;
                                font-size: 24px;
                            }}
                            .password-box {{
                                background-color: #e6f7ff;
                                padding: 10px 20px;
                                margin: 20px 0;
                                text-align: center;
                                font-size: 20px;
                                border: 1px solid #b3e0ff;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-content'>
                            <div class='header'>密碼重置通知</div>
                            <p>親愛的 {result.Data.Name},</p>
                            <p>您的密碼已成功重置。以下是您的新密碼:</p>
                            <div class='password-box'>{newPw}</div>
                            <p>請記得登入系統後立即更改您的密碼以確保帳戶安全。</p>
                            <p>謝謝，<br>䒳芮多媒體科技工作室團隊</p>
                        </div>
                    </body>
                </html>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent);

                var response = await _sendGridClient.SendEmailAsync(msg);
                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    var resetResult = await _logindomain.GetResetStaffPasswordAsync(request.Account, request.Email, newPw);
                    if (resetResult.Success)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}