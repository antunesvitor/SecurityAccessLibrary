using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SecurityAPI
{
    public class VerifyAccess : ActionFilterAttribute
    {
        private string FirstMessage;
        private string FinalMessage;

        public VerifyAccess(string firstMessage, string finalMessage){
            FirstMessage = firstMessage;
            FinalMessage = finalMessage;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"============================={FirstMessage}===================================");
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine($"============================={FinalMessage}===================================");
        }
    }
}
