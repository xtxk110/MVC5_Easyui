using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MvcEasyui
{
    public static class Common
    {
        /// <summary>
        /// ajax访问返回特定信息(不包括数据查询)
        /// </summary>
        /// <param name="execResult">数据库执行(EXECUTE)返回的结果,-1,执行失败</param>
        /// <param name="action">对应的操作类型</param>
        /// <param name="ext">额外的返回信息</param>
        /// <returns></returns>
        public static ResultInfo AjaxInfo(int execResult,string action,string ext="")
        {
            ResultInfo info = new ResultInfo();
            info.Type = action;
            if (execResult >= 0)
            {
                info.Result = true;
                info.Msg = "【"+action+"】数据成功 "+ext;
            }
            else
            {
                info.Result = false;
                info.Msg= "【" +action+ "】数据失败 "+ext;
            }
            return info;
        }
        /// <summary>
        /// 生成以日期(yyyyMMddHHmm)开头的16位随机数字符
        /// </summary>
        /// <returns></returns>
        public static string NewId()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("yyyyMMddHHmm").ToString());
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            sb.Append(rd.Next(1000, 9999).ToString());

            return sb.ToString();
        }

        public static List<TResult> ToList<TSource,TResult>(this IEnumerable<TSource> source) where TResult:class
        {
            List<TResult> result = new List<TResult>();
            if(source != null)
            {
                foreach (TSource item in source)
                {
                    TResult r = item as TResult;
                    if (r == null)
                        break;
                    else
                        result.Add(r);
                }
            }
            return result;
        }
    }
}