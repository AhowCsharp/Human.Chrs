using LineTag.Core.Domain.Admin;
using LineTag.Core.Enums;
using LineTag.Core.Models;
using LineTag.Core.Services;
using LineTag.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LineTag.Admin.Infra.Attribute
{
    public class ApLineUserIdAuthAttribute : System.Attribute, IAsyncAuthorizationFilter
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

            if (AllowSkipLineUserId)
            {
                // 不需 LINE UserId 的權杖
                var currentUser = userService.GetCurrentUser();
                if (currentUser.SkipLineUserId)
                {
                    currentUser.Name = "Biz";
                    currentUser.AuthType = AuthType.Administrator;

                    return;
                }
            }

            if (!context.HttpContext.Request.Headers.ContainsKey("X-Ap-LineUserId"))
            {
                var response = new ResultErrorResponse(ResultError.D0301LUID);
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };

                return;
            }

            try
            {
                var loginDomain = context.HttpContext.RequestServices.GetService(typeof(LoginDomain)) as LoginDomain;
                var x_officialAccountId = context.HttpContext.Request.Headers["X-Ap-OfficialAccountId"].ToString();
                var officialAccountId = Convert.ToInt32(x_officialAccountId);
                var lineUserId = context.HttpContext.Request.Headers["X-Ap-LineUserId"].ToString();

                var admin = await loginDomain.GetAdminWithSaltHashAsync(officialAccountId, lineUserId);
                if (admin == null)
                {
                    var response = new ResultErrorResponse(ResultError.D0303LU);
                    context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else
                {
                    userService.SetCurrentAdmin(admin);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger<ApLineUserIdAuthAttribute>();

                    logger.LogError(ex, "X-Ap-LineUserId");
                }
                catch
                {
                }

                var response = new ResultErrorResponse(ResultError.D0304LU);
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}