using System;
using System.Collections.Generic;
using System.Text;

namespace ArshidaCoreUtility.RestFullAPI
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SearchableDecimalAttribute : SearchableAttribute
    {
        public SearchableDecimalAttribute()
        {
            ExpressionProvider = new DecimalToIntSearchExpressionProvider();
        }
    }
}
