<%@ Page Title="" Language="C#" MasterPageFile="~/master/ManageStyle.master" AutoEventWireup="true" CodeFile="accounts.aspx.cs" Inherits="manage_accounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1_head" Runat="Server">
    <title>人员管理 - NC电话订单管理系统后台</title>
    <style type="text/css">
        ol > li, ol> li > ul > li{
            padding:5px 5px 5px 20px;
        }
        #divAddUserInfo{
            margin:10px 0 5px 30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="nctelrecord_body" Runat="Server">
    <div runat="server" id="navigation" visible="true">
        <div class="panel panel-default">
            <div class="panel-heading">功能区域</div>
            <div class="panel-body">
                <div class="form-group">
                    <div class="col-md-offset-1 col-md-2">
                        <asp:Button ID="btnRevise" runat="server" Text="用户资料修改" CssClass="btn btn-danger btn-block" OnClick="btnRevise_Click" />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnAddUser" runat="server" Text="增加单个用户" CssClass="btn btn-primary btn-block" OnClick="btnAddUser_Click" />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnBatchAdd" runat="server" Text="批量增加多用户" CssClass="btn btn-success btn-block" OnClick="btnBatchAdd_Click" />
                    </div>
                </div>
            </div>
        </div>
<%--        <div class="panel panel-success" runat="server" visible="false">
            <div class="panel-heading">查找用户</div>
            <div class="panel-body">
                <div class="form-group">
                    <div class="col-md-offset-2 col-md-6">
                        <input runat="server" id="inputSearchByCardID" type="text" class="form-control" placeholder="输入一卡通卡号" />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnSearchByCardID" runat="server" Text="搜索" CssClass="btn btn-primary btn-block" OnClick="btnSearchByCardID_Click" />
                    </div>
                </div>
            </div>
        </div>--%>
        <%-- 提示框 --%>
        <div runat="server" id="divAlterSuccess" visible="false" class="alert alert-success alert-dismissable" role="alert">
            <button class="close" type="button" data-dismiss="alert">&times;</button>
            <asp:Label ID="labelAlterSuccess" runat="server" Text=""></asp:Label>
        </div>
        <div runat="server" id="divAlterDanger" visible="false" class="alert alert-danger alert-dismissable" role="alert">
            <button class="close" type="button" data-dismiss="alert">&times;</button>
            <asp:Label ID="labelAlterDanger" runat="server" Text=""></asp:Label>
        </div>
        <%-- 用户列表 --%>
        <div runat="server" id="Revise" visible="true">
            <div class="panel panel-info">
                <div class="panel-heading">用户列表</div>
                <div class="panel-body">
                    <p>系统共搜索到符合条件的用户<strong runat="server" id="ReviseShowMemberNum"></strong>名：</p>
                </div>
                <div class="table-responsive">
                    <asp:Table ID="tableShowUser" runat="server" CssClass="table table-bordered"></asp:Table>
                </div>
                <div class="panel-footer">
                    <ul style="display: table; margin: 0 auto;" class="pagination pagination" runat="server" id="PaginationPage"></ul>
                </div>
            </div>
        </div>
        <%-- 增加用户 --%>
        <div runat="server" id="AddUser" visible="false">
            <div class="panel panel-info">
                <div class="panel-heading" runat="server" id="divAddUserHeader">增加用户</div>
                <div id="divAddUserInfo">
                    <strong style="margin-bottom: 10px; display: block;">表格提交要求：</strong>
                    <ol>
                        <li>除了<code>其他手机</code>项目外，其他项目必须填写。</li>
                        <li>在<code>修改用户信息模式</code>下，若不需要修改密码，则在密码处留空。</li>
                    </ol>
                </div>
                <div class="panel-body">
                    <div class="form-group" runat="server" id="divAddUserGroup">
                        <label for="inputCardID" class="col-md-2 control-label">一卡通卡号</label>
                        <div class="col-md-4">
                            <input runat="server" type="text" class="form-control" id="inputCardID" placeholder="Card Number" />
                        </div>
                        <label for="inputRealName" class="col-md-2 control-label">真实姓名</label>
                        <div class="col-md-4">
                            <input runat="server" type="text" class="form-control" id="inputRealName" placeholder="Real Name" />
                        </div>
                    </div>
                    <div class="form-group" runat="server" id="divAddPasswordGroup">
                        <label for="inputPasswordSet" class="col-md-2 control-label">密码</label>
                        <div class="col-md-4">
                            <input runat="server" type="password" class="form-control" id="inputPasswordSet" placeholder="Password" />
                        </div>
                        <label for="inputPasswordRepeat" class="col-md-2 control-label">确认密码</label>
                        <div class="col-md-4">
                            <input runat="server" type="password" class="form-control" id="inputPasswordRepeat" placeholder="Repeat Password" />
                        </div>
                    </div>
                    <div class="form-group" runat="server" id="divAddTelephoneGroup">
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
                            <select runat="server" id="selectUserLevel" class="form-control">
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
                        <div class="col-md-offset-3 col-md-3" runat="server" id="divbtnRegister">
                            <asp:Button ID="btnRegister" runat="server" Text="注册" CssClass="btn btn-primary btn-block" OnClick="btnRegister_Click" />
                        </div>  
                        <div class="col-md-offset-3 col-md-3" runat="server" id="divbtnAlter" visible="false">
                            <asp:Button ID="btnAlter" runat="server" Text="更新信息" CssClass="btn btn-success btn-block" OnClick="btnAlter_Click"  />
                        </div>                  
                        <div class="col-md-3">
                            <asp:Button ID="btnReset" runat="server" Text="重置" CssClass="btn btn-default btn-block" OnClick="btnReset_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <%-- 删除用户 --%>
        <div id="DelUser" runat="server" visible="false">
            <div class="panel panel-danger">
                <div class="panel-heading">
                    删除用户
                </div>
                <div class="panel-body">
                    <p>您是否确认删除用户<strong><asp:Label ID="DelUserRealName" runat="server" Text=""></asp:Label></strong>（一卡通号：<strong><asp:Label ID="DelUserCardID" runat="server" Text=""></asp:Label>）</strong>？</p>
                    <p>本操作一旦生效，将无法撤回！</p>
                </div>
                <div class="panel-footer">
                    <div class="form-group">
                        <div class="col-md-offset-8 col-md-2">
                            <asp:Button ID="btnDelUserConfirm" runat="server" Text="确认删除" CssClass="btn btn-block btn-danger" OnClick="btnDelUserConfirm_Click" />
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnDelUserReturn" runat="server" Text="返回列表" CssClass="btn btn-block btn-success" OnClick="btnDelUserReturn_Click"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <%-- 批量增加用户 --%>
        <div id="ImportUserSheet" runat="server" visible="false">
            <div class="panel panel-primary">
                <div class="panel-heading">批量增加用户</div>
                <div class="panel-body">
                    <p class="col-md-3" style="display: block;">
                        <a href="ImportTemplet.xls" class="btn btn-block btn-danger">下载批量表格</a>
                    </p>
                    <div style="clear: both;">
                        <br />
                        <strong style="margin-bottom: 10px; display: block;">Excel表格提交要求：</strong>
                        <ol>                            
                            <li>保存的时候保存为Excel 03版本，后缀名为<code>.xls</code>。</li>
                            <li>如网页报错，请将原Excel及错误截图发送到 <code>twinstarq@tqinit.com</code> ，由管理员定位问题。</li>
                            <li>表格大小<code>小于2M</code>，超过2M请自行划分为多个表格分别上传。</li>
                        </ol>
                        <strong style="margin-bottom: 10px; display: block;">Excel表格填写要求：</strong>
                        <ol>
                            <li>粘贴时选择<code>只保留文本</code>，或者直接输入对应部分，不要更改文档格式。</li>
                            <li><code>姓名</code>、<code>密码</code>、<code>一卡通卡号</code>、<code>手机号码1</code>、<code>用户等级</code>为必填字段，<code>手机号码2</code>若有则填，无则不填。</li>
                            <li>用户等级填写说明：（填英文）
                                <ul style="margin-top: 10px; display: block;">
                                    <li>学生助理：<code>Stud</code></li>
                                    <li>学生负责人：<code>StudAdmin</code></li>
                                    <li>老师：<code>TeacherAdmin</code></li>
                                </ul>
                            </li>
                        </ol>
                    </div>
                </div>
                <div class="panel-footer">
                    <asp:FileUpload ID="XlsFileUpload" runat="server" />
                    <asp:Button ID="XlsFileUploadSubmit" runat="server" Text="上传" CssClass="btn btn-block btn-success" OnClick="XlsFileUploadSubmit_Click"/>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

