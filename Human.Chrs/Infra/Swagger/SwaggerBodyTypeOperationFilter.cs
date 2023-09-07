using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LineTag.Admin.Infra.Swagger
{
    public class SwaggerBodyTypeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var bodyTypeAttribute = context.ApiDescription.CustomAttributes().OfType<OpenApiRequestBodyTypeAttribute>().FirstOrDefault();

            if (bodyTypeAttribute != null)
            {
                var schema = context.SchemaGenerator.GenerateSchema(bodyTypeAttribute.BodyType, context.SchemaRepository);

                operation.RequestBody = new OpenApiRequestBody();

                string[] contentTypes;
                if (bodyTypeAttribute.ContentTypes != null)
                {
                    contentTypes = bodyTypeAttribute.ContentTypes;
                }
                else
                {
                    contentTypes = operation.Responses.Where(x => x.Key == "200").SelectMany(x => x.Value.Content).Select(x => x.Key).ToArray();
                }

                foreach (var contentType in contentTypes)
                {
                    operation.RequestBody.Content.Add(KeyValuePair.Create(contentType, new OpenApiMediaType { Schema = schema }));
                }
            }
        }
    }
}