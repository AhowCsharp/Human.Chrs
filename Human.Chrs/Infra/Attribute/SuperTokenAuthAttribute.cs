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
    public class SuperTokenAuthAttribute : System.Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //過濾有設定 AllowAnonymous 屬性的Action
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            if (!context.HttpContext.Request.Headers.ContainsKey("X-Ap-SuperToken"))
            {
                var response = new { error = "Unauthorized", code = "沒有權杖" };
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };

                return;
            }

            try
            {
                var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
                string superTokenSetting = configuration["SuperToken"];

                string superToken = context.HttpContext.Request.Headers["X-Ap-SuperToken"].ToString();
                if (!CryptHelper.VerifySaltHashPlus(superToken, superTokenSetting))
                {
                    var response = new { error = "Token Error", code = "權杖錯誤" };
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