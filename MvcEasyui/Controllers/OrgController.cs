using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcEasyui.Models;

namespace MvcEasyui.Controllers
{
    /// <summary>
    /// 组织部门控制器
    /// </summary>
    public class OrgController : BaseController
    {
        // GET: Org
        public ActionResult Show()
        {
            return View();
        }
        public JsonResult Get()
        {
            List<OrgViewModel> list = DbHelper.GetOrgList(null,true);
            List<TreeModel> result = new List<TreeModel>();
            DbHelper.SetOrgTreeList(result, list);
            return Json(result);
        }
    }
}