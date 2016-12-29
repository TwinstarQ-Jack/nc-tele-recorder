<%@ Page Title="" Language="C#" MasterPageFile="~/master/ManageStyle.master" AutoEventWireup="true" CodeFile="blocks.aspx.cs" Inherits="manage_blocks" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1_head" Runat="Server">
    <title>地区管理 - NC电话订单管理系统后台</title>
    <style type="text/css">
        ol > li, ol> li > ul > li{
            padding:5px 5px 5px 20px;
        }
        #divAddUserInfo{
            margin:10px 0 5px 30px;
        }
    </style></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="nctelrecord_body" Runat="Server">
    <div runat="server" id="navigation" visible="true">
        <%-- 功能区域前端 --%>
        <div class="panel panel-default">
            <div class="panel-heading">功能区域</div>
            <div class="panel-body">
                <div class="form-group">
                    <div class="col-md-offset-1 col-md-2">
                        <asp:Button ID="btnAddBlock" runat="server" Text="添加单个地区" CssClass="btn btn-danger btn-block" OnClick="btnAddBlock_Click" />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnBatchAddBlock" runat="server" Text="批量导入" CssClass="btn btn-primary btn-block" OnClick="btnBatchAddBlock_Click" />
                    </div>
                </div>
            </div>
        </div>
        <%-- 功能区域末端 --%>
        <%-- 提示框 --%>
        <div runat="server" id="divAlterSuccess" visible="false" class="alert alert-success alert-dismissable" role="alert">
            <button class="close" type="button" data-dismiss="alert">&times;</button>
            <asp:Label ID="labelAlterSuccess" runat="server" Text=""></asp:Label>
        </div>
        <div runat="server" id="divAlterDanger" visible="false" class="alert alert-danger alert-dismissable" role="alert">
            <button class="close" type="button" data-dismiss="alert">&times;</button>
            <asp:Label ID="labelAlterDanger" runat="server" Text=""></asp:Label>
        </div>
        <%-- 提示框末端 --%>
        <%-- 增加单区域 --%>
        <div class="panel panel-danger" runat="server" id="AddBlock">
            <div class="panel-heading">增加区域</div>
            <div class="panel-body">
                <div class="form-group">
                    <label for="inputBlockID" class="col-md-2 control-label">地区编号</label>
                    <div class="col-md-4">
                        <input runat="server" type="text" class="form-control" id="inputBID" placeholder="Block ID" />
                    </div>
                    <label for="inputBlockName" class="col-md-2 control-label">地区名称</label>
                    <div class="col-md-4">
                        <input runat="server" type="text" class="form-control" id="inputBlockName" placeholder="Block Name" />
                    </div>
                </div>
            </div>
            <div class="panel-footer">
                <div class="form-group">
                    <div class="col-md-offset-5 col-md-2">
                        <asp:Button ID="btnSubmit" runat="server" Text="添加区域" CssClass="btn btn-primary btn-block" OnClick="btnSubmit_Click" />
                    </div>
                </div>
            </div>
        </div>
        <%-- 增加单区域末端 --%>
        <%-- 查看列表 --%>
        <div class="panel panel-info" runat="server" id="ReadList">
            <div class="panel-heading">查看地区列表</div>
            <div class="panel-body table-responsive">
                <asp:Table ID="tableShowBlocks" runat="server" CssClass="table table-bordered"></asp:Table>
            </div>
            <div class="panel-footer">
                <ul style="display: table; margin: 0 auto;" class="pagination pagination" runat="server" id="PaginationPage"></ul>
            </div>
        </div>
        <%-- 查看列表末端 --%>
        <%-- 批量导入 --%>
        <div class="panel panel-primary" runat="server" id="BatchAddBlock" visible="false">
            <div class="panel-heading">批量导入地区名单</div>
            <div class="panel-body">
                <p class="col-md-3" style="display: block;">
                    <a href="ImportBlocks.xls" class="btn btn-block btn-danger">下载批量表格</a>
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
                        <li><code>地区编号</code>、<code>地区名称</code>均为必填项，且<code>地区编号</code>小于11位。</li>
                    </ol>
                </div>
            </div>
            <div class="panel-footer">
                <asp:FileUpload ID="XlsFileUpload" runat="server" />
                <asp:Button ID="XlsFileUploadSubmit" runat="server" Text="上传" CssClass="btn btn-block btn-success" OnClick="XlsFileUploadSubmit_Click" />
            </div>
        </div>
        <%-- 批量导入末端 --%>
    </div>
</asp:Content>

