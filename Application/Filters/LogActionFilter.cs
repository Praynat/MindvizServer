using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace MindvizServer.Application.Filters
{
    public class LogActionFilter:IActionFilter
    {
        private string timestamp;
        private Stopwatch stopwatch=new Stopwatch();

        public void OnActionExecuting(ActionExecutingContext context)
        {
            timestamp= "["+DateTime.Now+"]";
            stopwatch.Start();
            Console.WriteLine($"Action {context.ActionDescriptor.DisplayName} is executing.");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            string path = context.HttpContext.Request.Path;
            string method=context.HttpContext.Request.Method;
            string? origin = context.HttpContext.Request.Headers["origin"].FirstOrDefault() ;
            stopwatch.Stop();
            string responseTime=stopwatch.ElapsedMilliseconds+"ms";
            int statusCode= context.HttpContext.Response.StatusCode;
            Console.WriteLine($"{timestamp} incoming request from {origin} {path} {method} {statusCode} - {responseTime}");
        }
    }
}
