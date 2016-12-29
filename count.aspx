<%@ Page Title="" Language="C#" MasterPageFile="~/master/Style.master" AutoEventWireup="true" CodeFile="count.aspx.cs" Inherits="count" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>个人清单统计 - NC电话订单管理系统后台</title>
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
    <div class="panel panel-default">
        <div class="panel-heading">按钮操作区</div>
        <div class="panel-body">
            <div class="form-group">
                <div class="col-md-offset-1 col-md-2">
                    <asp:Button ID="btnOrderUnfinished" runat="server" Text="未解决订单" CssClass="btn btn-block btn-primary" OnClick="btnOrderUnfinished_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnOrderThisMonth" runat="server" Text="当月已接订单" CssClass="btn btn-block btn-success" OnClick="btnOrderThisMonth_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnOrderHistory" runat="server" Text="以往处理订单" CssClass="btn btn-block btn-danger" OnClick="btnOrderHistory_Click" />
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

    <%-- 数据显示区 --%>
    <div class="DataShowHeader" style="margin-top: 10px;">
        <div class="Redefine-col-md-4 DataShowBlock" style="color: #fff; background-color: #337ab7; border-color: #2e6da4;">
            <div class="Redefine-col-md-3"><span class="glyphicon glyphicon-bookmark"></span></div>
            <div class="Redefine-col-md-9">
                <div>本月报单</div>
                <div>
                    <asp:Label ID="labelAddOrderNum" runat="server" Text="0"></asp:Label>
                </div>
            </div>
        </div>
        <div class="Redefine-col-md-4 DataShowBlock" style="color: #fff; background-color: #2a872a; border-color: #4cae4c;">
            <div class="Redefine-col-md-3"><span class="glyphicon glyphicon-map-marker"></span></div>
            <div class="Redefine-col-md-9">
                <div>本月处理</div>
                <div>
                    <asp:Label ID="labelDealOrderNum" runat="server" Text="0"></asp:Label>
                    <span class="badge" id="BadgeAtDeal_Unfinished" runat="server" visible="false" style="margin-left:5px;">0</span>
                </div>
            </div>
        </div>
        <div class="Redefine-col-md-4 DataShowBlock" style="color: #fff; background-color: #d9534f; border-color: #d43f3a;">
            <div class="Redefine-col-md-3"><span class="glyphicon glyphicon-book"></span></div>
            <div class="Redefine-col-md-9">
                <div>累计接单</div>
                <div>
                    <asp:Label ID="labelTotalOrderNum" runat="server" Text="0"></asp:Label>
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
                        <li>列表仅显示订单的<code>类型</code>、<code>接单时间</code>、<code>维修地址</code>、<code>联系方式</code>信息。</li>
                        <li><code>其他</code>信息：点击右侧<code>查看详细信息</code>栏目图标进入查看。</li>
                    </ol>
                </div>
            </div>
            <div class="table-responsive">
                <asp:Table ID="tableShowOrder_Unfinish" runat="server" CssClass="table table-bordered" Visible="false"></asp:Table>
            </div>
            <div class="panel-footer" runat="server" id="label_UnfinishedIsNULL" visible="false">
                你还没有尚未解决的订单，请继续保持！ 
            </div>
        </div>
    </div>
    <%-- 未解决订单 末端 --%>
    <%-- 已创建订单列表 --%>
    <div id="List_Of_Month" style="clear: both; padding-top: 15px;" runat="server" visible="false">
        <div class="panel panel-info">
            <div class="panel-heading">已创建订单</div>
            <div class="panel-body">
                <div class="divInfo">
                    <strong style="margin-bottom: 10px; display: block;">列表详细说明：</strong>
                    <ol>
                        <li>列表仅显示订单的<code>类型</code>、<code>接单时间</code>、<code>维修地址</code>、<code>联系方式</code>信息。</li>
                        <li><code>其他</code>信息：点击右侧<code>查看详细信息</code>栏目图标进入查看。</li>
                        <li>筛选条件：<code>当月</code>由<code>本人创建</code>的所有子订单。</li>
                    </ol>
                </div>
                您一共有符合条件的订单数<strong><label runat="server" id="label_Month">0</label></strong>条：
            </div>
            <div class="table-responsive">
                <asp:Table ID="tableShowOrder_Month" runat="server" CssClass="table table-bordered"></asp:Table>
            </div>
            <div class="panel-footer">
                <ul style="display: table; margin: 0 auto;" class="pagination pagination" runat="server" id="PaginationPage_Month"></ul>
            </div>
        </div>
    </div>
    <%-- 已创建订单列表 末端 --%>
    <%-- 已完成订单 --%>
    <div id="List_Of_History" style="clear: both; padding-top: 15px;" runat="server" visible="false">
        <div class="panel panel-success">
            <div class="panel-heading">已完成订单</div>
            <div class="panel-body">
                <div class="divInfo">
                    <strong style="margin-bottom: 10px; display: block;">列表详细说明：</strong>
                    <ol>
                        <li>列表仅显示订单的<code>类型</code>、<code>接单时间</code>、<code>维修地址</code>、<code>联系方式</code>信息。</li>
                        <li><code>其他</code>信息：点击右侧<code>查看详细信息</code>栏目图标进入查看。</li>
                        <li>筛选条件：<code>所有</code>由本人解决（标记<code>已解决</code>状态）的所有子订单。</li>
                    </ol>
                </div>
                您一共有符合条件的订单数<strong><label runat="server" id="label_History">0</label></strong>条：
            </div>
            <div class="table-responsive">
                <asp:Table ID="tableShowOrder_History" runat="server" CssClass="table table-bordered"></asp:Table>
            </div>
            <div class="panel-footer">
                <ul style="display: table; margin: 0 auto;" class="pagination pagination" runat="server" id="PaginationPage_History"></ul>
            </div>
        </div>
    </div>
    <%-- 已完成订单 末端 --%>
</asp:Content>

