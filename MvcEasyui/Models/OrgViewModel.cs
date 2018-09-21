using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcEasyui.Models
{
    /// <summary>
    /// 组织部门模型
    /// </summary>
    public class OrgViewModel:BaseModel
    {
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
        
    }
}