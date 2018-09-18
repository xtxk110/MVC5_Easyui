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
        /// 角色ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 角色名称
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
        /// <summary>
        /// 角色对应的权限ID列表
        /// </summary>
        public List<string> RightId { get; set; }
        /// <summary>
        /// 角色对应的权限名称列表
        /// </summary>
        public List<string> RightName { get; set; }
        /// <summary>
        /// 角色权限映射模型
        /// </summary>
        public List<RoleMenuModel> RoleMenuList { get; set; }
    }
}