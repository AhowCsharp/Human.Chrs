﻿using Microsoft.AspNetCore.Mvc;
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
    [Route("admin")]
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly ILogger<AdminController> _logger;
        private readonly AdminDomain _admindomain;
        private readonly UserService _userService;

        public AdminController(
            ILogger<AdminController> logger,
            UserService userService,
            AdminDomain admindomain)
        {
            _logger = logger;
            _userService = userService;
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
        /// 刪除員工薪資設定
        /// </summary>
        /// <param name="salarySettingRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpDelete]
        [Route("salarysetting")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteSalarySetting(int id)
        {
            try
            {
                var result = await _admindomain.DeleteSalarySettingAsunc(id);
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
        /// 取得員工列表
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("parttime")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPartTimeStaffs(int month)
        {
            //CompanyId StaffAccount StaffPassWord Department EntryDate LevelPosition WorkPosition Email StaffPhoneNumber Auth DepartmentId
            try
            {
                var result = await _admindomain.GetPartTimeStaffsAsync(month);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetPartTimeStaffs));

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
        [Route("amendrecord")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAmendrecords(DateTime start,DateTime end)
        {
            try
            {
                var result = await _admindomain.GetGetAmendrecordsAsync(start, end);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetAmendrecords));

                return ServerError500();
            }
        }

        /// <summary>
        /// 取得當前管理者詳細資訊
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("admindetail")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAdminDetail()
        {
            //CompanyId StaffAccount StaffPassWord Department EntryDate LevelPosition WorkPosition Email StaffPhoneNumber Auth DepartmentId
            try
            {
                var result = await _admindomain.GetAdminDetailAsync();
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
        /// 刪除員工資料
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpDelete]
        [Route("removestaff")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Removestaff(int id)
        {
            try
            {
                var result = await _admindomain.RemovestaffAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Removestaff));

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
        public async Task<IActionResult> ModifyRulesOfCompany(List<CompanyRuleRequest> requests)
        {
            try
            {
                var result = await _admindomain.UpdateRulesAsync(requests.Select(x => x.ToDTO()));
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
        /// 刪除管理者
        /// </summary>
        /// <param name="eventRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpDelete]
        [Route("manager")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            try
            {
                var result = await _admindomain.DeleteAdminAsync(id);
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
                _logger.LogError(ex, nameof(DeleteAdmin));

                return ServerError500();
            }
        }

        /// <summary>
        /// 停權復權管理者
        /// </summary>
        /// <param name="eventRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPut]
        [Route("manager")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SwitchAdminAsync(int id)
        {
            try
            {
                var result = await _admindomain.SwitchAdminAsync(id);
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
                _logger.LogError(ex, nameof(DeleteAdmin));

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
                    return Ok(result.Data);
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
        /// 部分工時排班紀錄
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
                var result = await _admindomain.EventsGetAsync();

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
        /// 刪除會議
        /// </summary>
        /// <param name="eventRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpDelete]
        [Route("meet")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMeet(int id)
        {
            try
            {
                var result = await _admindomain.DeleteMeetAsync(id);
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
                _logger.LogError(ex, nameof(DeleteMeet));

                return ServerError500();
            }
        }

        /// <summary>
        /// 會議紀錄
        /// </summary>
        /// <param name="ditanceRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("meetdetails")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> MeetEventsGet()
        {
            try
            {
                var result = await _admindomain.MeetEventsGetAsync();

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
        /// 部分工時人員排班
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
        public async Task<IActionResult> ParttimeWorkAdd(ParttimeWorkRequest request)
        {
            try
            {
                var result = await _admindomain.ParttimeWorkAdd(request.StaffId,request.EventStartDate, request.EventEndDate, request.StartTime, request.EndTime, request.Title, request.Detail, request.LevelStatus);

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
                _logger.LogError(ex, nameof(ParttimeWorkAdd));

                return ServerError500();
            }
        }

        /// <summary>
        /// 會議安排
        /// </summary>
        /// <param name="eventRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("meet")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> MeetAdd(MeetRequest request)
        {
            try
            {
                var result = await _admindomain.MeetAdd(request.ToDTO());

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
                _logger.LogError(ex, nameof(ParttimeWorkAdd));

                return ServerError500();
            }
        }

        /// <summary>
        /// 取得員工出勤狀況Excel
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("downloadexcel")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DownloadsStaffCheckrecordExcel(int staffId, int month)
        {
            try
            {
                var result = new CommonResult<FileContentResult>();
                var user = _userService.GetCurrentUser();
                var data = await _admindomain.GetExcelDatasAsync(staffId, month);

                var workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");
                ICellStyle headerStyle = workbook.CreateCellStyle();
                headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                headerStyle.FillPattern = FillPattern.SolidForeground;
                headerStyle.BorderTop = BorderStyle.Thin;
                headerStyle.BorderRight = BorderStyle.Thin;
                headerStyle.BorderBottom = BorderStyle.Thin;
                headerStyle.BorderLeft = BorderStyle.Thin;
                headerStyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                headerStyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                headerStyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                headerStyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.Color = NPOI.HSSF.Util.HSSFColor.Blue.Index;
                headerStyle.SetFont(font);

                //    "員工", "上班打卡時間", "下班打卡時間", "是否超出上班打卡範圍", "是否超出下班打卡範圍",
                //    "是否遲到", "是否早退", "遲到分鐘數", "早退分鐘數", "上班備註", "下班備註"

                IRow Row1 = sheet.CreateRow(0);
                ICell Row1Cell1 = Row1.CreateCell(0);
                Row1Cell1.SetCellValue("員工");
                Row1Cell1.CellStyle = headerStyle;

                ICell Row1Cell2 = Row1.CreateCell(1);
                Row1Cell2.SetCellValue("上班打卡時間");
                Row1Cell2.CellStyle = headerStyle;

                ICell Row1Cell3 = Row1.CreateCell(2);
                Row1Cell3.SetCellValue("上班打卡時間");
                Row1Cell3.CellStyle = headerStyle;

                ICell Row1Cell4 = Row1.CreateCell(3);
                Row1Cell4.SetCellValue("上班打卡時間");
                Row1Cell4.CellStyle = headerStyle;

                ICell Row1Cell5 = Row1.CreateCell(4);
                Row1Cell5.SetCellValue("是否超出上班打卡範圍");
                Row1Cell5.CellStyle = headerStyle;

                ICell Row1Cell6 = Row1.CreateCell(5);
                Row1Cell6.SetCellValue("是否超出下班打卡範圍");
                Row1Cell6.CellStyle = headerStyle;

                ICell Row1Cell7 = Row1.CreateCell(6);
                Row1Cell7.SetCellValue("是否遲到");
                Row1Cell7.CellStyle = headerStyle;

                ICell Row1Cell8 = Row1.CreateCell(7);
                Row1Cell8.SetCellValue("是否早退");
                Row1Cell8.CellStyle = headerStyle;

                ICell Row1Cell9 = Row1.CreateCell(8);
                Row1Cell9.SetCellValue("遲到分鐘數");
                Row1Cell9.CellStyle = headerStyle;

                ICell Row1Cell10 = Row1.CreateCell(9);
                Row1Cell10.SetCellValue("早退分鐘數");
                Row1Cell10.CellStyle = headerStyle;

                ICell Row1Cell11 = Row1.CreateCell(10);
                Row1Cell11.SetCellValue("上班備註");
                Row1Cell11.CellStyle = headerStyle;

                ICell Row1Cell12 = Row1.CreateCell(11);
                Row1Cell12.SetCellValue("下班備註");
                Row1Cell12.CellStyle = headerStyle;

                for (int i = 0; i < data.list.Count(); i++)
                {
                    var row = sheet.CreateRow(i + 1);
                    row.CreateCell(0).SetCellValue(data.staff.StaffName);
                    if (data.list[i].CheckInTime.HasValue)
                    {
                        var culture = new CultureInfo("zh-TW"); // 台灣的中文
                        var dayOfWeek = data.list[i].CheckInTime.Value.ToString("dddd", culture);
                        row.CreateCell(1).SetCellValue(dayOfWeek);
                    }
                    else if (data.list[i].CheckOutTime.HasValue)
                    {
                        var culture = new CultureInfo("zh-TW"); // 台灣的中文
                        var dayOfWeek = data.list[i].CheckOutTime.Value.ToString("dddd", culture);
                        row.CreateCell(1).SetCellValue(dayOfWeek);
                    }
                    else
                    {
                        row.CreateCell(1).SetCellValue("未知");
                    }
                    row.CreateCell(2).SetCellValue(data.list[i].CheckInTime?.ToString("yyyy-MM-dd HH:mm") ?? "尚未打卡");
                    row.CreateCell(3).SetCellValue(data.list[i].CheckOutTime?.ToString("yyyy-MM-dd HH:mm") ?? "尚未打卡");
                    row.CreateCell(4).SetCellValue(data.list[i].IsCheckInOutLocation == 1 ? "是" : "否");
                    row.CreateCell(5).SetCellValue(data.list[i].IsCheckOutOutLocation == 1 ? "是" : "否");
                    row.CreateCell(6).SetCellValue(data.list[i].IsCheckInLate == 1 ? "是" : "否");
                    row.CreateCell(7).SetCellValue(data.list[i].IsCheckOutEarly == 1 ? "是" : "否");
                    row.CreateCell(8).SetCellValue(data.list[i].CheckInLateTimes.ToString());
                    row.CreateCell(9).SetCellValue(data.list[i].CheckOutEarlyTimes.ToString());
                    row.CreateCell(10).SetCellValue(data.list[i].CheckInMemo);
                    row.CreateCell(11).SetCellValue(data.list[i].CheckOutMemo);
                }
                int maxColumnWidth = -1;

                // First, auto size all columns to get their ideal width
                for (int i = 0; i < 400; i++)  // For 30 columns
                {
                    sheet.AutoSizeColumn(i);
                    int width = sheet.GetColumnWidth(i);
                    if (width > maxColumnWidth)
                    {
                        maxColumnWidth = width;
                    }
                }

                // Now set all columns to the maximum width
                for (int i = 0; i < 400; i++)  // For 30 columns
                {
                    sheet.SetColumnWidth(i, maxColumnWidth + 600);
                }
                var tempFileName = Path.GetTempFileName() + ".xlsx";
                using (var fileStream = new FileStream(tempFileName, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                var content = System.IO.File.ReadAllBytes(tempFileName);
                System.IO.File.Delete(tempFileName);  // 刪除臨時檔案

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "出勤狀況單.xlsx");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(DownloadsStaffCheckrecordExcel));

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
        /// 審核加班
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPatch]
        [Route("overtime")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyOvertimeApply(VerifyOverTimeRequest request)
        {
            try
            {
                var result = await _admindomain.VerifyOvertimeApplyAsync(request.OverTimeLogId, request.IsPass);
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
                _logger.LogError(ex, nameof(VerifyOvertimeApply));

                return ServerError500();
            }
        }


        /// <summary>
        /// 審核補卡
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPatch]
        [Route("amendrecord")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyAmendrecord(VerifyAmendRecordRequest request)
        {
            try
            {
                var result = await _admindomain.VerifyAmendrecordAsync(request.AmendRecordId, request.IsPass);
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
                _logger.LogError(ex, nameof(VerifyAmendrecord));

                return ServerError500();
            }
        }

        /// <summary>
        /// 發放員工薪資
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("paymoney")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ReadyToPayMoney(ReadyToPayMoneyRequest request)
        {
            try
            {
                var result = await _admindomain.ReadyToPayMoneyAsync(request.ToDTO(), request.ChangeOverTimeToMoney);
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
                _logger.LogError(ex, nameof(ReadyToPayMoney));

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
        /// 員工加班列表
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("overtime")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOvertime(DateTime start, DateTime end)
        {
            try
            {
                var result = await _admindomain.GetAllOvertimeAsync(start, end);
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
                _logger.LogError(ex, nameof(GetAllOvertime));

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

        /// <summary>
        /// 管理者列表
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpGet]
        [Route("alladmins")]
        [ApTokenAuth]
        [ApCompanyIdAuthAttribute]
        [ApUserAuthAttribute]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAdmins()
        {
            try
            {
                var result = await _admindomain.GetAllAdminsAsync();
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
                _logger.LogError(ex, nameof(GetAllAdmins));

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
                var result = await _admindomain.UploadAvatarAsync(avatar);
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
        /// 新增部門
        /// </summary>
        /// <param name="eventRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPost]
        [Route("newdepartment")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateDepartment(NewDepartmentRequest request)
        {
            try
            {
                var result = await _admindomain.CreateDepartmentAsync(request.DepartmentName);
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
                _logger.LogError(ex, nameof(CreateDepartment));

                return ServerError500();
            }
        }

        /// <summary>
        /// 批次修改部門
        /// </summary>
        /// <param name="eventRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPatch]
        [Route("modifydepartment")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDepartment(List<UpdateDepartmentRequest> requests)
        {
            try
            {
                var result = await _admindomain.UpdateDepartmentAsync(requests.Select(x => x.ToDTO()));
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
                _logger.LogError(ex, nameof(UpdateDepartment));

                return ServerError500();
            }
        }

        /// <summary>
        /// 新增公司規定
        /// </summary>
        /// <param name="eventRequest">請求資料</param>
        /// <response code="200">OK</response>
        /// <response code="400">後端驗證錯誤、少參數、數值有誤、格式錯誤</response>
        /// <response code="403">無此權限</response>
        /// <response code="500">內部錯誤</response>
        /// <returns></returns>
        [HttpPatch]
        [Route("newrule")]
        [ApTokenAuth]
        [ApCompanyIdAuth]
        [ApUserAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateRule(CompanyRuleRequest request)
        {
            try
            {
                var result = await _admindomain.CreateRuleAsync(request.ToDTO());
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
                _logger.LogError(ex, nameof(CreateRule));

                return ServerError500();
            }
        }
    }
}