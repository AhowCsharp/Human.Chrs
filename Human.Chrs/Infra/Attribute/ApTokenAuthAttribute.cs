using LineTag.Core.Domain.Admin;
using LineTag.Core.Domain.DTO;
using LineTag.Core.Models;
using LineTag.Core.Repositories;
using LineTag.Core.Services;
using LineTag.Core.Utility;
using LineTag.Infrastructure.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace LineTag.Admin.Infra.Attribute
{
    public class ApTokenAuthAttribute : System.Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //過濾有設定 AllowAnonymous 屬性的Action
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            if (!context.HttpContext.Request.Headers.ContainsKey("X-Ap-Token"))
            {
                var response = new ResultErrorResponse(ResultError.D0301TOKEN);
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };

                return;
            }

            try
            {
                var applicationRepository = context.HttpContext.RequestServices.GetService(typeof(IApplicationRepository)) as IApplicationRepository;
                string apToken = context.HttpContext.Request.Headers["X-Ap-Token"].ToString();

                var ap = await applicationRepository.GetAsync(apToken);
                if (ap?.Expire > DateTimeHelper.TaipeiNow)
                {
                    var userService = (UserService)context.HttpContext.RequestServices.GetService(typeof(UserService));
                    userService.SetApToken(apToken);
                    userService.SetSkipLineUserId(ap.SkipLineUserId);
                }
                else
                {
                    var response = new ResultErrorResponse(ResultError.D0303TOKEN);
                    context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
            catch
            {
                var response = new ResultErrorResponse(ResultError.D0304TOKEN);
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}