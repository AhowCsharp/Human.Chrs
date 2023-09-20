using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Human.Chrs.Domain;
using Human.Chrs.Domain.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Human.Chrs.Infra.Attribute
{
    public class ApUserAuthAttribute : System.Attribute, IAsyncAuthorizationFilter
    {
        public bool AllowSkipLineUserId { get; set; } = false;

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //過濾有設定 AllowAnonymous 屬性的Action
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var userService = (UserService)context.HttpContext.RequestServices.GetService(typeof(UserService));

            if (!context.HttpContext.Request.Headers.ContainsKey("X-Ap-UserId"))
            {
                var response = new { error = "Unauthorized", code = "未註冊會員" };
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };

                return;
            }

            try
            {
                var loginDomain = context.HttpContext.RequestServices.GetService(typeof(LoginDomain)) as LoginDomain;
                if (!context.HttpContext.Request.Headers.ContainsKey("X-Ap-AdminToken"))
                {
                    var x_CompanyId = context.HttpContext.Request.Headers["X-Ap-CompanyId"].ToString();
                    var CompanyId = Convert.ToInt32(x_CompanyId);
                    var UserId = context.HttpContext.Request.Headers["X-Ap-UserId"].ToString();

                    var verifyResult = await loginDomain.GetUserWithSaltHashAsync(CompanyId, UserId);
                    if (!verifyResult.Success)
                    {
                        var response = new { error = "Unauthorized", code = "未註冊會員" };
                        context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
                    }
                    else
                    {
                        userService.SetCurrentUser(verifyResult.Data);
                    }
                }
                else
                {
                    var x_CompanyId = context.HttpContext.Request.Headers["X-Ap-CompanyId"].ToString();
                    var CompanyId = Convert.ToInt32(x_CompanyId);
                    var UserId = context.HttpContext.Request.Headers["X-Ap-UserId"].ToString();

                    var verifyResult = await loginDomain.GetAdminWithSaltHashAsync(CompanyId, UserId);
                    if (!verifyResult.Success)
                    {
                        var response = new { error = "Unauthorized", code = "未註冊管理者" };
                        context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
                    }
                    else
                    {
                        userService.SetCurrentUser(verifyResult.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger<ApUserAuthAttribute>();

                    logger.LogError(ex, "X-Ap-LineUserId");
                }
                catch
                {
                }

                var response = new { error = "Unauthorized", code = "未註冊會員" };
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}