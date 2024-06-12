using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace HnMicro.Framework.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private const int _errCodeException = -99999;
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment webHostEnvironment)
        {
            _next = next;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var message = ex.GetMessageException();
                var stacktrace = _webHostEnvironment.IsDevelopment() ? ex.GetStackTraceException() : string.Empty;
                var responseMessage = new ApiResponse<string>
                {
                    Message = new ApiResponseMessage(_errCodeException, message, stacktrace)
                };

                var response = context.Response;
                response.ContentType = "application/json";
                switch (ex.GetType().Name)
                {
                    case nameof(BadRequestException):
                        var exception = (BadRequestException)ex;
                        responseMessage.Message.ErrCode = exception.OverrideErrCode;
                        responseMessage.Message.ErrMsg = exception.OverrideErrMsg;
                        response.StatusCode = HttpStatusCode.BadRequest.ToInt();
                        await response.WriteAsync(responseMessage.ToString());
                        return;

                    case nameof(ForbiddenException):
                        response.StatusCode = HttpStatusCode.Forbidden.ToInt();
                        break;

                    case nameof(NotFoundException):
                        response.StatusCode = HttpStatusCode.NotFound.ToInt();
                        break;

                    case nameof(UnauthorizedException):
                        response.StatusCode = HttpStatusCode.Unauthorized.ToInt();
                        break;

                    case nameof(HnMicroException):
                    default:
                        response.StatusCode = HttpStatusCode.InternalServerError.ToInt();
                        break;
                }
                await response.WriteAsync(responseMessage.ToString());
            }
        }
    }
}
