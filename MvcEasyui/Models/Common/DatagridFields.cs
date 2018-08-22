using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcEasyui.Models
{
    /// <summary>
    /// easyui datagrid 字段模型
    /// </summary>
    public class DatagridFields
    {
        /// <summary>
        /// 存储easyui-datagrid 字段列表(key为字段,value为字段名称),使用Datagrid.cshtml通用模板时
        /// </summary>
        public Dictionary<string, string> DgDic { get; set; }
        /// <summary>
        /// 表格宽度
        /// </summary>
        public int FieldWidth { get; set; }
    }
}