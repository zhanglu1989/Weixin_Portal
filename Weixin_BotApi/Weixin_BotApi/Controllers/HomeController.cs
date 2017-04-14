using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace Weixin_BotApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ILog loginfo = LogManager.GetLogger(typeof(HomeController));
            loginfo.Error("测试，服务开启！");

            return View();
        }
    }
}
