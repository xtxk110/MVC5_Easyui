using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcEasyui.Models
{
    /// <summary>
    /// 角色模板
    /// </summary>
    public class RoleViewModel
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        public string Id { get; set; }
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
        public DateTime CreateDate { get; set; }
    }
}