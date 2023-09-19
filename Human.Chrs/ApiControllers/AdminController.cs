using Microsoft.AspNetCore.Mvc;
using Human.Chrs.Domain;
using Human.Chrs.Infra.Attribute;
using Human.Chrs.ViewModel.Request;
using Human.Chrs.ViewModel.Extension;

namespace LineTag.Admin.ApiControllers
{
    /// <response code="401">登入失敗、驗證失敗</response>
    [Route("admin")]
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly ILogger<AdminController> _logger;
        private readonly AdminDomain _admindomain;

        public AdminController(
            ILogger<AdminController> logger,
            AdminDomain admindomain)
        {
            _logger = logger;
            _admindomain = admindomain;
        }

        /// <summary>
        /// 新增員工
        /// </summary>
        /// <param name="checkRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("staff")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateStaff(NewStaffSaveRequest newStaffSaveRequest, StaffDetailSaveRequest newStaffDetailSaveRequest)
        {
            //CompanyId StaffAccount StaffPassWord Department EntryDate LevelPosition WorkPosition Email StaffPhoneNumber Auth DepartmentId
            try
            {
                var result = await _admindomain.InsertNewStaffAsync(newStaffSaveRequest.ToDTO(), newStaffDetailSaveRequest.ToDTO());
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
                _logger.LogError(ex, nameof(CreateStaff));

                return ServerError500();
            }
        }

        /// <summary>
        /// 新增員工
        /// </summary>
        /// <param name="checkRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("details")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStaffDetail(StaffDetailSaveRequest request)
        {
            //CompanyId StaffAccount StaffPassWord Department EntryDate LevelPosition WorkPosition Email StaffPhoneNumber Auth DepartmentId
            try
            {
                var result = await _admindomain.RecordStaffDetailAsync(request.ToDTO());
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
                _logger.LogError(ex, nameof(CreateStaff));

                return ServerError500();
            }
        }

        /// <summary>
        /// 取得員工列表
        /// </summary>
        /// <param name="checkRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("Infos")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStaffs()
        {
            //CompanyId StaffAccount StaffPassWord Department EntryDate LevelPosition WorkPosition Email StaffPhoneNumber Auth DepartmentId
            try
            {
                var result = await _admindomain.GetAllStaffAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(CreateStaff));

                return ServerError500();
            }
        }
    }
}