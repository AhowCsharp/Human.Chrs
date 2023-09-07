using LineTag.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Linq;

namespace LineTag.Admin.Infra.Swagger
{
    public class ProduceResponseTypeModelProvider : IApplicationModelProvider
    {
        public int Order => 1;

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            foreach (ControllerModel controller in context.Result.Controllers)
            {
                if (controller.Attributes.Any(x => x.GetType() == typeof(ApiControllerAttribute)))
                {
                    controller.Filters.Add(new ProducesResponseTypeAttribute(typeof(ResultErrorResponse), StatusCodes.Status401Unauthorized));

                    foreach (ActionModel action in controller.Actions)
                    {
                        if (action.ApiExplorer.IsVisible == true)
                        {
                            if (action.Attributes.All(x => x.GetType() != typeof(HttpGetAttribute)))
                            {
                                action.Filters.Add(new ProducesResponseTypeAttribute(typeof(ResultErrorResponse), StatusCodes.Status400BadRequest));
                            }

                            action.Filters.Add(new ProducesResponseTypeAttribute(typeof(ResultErrorResponse), StatusCodes.Status403Forbidden));
                            action.Filters.Add(new ProducesResponseTypeAttribute(typeof(ResultErrorResponse), StatusCodes.Status500InternalServerError));
                        }
                    }
                }
            }
        }
    }
}