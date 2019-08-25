using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using ShoppingApi.Shared.Exceptions;
using System;
using System.Diagnostics;
using System.Net;

namespace ShoppingApi.Tools
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(
                IHostingEnvironment hostingEnvironment,
                IModelMetadataProvider modelMetadataProvider,
                ILogger<CustomExceptionFilter> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var isDev = _hostingEnvironment.IsDevelopment();

            var error = new ErrorModel()
            {
                Message = isDev ? exception.Demystify().ToString() : exception.Message,
                StackTrace = isDev ? exception.StackTrace.Split('\n') : null
            };

            if (exception.InnerException != null && isDev)
            {
                error.InnnerException = new ErrorModel()
                {
                    Message = exception.InnerException.Message,
                    StackTrace = exception.InnerException.StackTrace.Split('\n')
                };
            }

            var correlationId = context.HttpContext.TraceIdentifier;
            _logger.LogError(exception, exception.Message, correlationId);

            context.Result = new ObjectResult(error)
            { StatusCode = GetStatusCode(exception) };
        }

        private class ErrorModel
        {
            public string Message { get; set; }
            public string[] StackTrace { get; set; }
            public ErrorModel InnnerException { get; set; }
        }

        private int GetStatusCode(Exception ex)
        {
            switch (ex)
            {
                case var _ when ex is CartNotFoundException:
                    return (int)HttpStatusCode.NotFound;
                case var _ when ex is ArgumentException:
                    return (int)HttpStatusCode.BadRequest;
                default:
                    return (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
