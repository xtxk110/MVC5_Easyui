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
                if (current.PageIndex == 1)
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
                return Common.AjaxInfo(-1, "INSERT", "账号重复");

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
            string del = "DELETE FROM Sys_User WHERE Id IN @idList    DELETE FROM Sys_UserRole WHERE UserId IN @idList  ";
           
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
        /// </summary>
        /// <param name="orgName">部门名称</param>
        /// <param name="onlyEnable">是否只查询可用的</param>
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
            var level = list.Where(m => m.Id.Length == 4).ToList();//level1
            foreach (var item in level)
            {
                result.Add(new TreeModel { id = item.Id, text = item.Name });
            }

            level = list.Where(m => m.Id.Length == 8).ToList();//level2
            SetTree(result, level);

            level = list.Where(m => m.Id.Length == 12).ToList();//level3
            foreach (var item in result)
            {
                if (item.children != null)
                    SetTree(item.children, level);
            }

            level = list.Where(m => m.Id.Length == 16).ToList();//level4
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

            level = list.Where(m => m.Id.Length == 20).ToList();//level5
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

            level = list.Where(m => m.Id.Length == 24).ToList();//level6
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
        /// <summary>
        /// 获取相应部门的用户,包括下级部门
        /// </summary>
        /// <param name="orgId">部门ID</param>
        /// <returns></returns>
        public static List<UserViewModel> GetUserListByOrg(string orgId)
        {
            string sql = "SELECT a.Id,a.Code,a.Name,a.IsEnable,a.Comment,b.Name PosName FROM Sys_User a LEFT JOIN Sys_Positional b ON a.PosId=b.Id WHERE CHARINDEX(@orgId, a.OrgId)>0 ORDER BY a.Name ";
            using (IDbConnection conn = DbConnection.Instance())
            {
                return conn.Query<UserViewModel>(sql, new { orgId=orgId }).ToList();
            }
        }
        /// <summary>
        /// 获取相应部门所有的上级部门
        /// </summary>
        /// <param name="orgId">部门ID</param>
        /// <returns></returns>
        public static List<OrgViewModel> GetOrgListById(string orgId)
        {
            string sql = "SELECT * FROM Sys_Organization WHERE CHARINDEX(Id, @orgId)>0 ORDER BY Id ";
            using (IDbConnection conn = DbConnection.Instance())
            {
                return conn.Query<OrgViewModel>(sql, new { orgId=orgId }).ToList();
            }
        }
        public static List<ComboboxModel> GetOrgCombo(List<OrgViewModel> list)
        {
            List<ComboboxModel> result = new List<ComboboxModel>();
            foreach (var item in list)
            {
                result.Add(new ComboboxModel { id = item.Id, text = item.Name });
            }
            return result;
        }
        /// <summary>
        /// 根据父级ID获取ID长度相同且最大的对象
        /// </summary>
        /// <param name="parentId">上级ID</param>
        /// <returns></returns>
        public static OrgViewModel GetOrg(string parentId)
        {
            string sql = "SELECT TOP 1 * FROM Sys_Organization WHERE ISNULL(ParentId, '') = @parentId ORDER BY Id DESC ";
            using (IDbConnection conn = DbConnection.Instance())
            {
                var list= conn.Query<OrgViewModel>(sql, new { parentId = parentId }).ToList();
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            }
        }
        /// <summary>
        /// 根据部门名称获取部门
        /// </summary>
        /// <param name="name">部门名称</param>
        /// <returns></returns>
        public static OrgViewModel GetOrg1(string name)
        {
            string sql = "SELECT TOP 1 * FROM Sys_Organization WHERE Name=@name ORDER BY Id DESC ";
            using (IDbConnection conn = DbConnection.Instance())
            {
                var list = conn.Query<OrgViewModel>(sql, new { name = name }).ToList();
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            }
        }
        /// <summary>
        /// 部门新增
        /// </summary>
        /// <param name="model">OrgViewModel</param>
        /// <returns></returns>
        public static ResultInfo SaveOrg(OrgViewModel model)
        {
            var org = GetOrg1(model.Name);
            if (org != null)
                return Common.AjaxInfo(-1,"INSERT", "部门名称重复");
            else
            {
                try
                {
                    using (IDbConnection conn = DbConnection.Instance())
                    {
                        string insert = "INSERT Sys_Organization VALUES (@Id,@ParentId,@Name,@IsDefault,@IsEnable,@Comment,@CreateDate)";
                        int result= conn.Execute(insert, model);
                        return Common.AjaxInfo(result, "INSERT");
                    }
                }catch(Exception e)
                {
                    return Common.AjaxInfo(-1, "INSERT", e.Message);
                }
            }
        }
        /// <summary>
        /// 部门编辑
        /// </summary>
        /// <param name="model">OrgViewModel</param>
        /// <returns></returns>
        public static ResultInfo EditOrg(OrgViewModel model)
        {
            
            try
            {
                using (IDbConnection conn = DbConnection.Instance())
                {
                    string update = "UPDATE Sys_Organization SET Name=@Name,Comment=@Comment,IsEnable=@IsEnable WHERE Id=@Id ";
                    int result = conn.Execute(update, model);
                    return Common.AjaxInfo(result, "UPDATE");
                }
            }
            catch (Exception e)
            {
                return Common.AjaxInfo(-1, "UPDATE", e.Message);
            }
        }

        public static ResultInfo DelOrg(List<string> id)
        {
            try
            {
                using (IDbConnection conn = DbConnection.Instance())
                {
                    string sql = "SELECT * FROM Sys_User WHERE OrgId IN @orgId ";
                    var list = conn.Query<UserViewModel>(sql, new { orgId = id }).ToList();
                    if (list.Count > 0)
                        return Common.AjaxInfo(-1, "DELETE", "此部门已创建用户,不能删除");
                    list = null;

                    string del = "DELETE Sys_Organization WHERE Id IN @Id ";
                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("@Id", id);
                    int result = conn.Execute(del, dp);
                    return Common.AjaxInfo(result, "DELETE");
                }
            }
            catch (Exception e)
            {
                return Common.AjaxInfo(-1, "DELETE", e.Message);
            }
        }

        public static List<string> GetRoleByUser(string userId)
        {
            string sql = "SELECT a.Name FROM Sys_Role a INNER JOIN Sys_UserRole b ON a.Id=b.RoleId WHERE b.UserId =@userId ";
            using (IDbConnection conn = DbConnection.Instance())
            {
                var list = conn.Query<string>(sql, new { userId = userId }).ToList();
                return list;
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
                    list = list.Where(m => m.Id.Length == 4).ToList();//获取第一级
                return list;
            }
        }

        public static List<PositionalViewModel> GetPosALL(string posName,bool onlyEnable)
        {
            using (IDbConnection conn = DbConnection.Instance())
            {
                return conn.Query<PositionalViewModel>("sp_GetPosList", new { isOnlyEnable = onlyEnable, name = posName }, commandType: CommandType.StoredProcedure).ToList();
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
        /// <summary>
        /// 获取对应职位的用户,包含下级职位
        /// </summary>
        /// <param name="posId"></param>
        /// <returns></returns>
        public static List<UserViewModel> GetUserByPos(string posId)
        {
            string sql = "SELECT a.Id,a.Code,a.Name,a.IsEnable,a.Comment,b.Name OrgName FROM Sys_User a LEFT JOIN Sys_Organization b ON a.OrgId=b.Id WHERE CHARINDEX(@posId, a.PosId)>0 ORDER BY a.Name ";
            using (IDbConnection conn = DbConnection.Instance())
            {
                return conn.Query<UserViewModel>(sql, new { posId = posId }).ToList();
            }
        }

        public static ResultInfo DelPos(List<string> posId)
        {
            try
            {
                using (IDbConnection conn = DbConnection.Instance())
                {
                    string sql = "SELECT * FROM Sys_User WHERE PosId IN @posId ";
                    var list = conn.Query<UserViewModel>(sql, new { posId = posId }).ToList();
                    if (list.Count > 0)
                        return Common.AjaxInfo(-1, "DELETE", "此职位已创建用户,不能删除");
                    list = null;

                    string del = "DELETE Sys_Positional WHERE Id IN @Id ";
                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("@Id", posId);
                    int result = conn.Execute(del, dp);
                    return Common.AjaxInfo(result, "DELETE");
                }
            }
            catch (Exception e)
            {
                return Common.AjaxInfo(-1, "DELETE", e.Message);
            }
        }
        public static List<ComboboxModel> GetPosCombo(List<PositionalViewModel> list)
        {
            List<ComboboxModel> result = new List<ComboboxModel>();
            foreach (var item in list)
            {
                result.Add(new ComboboxModel { id = item.Id, text = item.Name });
            }
            return result;
        }
        /// <summary>
        /// 根据父级ID获取ID长度相同且最大的职位对象
        /// </summary>
        /// <param name="parentId">上级ID</param>
        /// <returns></returns>
        public static OrgViewModel GetPos(string parentId)
        {
            string sql = "SELECT TOP 1 * FROM Sys_Positional WHERE ISNULL(ParentId, '') = @parentId ORDER BY Id DESC ";
            using (IDbConnection conn = DbConnection.Instance())
            {
                var list = conn.Query<OrgViewModel>(sql, new { parentId = parentId }).ToList();
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            }
        }
        /// <summary>
        /// 新增职位
        /// </summary>
        /// <param name="model">PositionalViewModel</param>
        /// <returns></returns>
        public static ResultInfo SavePos(PositionalViewModel model)
        {
            try
            {
                string sql = "SELECT * FROM Sys_Positional WHERE Name=@Name ";
                using (IDbConnection conn = DbConnection.Instance())
                {
                    var list = conn.Query<PositionalViewModel>(sql, new { Name = model.Name }).ToList();
                    if (list.Count > 0)
                        return Common.AjaxInfo(-1, "INSERT", "职位名称相同");

                    list = null;
                    sql = "INSERT Sys_Positional VALUES (@Id,@ParentId,@Name,@IsDefault,@IsEnable,@Comment,@CreateDate)";
                    var result = conn.Execute(sql, model);
                    return Common.AjaxInfo(result, "INSERT");

                }
            }
            catch(Exception e)
            {
                return Common.AjaxInfo(-1, "INSERT", e.Message);
            }
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model">PositionalViewModel</param>
        /// <returns></returns>
        public static ResultInfo EditPos(PositionalViewModel model)
        {
            try
            {
                using (IDbConnection conn = DbConnection.Instance())
                {
                    
                    string sql = "UPDATE Sys_Positional SET Name=@Name,Comment=@Comment,IsEnable=@IsEnable WHERE Id=@Id";
                    var result = conn.Execute(sql, model);
                    return Common.AjaxInfo(result, "UPDATE");

                }
            }
            catch (Exception e)
            {
                return Common.AjaxInfo(-1, "UPDATE", e.Message);
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
        /// <summary>
        /// 获取角色下的用户列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static List<UserViewModel> GetUserByRole(string roleId)
        {
            using (IDbConnection conn = DbConnection.Instance())
            {
                string sql = @"SELECT a.*,b.Name PosName,c.Name OrgName FROM Sys_User a LEFT JOIN Sys_Positional b ON a.PosId=b.Id LEFT JOIN Sys_Organization c ON a.OrgId=c.Id 
                              LEFT JOIN Sys_UserRole d ON a.Id = d.UserId WHERE d.RoleId = @roleId ";
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@roleId", roleId);
                return conn.Query<UserViewModel>(sql, dp).ToList();
            }
        }
        /// <summary>
        /// 获取ID值最大的一个ROLE对象
        /// </summary>
        /// <returns></returns>
        public static RoleViewModel GetRole()
        {
            string sql = " SELECT TOP 1 * FROM Sys_Role ORDER BY Id DESC ";
            using (IDbConnection conn = DbConnection.Instance())
            {
                var list= conn.Query<RoleViewModel>(sql).ToList();
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            }
        }
        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="model">RoleViewModel</param>
        /// <returns></returns>
        public static ResultInfo SaveRole(RoleViewModel model)
        {
            IDbTransaction tran = null;
            try
            {
                using (IDbConnection conn = DbConnection.Instance())
                {
                    string sql = "SELECT * FROM Sys_Role WHERE Name=@Name ";
                    var list = conn.Query<RoleViewModel>(sql, model).ToList();
                    if (list.Count > 0)
                        return Common.AjaxInfo(-1, "INSERT", "角色名重复");
                    tran = conn.BeginTransaction();
                    list = null;
                    sql = "DELETE FROM Sys_RoleMenu WHERE RoleId=@Id  INSERT INTO Sys_Role VALUES(@Id,@Name,@IsDefault,@IsEnable,@Comment,@CreateDate) ";
                    int result = conn.Execute(sql, model,transaction:tran);
                    if (model.RightId.Count > 0)
                    {
                        model.RoleMenuList = new List<RoleMenuModel>();
                        string sql1 = "INSERT Sys_RoleMenu VALUES (@Id,@MenuId,@RoleId,@CreateDate)";
                        foreach (var item in model.RightId)
                            model.RoleMenuList.Add(new RoleMenuModel { Id = Common.NewId(), RoleId = model.Id, MenuId = item, CreateDate = DateTime.Now });
                        result += conn.Execute(sql1, model.RoleMenuList, transaction:tran);
                    }
                    tran.Commit();

                    return Common.AjaxInfo(result, "INSERT");
                }
            }
            catch(Exception e)
            {
                tran.Rollback();
                return Common.AjaxInfo(-1, "INSERT", e.Message);
            }
        }
        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="model">RoleViewModel</param>
        /// <returns></returns>
        public static ResultInfo EditRole(RoleViewModel model)
        {
            IDbTransaction tran = null;
            try
            {
                using (IDbConnection conn = DbConnection.Instance())
                {
                    tran = conn.BeginTransaction();
                    string sql = " UPDATE Sys_Role SET Name=@Name,Comment=@Comment,IsEnable=@IsEnable WHERE Id=@Id ";
                    if (string.IsNullOrEmpty(model.Name)&&model.RightId!=null&&model.RightId.Count > 0)
                        sql = "DELETE FROM Sys_RoleMenu WHERE RoleId=@Id ";
                    int result = conn.Execute(sql, model,transaction:tran);
                    if (model.RightId != null&&model.RightId.Count > 0)
                    {
                        model.RoleMenuList = new List<RoleMenuModel>();
                        string sql1 = "INSERT Sys_RoleMenu VALUES (@Id,@MenuId,@RoleId,@CreateDate)";
                        foreach (var item in model.RightId)
                            model.RoleMenuList.Add(new RoleMenuModel { Id = Common.NewId(), RoleId = model.Id, MenuId = item, CreateDate = DateTime.Now });
                        result += conn.Execute(sql1, model.RoleMenuList,transaction:tran);
                    }
                    tran.Commit();

                    return Common.AjaxInfo(result, "UPDATE");
                }
            }
            catch (Exception e) { tran.Rollback(); return Common.AjaxInfo(-1, "UPDATE", e.Message); }
        }

        public static ResultInfo DelRole(List<string> roleId)
        {
            try
            {
                using (IDbConnection conn = DbConnection.Instance())
                {
                    string sql = "SELECT UserId FROM Sys_UserRole WHERE RoleId IN @roleId ";
                    var list = conn.Query<string>(sql, new { roleId = roleId }).ToList();
                    if(list.Count>0)
                        return Common.AjaxInfo(-1, "DELETE", "角色包含用户,不能删除");

                    list = null;
                    sql = "DELETE FROM Sys_Role WHERE Id IN @roleId  DELETE FROM Sys_RoleMenu WHERE RoleId IN @roleId";
                    int result = conn.Execute(sql, new { roleId = roleId });
                    return Common.AjaxInfo(result, "DELETE");
                }
            }catch(Exception e)
            {
                return Common.AjaxInfo(-1, "DELETE", e.Message);
            }
        }
        /// <summary>
        /// 通过角色ID获取匹配的权限名称
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public static List<string> GetRightByRole(string roleId)
        {
            string sql = "SELECT a.Name FROM Sys_Menu a INNER JOIN Sys_RoleMenu b ON a.Id=b.MenuId WHERE b.RoleId = @roleId";
            using (IDbConnection conn = DbConnection.Instance())
            {
                var list = conn.Query<string>(sql, new { roleId = roleId }).ToList();
                return list;
            }
        }
        #endregion

        #region 权限菜单
        /// <summary>
        /// 获取顶级菜单项
        /// </summary>
        /// <param name="current">BaseController</param>
        /// <param name="onlyEnable">是否只查询可用的权限菜单</param>
        /// <returns></returns>
        public static DatagridJson<RightViewModel> GetTopRightList(BaseController current,bool onlyEnable)
        {
            DatagridJson<RightViewModel> result = new DatagridJson<RightViewModel>();
            result.total = current.Total;
            using (IDbConnection conn = DbConnection.Instance())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.CreatePager(current.PageIndex, current.PageSize);
                parameters.Add("@total", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("isOnlyEnable", onlyEnable);
                parameters.Add("@name", current.Request["Name"]);
                parameters.Add("@rightId", current.Request["id"]);//当前选择的权限ID，用于获取下级列表
                parameters.Add("@roleId", current.Request["roleId"]);//查询权限是否属于此角色
                result.rows = conn.Query<RightViewModel>("sp_GetTopRightList", parameters, commandType: CommandType.StoredProcedure).ToList();
                if (string.IsNullOrEmpty(current.Request["id"]))
                {
                    if (current.PageIndex == 1)
                        result.total = parameters.Get<int>("@total");
                }
                else
                {
                    result.total = result.rows.Count;
                }
                

            }
            return result;
        }
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="menuid">权限ID</param>
        /// <returns></returns>
        public static ResultInfo DelRight(List<string> menuid)
        {
            try
            {
                using (IDbConnection conn = DbConnection.Instance())
                {
                    string sql = "SELECT * FROM Sys_RoleMenu WHERE MenuId in @menuId ";
                    var list = conn.Query<string>(sql, new { menuId = menuid }).ToList();
                    if (list.Count > 0)
                        return Common.AjaxInfo(-1, "DELETE", "权限已分发,不能删除");

                    list = null;
                    sql = "DELETE FROM Sys_Menu WHERE Id IN  @menuId ";
                    int result = conn.Execute(sql, new { menuId = menuid });
                    return Common.AjaxInfo(result, "DELETE");
                }
            }
            catch (Exception e)
            {
                return Common.AjaxInfo(-1, "DELETE", e.Message);
            }
        }
        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="model">RightViewModel</param>
        /// <returns></returns>
        public static ResultInfo SaveRight(RightViewModel model)
        {
            IDbTransaction tran = null;
            try
            {
                using (IDbConnection conn = DbConnection.Instance())
                {
                    string sql = "SELECT * FROM Sys_Menu WHERE Name=@Name ";
                    var list = conn.Query<RightViewModel>(sql, model).ToList();
                    if (list.Count > 0)
                        return Common.AjaxInfo(-1, "INSERT", "权限名称重复");
                    tran = conn.BeginTransaction();
                    list = null;
                    sql = "INSERT INTO Sys_Menu VALUES(@Id,@ParentId,@Type,@Name,@Path,@IsDefault,@IsEnable,@SortIndex,@Comment,@CreateDate) ";
                    int result = conn.Execute(sql, model, transaction: tran);
                    tran.Commit();

                    return Common.AjaxInfo(result, "INSERT");
                }
            }
            catch (Exception e)
            {
                tran.Rollback();
                return Common.AjaxInfo(-1, "INSERT", e.Message);
            }
        }
        /// <summary>
        /// 通过权限同父级权限下ID值排序最大对象
        /// </summary>
        /// <param name="parentId">父级ID</param>
        /// <returns></returns>
        public static RightViewModel GetRight(string parentId)
        {
            using (IDbConnection conn = DbConnection.Instance())
            {
                string sql = "SELECT TOP 1 * FROM Sys_Menu WHERE ISNULL(ParentId, '') = @parentId ORDER BY Id DESC ";
                var list = conn.Query<RightViewModel>(sql, new { parentId = parentId }).ToList();
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
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