using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcEasyui.Models;

namespace MvcEasyui.Controllers
{
    public class UserController : BaseController
    { 
        // GET: Sys
        [Ajax]
        [HttpPost]
        public ActionResult Show()
        {
            return View();
        }
        [Ajax]
        [HttpPost]
        public JsonResult Get()
        {
            var data= DbHelper.GetUserList(this,false);
            foreach(var item in data.rows)
            {
                item.RoleName = DbHelper.GetRoleByUser(item.Id);
            }
            return Json(data);
        }
        [HttpPost]
        [Ajax]
        public JsonResult Save(UserViewModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString("N");
                model.CreateDate = DateTime.Now;
                model.IsEnable = true;
                model.IsDefault = false;
                return Json(DbHelper.SaveUser(model));
            }
            else
            {
                model.IsDefault = false;
                return Json(DbHelper.EditUser(model));
            }
            
        }
        [Ajax]
        [HttpPost]
        public JsonResult Del(List<string> id)
        {
            return Json(DbHelper.DelUser(id));
        }
    }
}