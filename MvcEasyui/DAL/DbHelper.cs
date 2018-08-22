using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Common;
using Dapper;
using System.Web.Mvc;
using MvcEasyui.Models;

namespace MvcEasyui
{
    public class DbHelper
    {
        #region 用户操作
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="current">当前控制器的自定义基类控制器</param>
        /// <param name="onlyEnable">是否只查询启用的</param>
        /// <returns></returns>
        public static DatagridJson<UserViewModel> GetUserList(BaseController current, bool onlyEnable)
        {
            DatagridJson<UserViewModel> result = new DatagridJson<UserViewModel>();
            result.total = current.Total;
            using (IDbConnection conn = DbConnection.Instance())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.CreatePager(current.PageIndex, current.PageSize);
                parameters.Add("@total", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("isOnlyEnable", onlyEnable);
                parameters.Add("@code", current.Request["Code"]);
                parameters.Add("@name", current.Request["Name"]);
                parameters.Add("orgId", current.Request["OrgId"]);
                parameters.Add("@posId", current.Request["PosId"]);
                parameters.Add("@roleId", current.Request["RoleId"]);

                result.rows = conn.Query<UserViewModel>("sp_GetUserList", parameters, commandType: CommandType.StoredProcedure).ToList();
                if (result.total == 0)
                    result.total = parameters.Get<int>("@total");

            }
            return result;
        }
        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static ResultInfo SaveUser(UserViewModel user)
        {
            //检查登录账号是否重复
            UserViewModel single = GetUser(user.Code);
            if (single != null)
                return Common.AjaxInfo(0, "INSERT", "账号重复");

            using (IDbConnection conn = DbConnection.Instance())
            {
                int result = -1;
                IDbTransaction tran=null;
                try
                {
                    tran = conn.BeginTransaction();
                    string insert = "INSERT INTO Sys_User VALUES(@id,@code,@pwd,@name,@orgId,@posId,@isEnable,@isDefault,@comment,@createDate)";
                    result = conn.Execute(insert, new { id = user.Id, name = user.Name, code = user.Code, pwd = user.Pwd, orgId = user.OrgId, posId = user.PosId, isEnable = user.IsEnable, isDefault = user.IsDefault, comment = user.Comment, createDate = user.CreateDate },tran);
                    if (user.RoleId != null && user.RoleId.Count > 0 && result > 0)//增加用户角色
                    {
                        string insert1 = "INSERT INTO Sys_UserRole VALUES(@id,@userId,@roleId,@createDate)";
                        List<object> list = new List<object>();
                        foreach (var item in user.RoleId)
                        {
                            list.Add(new { id = Guid.NewGuid().ToString("N"), userId = user.Id, roleId = item, createDate = DateTime.Now });
                        }
                        result += conn.Execute(insert1, list,tran);
                    }
                    tran.Commit();
                    return Common.AjaxInfo(result, "INSERT");
                }
                catch(Exception e)
                {
                    result = -1;
                    tran.Rollback();
                    return Common.AjaxInfo(result, "INSERT",e.Message);
                }

                

            }
        }
        public static ResultInfo EditUser(UserViewModel user)
        {
            using (IDbConnection conn = DbConnection.Instance())
            {
                int result = -1;
                IDbTransaction tran = null;
                try
                {
                    tran= conn.BeginTransaction();
                    string update = @" DELETE FROM Sys_UserRole  WHERE UserId=@id 
                                   UPDATE Sys_User SET Code=@code,Name=@name,PosId=@posId,OrgId=@orgId,IsEnable=@isEnable,Comment=@comment WHERE Id=@id ";
                    result = conn.Execute(update, new { id = user.Id, code = user.Code, name = user.Name, orgId = user.OrgId, posId = user.PosId, isEnable = user.IsEnable, comment = user.Comment },tran);
                    if (user.RoleId != null && user.RoleId.Count > 0)//增加用户角色
                    {
                        string insert = "INSERT INTO Sys_UserRole VALUES(@id,@userId,@roleId,@createDate)";
                        List<object> list = new List<object>();
                        foreach (var item in user.RoleId)
                        {
                            list.Add(new { id = Guid.NewGuid().ToString("N"), userId = user.Id, roleId = item, createDate = DateTime.Now });
                        }
                        result += conn.Execute(insert, list,tran);
                    }
                    tran.Commit();
                    return Common.AjaxInfo(result, "UPDATE");
                }catch(Exception e)
                {
                    result = -1;
                    tran.Rollback();
                    return Common.AjaxInfo(result, "UPDATE",e.Message);
                }
            }
        }
        /// <summary>
        /// 根据账号获取用户
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static UserViewModel GetUser(string code)
        {
            string sql = @"SELECT a.*,b.Name OrgName,c.Name PosName FROM Sys_User a LEFT JOIN Sys_Organization b ON a.OrgId = b.Id 
                           LEFT JOIN Sys_Positional c ON a.PosId = c.Id WHERE a.Code=@code";
            using (IDbConnection conn = DbConnection.Instance())
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@code", code);
                return conn.Query<UserViewModel>(sql, dp).SingleOrDefault();
            }
        }
        /// <summary>
        /// 删除相应用户
        /// </summary>
        /// <param name="idList"></param>
        public static ResultInfo DelUser(List<string> idList)
        {
            string del = "DELETE FROM Sys_User WHERE Id IN @idList";
           
            using (IDbConnection conn = DbConnection.Instance())
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@idList", idList);
                int r = conn.Execute(del, dp);
                return Common.AjaxInfo(r, "DELETE");
            }
            
        }
        #endregion
        #region 部门操作
        /// <summary>
        /// 获取所有部门列表,数量不会很多,不分页
        /// <param name="orgName">部门名称</param>
        /// <param name="onlyEnable">是否只查询可用的</param>
        /// </summary>
        /// <returns></returns>
        public static List<OrgViewModel> GetOrgList(string orgName, bool onlyEnable)
        {
            using (IDbConnection conn = DbConnection.Instance())
            {
                return conn.Query<OrgViewModel>("sp_GetOrgList", new { isOnlyEnable = onlyEnable, name = orgName }, commandType: CommandType.StoredProcedure).ToList();
            }
        }
        /// <summary>
        /// 组织部门转换成TREE结构
        /// </summary>
        /// <param name="result"></param>
        /// <param name="list"></param>
        public static void SetOrgTreeList(List<TreeModel> result, List<OrgViewModel> list)
        {
            var level = list.Where(m => m.Id.Length == 3).ToList();//level1
            foreach (var item in level)
            {
                result.Add(new TreeModel { id = item.Id, text = item.Name });
            }

            level = list.Where(m => m.Id.Length == 6).ToList();//level2
            SetTree(result, level);

            level = list.Where(m => m.Id.Length == 9).ToList();//level3
            foreach (var item in result)
            {
                if (item.children != null)
                    SetTree(item.children, level);
            }

            level = list.Where(m => m.Id.Length == 12).ToList();//level4
            foreach (var item in result)
            {
                if (item.children != null)
                {
                    foreach (var item1 in item.children)
                    {
                        if (item1.children != null)
                            SetTree(item1.children, level);
                    }
                }
            }

            level = list.Where(m => m.Id.Length == 15).ToList();//level5
            foreach (var item in result)
            {
                if (item.children != null)
                {
                    foreach (var item1 in item.children)
                    {
                        if (item1.children != null)
                        {
                            foreach (var item2 in item1.children)
                            {
                                if (item2.children != null)
                                    SetTree(item2.children, level);
                            }

                        }

                    }
                }
            }

            level = list.Where(m => m.Id.Length == 18).ToList();//level6
            foreach (var item in result)
            {
                if (item.children != null)
                {
                    foreach (var item1 in item.children)
                    {
                        if (item1.children != null)
                        {
                            foreach (var item2 in item1.children)
                            {
                                if (item2.children != null)
                                {
                                    foreach (var item3 in item2.children)
                                    {
                                        if (item3.children != null)
                                            SetTree(item3.children, level);
                                    }
                                }
                            }

                        }

                    }
                }
            }

        }
        private static void SetTree(List<TreeModel> tree, List<OrgViewModel> levelList)
        {
            foreach (var item in tree)
            {
                foreach (var item1 in levelList)
                {
                    if (item.id == item1.ParentId)
                    {
                        if (item.children == null)
                            item.children = new List<TreeModel> { new TreeModel { id = item1.Id, text = item1.Name } };
                        else
                            item.children.Add(new TreeModel { id = item1.Id, text = item1.Name });
                    }
                }
            }
        }
        #endregion
        #region 职位操作
        /// <summary>
        /// 获取所有职位列表,数量不会很多,不分页
        /// <param name="posId">职位ID</param>
        /// <param name="onlyEnable">是否只查询可用的</param>
        /// </summary>
        /// <returns></returns>
        public static List<PositionalViewModel> GetPosList(string posId, bool onlyEnable)
        {
            using (IDbConnection conn = DbConnection.Instance())
            {
                DynamicParameters parameters = new DynamicParameters();
                string sql = string.Empty;
                if (onlyEnable)
                    sql = "SELECT b.*,(SELECT COUNT(Id) FROM Sys_Positional a WHERE a.ParentId=b.Id) as SubCount FROM Sys_Positional b  WHERE b.IsEnable=1 ";
                else
                    sql = "SELECT b.*,(SELECT COUNT(Id) FROM Sys_Positional a WHERE a.ParentId=b.Id) as SubCount FROM Sys_Positional b WHERE 1=1  ";
                if (!string.IsNullOrEmpty(posId))
                {
                    sql = sql + " AND b.ParentId=@posId ";
                    parameters.Add("@posId", posId);
                }

                sql = sql + " ORDER BY b.Id ";
                var list = conn.Query<PositionalViewModel>(sql, parameters).ToList();
                if (string.IsNullOrEmpty(posId))
                    list = list.Where(m => m.Id.Length == 3).ToList();//获取第一级
                return list;
            }
        }

        /// <summary>
        ///职位转换成TREE结构
        /// </summary>
        /// <param name="result"></param>
        /// <param name="list"></param>
        public static void SetPosTreeList(List<TreeModel> result, List<PositionalViewModel> list)
        {
            foreach (var item in list)
            {
                TreeModel tree = new TreeModel { id = item.Id, text = item.Name };
                if (item.SubCount > 0)
                    tree.state = "closed";
                else
                    tree.state = "open";
                result.Add(tree);
            }

        }
        #endregion
        #region 角色操作
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <param name="onlyEnable">是否只查询启用的</param>
        /// <returns></returns>
        public static List<RoleViewModel> GetRoleList(string roleName, bool onlyEnable)
        {
            using (IDbConnection conn = DbConnection.Instance())
            {
                return conn.Query<RoleViewModel>("sp_GetRoleList", new { isOnlyEnable = onlyEnable, name = roleName }, commandType: CommandType.StoredProcedure).ToList();
            }
        }
        public static List<ComboboxModel> GetRoleCombo(List<RoleViewModel> list)
        {
            List<ComboboxModel> result = new List<ComboboxModel>();
            foreach(var item in list)
            {
                result.Add(new ComboboxModel { id = item.Id, text = item.Name });
            }
            return result;
        }
        /// <summary>
        /// 通过用户ID,获取角色列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public static List<RoleViewModel> GetRoleList(string userId)
        {
            string sql = "SELECT a.Id,a.Name FROM Sys_Role a INNER JOIN Sys_UserRole b ON a.Id=b.RoleId WHERE b.UserId=@userId";
            using (IDbConnection conn = DbConnection.Instance())
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@userId", userId);
                return conn.Query<RoleViewModel>(sql, dp).ToList();
            }
        }
        #endregion

    }
    /// <summary>
    /// 针对SQLSERVER的数据库连接
    /// </summary>
    public class DbConnection
    {
        private static string ConnStr { get { return ConfigurationManager.ConnectionStrings["conn"].ConnectionString; } }
        public static IDbConnection Instance()
        {
            IDbConnection conn = null;
            try
            {
                conn = new SqlConnection(ConnStr);
                conn.Open();
                return conn;
            }
            catch { if (conn != null) { conn.Dispose(); conn = null; } return conn; }
        }
    }
}