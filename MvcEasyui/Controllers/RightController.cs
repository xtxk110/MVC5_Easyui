using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcEasyui.Models;

namespace MvcEasyui.Controllers
{
    /// <summary>
    /// 权限控制器,目前包括菜单权限,按钮权限,数据权限
    /// </summary>
    public class RightController : BaseController
    {
        // GET: Right
        [HttpPost]
        [Ajax]
        public ActionResult Show()
        {
            return View();
        }
        /// <summary>
        /// 获取顶级的权限菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Ajax]
        public JsonResult GetTop()
        {
            var grid = DbHelper.GetTopRightList(this, false);
            if (string.IsNullOrEmpty(Request["id"]))
                return Json(grid);
            else  //获取下级列表，不用分页
                return Json(grid.rows);

        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Ajax]
        public JsonResult Del(List<string> id)
        {
            var result = DbHelper.DelRight(id);
            return Json(result);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Save(RightViewModel model)
        {
            if (string.IsNullOrEmpty(model.ParentId))
                model.ParentId = "";
            if (string.IsNullOrEmpty(model.Id))//新增
            {
                var org = DbHelper.GetRight(model.ParentId);
                if (org == null)
                    model.Id = model.ParentId + "0001";
                else
                {
                    var temp = Int64.Parse(org.Id) + 1;
                    model.Id = temp.ToString().PadLeft(org.Id.Length, '0');
                }
                model.IsDefault = false;
                model.IsEnable = true;
                model.CreateDate = DateTime.Now;
                return Json(DbHelper.SaveRight(model));
            }
            else
            {
                //var json = DbHelper.EditOrg(model);
                return Json("");
            }
        }
    }
}