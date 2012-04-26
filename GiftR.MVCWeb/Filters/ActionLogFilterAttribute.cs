using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GiftR.Common;

namespace GiftR.MVCWeb.Filters
{
    public class ActionLogFilterAttribute : ActionFilterAttribute, IActionFilter
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoggingManager.Logger.Info(filterContext.Controller);
        }
    }
}