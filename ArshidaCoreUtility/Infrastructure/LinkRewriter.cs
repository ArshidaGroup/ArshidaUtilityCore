﻿using System;
using System.Collections.Generic;
using System.Text;
using ArshidaCoreUtility.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArshidaCoreUtility.Infrastructure
{
    public class LinkRewriter
    {
        private readonly IUrlHelper _urlHelper;

        public LinkRewriter(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public Link Rewrite(Link original)
        {
            if (original == null) return null;
            return new Link
            {
                Href = _urlHelper.Link(original.RouteName, original.RouteValue),
                Method = original.Method,
                Relations = original.Relations
            };
        }
    }
}
