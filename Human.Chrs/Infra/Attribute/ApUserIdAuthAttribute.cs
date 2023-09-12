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
    public class ApUserIdAuthAttribute : System.Attribute, IAsyncAuthorizationFilter
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

            if (!context.HttpContext.Request.Headers.ContainsKey("X-Ap-Account"))
            {
                var response = new { error = "Unauthorized", code = "未註冊會員" };
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };

                return;
            }

            try
            {
                var loginDomain = context.HttpContext.RequestServices.GetService(typeof(LoginDomain)) as LoginDomain;
                var x_CompanyId = context.HttpContext.Request.Headers["X-Ap-CompanyId"].ToString();
                var CompanyId = Convert.ToInt32(x_CompanyId);
                var Account = context.HttpContext.Request.Headers["X-Ap-Account"].ToString();

                var admin = await loginDomain.GetAdminWithSaltHashAsync(CompanyId, Account);
                if (admin == null)
                {
                    var response = new { error = "Unauthorized", code = "未註冊會員" };
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
                    var logger = loggerFactory.CreateLogger<ApUserIdAuthAttribute>();

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