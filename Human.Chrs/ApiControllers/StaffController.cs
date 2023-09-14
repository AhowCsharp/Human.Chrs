using Microsoft.AspNetCore.Mvc;
using Human.Chrs.Domain;
using Human.Chrs.Infra.Attribute;
using Human.Chrs.ViewModel.Request;

namespace LineTag.Admin.ApiControllers
{
    /// <response code="401">登入失敗、驗證失敗</response>
    [Route("check")]
    [ApiController]
    public class StaffController : BaseController
    {
        private readonly ILogger<StaffController> _logger;
        private readonly CheckInAndOutDomain _checkdomain;
        private readonly StaffDomain _staffdomain;

        public StaffController(
            ILogger<StaffController> logger,
            StaffDomain staffdomain,
            CheckInAndOutDomain checkdomain)
        {
            _logger = logger;
            _staffdomain = staffdomain;
            _checkdomain = checkdomain;
        }

        /// <summary>
        /// 打卡
        /// </summary>
        /// <param name="checkRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("checkinout")]
        [ApTokenAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckInAndOut(CheckRequest checkRequest)
        {
            try
            {
                var result = await _checkdomain.CheckInOutAsync(checkRequest.Longitude, checkRequest.Latitude, checkRequest.Memo);

                if (result.Success)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(CheckInAndOut));

                return ServerError500();
            }
        }

        /// <summary>
        /// 報加班
        /// </summary>
        /// <param name="checkRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("overtime")]
        [ApTokenAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterOverTime(OverTimeRequest overtimeRequest)
        {
            try
            {
                var result = await _checkdomain.InsertOverTimeAsync(overtimeRequest.StaffId, overtimeRequest.CompanyId, overtimeRequest.Hours, overtimeRequest.Reason);

                if (result.Success)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RegisterOverTime));

                return ServerError500();
            }
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
        [HttpGet]
        [Route("distance")]
        [ApTokenAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckDistance(DistanceRequest checkRequest)
        {
            try
            {
                var result = await _checkdomain.CheckDistanceAsync(checkRequest.CompanyId, checkRequest.Longitude, checkRequest.Latitude);

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
                _logger.LogError(ex, nameof(CheckDistance));

                return ServerError500();
            }
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
        [HttpGet]
        [Route("view")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStaffViewInfoAsync()
        {
            try
            {
                var result = await _staffdomain.GetStaffViewInfoAsync();
                return Ok(result);
                //if (result.Success)
                //{
                //    return Ok(result.Data);
                //}
                //else
                //{
                //    return BadRequest(result.Errors);
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(CheckDistance));

                return ServerError500();
            }
        }
    }
}