using Microsoft.AspNetCore.Mvc;
using Human.Chrs.Domain;
using Human.Chrs.Infra.Attribute;
using Human.Chrs.ViewModel.Request;
using Human.Repository.EF;

namespace LineTag.Admin.ApiControllers
{
    /// <response code="401">登入失敗、驗證失敗</response>
    [Route("staff")]
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
        [ApCompanyIdAuth]
        [ApUserAuth]
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
        /// 補打打卡
        /// </summary>
        /// <param name="checkRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("amendcheck")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ApplyAmendCheckRecord(AmendCheckCheckRequest request)
        {
            try
            {
                var result = await _staffdomain.ApplyAmendCheckRecordAsync(request.CheckDate, request.CheckTime, request.Reason, request.CheckType);

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
                _logger.LogError(ex, nameof(ApplyAmendCheckRecord));

                return ServerError500();
            }
        }

        /// <summary>
        /// 補打打卡
        /// </summary>
        /// <param name="checkRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("amendchecklist")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAmendCheckRecord()
        {
            try
            {
                var result = await _staffdomain.GetAmendCheckRecordAsync();

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
                _logger.LogError(ex, nameof(GetAmendCheckRecord));

                return ServerError500();
            }
        }

        /// <summary>
        /// 報加班
        /// </summary>
        /// <param name="overtimeRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("overtime")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterOverTime(OverTimeRequest overtimeRequest)
        {
            try
            {
                var result = await _checkdomain.InsertOverTimeAsync(overtimeRequest.ChooseDate, overtimeRequest.Hours, overtimeRequest.Reason);

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
        /// 申請休假
        /// </summary>
        /// <param name="vacationRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("vacation")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ApplyVacation(VacationRequest vacationRequest)
        {
            try
            {
                var result = await _staffdomain.ApplyVacationAsync(vacationRequest.Type, vacationRequest.StartDate, vacationRequest.EndDate, vacationRequest.Hours, vacationRequest.Reason);

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
                _logger.LogError(ex, nameof(ApplyVacation));

                return ServerError500();
            }
        }

        /// <summary>
        /// 備忘錄註冊
        /// </summary>
        /// <param name="eventRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("event")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> EventAdd(EventRequest eventRequest)
        {
            try
            {
                var result = await _staffdomain.EventAddAsync(eventRequest.EventStartDate, eventRequest.EventEndDate, eventRequest.StartTime, eventRequest.EndTime, eventRequest.Title, eventRequest.Detail, eventRequest.LevelStatus);

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
                _logger.LogError(ex, nameof(EventAdd));

                return ServerError500();
            }
        }

        /// <summary>
        /// 驗證地理位置
        /// </summary>
        /// <param name="ditanceRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("distance")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckDistance(DistanceRequest ditanceRequest)
        {
            try
            {
                var result = await _checkdomain.CheckDistanceAsync(ditanceRequest.CompanyId, ditanceRequest.Longitude, ditanceRequest.Latitude);

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
        /// 得到員工個人詳細訊息
        /// </summary>
        /// <param name="">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("detail")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDetailInfo(int? id)
        {
            try
            {
                var result = await _staffdomain.GetStaffDetailAsync(id);

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
                _logger.LogError(ex, nameof(GetDetailInfo));

                return ServerError500();
            }
        }

        /// <summary>
        /// 備忘錄讀取
        /// </summary>
        /// <param name="ditanceRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("eventdetails")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> EventsGet()
        {
            try
            {
                var result = await _staffdomain.EventsGetAsync();

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
                _logger.LogError(ex, nameof(EventsGet));

                return ServerError500();
            }
        }

        /// <summary>
        /// 員工出勤狀況
        /// </summary>
        /// <param name="ditanceRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("checkdetails")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCheckList(DateTime? start, DateTime? end, int staffId)
        {
            try
            {
                var result = await _staffdomain.GetCheckListAsync(start, end, staffId);

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
                _logger.LogError(ex, nameof(GetCheckList));

                return ServerError500();
            }
        }

        /// <summary>
        /// 個人出勤狀況
        /// </summary>
        /// <param name="ditanceRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("personalchecks")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPersonalChecks(int month)
        {
            try
            {
                var result = await _staffdomain.GetPersonalChecksAsync(month);

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
                _logger.LogError(ex, nameof(GetPersonalChecks));

                return ServerError500();
            }
        }

        /// <summary>
        /// 薪資單列表
        /// </summary>
        /// <param name="ditanceRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("salary")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSalaryList()
        {
            try
            {
                var result = await _staffdomain.GetIncomeLogsAsync();

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
                _logger.LogError(ex, nameof(GetSalaryList));

                return ServerError500();
            }
        }

        /// <summary>
        /// 薪資單列表
        /// </summary>
        /// <param name="ditanceRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("salarydetail")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSalaryDetail(int id)
        {
            try
            {
                var result = await _staffdomain.GetSalaryDetailAsync(id);

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
                _logger.LogError(ex, nameof(GetSalaryDetail));

                return ServerError500();
            }
        }

        /// <summary>
        /// 加班記錄列表
        /// </summary>
        /// <param name="ditanceRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("overtime")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetovertimeList(int? staffId, DateTime start, DateTime end)
        {
            try
            {
                var result = await _staffdomain.GetovertimeListAsync(staffId, start, end);

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
                _logger.LogError(ex, nameof(GetovertimeList));

                return ServerError500();
            }
        }

        /// <summary>
        /// 加班記錄列表
        /// </summary>
        /// <param name="ditanceRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("monthovertime")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetThisMonthOvertimeList(int month)
        {
            try
            {
                var result = await _staffdomain.GetThisMonthOvertimeListAsync(month);

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
                _logger.LogError(ex, nameof(GetThisMonthOvertimeList));

                return ServerError500();
            }
        }

        /// <summary>
        /// 登入後畫面
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
        public async Task<IActionResult> GetStaffViewInfoAsync(double longitude, double latitude)
        {
            try
            {
                var result = await _staffdomain.GetStaffViewInfoAsync(longitude, latitude);
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
        /// 大頭貼更新
        /// </summary>
        /// <param name="eventRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("avatar")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            try
            {
                if (avatar == null || avatar.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No image provided" });
                }
                var result = await _staffdomain.UploadAvatarAsync(avatar);
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
                _logger.LogError(ex, nameof(UploadAvatar));

                return ServerError500();
            }
        }

        /// <summary>
        /// 刪除事件
        /// </summary>
        /// <param name="eventRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpDelete]
        [Route("event")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var result = await _staffdomain.DeleteEventAsync(id);
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
                _logger.LogError(ex, nameof(DeleteEvent));

                return ServerError500();
            }
        }
    }
}