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
        public ActionResult Show()
        {
            return View();
        }
        /// <summary>
        /// 显示下拉列表用
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 获取角色列表,不分页
        /// </summary>
        /// <param name="Name">角色名称</param>
        /// <returns></returns>
        [HttpPost]
        [Ajax]
        public JsonResult GetAll(string Name)
        {
            var list = DbHelper.GetRoleList(Name, false);
            foreach (var item in list)
                item.RightName = DbHelper.GetRightByRole(item.Id);
            return Json(list);
        }
        /// <summary>
        /// 获取角色下的用户列表
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        [HttpPost]
        [Ajax]
        public JsonResult GetUser(string id)
        {
            var list = DbHelper.GetUserByRole(id);
            return Json(list);
        }
        /// <summary>
        /// 新增,编辑
        /// </summary>
        /// <param name="model">RoleViewModel</param>
        /// <returns></returns>
        [HttpPost]
        [Ajax]
        public JsonResult Save(RoleViewModel model)
        {
            if (string.IsNullOrEmpty(model.Id))//新增
            {
                var role = DbHelper.GetRole();
                if (role == null)
                    model.Id =  "0001";
                else
                {
                    var temp = Int64.Parse(role.Id) + 1;
                    model.Id = temp.ToString().PadLeft(role.Id.Length, '0');
                }
                model.IsDefault = false;
                model.IsEnable = true;
                model.CreateDate = DateTime.Now;
                return Json(DbHelper.SaveRole(model));
            }
            else
            {
                var json = DbHelper.EditRole(model);
                return Json(json);
            }
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Ajax]
        public JsonResult Del(List<string> id)
        {
            var result = DbHelper.DelRole(id);
            return Json(result);
        }
        /// <summary>
        /// 获取可用的权限列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Ajax]
        public JsonResult GetRightCombotree()
        { 
            var all = DbHelper.GetAllRights(this, true);
            List<RightViewModel> first = all.Where(m => string.IsNullOrEmpty(m.ParentId)).ToList();
            var treeResult = new List<TreeModel>();
            DbHelper.SetTreeList(treeResult, all.ToList<RightViewModel, BaseModel>(), first.ToList<RightViewModel, BaseModel>());
            
            return Json(treeResult);
        }
    }
}