<%@ Page Title="" Language="C#" MasterPageFile="~/master/Style.master" AutoEventWireup="true" CodeFile="query.aspx.cs" Inherits="query" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>查询订单 - NC电话订单管理系统后台</title>
    <script src="js/My97DatePicker/WdatePicker.js"></script>
    <style type="text/css">
        table{
            margin:0 auto;
        }
        table tr td {
            text-align: center;
            padding-left: 10px;
            padding-top: 10px;
        }
        .input_SearchDay {
            text-align: center;
            height: 100%;
            width: 100%;
            border: 0px;
        }
        .headerOfBlank {
            text-align: center;
            border: 1px solid black;
            margin: 10px 0;
            padding: 5px 0;
        }
        .contentcenter {
            text-align: center;
        }
        ol > li, ol > li > ul > li {
            padding: 5px 5px 5px 20px;
        }
        .divInfo {
            margin: 10px 0 5px 30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="nctelrecord" runat="Server">
    <%-- 支持UpdatePanel Ajax --%>
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
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
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <%-- 正式区域 --%>
    <div class="panel panel-default">
        <div class="panel-heading">搜索条件</div>
        <div class="panel-body">
            <div class="divInfo">
                <strong style="margin-bottom: 10px; display: block;">表格提交要求：</strong>
                <ol>
                    <li>带<code>*</code>的项目必须填写。</li>
                    <li>其他条件均为<code>可选择填写</code>条件。</li>
                    <li>由于搜索功能为<code>全匹配</code>搜索，不确定的项目请留空。</li>
                    <li>目前还没有做出搜索结果分页的解决方案，在搜索时尽量准确，否则可能因为条目太多，看起来比较麻烦。</li>
                </ol>
            </div>
            <table>
                <tr>
                    <td>*查询日期范围</td>
                    <td>
                        <input type="text" onclick="WdatePicker()" runat="server" id="input_SearchDay_Begin" class="form-control input_SearchDay" /></td>
                    <td>至</td>
                    <td>
                        <input type="text" onclick="WdatePicker()" runat="server" id="input_SearchDay_End" class="form-control input_SearchDay" /></td>
                </tr>
                <tr>
                    <td>报单人姓名</td>
                    <td><input type="text" runat="server" id="input_Search_Name" class="form-control input_SearchDay"/></td>
                    <td>电话</td>
                    <td><input type="text" runat="server" id="input_Search_Tele" class="form-control input_SearchDay"/></td>
                </tr>
                <tr>
                    <td>用户地区</td>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel_Search_Block" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="DDL_Search_Block" runat="server" CssClass="form-control"></asp:DropDownList>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="DDL_Search_Block" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td>-</td>
                    <td><input type="text" runat="server" id="input_Search_BlockRoom" class="form-control" /></td>
                </tr>
                <tr>
                    <td>问题类型</td>
                    <td><asp:DropDownList ID="DDL_Search_Type1" runat="server" CssClass="form-control" OnSelectedIndexChanged="DDLBind_QType1_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
                    <td>-</td>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel_Search_DDLType2" runat="server" RenderMode="Inline" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                                <asp:DropDownList ID="DDL_Search_Type2" runat="server" CssClass="form-control"></asp:DropDownList>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="DDL_Search_Type1" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>处理人</td>
                    <td colspan="2">
                        <asp:UpdatePanel ID="UpdatePanel_Search_DDLDeal" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="DDL_Search_Deal" runat="server" CssClass="form-control"></asp:DropDownList>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="DDL_Search_Deal" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
        <div class="panel-footer">
            <div class="form-group">
                <div class="col-md-offset-4 col-md-4">
                    <asp:Button Text="搜索" ID="btnSearch" runat="server" CssClass="btn btn-block btn-danger" OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>
    </div>
    <%-- 结果显示区 --%>
    <asp:UpdatePanel class="panel panel-primary" ID="UpdatePanel_SearchResult" runat="server" RenderMode="Block" UpdateMode="Conditional">
        <%-- 需要更新的区域 --%>
        <ContentTemplate>
            <div class="panel-heading">搜索结果</div>
            <div class="panel-body">根据条件，共搜索结果<code><label runat="server" id="label_Update_DBResult">0</label></code>条：</div>
            <div class="table-responsive">
                <asp:Table ID="table_Update_ShowResult" runat="server" Visible="false" CssClass="table table-bordered "></asp:Table>
            </div>
            <div class="panel-footer"></div>
        </ContentTemplate>
        <%-- Triggers:绑定异步刷新按钮ID --%>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
