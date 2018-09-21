using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcEasyui.Models;

namespace MvcEasyui.Controllers
{
    /// <summary>
    /// 职位控制器
    /// </summary>
    public class PosController : BaseController
    {
        // GET: Pos
        [Ajax]
        [HttpPost]
        public ActionResult Show()
        {
            return View();
        }
        /// <summary>
        /// 返回TreeModel格式的数据列表
        /// </summary>
        /// <returns></returns>
        [Ajax]
        [HttpPost]
        public JsonResult Get()
        {
            //string posId = Request["id"];
            //List<PositionalViewModel> list = DbHelper.GetPosList(posId,true);
            List<PositionalViewModel> all = DbHelper.GetPosALL(null, true);
            List<PositionalViewModel> first = all.Where(m => string.IsNullOrEmpty(m.Id)).ToList();
            List<TreeModel> result = new List<TreeModel>();
            DbHelper.SetTreeList(result,all.ToList<PositionalViewModel,BaseModel>(),first.ToList<PositionalViewModel, BaseModel>());
            return Json(result);
        }
        [Ajax]
        [HttpPost]
        public JsonResult GetAll(string Name)
        {
            var list = DbHelper.GetPosALL(Name, false);
            return Json(list);
        }
        /// <summary>
        /// 获取此职位下的用户,包含下级职位
        /// </summary>
        /// <param name="id">职位ID</param>
        /// <returns></returns>
        [Ajax]
        [HttpPost]
        public JsonResult GetUser(string id)
        {
            var list = DbHelper.GetUserByPos(id);
            foreach(var item in list)
            {
                item.RoleName = DbHelper.GetRoleByUser(item.Id);
            }
            return Json(list);
        }
        /// <summary>
        /// 新增时,返回对应格式的COMBO列表
        /// </summary>
        /// <returns></returns>
        [Ajax]
        [HttpPost]
        public JsonResult GetPosCombo()
        {
            var list = DbHelper.GetPosALL(null, true);
            return Json(DbHelper.GetPosCombo(list));
        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Ajax]
        [HttpPost]
        public JsonResult Del(List<string> id)
        {
            var result = DbHelper.DelPos(id);
            return Json(result);
        }
        /// <summary>
        /// 新增,编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Ajax]
        [HttpPost]
        public JsonResult Save(PositionalViewModel model)
        {
            if (string.IsNullOrEmpty(model.ParentId))
                model.ParentId = "";
            if (string.IsNullOrEmpty(model.Id))//新增
            {
                var org = DbHelper.GetPos(model.ParentId);
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
                return Json(DbHelper.SavePos(model));
            }
            else
            {
                var json = DbHelper.EditPos(model);
                return Json(json);
            }
        }
    }
}