using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcEasyui
{
    /// <summary>
    /// 每个控制器所需要参数都继承此类
    /// </summary>
    public class BaseRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 页索引
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 数据总行数
        /// </summary>
        public int Total { get; set; }
    }
}