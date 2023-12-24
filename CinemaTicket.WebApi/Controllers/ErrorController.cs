using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using CinemaTicket.Infrastructure.Exceptions;

namespace CinemaTicket.WebApi.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var problemDetails = new CustomExceptionDetails();
            var statusCode = 500;
            if (context.Error is CustomException exception)
            {
                statusCode = 400;
                problemDetails.Message = exception.Message;
            }
            if (context.Error is NotFoundException)
            {
                statusCode = 404;
            }
            problemDetails.Type = context.Error.GetType().Name;
            if (!webHostEnvironment.IsProduction())
            {
                problemDetails = new CustomDevExceptionDetails
                {
                    Type = problemDetails.Type,
                    Message = context.Error.Message,
                    StackTrace = context.Error.StackTrace,
                };
            }
            return new ObjectResult(problemDetails)
            {
                StatusCode = statusCode
            };
        }
    }
}
