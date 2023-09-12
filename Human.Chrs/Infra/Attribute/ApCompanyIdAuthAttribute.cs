using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain.IRepository;

namespace Human.Chrs.Infra.Attribute
{
    public class ApCompanyIdAuthAttribute : System.Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //過濾有設定 AllowAnonymous 屬性的Action
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            if (!context.HttpContext.Request.Headers.ContainsKey("X-Ap-CompanyId"))
            {
                var response = new { error = "Unauthorized", code = "公司尚未註冊" };
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };

                return;
            }

            try
            {
                var comapnyRepository = context.HttpContext.RequestServices.GetService(typeof(ICompanyRepository)) as ICompanyRepository;
                var x_CompanyId = context.HttpContext.Request.Headers["X-Ap-CompanyId"].ToString();
                var CompanyId = Convert.ToInt32(x_CompanyId);

                if (await comapnyRepository.IsAvailableCompanyAsync(CompanyId))
                {
                    var userService = context.HttpContext.RequestServices.GetService(typeof(UserService)) as UserService;
                    userService.SetCompanyId(CompanyId);
                }
                else
                {
                    var response = new { error = "Unauthorized", code = "公司尚未註冊" };
                    context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
            catch
            {
                var response = new { error = "Unauthorized", code = "公司尚未註冊" };
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}