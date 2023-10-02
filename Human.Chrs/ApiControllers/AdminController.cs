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
        /// <param name="newStaffSaveRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("newstaff")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrEditStaff(StaffSaveRequest newStaffSaveRequest)
        {
            //CompanyId StaffAccount StaffPassWord Department EntryDate LevelPosition WorkPosition Email StaffPhoneNumber Auth DepartmentId
            try
            {
                var result = await _admindomain.CreateOrEditStaffAsync(newStaffSaveRequest.ToDTO());
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
                _logger.LogError(ex, nameof(CreateOrEditStaff));

                return ServerError500();
            }
        }

        /// <summary>
        /// 增修員工薪資設定
        /// </summary>
        /// <param name="salarySettingRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("salarysetting")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrEditSalarySetting(SalarySettingRequest salarySettingRequest)
        {
            //CompanyId StaffAccount StaffPassWord Department EntryDate LevelPosition WorkPosition Email StaffPhoneNumber Auth DepartmentId
            try
            {
                var result = await _admindomain.CreateOrEditSalarySetting(salarySettingRequest.ToDTO());
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
                _logger.LogError(ex, nameof(CreateOrEditSalarySetting));

                return ServerError500();
            }
        }

        /// <summary>
        /// 新增員工詳細資料
        /// </summary>
        /// <param name="request">請求資料</param>
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
        public async Task<IActionResult> CreateOrEditStaffDetail(StaffDetailSaveRequest request)
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
                _logger.LogError(ex, nameof(CreateOrEditStaffDetail));

                return ServerError500();
            }
        }

        /// <summary>
        /// 取得員工列表
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("staffs")]
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
                _logger.LogError(ex, nameof(GetStaffs));

                return ServerError500();
            }
        }

        /// <summary>
        /// 取得部門列表
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("departments")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var result = await _admindomain.GetDepartmentsOfCompanyAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetDepartments));

                return ServerError500();
            }
        }

        /// <summary>
        /// 取得部門規定
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("rules")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRulesOfCompany()
        {
            try
            {
                var result = await _admindomain.GetRulesOfCompanyAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetRulesOfCompany));

                return ServerError500();
            }
        }

        /// <summary>
        /// 修改部門規定
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("modifyrule")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ModifyRulesOfCompany(CompanyRuleRequest request)
        {
            try
            {
                var result = await _admindomain.CreateOrUpdateRuleAsync(request.ToDTO());
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
                _logger.LogError(ex, nameof(GetRulesOfCompany));

                return ServerError500();
            }
        }

        /// <summary>
        /// 新增或修改管理者
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("manager")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrEditAdmin(AdminSaveRequest request)
        {
            try
            {
                var result = await _admindomain.CreateOrEditAdminAsync(request.ToDTO());
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
                _logger.LogError(ex, nameof(CreateOrEditAdmin));

                return ServerError500();
            }
        }

        /// <summary>
        /// 取得員工出勤狀況統計
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("checkrecords")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStaffAttendance(int staffId, DateTime start, DateTime end)
        {
            try
            {
                var result = await _admindomain.GetStaffAttendanceAsync(staffId, start, end);
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
                _logger.LogError(ex, nameof(GetStaffAttendance));

                return ServerError500();
            }
        }

        /// <summary>
        /// 審核員工請假
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPatch]
        [Route("vacation")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyVacations(VerifyVacationRequest request)
        {
            try
            {
                var result = await _admindomain.VerifyVacationsAsync(request.VacationId, request.IsPass);
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
                _logger.LogError(ex, nameof(VerifyVacations));

                return ServerError500();
            }
        }

        /// <summary>
        /// 審核員工薪資
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("paymoeny")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStaffSalaryView(int id)
        {
            try
            {
                var result = await _admindomain.GetStaffSalaryViewAsync(id);
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
                _logger.LogError(ex, nameof(GetStaffSalaryView));

                return ServerError500();
            }
        }

        /// <summary>
        /// 員工請假列表
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("vacations")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllVacations(DateTime start, DateTime end)
        {
            try
            {
                var result = await _admindomain.GetAllVacationApplicationAsync(start, end);
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
                _logger.LogError(ex, nameof(GetAllVacations));

                return ServerError500();
            }
        }

        /// <summary>
        /// 員工薪資列表
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("salarysettings")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSalarySetting()
        {
            try
            {
                var result = await _admindomain.GetAllSalarySettingAsync();
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
                _logger.LogError(ex, nameof(GetAllSalarySetting));

                return ServerError500();
            }
        }
    }
}