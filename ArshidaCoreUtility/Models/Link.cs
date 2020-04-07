using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ArshidaCoreUtility.Models
{
    public class Link
    {
        public const string GetMethod = "Get";

        public static Link To(string RouteName, object RouteValue = null)
            => new Link
            {
                RouteName = RouteName,
                RouteValue = RouteValue,
                Method = GetMethod,
                Relations = null

            };
        
        [JsonProperty(Order = -4)]
        public string Href { get; set; }
        
        [JsonProperty(Order = -3, PropertyName = "rel", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Relations { get; set; }
        
        [JsonProperty(Order = -2, PropertyName = "method",DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(GetMethod)]
        public string Method { get; set; }

        // stores the route name before being rewritten by the LinkRewritingFilter
        [JsonIgnore] 
        public string RouteName { get; set; }


        // stores the route name before being rewritten by the LinkRewritingFilter
        [JsonIgnore]
        public object RouteValue { get; set; }
    }
}
