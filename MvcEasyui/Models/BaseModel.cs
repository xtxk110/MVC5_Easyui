using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcEasyui.Models
{
    public class BaseModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 父节点ID
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 名称,假如是用户模型则为姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 子节点数量
        /// </summary>
        public int SubCount { get; set; }
        /// <summary>
        /// 当前节点是否选中
        /// </summary>
        [JsonProperty("checked")]
        public bool IsChecked { get; set; }
    }
}