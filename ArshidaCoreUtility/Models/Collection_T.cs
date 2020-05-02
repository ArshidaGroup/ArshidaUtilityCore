using System;
using System.Collections.Generic;
using System.Text;

namespace ArshidaCoreUtility.Models
{
    public class Collection<T>: Resource
    {
        public T[] value { get; set; }

    }
}
