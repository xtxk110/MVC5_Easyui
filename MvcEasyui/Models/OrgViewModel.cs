using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcEasyui.Models
{
    /// <summary>
    /// 组织部门模型
    /// </summary>
    public class OrgViewModel
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 上级部门ID,最顶层为NULL
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 是否预置
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 子级数量
        /// </summary>
        public int SubCount { get; set; }
        public DateTime CreateDate { get; set; }
    }
}