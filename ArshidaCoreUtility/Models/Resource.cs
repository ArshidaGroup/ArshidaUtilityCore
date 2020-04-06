using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;

namespace ArshidaCoreUtility.Models
{
    public abstract class Resource:Link
    {
        [JsonProperty(Order =  -2)]
        [NotMapped]
        public  Link Self { get; set; }
    }
}
