using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cinema.API.Filters;

public class HttpResponseExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = new ObjectResult(new { 
            error = context.Exception.Message, 
            date = DateTime.UtcNow 
        }) { StatusCode = 400 };
        context.ExceptionHandled = true;
    }
}