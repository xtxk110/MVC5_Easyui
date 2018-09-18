using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcEasyui.Models
{
    /// <summary>
    /// easyui tree模型
    /// </summary>
    public class TreeModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public string state { get; set; }
        public bool @checked { get; set; }
        public object attributes { get; set; }
        public List<TreeModel> children { get; set; }
        public int level { get; set; }
    }
}