<%@ Page Title="" Language="C#" MasterPageFile="~/master/ManageStyle.master" AutoEventWireup="true" CodeFile="count.aspx.cs" Inherits="manage_count" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1_head" runat="Server">
    <title>地区管理 - NC电话订单管理系统后台</title>
    <style type="text/css">
        table tr td {
            text-align: center;
            padding-left: 10px;
            padding-top: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="nctelrecord_body" runat="Server">
    <div class="panel panel-default">
        <div class="panel-heading">查看范围</div>
        <div class="panel-body">
            <div class="form-group">
                <div class="col-md-offset-1 col-md-2">
                    <asp:Button ID="UserTable" runat="server" Text="按用户" CssClass="btn btn-block btn-danger" OnClick="UserTable_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="QType1Table" runat="server" Text="按问题父分类" CssClass="btn btn-block btn-info" OnClick="QType1Table_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="QType2Table" runat="server" Text="按问题子分类" CssClass="btn btn-block btn-success" OnClick="QType2Table_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="BlockTable" runat="server" Text="按地区" CssClass="btn btn-block btn-primary" OnClick="BlockTable_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnTotalTable" runat="server" Enabled="false" Text="总体情况" CssClass="btn btn-block btn-warning" />
                </div>
            </div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel_Top" runat="server" RenderMode="Block" UpdateMode="Conditional" class="table-responsive">
                <ContentTemplate>
                    <asp:HiddenField ID="HiddenField_ChooseType" runat="server" />
                    <div class="table-responsive">
                        <table style="margin: 0 auto;">
                            <tr>
                                <td>已选择查询表：</td>
                                <td>
                                    <label id="label_QueryType" runat="server">&nbsp;</label>
                                </td>
                                <td>&nbsp;</td>
                                <td>选择查询月份</td>
                                <td><asp:DropDownList ID="DDL_ChooseYear" runat="server" CssClass="col-md-3 form-control"></asp:DropDownList></td>
                                <td><asp:DropDownList ID="DDL_ChooseMonth" runat="server" CssClass="col-md-3 form-control"></asp:DropDownList></td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>选择问题父分类</td>
                                <td>
                                    <asp:DropDownList ID="DDL_QType1" runat="server" Enabled="false" CssClass="form-control"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="UserTable" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="QType1Table" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="QType2Table" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="BlockTable" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <asp:UpdatePanel ID="UpdatePanel_Footer" class="panel-footer" runat="server" RenderMode="Block" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="form-group">
                    <div class="col-md-offset-4 col-md-2">
                        <asp:Button ID="btnQuery" runat="server" Enabled="false" Text="按条件查询" CssClass="btn btn-block btn-primary" OnClick="btnQuery_Click" />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnOutputToExcel" runat="server" Enabled="false" Text="输出xls文档" CssClass="btn btn-block btn-danger" />
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="UserTable" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="QType1Table" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="QType2Table" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="BlockTable" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnTotalTable" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

        <%-- 提示框 --%>
    <asp:UpdatePanel ID="UpdatePanel_Danger" runat="server" RenderMode="Block" UpdateMode="Conditional">
        <%-- 需要更新的区域 --%>
        <ContentTemplate>
            <div runat="server" id="divAlterDanger" visible="false" class="alert alert-danger alert-dismissable" role="alert">
                <button class="close" type="button" data-dismiss="alert">&times;</button>
                <asp:Label ID="labelAlterDanger" runat="server" Text=""></asp:Label>
            </div>
        </ContentTemplate>
        <%-- Triggers:绑定异步刷新按钮ID --%>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnQuery" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnTotalTable" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>


    <%-- 表格显示区 --%>
    <asp:UpdatePanel RenderMode="Block" UpdateMode="Conditional" ID="UpdatePanel_ShowTable" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" id="divShowTable" runat="server" visible="false">
                <div class="panel-heading" id="label_ResultPanel_Heading" runat="server">
                    搜索结果
                </div>
                <div class="panel-body">
                    <div class="table-responsive">
                        <label id="labelShowTable_Body" runat="server" visible="false">暂时没有查询到数据。</label>
                        <asp:Table ID="ShowResultTable" runat="server" CssClass="table table-bordered"></asp:Table>
                    </div>
                </div>
                <div class="panel-footer">
                    本列表最新更新时间：
                     <label id="label_ResultPanel_Footer" runat="server"></label>
                </div>

            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnQuery" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnTotalTable" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

