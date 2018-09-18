using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcEasyui
{
    /// <summary>
    /// 需要分页时,或使用easyui datagrid分页时,请关注PageSize,PageIndex,Total三个属性
    /// </summary>
    public class BaseController : Controller
    {
        // GET: Base
        /// <summary>
        /// easyui datagrid默认页面数据大小参数;其他分页客客户端统一传此参数.对应客户端参数 rows
        /// </summary>
        public int PageSize {
            get {
                if (string.IsNullOrWhiteSpace(Request["rows"]))
                {
                    if (string.IsNullOrWhiteSpace(Request["PageSize"]))
                        return 1000;
                    else
                        return int.Parse(Request["PageSize"]);
                }
                else
                    return int.Parse(Request["rows"]);
            }
        }
        /// <summary>
        /// easyui datagrid默认页索引,从1开始;其他分页客客户端统一传此参数.对应客户端参数 page
        /// </summary>
        public int PageIndex {
            get {
                if (string.IsNullOrWhiteSpace(Request["page"]))
                {
                    if (string.IsNullOrWhiteSpace(Request["PageIndex"]))
                        return 1;
                    else
                        return int.Parse(Request["PageIndex"]);
                }
                else
                    return int.Parse(Request["page"]);
            }
        }
        /// <summary>
        /// easyui datagrid默认数据总数,客户端请求第1页时,服务端返回总数(实际查询时赋值真实数据),之后页数,客户端发起请求均带上此参数及值.对应客户端参数 total
        /// </summary>
        private int _total;
        public int Total {
            get {
                if (string.IsNullOrWhiteSpace(Request["total"]))
                {
                    if (string.IsNullOrWhiteSpace(Request["Total"]))
                        return _total;
                    else
                        return int.Parse(Request["Total"]);
                }
                else
                    return int.Parse(Request["total"]);
            }
            set { _total = value; }
        }
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {

            base.OnAuthorization(filterContext);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
        protected  override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new CustomJsonResult() { Data = data, ContentType = contentType, ContentEncoding = contentEncoding, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        protected new  JsonResult Json(object data)
        {
            return this.Json(data, null, Encoding.Default, JsonRequestBehavior.AllowGet);
        }
    }
}