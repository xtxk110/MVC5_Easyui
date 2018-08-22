using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MvcEasyui.Models
{
    public class UserInfo
    {
        [Display(Name ="账号")]
        public string User { get; set; }
        [Display(Name = "密码")]
        public string Pwd { get; set; }
    }
}