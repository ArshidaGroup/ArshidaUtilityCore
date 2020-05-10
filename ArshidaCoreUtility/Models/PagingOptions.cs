using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArshidaCoreUtility.Models
{
    public class PagingOptions
    {
        [Range(0, 99999, ErrorMessage = "Offset must be greater than 0")]
        public int? Offset { get; set; }

        [Range(1, 99999, ErrorMessage = "Limit must be greater than 0 and less than 100")]
        public int? Limit { get; set; }
    }
}
