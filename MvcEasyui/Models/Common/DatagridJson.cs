using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcEasyui.Models
{
    public class DatagridJson<T>
    {
        /// <summary>
        /// easyui datagrid数据总行数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// easyui datagrid数据行
        /// </summary>
        public List<T> rows { get; set; }
    }
}