using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcEasyui.Models;

namespace MvcEasyui.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.Title = "登录";
            ViewBag.Footer = "XXXX管理系统登录界面";
            return View(new UserInfo());
        }
        [HttpPost]
        public ActionResult Index(UserInfo info)
        {
            string errorKey = "error";
            string u = "admin";
            string p = "123456";
            bool validate = true;
            if (string.IsNullOrWhiteSpace(info.User) || string.IsNullOrWhiteSpace(info.Pwd))
            {
                ModelState.AddModelError(errorKey, "用户或口令不能为空");
                validate = false;
            } else if (info.Pwd.Length < 6)
            {
                ModelState.AddModelError(errorKey, "密码长度不小于6");
                validate = false;
            }else if (info.User != u || info.Pwd != p)
            {
                ModelState.AddModelError(errorKey, "用户及密致不匹配");
                validate = false;
            }

            if (validate)
            {
                return new RedirectResult("/Home/Index");
            }
            else
                return View(info);
        }
    }
}