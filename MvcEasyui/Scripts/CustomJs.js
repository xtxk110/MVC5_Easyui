var Total = 0;

//jquery 方法扩展 FORM表单序列化为JSON
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};
function ShowInfo(title, msg) {
    $.messager.show({
        title: title,
        msg: msg,
        timeout: 1000,
        showType: 'show'
    });
}
//用于用户TITLE显示角色,行中必须有RoleName 数组
function ShowRole(value, row, index) {
    if (row.RoleName.length > 0) {
        var t = row.RoleName.join("\n\t");
        t = "所属角色:\n\t" + t;
        return '<span title="' + t + '" style="color:blue">' + value + '</span>'
    } else {
        return value;
    }
}

function RightType(value, row, index){
    if (value == 0)
        return "MENU";
    else
        return "BUTTON";
}

function ShowRight(value, row, index) {
    if (row.RightName.length > 0) {
        var t = row.RightName.join("\n\t");
        t = "权限列表:\n\t" + t;
        return '<span title="' + t + '" style="color:blue">' + value + '</span>'
    } else {
        return value;
    }
}
function EditRight(value, row, index) {
    return '<span style="color:red">权限编辑</span>';
}

//使用easyui 清除表单数据
function FormClear(formId) {
    $('#' + formId).form('clear');
}
//使用easyui from提交方式,自动序列化from表单为对应的JSON对象
//参数: form id 选择器(不包含#),url 提交的服务地址,dialogId easyui-dialog id,datagrid_id
function FormSubmit(id, url,data,dialogId,datagrid_id,datagrid_url) {
    $("#" + id).form('submit', {
        //url: url,
        onSubmit: function () {
            var isValid = $(this).form('validate');
            if (!isValid) {
                ShowInfo("ErrorInfo", "请查检表单数据");
            } else {//使用ajax提交数据
                
                $.ajax({
                    url: url,
                    //contentType:'application/json',
                    type:'POST',
                    dataType: 'json',
                    data: data,
                    success: function (data) {
                        if (data.Result) {
                            $("#" + dialogId).dialog('close'); $("#" + id).form('clear');
                            $("#" + datagrid_id).datagrid('load', { url: datagrid_url});
                        } else
                            ShowInfo("ErrorInfo", data.Msg);
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ShowInfo("ErrorInfo", textStatus);
                        }
                });
            }
            //return isValid;	
            return false;//阻止默认表单提交
        },
        success: function () {
            
        }
    });
}
//easyui-datagrid 提交时,把total总数参数传递到服务器,免去服务器每次都返回总数
//参数:datagrid_id, url, total_id:第一次获取时存储总数值的非表单元素
function DatagridTotalInit(datagrid_id, url, total_id) {
    $("#" + datagrid_id).datagrid({
        url: url,
        pageSize: 20,
        pageList: [20, 30, 50],
        onBeforeLoad: function (param) {
            param.total = $("#" + total_id).text();
        },
        loadFilter: function (data) {
            $("#" + total_id).text(data.total); return data;
        }
    });
}

//----------------------------------------用户界面特定JS--------------------
function UserViewInit() {
    $("#btn-ok").on("click", function (e) {
        e.preventDefault();
        $.messager.confirm({
            title: '提示',
            msg: '确定此操作吗?',
            fn: function (r) {
                if (r) {
                    if ($("#dialog_add").dialog('options').title.indexOf("查询") > -1) {
                        $("#dg").datagrid('load', $("#fm_add").serializeObject());
                        $("#dialog_add").dialog('close');
                    } else
                        FormSubmit("fm_add", "/User/Save", $("#fm_add").serialize(), "dialog_add", "dg","/User/Get");
                } else
                    return false;
            }
        });
        
    });
    $("#btn-cancel").on("click", function (e) {
        e.preventDefault();
        $("#dialog_add").dialog('close');
    });
    $("#btn_add").on("click", function (e) {
        e.preventDefault();
        $("#Pwd").textbox({ required: true });
        $("#Code").textbox({ required: true });
        $("#Name").textbox({ required: true });
        $("#OrgId").combotree({ required: true, url: '/Org/Get' });
        $("#PosId").combotree({ required: true, url: '/Pos/Get' });
        $("#RoleId").combobox({ required: true, url: '/Role/Get' });
        $("#div-pwd").show();
        $("#comment-div").show();
        $("#div-enable").hide();
        $("#Id").val("");
        $("#dialog_add").dialog({
            title: '增加用户',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
    });
    $("#btn_edit").on("click", function (e) {
        e.preventDefault();
        var rows = $("#dg").datagrid('getSelections');
        if (rows.length == 0) {
            ShowInfo("ErrorInfo", "请选择一条数据,再编辑");
            return false;
        }
        if (rows.length > 1) {
            ShowInfo("ErrorInfo", "不能同时多条数据编辑");
            return false;
        }
        $("#div-pwd").hide();
        $("#div-enable").show();
        $("#comment-div").show();
        $("#dialog_add").dialog({
            title: '编辑用户',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
        $("#Id").val(rows[0].Id);
        $("#Code").textbox({ required: true }).textbox('setValue', rows[0].Code);
        $("#Name").textbox({ required: true }).textbox('setValue', rows[0].Name);
        $("#OrgId").combotree({ required: true, url: '/Org/Get' }).combotree('setValue', rows[0].OrgId);
        $("#PosId").combotree({ required: true, url: '/Pos/Get' }).combotree('setValue', rows[0].PosId);
        $("#RoleId").combobox({ required: false, url: '/Role/Get?userId=' + rows[0].Id });
        $("#Pwd").textbox({ required: false }).textbox('setValue', "");
        if (rows[0].IsEnable == true)
            $("#IsEnable").switchbutton('check');
        $("#Comment").textbox('setValue', rows[0].Comment);
       
    });
    $("#btn_query").on("click", function (e) {
        e.preventDefault();
        $("#div-enable").hide();
        $("#comment-div").hide();
        $("#div-pwd").hide();
        $("#Pwd").textbox({ required: false });
        $("#Code").textbox({ required: false });
        $("#Name").textbox({ required: false });
        $("#OrgId").combotree({ required: false, url: '/Org/Get' });
        $("#PosId").combotree({ required: false, url: '/Pos/Get' });
        $("#RoleId").combotree({ required: false, url: '/Role/Get' });
        $("#dialog_add").dialog({
            title: '查询用户',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
    });
    $("#btn_delete").on("click", function (e) {
        e.preventDefault();
        var rows = $("#dg").datagrid('getSelections');
        if (rows.length == 0) {
            ShowInfo("ErrorInfo", "至少选择一条数据");
            return false;
        }
        $.messager.confirm({
            title: '提示',
            msg: '确定删除吗?',
            fn: function (r) {
                if (r) {
                    var idArr = [];
                    for (i = 0; i < rows.length; i++) {
                        idArr.push(rows[i].Id);
                    }
                    $.ajax({
                        url: '/User/Del',
                        type: 'POST',
                        dataType: 'json',
                        data: { id: idArr },
                        success: function (data) {
                            if (data.Result)
                                $("#dg").datagrid('load', { url: '/User/Get' });
                            else
                                ShowInfo("ErrorInfo", data.Msg);
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            ShowInfo("ErrorInfo", textStatus);
                        }
                    });
                } else
                    return false;
            }
        });
        
    });
}

//----------------------------------------部门界面特定JS--------------------------------------------------------
function OrgViewInit() {
    $('#dg_org').datagrid({
        url:'/Org/GetAll',
        view: detailview,
        detailFormatter: function (index, row) {
            return '<div style="padding:2px;position:relative;"><table class="ddv"></table></div>';
        },
        onExpandRow: function (index, row) {
            var ddv = $(this).datagrid('getRowDetail', index).find('table.ddv');
            ddv.datagrid({
                url: '/Org/GetUser?id=' + row.Id,
                fitColumns: true,
                singleSelect: true,
                rownumbers: true,
                loadMsg: '',
                height: 'auto',
                columns: [[
                    { field: 'RoleName', align: 'center', hidden:true },
                    { field: 'Code', title: '账号', align:'center', width:20},
                    {field: 'Name', title: '姓名', align: 'center', width: 20, formatter:ShowRole},
                    { field: 'PosName', title: '职位', align: 'center', width: 20 },
                    { field: 'IsEnable', title: '是否启用', align: 'center', width: 10 },
                    { field: 'Comment', title: '备注', align: 'center', width: 30 }
                ]],
                onResize: function () {
                    $('#dg_org').datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#dg_org').datagrid('fixDetailRowHeight', index);
                    }, 0);
                }
            });
            $('#dg_org').datagrid('fixDetailRowHeight', index);
        }
    });
    $("#btn-ok").on("click", function (e) {
        e.preventDefault();
        $.messager.confirm({
            title: '提示',
            msg: '确定此操作吗?',
            fn: function (r) {
                if (r) {
                    if ($("#dialog_add").dialog('options').title.indexOf("查询") > -1) {
                        $("#temp").val($("#Name").textbox('getText'));
                        $("#dg_org").datagrid('load', { Name: $("#Name").textbox('getText') });
                        $("#dialog_add").dialog('close');
                    } else
                        FormSubmit("fm_add", "/Org/Save", $("#fm_add").serialize(), "dialog_add", "dg_org","/Org/GetAll");
                } else
                    return false;
            }
        });

    });
    $("#btn-cancel").on("click", function (e) {
        e.preventDefault();
        $("#dialog_add").dialog('close');
    });
    $("#btn_add").on("click", function (e) {
        e.preventDefault();
        $("#Name").textbox({ required: true });
        $("#ParentId").combobox({ url: '/Org/GetOrgCombo' });
        $("#comment-div").show();
        $("#div-parent").show();
        $("#div-enable").hide();
        $("#Id").val("");
        $("#dialog_add").dialog({
            title: '增加部门',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
    });
    $("#btn_edit").on("click", function (e) {
        e.preventDefault();
        var rows = $("#dg_org").datagrid('getSelections');
        if (rows.length == 0) {
            ShowInfo("ErrorInfo", "请选择一条数据,再编辑");
            return false;
        }
        if (rows.length > 1) {
            ShowInfo("ErrorInfo", "不能同时多条数据编辑");
            return false;
        }
        $("#div-enable").show();
        $("#comment-div").show();
        $("#div-parent").hide();
        $("#dialog_add").dialog({
            title: '编辑部门',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
        $("#Id").val(rows[0].Id);
        $("#Name").textbox({ required: true }).textbox('setValue', rows[0].Name);
        if (rows[0].IsEnable == true)
            $("#IsEnable").switchbutton('check');
        $("#Comment").textbox('setValue', rows[0].Comment);
        
    });
    $("#btn_query").on("click", function (e) {
        e.preventDefault();
        $("#div-enable").hide();
        $("#comment-div").hide();
        $("#div-parent").hide();
        $("#Name").textbox({ required: false });
        $("#dialog_add").dialog({
            title: '查询部门',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
        $("#Name").textbox('setText', $("#temp").val());
    });
    $("#btn_delete").on("click", function (e) {
        e.preventDefault();
        var rows = $("#dg_org").datagrid('getSelections');
        if (rows.length == 0) {
            ShowInfo("ErrorInfo", "至少选择一条数据");
            return false;
        }
        $.messager.confirm({
            title: '提示',
            msg: '确定删除吗?',
            fn: function (r) {
                if (r) {
                    var idArr = [];
                    for (i = 0; i < rows.length; i++) {
                        idArr.push(rows[i].Id);
                    }
                    $.ajax({
                        url: '/Org/Del',
                        type: 'POST',
                        dataType: 'json',
                        data: { id: idArr },
                        success: function (data) {
                            if (data.Result)
                                $("#dg_org").datagrid('load', { url: '/Org/ GetAll' });
                            else
                                ShowInfo("ErrorInfo", data.Msg);
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            ShowInfo("ErrorInfo", textStatus);
                        }
                    });
                } else
                    return false;
            }
        });

    });
}
//-----------------------------------------职位界面特定JS-------------------------------------------------------------------
function PosViewInit() {
    $('#dg_pos').datagrid({
        url: '/Pos/GetAll',
        view: detailview,
        detailFormatter: function (index, row) {
            return '<div style="padding:2px;position:relative;"><table class="ddv"></table></div>';
        },
        onExpandRow: function (index, row) {
            var ddv = $(this).datagrid('getRowDetail', index).find('table.ddv');
            ddv.datagrid({
                url: '/Pos/GetUser?id=' + row.Id,
                fitColumns: true,
                singleSelect: true,
                rownumbers: true,
                loadMsg: '',
                height: 'auto',
                columns: [[
                    { field: 'RoleName', align: 'center', hidden: true },
                    { field: 'Code', title: '账号', align: 'center', width: 20 },
                    { field: 'Name', title: '姓名', align: 'center', width: 20, formatter: ShowRole },
                    { field: 'OrgName', title: '部门', align: 'center', width: 20 },
                    { field: 'IsEnable', title: '是否启用', align: 'center', width: 10 },
                    { field: 'Comment', title: '备注', align: 'center', width: 30 }
                ]],
                onResize: function () {
                    $('#dg_pos').datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#dg_pos').datagrid('fixDetailRowHeight', index);
                    }, 0);
                }
            });
            $('#dg_pos').datagrid('fixDetailRowHeight', index);
        }
    });
    $("#btn-ok").on("click", function (e) {
        e.preventDefault();
        $.messager.confirm({
            title: '提示',
            msg: '确定此操作吗?',
            fn: function (r) {
                if (r) {
                    if ($("#dialog_add").dialog('options').title.indexOf("查询") > -1) {
                        $("#temp").val($("#Name").textbox('getText'));
                        $("#dg_pos").datagrid('load', { Name: $("#Name").textbox('getText') });
                        $("#dialog_add").dialog('close');
                    } else
                        FormSubmit("fm_add", "/Pos/Save", $("#fm_add").serialize(), "dialog_add", "dg_pos", "/Pos/GetAll");
                } else
                    return false;
            }
        });

    });
    $("#btn-cancel").on("click", function (e) {
        e.preventDefault();
        $("#dialog_add").dialog('close');
    });
    $("#btn_add").on("click", function (e) {
        e.preventDefault();
        $("#Name").textbox({ required: true });
        $("#ParentId").combobox({ url: '/Pos/GetPosCombo' });
        $("#comment-div").show();
        $("#div-parent").show();
        $("#div-enable").hide();
        $("#Id").val("");
        $("#dialog_add").dialog({
            title: '增加职位',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
    });
    $("#btn_edit").on("click", function (e) {
        e.preventDefault();
        var rows = $("#dg_pos").datagrid('getSelections');
        if (rows.length == 0) {
            ShowInfo("ErrorInfo", "请选择一条数据,再编辑");
            return false;
        }
        if (rows.length > 1) {
            ShowInfo("ErrorInfo", "不能同时多条数据编辑");
            return false;
        }
        $("#div-enable").show();
        $("#comment-div").show();
        $("#div-parent").hide();
        $("#dialog_add").dialog({
            title: '编辑职位',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
        $("#Id").val(rows[0].Id);
        $("#Name").textbox({ required: true }).textbox('setValue', rows[0].Name);
        if (rows[0].IsEnable == true)
            $("#IsEnable").switchbutton('check');
        $("#Comment").textbox('setValue', rows[0].Comment);

    });
    $("#btn_query").on("click", function (e) {
        e.preventDefault();
        $("#div-enable").hide();
        $("#comment-div").hide();
        $("#div-parent").hide();
        $("#Name").textbox({ required: false });
        $("#dialog_add").dialog({
            title: '查询职位',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
        $("#Name").textbox('setText', $("#temp").val());
    });
    $("#btn_delete").on("click", function (e) {
        e.preventDefault();
        var rows = $("#dg_pos").datagrid('getSelections');
        if (rows.length == 0) {
            ShowInfo("ErrorInfo", "至少选择一条数据");
            return false;
        }
        $.messager.confirm({
            title: '提示',
            msg: '确定删除吗?',
            fn: function (r) {
                if (r) {
                    var idArr = [];
                    for (i = 0; i < rows.length; i++) {
                        idArr.push(rows[i].Id);
                    }
                    $.ajax({
                        url: '/Pos/Del',
                        type: 'POST',
                        dataType: 'json',
                        data: { id: idArr },
                        success: function (data) {
                            if (data.Result)
                                $("#dg_pos").datagrid('load', { url: '/Pos/ GetAll' });
                            else
                                ShowInfo("ErrorInfo", data.Msg);
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            ShowInfo("ErrorInfo", textStatus);
                        }
                    });
                } else
                    return false;
            }
        });

    });
}
//------------------------------------------角色界面特定JS--------------------------------------------------------------------------
function RoleViewInit() {
    $('#dg_role').datagrid({
        url: '/Role/GetAll',
        view: detailview,
        detailFormatter: function (index, row) {
            return '<div style="padding:2px;position:relative;"><table class="ddv"></table></div>';
        },
        onClickCell: function (index, field, value) {
            if (field.indexOf("Operator") > -1) {
                $("#dg_role").datagrid('clearSelections').datagrid('clearChecked').datagrid('selectRow', index);
                $("#dg_role").datagrid('selectRow', index);
                var row = $("#dg_role").datagrid('getSelected');
                $("#div-enable").hide();
                $("#comment-div").hide();
                $("#name-div").hide();
                $("#right-div").show();
                $("#Name").textbox({ required: false });
                
                $("#Rights").tree({
                    url: '/Role/GetRightCombotree', checkbox: true, lines: true, onBeforeLoad: function (node, param) {
                        param.roleId = row.Id
                    }
                });
                $("#dialog_add").dialog({
                    title: '权限编辑',
                    onBeforeClose: FormClear('fm_add')
                }).dialog('open');
                $("#Id").val(row.Id);
            }
           
        },
        onExpandRow: function (index, row) {
            var ddv = $(this).datagrid('getRowDetail', index).find('table.ddv');
            ddv.datagrid({
                url: '/Role/GetUser?id=' + row.Id,
                fitColumns: true,
                singleSelect: true,
                rownumbers: true,
                loadMsg: '',
                height: 'auto',
                columns: [[
                    { field: 'Code', title: '账号', align: 'center', width: 20 },
                    { field: 'Name', title: '姓名', align: 'center', width: 20 },
                    { field: 'OrgName', title: '部门', align: 'center', width: 10 },
                    { field: 'PosName', title: '职位', align: 'center', width: 10 },
                    { field: 'IsEnable', title: '是否启用', align: 'center', width: 10 },
                    { field: 'Comment', title: '备注', align: 'center', width: 30 }
                ]],
                onResize: function () {
                    $('#dg_role').datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#dg_role').datagrid('fixDetailRowHeight', index);
                    }, 0);
                }
            });
            $('#dg_role').datagrid('fixDetailRowHeight', index);
        }
    });
    $("#btn-ok").on("click", function (e) {
        e.preventDefault();
        $.messager.confirm({
            title: '提示',
            msg: '确定此操作吗?',
            fn: function (r) {
                if (r) {
                    if ($("#dialog_add").dialog('options').title.indexOf("查询") > -1) {
                        $("#temp").val($("#Name").textbox('getText'));
                        $("#dg_role").datagrid('load', { Name: $("#Name").textbox('getText') });
                        $("#dialog_add").dialog('close');
                    } else {
                        var data = $("#fm_add").serializeObject();
                        data["RightId"] = [];
                        try {
                            var checked = $("#Rights").tree('getChecked');
                            for (i = 0; i < checked.length; i++)
                                data["RightId"].push(checked[i].id);
                        } catch(e){}
                        FormSubmit("fm_add", "/Role/Save", data, "dialog_add", "dg_role", "/Role/GetAll");
                    }
                       
                } else
                    return false;
            }
        });

    });
    $("#btn-cancel").on("click", function (e) {
        e.preventDefault();
        $("#dialog_add").dialog('close');
    });
    $("#btn_add").on("click", function (e) {
        e.preventDefault();
        $("#name-div").show();
        $("#Name").textbox({ required: true });
        $("#comment-div").show();
        $("#right-div").show();
        $("#div-enable").hide();
        $("#Id").val("");
        $("#Rights").tree({ url: '/Role/GetRightCombotree', checkbox: true, lines: true});
        $("#dialog_add").dialog({
            title: '增加角色',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
    });
    $("#btn_edit").on("click", function (e) {
        e.preventDefault();
        var rows = $("#dg_role").datagrid('getSelections');
        if (rows.length == 0) {
            ShowInfo("ErrorInfo", "请选择一条数据,再编辑");
            return false;
        }
        if (rows.length > 1) {
            ShowInfo("ErrorInfo", "不能同时多条数据编辑");
            return false;
        }
        try {
            var nodes = $("#Rights").tree('getChecked');
            for (i = 0; i < nodes.length; i++)
                $("#Rights").tree('uncheck', nodes[i].target);
        } catch(e){}

        $("#name-div").show();
        $("#div-enable").show();
        $("#comment-div").show();
        $("#right-div").hide();
        $("#dialog_add").dialog({
            title: '编辑角色',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
        $("#Id").val(rows[0].Id);
        $("#Name").textbox({ required: true }).textbox('setValue', rows[0].Name);
        if (rows[0].IsEnable == true)
            $("#IsEnable").switchbutton('check');
        $("#Comment").textbox('setValue', rows[0].Comment);

    });
    $("#btn_query").on("click", function (e) {
        e.preventDefault();
        $("#name-div").show();
        $("#div-enable").hide();
        $("#comment-div").hide();
        $("#right-div").hide();
        $("#Name").textbox({ required: false });
        $("#dialog_add").dialog({
            title: '查询角色',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
        $("#Name").textbox('setText', $("#temp").val());
    });
    $("#btn_delete").on("click", function (e) {
        e.preventDefault();
        var rows = $("#dg_role").datagrid('getSelections');
        if (rows.length == 0) {
            ShowInfo("ErrorInfo", "至少选择一条数据");
            return false;
        }
        $.messager.confirm({
            title: '提示',
            msg: '确定删除吗?',
            fn: function (r) {
                if (r) {
                    var idArr = [];
                    for (i = 0; i < rows.length; i++) {
                        idArr.push(rows[i].Id);
                    }
                    $.ajax({
                        url: '/Role/Del',
                        type: 'POST',
                        dataType: 'json',
                        data: { id: idArr },
                        success: function (data) {
                            if (data.Result)
                                $("#dg_role").datagrid('load', { url: '/Role/ GetAll' });
                            else
                                ShowInfo("ErrorInfo", data.Msg);
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            ShowInfo("ErrorInfo", textStatus);
                        }
                    });
                } else
                    return false;
            }
        });

    });
}
//---------------------------------------------权限菜单界面特定JS-----------------------------------------------------------------------------------
function RightViewInit() {
    $('#dg_right').treegrid({
        url: '/Right/GetTop',
        pageSize: 100,
        pageList: [100, 150],
        onBeforeLoad: function (row,param) {
            param.total = $("#total").text();
        },
        loadFilter: function (data, parentId) {
            $("#total").text(data.total); return data;
        },
        onLoadSuccess: function (data) {
            delete $(this).treegrid('options').queryParams['id'];
        }
    });
    $("#btn-ok").on("click", function (e) {
        e.preventDefault();
        $.messager.confirm({
            title: '提示',
            msg: '确定此操作吗?',
            fn: function (r) {
                if (r) {
                    if ($("#dialog_add").dialog('options').title.indexOf("查询") > -1) {
                        $("#temp").val($("#Name").textbox('getText'));
                        $("#dg_right").treegrid('load', { Name: $("#Name").textbox('getText') });
                        $("#dialog_add").dialog('close');
                    } else
                        FormSubmit("fm_add", "/Right/Save", $("#fm_add").serialize(), "dialog_add", "dg_right", "/Right/GetTop");
                } else
                    return false;
            }
        });

    });
    $("#btn-cancel").on("click", function (e) {
        e.preventDefault();
        $("#dialog_add").dialog('close');
    });
    $("#btn_add").on("click", function (e) {
        e.preventDefault();
        $("#Name").textbox({ required: true });
        $("#parent-div").show();
        $("#path-div").show();
        $("#type-div").show();
        $("#sort-div").show();
        $("#comment-div").show();
        $("#div-enable").hide();
        $("#Id").val("");
        $("#nodeId").text("");
        $("#ParentId").combotree({
            url: '/Role/GetRightCombotree', lines: true, onSelect: function (node) {
                var nodeid = $("#nodeId").text();
                if (nodeid == node.id) {
                    $("#ParentId").combotree("clear");
                    $("#nodeId").text("");
                }   
                else 
                    $("#nodeId").text(node.id);
            }
        });
        $("#dialog_add").dialog({
            title: '增加权限',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
    });
    $("#btn_edit").on("click", function (e) {
        e.preventDefault();
        var rows = $("#dg_role").datagrid('getSelections');
        if (rows.length == 0) {
            ShowInfo("ErrorInfo", "请选择一条数据,再编辑");
            return false;
        }
        if (rows.length > 1) {
            ShowInfo("ErrorInfo", "不能同时多条数据编辑");
            return false;
        }
        $("#parent-div").hide();
        $("#path-div").show();
        $("#type-div").show();
        $("#sort-div").show();
        $("#comment-div").show();
        $("#div-enable").show();
        $("#comment-div").show();
        $("#dialog_add").dialog({
            title: '编辑权限',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
        $("#Id").val(rows[0].Id);
        $("#Name").textbox({ required: true }).textbox('setValue', rows[0].Name);
        if (rows[0].IsEnable == true)
            $("#IsEnable").switchbutton('check');
        $("#UrlPath").textbox('setValue', rows[0].UrlPath);
        if (rows[0].Type == 0)
            $("#Type").attr("checked", true);
        else
            $("#Type1").attr("checked", true);
        $("#Comment").textbox('setValue', rows[0].Comment);

    });
    $("#btn_query").on("click", function (e) {
        e.preventDefault();
        $("#div-enable").hide();
        $("#parent-div").hide();
        $("#path-div").hide();
        $("#type-div").hide();
        $("#sort-div").hide();
        $("#comment-div").hide();
        $("#Name").textbox({ required: false });
        $("#dialog_add").dialog({
            title: '查询用户',
            onBeforeClose: FormClear('fm_add')
        }).dialog('open');
        $("#Name").textbox('setText', $("#temp").val());
    });
    $("#btn_delete").on("click", function (e) {
        e.preventDefault();
        var rows = $("#dg_right").datagrid('getSelections');
        if (rows.length == 0) {
            ShowInfo("ErrorInfo", "至少选择一条数据");
            return false;
        }
        $.messager.confirm({
            title: '提示',
            msg: '确定删除吗?',
            fn: function (r) {
                if (r) {
                    var idArr = [];
                    for (i = 0; i < rows.length; i++) {
                        idArr.push(rows[i].Id);
                    }
                    $.ajax({
                        url: '/Right/Del',
                        type: 'POST',
                        dataType: 'json',
                        data: { id: idArr },
                        success: function (data) {
                            if (data.Result)
                                $("#dg_right").datagrid('load', { url: '/Right/GetTop' });
                            else
                                ShowInfo("ErrorInfo", data.Msg);
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            ShowInfo("ErrorInfo", textStatus);
                        }
                    });
                } else
                    return false;
            }
        });

    });
}
