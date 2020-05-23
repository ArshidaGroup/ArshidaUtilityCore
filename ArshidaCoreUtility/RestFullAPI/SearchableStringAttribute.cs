using System;
using System.Collections.Generic;
using System.Text;

namespace ArshidaCoreUtility.RestFullAPI
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SearchableStringAttribute : SearchableAttribute
    {
        public SearchableStringAttribute()
        {
            ExpressionProvider = new StringSearchExpressionProvider();
        }
    }
}
