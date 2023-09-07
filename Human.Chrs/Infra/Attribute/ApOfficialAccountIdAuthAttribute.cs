using LineTag.Core.Domain.Admin;
using LineTag.Core.Models;
using LineTag.Core.Repositories;
using LineTag.Core.Repositories.New;
using LineTag.Core.Services;
using LineTag.Core.Utility;
using LineTag.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LineTag.Admin.Infra.Attribute
{
    public class ApOfficialAccountIdAuthAttribute : System.Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //過濾有設定 AllowAnonymous 屬性的Action
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            if (!context.HttpContext.Request.Headers.ContainsKey("X-Ap-OfficialAccountId"))
            {
                var response = new ResultErrorResponse(ResultError.D0301OAID);
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };

                return;
            }

            try
            {
                var officialAccountRepository = context.HttpContext.RequestServices.GetService(typeof(INewOfficialAccountRepository)) as INewOfficialAccountRepository;
                var x_officialAccountId = context.HttpContext.Request.Headers["X-Ap-OfficialAccountId"].ToString();
                var officialAccountId = Convert.ToInt32(x_officialAccountId);

                if (await officialAccountRepository.IsAvailableOfficialAccountAsync(officialAccountId))
                {
                    var userService = context.HttpContext.RequestServices.GetService(typeof(UserService)) as UserService;
                    userService.SetOfficialAccountId(officialAccountId);
                }
                else
                {
                    var response = new ResultErrorResponse(ResultError.D0303OA);
                    context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
            catch
            {
                var response = new ResultErrorResponse(ResultError.D0304OA);
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}