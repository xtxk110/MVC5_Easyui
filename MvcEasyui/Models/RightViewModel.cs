﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace MvcEasyui.Models
{
    /// <summary>
    /// 权限模型
    /// </summary>
    public class RightViewModel:BaseModel
    {
        /// <summary>
        /// easyui tree 默认使用id
        /// </summary>
        //public string id { get; set; }
        /// <summary>
        /// easyui tree node状态 open,closed
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 权限类型:0为MENU菜单类型;1为BUTTON 页面里的按钮功能类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 页面路径
        /// </summary>
        public string UrlPath { get; set; }
        /// <summary>
        /// 是否预置
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 排序号,默认1,越小排名靠前
        /// </summary>
        public int SortIndex { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }
    }
}