<%@ Page Title="" Language="C#" MasterPageFile="~/master/Style.master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>首页 - NC电话订单管理系统</title>
    <link href="css/datashow.css" rel="stylesheet" />
    <style type="text/css">
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
    <%-- 提示框 --%>
    <div runat="server" id="divAlterSuccess" visible="false" class="alert alert-success alert-dismissable" role="alert">
        <button class="close" type="button" data-dismiss="alert">&times;</button>
        <asp:Label ID="labelAlterSuccess" runat="server" Text=""></asp:Label>
    </div>
    <div runat="server" id="divAlterDanger" visible="false" class="alert alert-danger alert-dismissable" role="alert">
        <button class="close" type="button" data-dismiss="alert">&times;</button>
        <asp:Label ID="labelAlterDanger" runat="server" Text=""></asp:Label>
    </div>

    <%-- 数据显示区 --%>
    <div class="DataShowHeader" style="margin-top: 10px;">
        <div class="Redefine-col-md-4 DataShowBlock" style="color: #fff; background-color: #337ab7; border-color: #2e6da4;">
            <div class="Redefine-col-md-3"><span class="glyphicon glyphicon-arrow-right"></span></div>
            <div class="Redefine-col-md-9">
                <div>今日订单</div>
                <div>
                    <asp:Label ID="labelTodayOrder" runat="server" Text="0"></asp:Label>
                </div>
            </div>
        </div>
        <div class="Redefine-col-md-4 DataShowBlock" style="color: #fff; background-color: #2a872a; border-color: #4cae4c;">
            <div class="Redefine-col-md-3"><span class="glyphicon glyphicon-cloud"></span></div>
            <div class="Redefine-col-md-9">
                <div>本月订单</div>
                <div>
                    <asp:Label ID="labelMonthOrder" runat="server" Text="0"></asp:Label>
                </div>
            </div>
        </div>
        <div class="Redefine-col-md-4 DataShowBlock" style="color: #fff; background-color: #d9534f; border-color: #d43f3a;">
            <div class="Redefine-col-md-3"><span class="glyphicon glyphicon-alert"></span></div>
            <div class="Redefine-col-md-9">
                <div>未处理单</div>
                <div>
                    <asp:Label ID="labelUnFinishedOrder" runat="server" Text="0"></asp:Label>
                </div>
            </div>
        </div>
    </div>
    <%-- 数据显示区 末端 --%>
    <%-- 未解决订单 --%>
    <div id="List_Of_Unfinished" style="clear: both; padding-top: 15px;" runat="server">
        <div class="panel panel-danger">
            <div class="panel-heading">未解决订单&nbsp;&nbsp;<span class="badge" id="Badge_Unfinished" runat="server" visible="false">0</span></div>
            <div class="panel-body">
                <div class="divInfo">
                    <strong style="margin-bottom: 10px; display: block;">列表详细说明：</strong>
                    <ol>
                        <li>列表仅显示订单的<code>类型</code>、<code>接单时间</code>、<code>维修地址</code>、<code>联系方式</code>、<code>接单人</code>信息。</li>
                        <li><code>其他</code>信息：点击右侧<code>查看详细信息</code>栏目图标进入查看。</li>
                    </ol>
                </div>
            </div>
            <div class="table-responsive">
                <asp:Table ID="tableShowOrder_Unfinish" runat="server" CssClass="table table-bordered" Visible="false"></asp:Table>
            </div>
            <div class="panel-footer" runat="server" id="label_UnfinishedIsNULL" visible="false">
                现在还没有尚未解决的订单。
            </div>
        </div>
    </div>
    <%-- 未解决订单 末端 --%>
    <%-- 今日已解决订单 --%>
    <div id="List_Of_TodayFinished" style="clear: both; padding-top: 10px;" runat="server">
        <div class="panel panel-primary">
            <div class="panel-heading">今日已解决订单</div>
            <div class="panel-body" id="panel_TodayFinished_Body" runat="server">
                <div class="divInfo">
                    <strong style="margin-bottom: 10px; display: block;">列表详细说明：</strong>
                    <ol>
                        <li>列表仅显示订单的<code>类型</code>、<code>接单时间</code>、<code>维修地址</code>、<code>联系方式</code>、<code>接单人</code>信息。</li>
                        <li><code>其他</code>信息：点击右侧<code>查看详细信息</code>栏目图标进入查看。</li>
                    </ol>
                </div>
            </div>
            <div class="table-responsive">
                <asp:Table ID="tableShowOrder_TodayFinished" runat="server" CssClass="table table-bordered"></asp:Table>
            </div>
        </div>
    </div>
</asp:Content>

