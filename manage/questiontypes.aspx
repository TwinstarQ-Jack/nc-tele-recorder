<%@ Page Title="" Language="C#" MasterPageFile="~/master/ManageStyle.master" AutoEventWireup="true" CodeFile="questiontypes.aspx.cs" Inherits="manage_questiontypes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1_head" Runat="Server">
    <title>问题分类 - NC电话订单管理系统</title>
    <style type="text/css">
        ol > li, ol > li > ul > li {
            padding: 5px 5px 5px 20px;
        }

        .divInfo {
            margin: 10px 0 5px 30px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="nctelrecord_body" runat="Server">
    <div class="panel panel-default">
        <div class="panel-heading">功能区域</div>
        <div class="panel-body">
            <div class="form-group">
                <div class="col-md-offset-1 col-md-2">
                    <asp:Button ID="btnShowTypes" runat="server" Text="显示当前分类" CssClass="btn btn-danger btn-block" OnClick="btnShowTypes_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnAddType1" runat="server" Text="增加父分类(T1)" CssClass="btn btn-primary btn-block" OnClick="btnAddType1_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnAddType2" runat="server" Text="增加子分类(T2)" CssClass="btn btn-success btn-block" OnClick="btnAddType2_Click" />
                </div>
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
    <%-- 提示框末端 --%>
    <%-- 显示当前分类 --%>
    <div runat="server" id="divShowTypes">
        <div class="panel panel-danger">
            <div class="panel-heading">显示当前分类</div>
            <div class="panel-body table-responsive">
                <asp:Table ID="tableShowTypes" runat="server" CssClass="table table-bordered"></asp:Table>
            </div>
            <div class="panel-footer">
                <ul style="display: table; margin: 0 auto;" class="pagination pagination" runat="server" id="PaginationPage"></ul>
            </div>
        </div>
    </div>
    <%-- 显示当前分类 末端 --%>
    <%-- 增加父分类 --%>
    <div runat="server" id="divAddType1" visible="false">
        <div class="panel panel-primary">
            <div class="panel-heading">增加父分类（T1）</div>
            <div class="panel-body">
                <div class="divInfo">
                    <strong style="margin-bottom: 10px; display: block;">功能说明：</strong>
                    <ol>
                        <li>本功能增加问题分类的<code>大项（父项）</code>，名称必须填写。</li>
                    </ol>
                </div>
                <div class="form-group">
                    <label for="lableforType1Name" class="col-md-offset-2 col-md-2 control-label">分类1名称</label>
                    <div class="col-md-6">
                        <input runat="server" type="text" class="form-control" id="inputType1Name" placeholder="Question Type Lv1 Name" />
                    </div>
                </div>
            </div>
            <div class="panel-footer">
                <div class="form-group">
                    <div class="col-md-offset-5 col-md-2">
                        <asp:Button ID="btnSubmitType1" runat="server" Text="添加分类" CssClass="btn btn-primary btn-block" OnClick="btnSubmitType1_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%-- 增加父分类 末端 --%>
    <%-- 增加子分类 --%>
    <div runat="server" id="divAddType2" visible="false">
        <div class="panel panel-primary">
            <div class="panel-heading">增加子分类（T2）</div>
            <div class="panel-body">
                <div class="divInfo">
                    <strong style="margin-bottom: 10px; display: block;">功能说明：</strong>
                    <ol>
                        <li>本功能增加问题分类的<code>具体项（子项）</code>，必须在<code>选择父项</code>的基础上，再填写子项名称。</li>
                    </ol>
                </div>
                <div class="form-group">
                    <label for="selectType1" class="col-md-2 control-label">选择父分类</label>
                    <div class="col-md-6">
                        <asp:DropDownList ID="DropDownListType1Num" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                </div>
                <div class="form-group">
                    <label for="lableforType2Name" class="col-md-offset-2 col-md-2 control-label">分类2名称</label>
                    <div class="col-md-6">
                        <input runat="server" type="text" class="form-control" id="inputType2Name" placeholder="Question Type Lv2 Name" />
                    </div>
                </div>
            </div>
            <div class="panel-footer">
                <div class="form-group">
                    <div class="col-md-offset-5 col-md-2">
                        <asp:Button ID="btnSubmitType2" runat="server" Text="添加分类" CssClass="btn btn-primary btn-block" OnClick="btnSubmitType2_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

