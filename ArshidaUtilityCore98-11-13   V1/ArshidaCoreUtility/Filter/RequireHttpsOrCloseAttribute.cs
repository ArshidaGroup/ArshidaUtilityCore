using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ArshidaCoreUtility.Filter
{
    public class RequireHttpsOrCloseAttribute: RequireHttpsAttribute
    {
        protected override void HandleNonHttpsRequest(AuthorizationFilterContext filterContext)
        {
           // base.HandleNonHttpsRequest(filterContext);
           filterContext.Result = new StatusCodeResult(400);
        }
    }
}
