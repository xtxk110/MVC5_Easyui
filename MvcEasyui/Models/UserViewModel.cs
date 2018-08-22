using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcEasyui.Models
{
    /// <summary>
    /// 用户界面模型
    /// </summary>
    public class UserViewModel
    {
        public string Id { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 口令
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 组织部门ID
        /// </summary>
        public string OrgId { get; set; }
        /// <summary>
        /// 组织部门名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 职位ID
        /// </summary>
        public string PosId { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>
        public string PosName { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public List<string> RoleId { get; set; }
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