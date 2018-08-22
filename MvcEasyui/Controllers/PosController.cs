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
        public ActionResult Show()
        {
            return View();
        }
        public JsonResult Get()
        {
            string posId = Request["id"];
            List<PositionalViewModel> list = DbHelper.GetPosList(posId,true);
            List<TreeModel> result = new List<TreeModel>();
            DbHelper.SetPosTreeList(result, list);
            return Json(result);
        }
    }
}