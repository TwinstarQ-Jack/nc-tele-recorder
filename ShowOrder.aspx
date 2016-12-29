<%@ Page Title="" Language="C#" MasterPageFile="~/master/Style.master" AutoEventWireup="true" CodeFile="ShowOrder.aspx.cs" Inherits="ShowOrder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>查看订单 - NC电话订单管理系统后台</title>
    <style type="text/css">
        .labelcontent {
            font-weight: 100;
        }

        a.btn {
            margin: 8px 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="nctelrecord" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <%-- 保存子订单Type1ID、Type2ID --%>
    <asp:HiddenField ID="HiddenField_Type1ID" runat="server" />
    <asp:HiddenField ID="HiddenField_Type2ID" runat="server" />
    <asp:HiddenField ID="HiddenField_Type1ID_Name" runat="server" />
    <asp:HiddenField ID="HiddenField_Type2ID_Name" runat="server" />
    <asp:HiddenField ID="HiddenField_DealName" runat="server" />
    <asp:HiddenField ID="HiddenField_DealName_UID" runat="server" />
    <asp:HiddenField ID="HiddenField_BlocksID" runat="server" />
    <asp:HiddenField ID="HiddenField_Blocks_Name" runat="server" />
    <%-- 提示框 --%>
    <div runat="server" id="divAlterSuccess" visible="false" class="alert alert-success alert-dismissable" role="alert">
        <button class="close" type="button" data-dismiss="alert">&times;</button>
        <asp:Label ID="labelAlterSuccess" runat="server" Text=""></asp:Label>
    </div>
    <div runat="server" id="divAlterDanger" visible="false" class="alert alert-danger alert-dismissable" role="alert">
        <button class="close" type="button" data-dismiss="alert">&times;</button>
        <asp:Label ID="labelAlterDanger" runat="server" Text=""></asp:Label>
    </div>

    <%-- 父订单消息 --%>
    <div class="panel panel-primary">
        <div class="panel-heading">
            订单号&nbsp;<code><label runat="server" id="labelOrderID_heading">NULL</label></code>&nbsp;具体情况
        </div>
        <div class="panel-body">
            <div class="form-group">
                <label for="labelOrderID" class="control-label col-md-2">订单ID</label>
                <div class="col-md-1">
                    <label runat="server" id="labelOrderID_OrderID" class="control-label labelcontent">NULL</label>
                </div>
                <label for="labelDealPeopleNum" class="control-label col-md-2">处理人数</label>
                <div class="col-md-2">
                    <label runat="server" id="labelOrder_DealNum" class="control-label labelcontent">NULL</label>
                </div>
                <label for="labelStatus" class="control-label col-md-2">订单状态</label>
                <div class="col-md-3">
                    <label runat="server" id="labelOrder_Status" class="control-label labelcontent">NULL</label>
                </div>
            </div>
            <div class="form-group">
                <label for="labelQName" class="control-label col-md-2">报单人姓名</label>
                <div class="col-md-1">
                    <label runat="server" id="labelOrder_QName" class="control-label labelcontent">NULL</label>
                </div>
                <label for="labelQID" class="control-label col-md-2">使用账号</label>
                <div class="col-md-2">
                    <label runat="server" id="labelOrder_QID" class="control-label labelcontent">NULL</label>
                </div>
                <label for="labelTelephone" class="control-label col-md-2">联系电话</label>
                <div class="col-md-3">
                    <label runat="server" id="labelOrder_Tele" class="control-label labelcontent">NULL</label>
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-md-2">问题描述</label>
                <div class="col-md-10">
                    <label runat="server" id="labelOrder_Description" class="control-label labelcontent">NULL</label>
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-md-2">报单地点</label>
                <div class="col-md-4">
                    <label runat="server" id="labelOrder_Blocks" class="control-label labelcontent">NULL</label>
                </div>
                <label class="control-label col-md-2">最后修改日期</label>
                <div class="col-md-4">
                    <label runat="server" id="labelOrder_StampTime" class="control-label labelcontent">NULL</label>
                </div>
            </div>
        </div>
    </div>
    <%-- 父订单信息 结尾 --%>

    <%-- 订单操作功能区 --%>
    <div class="panel panel-success">
        <div class="panel-heading">订单操作功能区</div>
        <div class="panel-body">
            <%-- 子订单操作 --%>
            <div class="form-group">
                <label for="SubOrderExecute" class="control-label col-md-2">子订单操作：</label>
                <div class="col-md-2" runat="server" id="abtnSubOrderReceive" visible="false">
                    <asp:Button ID="btnSubOrderReceive" runat="server" Text="接收订单" CssClass="btn btn-block btn-success" OnClick="btnSubOrderReceive_Click" />
                </div>
                <div class="col-md-2" runat="server" id="abtnSubOrderConfirm">
                    <a href="#" class="btn btn-block btn-success" data-toggle="modal" data-target="#dealConfirm">问题已解决</a>
                </div>
                <div class="col-md-2" runat="server" id="abtnSubOrderTransfer" visible="false">
                    <a href="#" class="btn btn-block btn-info" data-toggle="modal" data-target="#dealTransfer">转让订单</a>
                </div>
                <div class="col-md-2" runat="server" id="abtnSubOrderRemarks" visible="false">
                    <a href="#" class="btn btn-block btn-primary" data-toggle="modal" data-target="#dealRemarks">增加备注</a>
                </div>
                <div class="col-md-2" runat="server" id="abtnSubOrderCancel" visible="false">
                    <a href="#" class="btn btn-block btn-danger" data-toggle="modal" data-target="#dealCancel">取消子订单</a>
                </div>
                <div class="col-md-2" runat="server" id="abtnSubOrderAdd">
                    <a href="../order.aspx?page=1" class="btn btn-block btn-primary">增加子订单</a>
                </div>
            </div>
            <%-- 管理员操作 --%>
        </div>
    </div>
    <%-- 订单操作功能区（结尾） --%>

    <%-- 子订单信息（倒序） --%>
    <div runat="server" id="CreateSubOrder">
    </div>
    <%-- 子订单信息（结束） --%>

    <%-- 弹出框实现确认订单 --%>
    <div class="modal fade" id="dealConfirm" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    <h4 class="modal-title">确认子订单</h4>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label for="modal_SubOrder_DealKey" class="control-label col-md-3">子订单编号</label>
                        <div class="col-md-9">
                            <label class="control-label" runat="server" id="modal_SubOrder_Deal">NULL</label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="modal_SubOrder_DealTime" class="control-label col-md-3">提交时间</label>
                        <div class="col-md-9">
                            <label class="control-label" runat="server" id="modal_SubOrder_DealTime">NULL</label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="modal_SubOrder_Feedback" class="control-label col-md-3">情况描述</label>
                        <div class="col-md-9">
                            <textarea runat="server" class="form-control" rows="6" id="modal_SubOrder_DealFeedback"></textarea>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="labelQuestionType" class="control-label col-md-3">问题分类</label>
                        <div class="col-md-4">
                            <asp:DropDownList ID="DropDownListQType1" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="DDLBind_QType2_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <asp:UpdatePanel class="col-md-5" ID="UpdatePanel_DDLQType2" RenderMode="Block" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="DropDownListQType2" runat="server" CssClass="form-control"></asp:DropDownList>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="DropDownListQType1" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <div class="form-group">
                        <label for="modal_SubOrder_UploadImg" class="control-label col-md-3">上传反馈图片</label>
                        <div class="col-md-9">
                            <label class="control-label" runat="server" id="modal_SubOrder_UploadClose">该功能暂时无法使用</label></div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">关闭</button>
                    <asp:Button ID="modal_SubOrder_Submit" runat="server" Text="确认订单" CssClass="btn btn-success" OnClick="modal_SubOrder_Submit_Click" />
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->

    <%-- 弹出框实现取消订单 --%>
    <div class="modal fade" id="dealCancel" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    <h4 class="modal-title">取消子订单</h4>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label for="modal_SubOrder_DealKey" class="control-label col-md-3">子订单编号</label>
                        <div class="col-md-9">
                            <label class="control-label" runat="server" id="modal_Cancel_Deal">NULL</label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="modal_SubOrder_DealTime" class="control-label col-md-3">提交时间</label>
                        <div class="col-md-9">
                            <label class="control-label" runat="server" id="modal_Cancel_DealTime">NULL</label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="modal_SubOrder_Feedback" class="control-label col-md-3">情况描述</label>
                        <div class="col-md-9">
                            <textarea runat="server" class="form-control" rows="6" id="modal_Cancel_DealFeedback"></textarea>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="modal_SubOrder_UploadImg" class="control-label col-md-3">上传反馈图片</label>
                        <div class="col-md-9">
                            <label class="control-label" runat="server" id="modal_Cancel_UploadClose">该功能暂时无法使用</label>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">关闭</button>
                    <asp:Button ID="modal_SubOrder_Cancel" runat="server" Text="取消订单" CssClass="btn btn-success" OnClick="modal_SubOrder_Cancel_Click" />
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->

    <%-- 转让订单 --%>
    <div class="modal fade" id="dealTransfer" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    <h4 class="modal-title">转让子订单</h4>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label class="control-label col-md-2">转让给</label>
                        <div class="col-md-offset-1 col-md-4">
                            <asp:DropDownList ID="DropDownList_TransferUser" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">关闭</button>
                    <asp:Button ID="modal_SubOrder_Transfer" runat="server" Text="转让订单" CssClass="btn btn-success" OnClick="modal_SubOrder_Transfer_Click" />
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->

    <%-- 添加备注 --%>
    <div class="modal fade" id="dealRemarks" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    <h4 class="modal-title">添加备注</h4>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label for="modal_Remarks_DealKey" class="control-label col-md-3">子订单编号</label>
                        <div class="col-md-9">
                            <label class="control-label" runat="server" id="modal_Remarks_Deal">NULL</label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="modal_Remarks_Content" class="control-label col-md-3">备注内容</label>
                        <div class="col-md-9">
                            <textarea runat="server" class="form-control" rows="6" id="modal_Remarks_Content"></textarea>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">关闭</button>
                    <asp:Button ID="modal_SubOrder_Remarks" runat="server" Text="提交备注" CssClass="btn btn-success"  OnClick="modal_SubOrder_Remarks_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

