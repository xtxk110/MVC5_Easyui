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

//使用easyui 清除表单数据
function FormClear(formId) {
    $('#' + formId).form('clear');
}
//使用easyui from提交方式,自动序列化from表单为对应的JSON对象
//参数: form id 选择器(不包含#),url 提交的服务地址,dialogId easyui-dialog id,datagrid_id
function FormSubmit(id, url,data,dialogId,datagrid_id) {
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
                            $("#" + datagrid_id).datagrid('load', {url:'/User/Get'});
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
                        //$("#dg").datagrid({ url: "/User/Get?" + $("#fm_add").serialize() });
                        $("#dg").datagrid('load', { Name: 'ad' });
                        $("#dialog_add").dialog('close');
                    } else
                        FormSubmit("fm_add", "/User/Save", $("#fm_add").serialize(), "dialog_add", "dg");
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
        $("#OrgId").combotree({ required: true }).combotree({ url: '/Org/Get' });
        $("#PosId").combotree({ required: true }).combotree({ url: '/Pos/Get' });
        $("#RoleId").combobox({ required: true }).combobox({ url: '/Role/Get' });
        $("#div-pwd").show();
        $("#comment-div").show();
        $("#div-enable").hide();
        $("#Id").val("");
        $("#dialog_add").dialog({
            title: '增加用户'
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
        $("#dialog_add").dialog({
            title: '编辑用户'
        }).dialog('open');
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
            title: '查询用户'
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

//------------------------------------------------------------------------------------------------