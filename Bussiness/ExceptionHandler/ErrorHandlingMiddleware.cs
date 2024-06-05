using Common.ExceptionHandler.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Common.ExceptionHandler
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(BadRequestException e)
            {
                await HandleBadRequestExceptionAsync(context, e);
            }
            catch (NotFoundException e)
            {
                await HandleNotFoundExceptionAsync(context, e);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var message = exception.Message.ToString();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            var errorResponse = new ErrorResponse(message, 500);
            var jsonErrorResponse = JsonConvert.SerializeObject(errorResponse);
            return context.Response.WriteAsync(jsonErrorResponse);
        }

        private static Task HandleBadRequestExceptionAsync(HttpContext context, BadRequestException exception)
        {
            var message = exception.Message.ToString();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 400;
            var errorResponse = new ErrorResponse(message, 400);
            var jsonErrorResponse = JsonConvert.SerializeObject(errorResponse);
            return context.Response.WriteAsync(jsonErrorResponse);
        }

        private static Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException exception)
        {
            var message = exception.Message.ToString();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 400;
            var errorResponse = new ErrorResponse(message, 400);
            var jsonErrorResponse = JsonConvert.SerializeObject(errorResponse);
            return context.Response.WriteAsync(jsonErrorResponse);
        }
    }

}
