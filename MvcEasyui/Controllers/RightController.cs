using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcEasyui.Controllers
{
    /// <summary>
    /// 权限控制器,目前包括菜单权限,按钮权限,数据权限
    /// </summary>
    public class RightController : Controller
    {
        // GET: Right
        public ActionResult Index()
        {
            return View();
        }
    }
}