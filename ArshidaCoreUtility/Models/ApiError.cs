using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ArshidaCoreUtility.Models
{
    public class ApiError
    {
        public ApiError(ModelStateDictionary modelState)
        {
            ErrorMessage = "Invalid parameters.";
            Details = modelState
                .FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors
                .FirstOrDefault().ErrorMessage;
        }
        public ApiError()
        {
            dateTime = DateTime.Now;
            Id = PersianDate.NowString();
        }
        public string Id { get; set; }
        public string ErrorMessage { get; set; }
        public string Details { get; set; }
        public DateTime dateTime { get; set; }
        public string Result { get; set; }
    }
}
