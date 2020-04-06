using System;
using System.Collections.Generic;
using System.Text;

namespace ArshidaCoreUtility.Models
{
    public class ApiError
    {
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
