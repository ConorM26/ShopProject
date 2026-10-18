using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace ShopWeb.Filters;

public class ShopApiExceptionFilter(
    ILogger<ShopApiExceptionFilter> logger,
    IModelMetadataProvider metadataProvider) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not (HttpRequestException
            or TimeoutRejectedException
            or BrokenCircuitException))
        {
            return; // not an API problem: let normal error handling run
        }

        logger.LogError(context.Exception, "Shop API call failed for {Path}",
            context.HttpContext.Request.Path);

        context.Result = new ViewResult
        {
            ViewName = "ApiUnavailable",
            StatusCode = StatusCodes.Status503ServiceUnavailable,
            ViewData = new ViewDataDictionary(metadataProvider,
                context.ModelState)
        };

        context.ExceptionHandled = true;
    }
}