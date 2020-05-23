using System;
using System.Collections.Generic;
using System.Text;

namespace ArshidaCoreUtility.RestFullAPI
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SortableAttribute : Attribute
    {
        public string EntityProperty { get; set; }

        public bool Default { get; set; }
    }
}
