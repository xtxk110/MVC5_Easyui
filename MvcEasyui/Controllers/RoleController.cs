using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcEasyui.Models;

namespace MvcEasyui.Controllers
{
    /// <summary>
    /// 角色控制器
    /// </summary>
    public class RoleController : BaseController
    {
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Ajax]
        public JsonResult Get()
        {
            if (string.IsNullOrEmpty(Request["userId"]))
            {
                List<RoleViewModel> list = DbHelper.GetRoleList(Request["Name"], true);
                return Json(DbHelper.GetRoleCombo(list));
            }
            else
            {
                List<ComboboxModel> all = DbHelper.GetRoleCombo(DbHelper.GetRoleList(Request["Name"], true));
                List<RoleViewModel> list = DbHelper.GetRoleList(Request["userId"]);
                foreach(var item in all)
                {
                    foreach (var item1 in list)
                    {
                        if (item1.Id == item.id)
                        {
                            item.selected = true;
                            break;
                        }
                    }
                }
                return Json(all);
            }
           
        }
    }
}