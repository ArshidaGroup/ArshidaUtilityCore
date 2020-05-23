using System;
using System.Collections.Generic;
using System.Text;

namespace ArshidaCoreUtility.RestFullAPI
{
    public class RootResponse : Resource
    {
        public Link Info { get; set; }

        public Link Rooms { get; set; }
    }
}
