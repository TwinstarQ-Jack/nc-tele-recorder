<%@ Page Title="" Language="C#" MasterPageFile="~/master/Style.master" AutoEventWireup="true" CodeFile="order.aspx.cs" Inherits="order" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>增加订单 - NC电话订单管理系统</title>
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
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <%-- 隐藏域 --%>
    <asp:HiddenField ID="HiddenField_BlockID" runat="server" />
    <asp:HiddenField ID="HiddenField_BlockID_Name" runat="server" />
    <div runat="server" id="navigation" visible="true">
        <div class="panel panel-default">
            <div class="panel-heading">功能区域</div>
            <div class="panel-body">
                <div class="form-group">
                    <div class="col-md-offset-1 col-md-2">
                        <asp:Button ID="btnAddNewOrder" runat="server" Text="添加新订单" CssClass="btn btn-danger btn-block" OnClick="btnAddNewOrder_Click" />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnAdditionNewOrder" runat="server" Text="追加订单状态" CssClass="btn btn-primary btn-block" OnClick="btnAdditionNewOrder_Click" />
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

        <%-- 新订单表格框 --%>
        <div class="panel panel-info" runat="server" id="divAddNewOrder">
            <div class="panel-heading">添加新订单</div>
            <div class="panel-body">
                <div class="divInfo">
                    <strong style="margin-bottom: 10px; display: block;">表格提交要求：</strong>
                    <ol>
                        <li><code>所有项目</code>都必须填写。</li>
                        <li>在<code>添加子订单模式</code>下，只能修改订单问题类型、指派人。</li>
                        <li><code>子订单</code>说明：<code>0001</code>为增加的新父订单，<code>大于0001</code>则为父订单的子订单。</li>
                    </ol>
                </div>
                <%-- Part 1:系统编号 --%>
                <div class="col-md-12 headerOfBlank">系统生成订单信息</div>
                <div class="form-group">
                    <label for="labelOrderID" class="control-label col-md-2">订单号</label>
                    <div class="col-md-4">
                        <input type="text" runat="server" disabled="disabled" class="form-control contentcenter" id="inputOrderID" placeholder="OrderID" />
                    </div>
                    <label for="labelSubOrderID" class="control-label col-md-2">子订单号</label>
                    <div class="col-md-4">
                        <input type="text" runat="server" disabled="disabled" class="form-control contentcenter" id="inputSubOrderID" placeholder="OrderSubID" />
                    </div>
                    <label for="labelOrderState" class="control-label col-md-2">订单总状态</label>
                    <div class="col-md-4">
                        <select runat="server" id="selectStateCode" class="form-control" disabled="disabled">
                            <option value="0">未处理订单</option>
                            <option value="1">正在处理</option>
                            <option value="2">需要跟进</option>
                            <option value="3">已解决</option>
                            <option value="4">已取消</option>
                        </select>
                    </div>
                </div>
                <%-- Part 2:报单人信息 --%>
                <div class="col-md-12 headerOfBlank">报单人信息</div>
                <div class="form-group" runat="server" id="divQID">
                    <label for="labelQID" class="control-label col-md-2">上网账号</label>
                    <div class="col-md-4">
                        <input type="text" runat="server" class="form-control" id="inputQID" placeholder="Customer Account" />
                    </div>
                </div>
                <div class="form-group" runat="server" id="divQnametele">
                    <label for="labelQname" class="control-label col-md-2">称呼</label>
                    <div class="col-md-4">
                        <input type="text" runat="server" class="form-control" id="inputQname" placeholder="Customer Name" />
                    </div>
                    <label for="labelQtelephone" class="control-label col-md-2">联系方式</label>
                    <div class="col-md-4">
                        <input type="text" runat="server" class="form-control" id="inputQtelephone" placeholder="Contact Information" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="labelOrderState" class="control-label col-md-2">问题描述</label>
                    <div class="col-md-10">
                        <textarea rows="4" runat="server" class="form-control" id="textareaQdescription"></textarea>
                    </div>
                </div>
                <div class="form-group">
                    <label for="labelBlockID" class="control-label col-md-2">地点</label>
                    <div class="col-md-3">
                        <asp:DropDownList ID="DropDownListBlockID" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="col-md-2">
                        <input type="text" runat="server" class="form-control contentcenter" id="inputBlockRoom" placeholder="Room Numer" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="labelQuestionType" class="control-label col-md-2">问题分类</label>
                    <div class="col-md-5">
                        <asp:DropDownList ID="DropDownListQType1" runat="server" CssClass="form-control" OnSelectedIndexChanged="DDLBind_QType1_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                    </div>
                    <div>
                        <asp:UpdatePanel class="col-md-5" ID="UpdatePanel_DDLQType2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="DropDownListQType2" runat="server" CssClass="form-control"></asp:DropDownList>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="DropDownListQType1" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <%-- Part 3:填表人信息 --%>
                <div class="col-md-12 headerOfBlank">填表人信息</div>
                <div class="form-group">
                    <label for="labelLogCardID" class="control-label col-md-2">一卡通卡号</label>
                    <div class="col-md-2">
                        <input type="text" runat="server" class="form-control contentcenter" id="inputLogCardID" placeholder="Logger CardID" disabled="disabled" />
                    </div>
                    <label for="labelLogRealName" class="control-label col-md-1">姓名</label>
                    <div class="col-md-2">
                        <input type="text" runat="server" class="form-control contentcenter" id="inputLogRealName" placeholder="Logger RealName" disabled="disabled" />
                    </div>
                    <label for="labelLogTime" class="control-label col-md-2">登记时间</label>
                    <div class="col-md-3">
                        <input type="text" runat="server" class="form-control contentcenter" id="inputLogTime" placeholder="Log Time" disabled="disabled" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="labelAppointer" class="control-label col-md-2">指派给</label>
                    <div class="col-md-3">
                        <asp:DropDownList ID="DropDownListAppointer" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="panel-footer">
                <div class="form-group">
                    <div class="col-md-offset-2 col-md-3">
                        <asp:Button ID="btnSubmit" runat="server" Text="添加订单" CssClass="btn btn-block btn-primary" OnClick="btnSubmit_Click" />
                    </div>
                    <div class="col-md-offset-2 col-md-3">
                        <asp:Button ID="btnReset" runat="server" Text="重置表单" CssClass="btn btn-block btn-default" />
                    </div>
                </div>
            </div>
        </div>
        <%-- 新订单表格框 末端--%>
        <%-- 同用户追加订单显示历史记录 --%>
        <div class="panel panel-success" runat="server" id="divAddSubOrder" visible="false">
            <div class="panel-heading">两天内订单追加</div>
            <div class="panel-body">
                <div class="divInfo">
                    <strong style="margin-bottom: 10px; display: block;">功能说明：</strong>
                    <ol>
                        <li>本功能只显示父订单状态为<code>已解决</code>或<code>已关闭</code>下的最新子订单。</li>
                        <li>超过<code>2天</code>的父订单（以最新的子订单的解决时间计算）无法增加新的子订单。</li>
                        <li>如果原父订单未解决，请先解决并进行确认后，再添加子订单。</li>
                        <li><code>订单状态</code>说明：<code>3</code>为已处理订单，<code>4</code>为已关闭订单。</li>
                    </ol>
                </div>
            </div>
            <div class="table-responsive">
                <asp:Table ID="tableAddSubOrder" runat="server" CssClass="table table-bordered"></asp:Table>
            </div>
            <div class="panel-footer">
                <ul style="display: table; margin: 0 auto;" class="pagination pagination" runat="server" id="PaginationPage"></ul>
            </div>
        </div>
        <%-- 同用户追加订单显示历史记录 末端--%>
    </div>
</asp:Content>

