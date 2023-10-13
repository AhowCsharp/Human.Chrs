using Microsoft.AspNetCore.Mvc;
using Human.Chrs.Domain;
using Human.Chrs.Infra.Attribute;
using Human.Chrs.ViewModel.Request;
using Human.Chrs.ViewModel.Extension;
using Human.Chrs.Domain.CommonModels;
using Human.Chrs.Domain.Helper;
using Human.Chrs.Domain.Services;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using NPOI.SS.Util;

namespace LineTag.Admin.ApiControllers
{
    /// <response code="401">登入失敗、驗證失敗</response>
    [Route("super")]
    [ApiController]
    public class SuperController : BaseController
    {
        private readonly ILogger<SuperController> _logger;
        private readonly SuperDomain _superdomain;
        private readonly UserService _userService;

        public SuperController(
            ILogger<SuperController> logger,
            UserService userService,
            SuperDomain admindomain)
        {
            _logger = logger;
            _userService = userService;
            _superdomain = admindomain;
        }

        /// <summary>
        /// 新增簽約公司
        /// </summary>
        /// <param name="newStaffSaveRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("newcompany")]
        [ApTokenAuth]
        [ApUserAuth]
        [ApCompanyIdAuth]
        [SuperTokenAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCompany(NewCompanyRequest request)
        {
            try
            {
                var result = await _superdomain.CreateOrEditCompanyAsync(request.ToDTO());
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
                _logger.LogError(ex, nameof(CreateCompany));

                return ServerError500();
            }
        }

        /// <summary>
        /// 公司列表
        /// </summary>
        /// <param name="newStaffSaveRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("companys")]
        [ApTokenAuth]
        [ApUserAuth]
        [ApCompanyIdAuth]
        [SuperTokenAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCompany()
        {
            try
            {
                var result = await _superdomain.GetAllCompanyAsync();
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
                _logger.LogError(ex, nameof(GetCompany));

                return ServerError500();
            }
        }

        /// <summary>
        /// 合約選項列表
        /// </summary>
        /// <param name="newStaffSaveRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("contracttypes")]
        [ApTokenAuth]
        [ApUserAuth]
        [ApCompanyIdAuth]
        [SuperTokenAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContractList()
        {
            try
            {
                var result = await _superdomain.GetAllContractTypeAsync();
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
                _logger.LogError(ex, nameof(GetCompany));

                return ServerError500();
            }
        }
    }
}