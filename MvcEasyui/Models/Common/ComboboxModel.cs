using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcEasyui.Models
{
    /// <summary>
    /// easyui combo结构模板
    /// </summary>
    public class ComboboxModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool selected { get; set; }
    }
}