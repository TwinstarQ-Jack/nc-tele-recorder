<%@ Page Title="" Language="C#" MasterPageFile="~/master/Member.master" AutoEventWireup="true" CodeFile="data.aspx.cs" Inherits="member_data" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1_head" runat="Server">
    <title>资料维护 - NC电话订单管理系统后台</title>
    <style type="text/css">
        ol > li, ol > li > ul > li {
            padding: 5px 5px 5px 20px;
        }

        #divAddUserInfo {
            margin: 10px 0 5px 30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="nctelrecord_body" Runat="Server">
    <div>
        <div class="panel panel-danger">
            <div class="panel-heading">注意事项</div>
            <div class="panel-body">
                <strong style="margin-bottom: 10px; display: block;">表格提交要求：</strong>
                    <ol>
                        <li>除了<code>其他手机</code>项目外，其他项目必须填写。</li>
                        <li>在<code>修改用户信息模式</code>下，若不需要修改密码，则在密码处留空。</li>
                    </ol>
            </div>
        </div>
    </div>
    <%-- 提示框 --%>
    <div runat="server" id="divAlterSuccess" visible="false" class="alert alert-success alert-dismissable" role="alert">
        <button class="close" type="button" data-dismiss="alert">&times;</button>
        <asp:Label ID="labelAlterSuccess" runat="server" Text=""></asp:Label>
    </div>
    <div runat="server" id="divAlterDanger" visible="false" class="alert alert-danger alert-dismissable" role="alert">
        <button class="close" type="button" data-dismiss="alert">&times;</button>
        <asp:Label ID="labelAlterDanger" runat="server" Text=""></asp:Label>
    </div>
    <div runat="server" id="ChangeUserData">
        <div class="panel panel-primary">
            <div class="panel-heading" runat="server" id="divChangeUserDataHeader">修改信息</div>
            <div class="panel-body">
                <div class="form-group" runat="server" id="divChangeUserDataGroup">
                    <label for="inputUserName" class="col-md-2 control-label">一卡通卡号</label>
                    <div class="col-md-4">
                        <input runat="server" type="text" class="form-control" id="inputCardID" placeholder="Card Number" />
                    </div>
                    <label for="inputRealName" class="col-md-2 control-label">真实姓名</label>
                    <div class="col-md-4">
                        <input runat="server" type="text" class="form-control" id="inputRealName" placeholder="Real Name" />
                    </div>
                </div>
                <div class="form-group" runat="server" id="divChangeUserDataPasswordGroup">
                    <label for="inputPasswordSet" class="col-md-2 control-label">密码</label>
                    <div class="col-md-4">
                        <input runat="server" type="password" class="form-control" id="inputPasswordSet" placeholder="Password" />
                    </div>
                    <label for="inputPasswordRepeat" class="col-md-2 control-label">确认密码</label>
                    <div class="col-md-4">
                        <input runat="server" type="password" class="form-control" id="inputPasswordRepeat" placeholder="Repeat Password" />
                    </div>
                </div>
                <div class="form-group" runat="server" id="divChangeUserDataTelephoneGroup">
                    <label for="inputTelephone" class="col-md-2 control-label">手机号码</label>
                    <div class="col-md-4">
                        <input runat="server" type="text" class="form-control" id="inputTelephone" placeholder="Telephone" />
                    </div>
                    <label for="inputTelephone" class="col-md-2 control-label">其他手机</label>
                    <div class="col-md-4">
                        <input runat="server" type="text" class="form-control" id="inputTelephone2" placeholder="Telephone" />
                    </div>
                </div>
                <div class="form-group" runat="server" id="divAddSelectUserLevel">
                    <label for="selectUserLevel" class="col-md-2 control-label">管理级别</label>
                    <div class="col-md-4">
                        <select runat="server" id="selectUserLevel" class="form-control" disabled="disabled">
                            <option value="Stud">学生助理</option>
                            <option value="StudAdmin">学生助理负责人</option>
                            <option value="TeacherAdmin">老师</option>
                        </select>
                    </div>
                </div>
                <div class="form-group" runat="server" id="divAddSystemAutoMake">
                    <label for="inputRegisterTime" class="col-md-2 control-label">注册时间</label>
                    <div class="col-md-4">
                        <input disabled="disabled" runat="server" type="text" class="form-control" id="inputRegisterTime" placeholder="Register Time" />
                    </div>
                    <label for="inputDealingUser" class="col-md-2 control-label">经办人</label>
                    <div class="col-md-4">
                        <input disabled="disabled" runat="server" type="text" class="form-control" id="inputDealingUser" placeholder="" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-offset-3 col-md-3" runat="server" id="divbtnAlter">
                        <asp:Button ID="btnAlter" runat="server" Text="更新信息" CssClass="btn btn-success btn-block" OnClick="btnAlter_Click" />
                    </div>
                    <div class="col-md-3">
                        <asp:Button ID="btnReset" runat="server" Text="重置" CssClass="btn btn-default btn-block" OnClick="btnReset_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

