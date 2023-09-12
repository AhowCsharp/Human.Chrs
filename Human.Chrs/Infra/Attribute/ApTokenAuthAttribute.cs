using Human.Chrs.Domain.Helper;
using Human.Chrs.Domain.IRepository;
using Human.Chrs.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace Human.Chrs.Infra.Attribute
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
                var response = new { error = "Unauthorized", code = "沒有權杖" };
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
                }
                else
                {
                    var response = new { error = "Token Expired", code = "權杖過期" };
                    context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
            catch
            {
                var response = new { error = "Token Error", code = "權杖錯誤" };
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}