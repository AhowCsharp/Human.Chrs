using Human.Chrs.Infra.Attribute;
using Human.Chrs.Infra.Swagger;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Human.Chrs.Infra.Swagger
{
    public class AddRequireHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters = operation.Parameters?.Any() == true ? operation.Parameters : new List<OpenApiParameter>();

            if (context.ApiDescription.CustomAttributes().Any(x => x.GetType() == typeof(ApTokenAuthAttribute)))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
#if DEBUG
                    Example = new Microsoft.OpenApi.Any.OpenApiString("6a746132-7823-4842-9d3e-ad90242cb8e6"),
#endif
                    Name = "X-Ap-Token",
                    Description = "Application憑證",
                    In = ParameterLocation.Header,
                    Required = true
                });
            }

            if (context.ApiDescription.CustomAttributes().Any(x => x.GetType() == typeof(ApCompanyIdAuthAttribute)))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
#if DEBUG
                    Example = new Microsoft.OpenApi.Any.OpenApiString("1"),
#endif
                    Name = "X-Ap-CompanyId",
                    Description = "CompanyId",
                    In = ParameterLocation.Header,
                    Required = true
                });
            }

            if (context.ApiDescription.CustomAttributes().Any(x => x.GetType() == typeof(ApUserAuthAttribute)))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
#if DEBUG
                    //Example = new Microsoft.OpenApi.Any.OpenApiString("U234a729732732d29f0b1ff1dd9ab4baa,AN34pC0dnUUujrtOLziJrqq8USq6k3/fwEqppe5BajwOFMCrQYHTuHUFbAb/I6/x0A=="), // Loki Tao (working)
                    Example = new Microsoft.OpenApi.Any.OpenApiString("1,AOzIDKKxvfFR0U4c3TCM0I8hSQFfUedykUFrwRpSKB5OXdM5UhYCqDF8zx1Zl5CjOVyhelXFcKoklf3rVyjN+coil22SgF5YYSwATAiKN5gH"), // 阿棻
#endif
                    Name = "X-Ap-UserId",
                    Description = "UserID",
                    In = ParameterLocation.Header,
                    Required = false
                });
            }
        }
    }
}