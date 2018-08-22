using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;

namespace MvcEasyui
{
    public static class ExtendFunction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="pageIndex">页码数</param>
        /// <param name="pageSize">页大小</param>
        public static void CreatePager(this DynamicParameters parameters,int pageIndex,int pageSize)
        {
            parameters.Add("@pageIndex", pageIndex);
            parameters.Add("@pageSize", pageSize);
        }
    }
}