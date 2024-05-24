using MyProjectDomain.Result;
using System.Net;
using ILogger = Serilog.ILogger;

namespace MyProjectAPI.Middlewares
{
    public class ExeptionHandlingMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _requestDelegate;

        public ExeptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _requestDelegate(httpContext);
            }
            catch (Exception ex)
            {
                await HandlerExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandlerExceptionAsync(HttpContext httpContext, Exception exception)
        {
            _logger.Error(exception, exception.Message);

            var errorMessage = exception.Message;
            var responce = exception switch
            {
                UnauthorizedAccessException => new BaseResult() { ErrorMessage = errorMessage, ErrorCode = (int)HttpStatusCode.Unauthorized },
                _ => new BaseResult() { ErrorMessage = "Internal server error", ErrorCode = (int)HttpStatusCode.InternalServerError }
            };

            httpContext.Response.StatusCode = (int)responce.ErrorCode;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(responce);
        }
    }
}
