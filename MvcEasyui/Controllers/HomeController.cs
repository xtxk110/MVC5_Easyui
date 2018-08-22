using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcEasyui.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Ttile = "XXX管理系统";
            ViewBag.Footer = "XXX管理系统  版权所有";
            return View();
        }
    }
}