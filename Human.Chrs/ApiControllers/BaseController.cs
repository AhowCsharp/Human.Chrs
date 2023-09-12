using Microsoft.AspNetCore.Mvc;

namespace LineTag.Admin.ApiControllers
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public virtual ObjectResult BadRequest400(IEnumerable<string> commonErrors)
        {
            return StatusCode(StatusCodes.Status400BadRequest, commonErrors);
        }

        [NonAction]
        public virtual ObjectResult Unauthorized401()
        {
            return StatusCode(StatusCodes.Status401Unauthorized, string.Empty);
        }

        [NonAction]
        public virtual ObjectResult Forbidden403()
        {
            return StatusCode(StatusCodes.Status403Forbidden, string.Empty);
        }

        [NonAction]
        public virtual ObjectResult ServerError500()
        {
            return StatusCode(StatusCodes.Status500InternalServerError, string.Empty);
        }

        [NonAction]
        public virtual ObjectResult ServerError500(string message)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
    }
}