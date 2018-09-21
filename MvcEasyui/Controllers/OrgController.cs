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
        [Ajax]
        [HttpPost]
        public ActionResult Show()
        {
            return View();
        }
        /// <summary>
        /// 返回 TreeModel格式的数据
        /// </summary>
        /// <returns></returns>
        [Ajax]
        [HttpPost]
        public JsonResult Get()
        {
            List<OrgViewModel> all = DbHelper.GetOrgList(null,true);
            List<OrgViewModel> first = all.Where(m => string.IsNullOrEmpty(m.ParentId)).ToList();
            List<TreeModel> result = new List<TreeModel>();
            DbHelper.SetTreeList(result, all.ToList<OrgViewModel,BaseModel>(),first.ToList<OrgViewModel, BaseModel>());
            return Json(result);
        }
        [Ajax]
        [HttpPost]
        public JsonResult GetAll(string Name)
        {
            var list = DbHelper.GetOrgList(Name, false);
            return Json(list);
        }
        /// <summary>
        /// 获取相应部门的用户
        /// </summary>
        /// <param name="id">部门ID</param>
        /// <returns></returns>
        [Ajax]
        [HttpPost]
        public JsonResult GetUser(string id)
        {
            var list = DbHelper.GetUserListByOrg(id);
            foreach(var item in list)
            {
                item.RoleName = DbHelper.GetRoleByUser(item.Id);
            }
            return Json(list);
        }
        [Ajax]
        [HttpPost]
        public JsonResult GetOrgCombo()
        {
            var list = DbHelper.GetOrgList(null, true);
            return Json(DbHelper.GetOrgCombo(list));
        }
        /// <summary>
        /// 新增,编辑
        /// </summary>
        /// <param name="model">OrgViewModel</param>
        /// <returns></returns>
        [Ajax]
        [HttpPost]
        public JsonResult Save(OrgViewModel model)
        {
            if (string.IsNullOrEmpty(model.ParentId))
                model.ParentId = "";
            if (string.IsNullOrEmpty(model.Id))//新增
            {
                var org = DbHelper.GetOrg(model.ParentId);
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
                return Json(DbHelper.SaveOrg(model));
            }
            else
            {
                var json = DbHelper.EditOrg(model);
                return Json(json);
            }
        }
        [Ajax]
        [HttpPost]
        public JsonResult Del(List<string> id)
        {
            var result = DbHelper.DelOrg(id);
            return Json(result);
        }
    }
}