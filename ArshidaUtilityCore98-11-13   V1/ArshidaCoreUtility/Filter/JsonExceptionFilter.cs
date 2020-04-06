using System;
using System.Collections.Generic;
using System.Text;
using ArshidaCoreUtility.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ArshidaCoreUtility.Filter
{
    public class JsonExceptionFilter:IExceptionFilter
    {
        private readonly IHostingEnvironment _environment;

        public JsonExceptionFilter(IHostingEnvironment evnvironment)
        {
            _environment = evnvironment;
        }
        public void OnException(ExceptionContext context)
        {
            var error = new ApiError();
            if (_environment.IsDevelopment())
            {
                error.ErrorMessage = context.Exception.Message;
                error.Details = context.Exception.StackTrace;
            }
            else
            {
                error.ErrorMessage = "A server error occurred";
                error.Details = context.Exception.Message;
            }

            context.Result = new ObjectResult(error)
            {
                StatusCode = 500
            };
        }
    }
}
