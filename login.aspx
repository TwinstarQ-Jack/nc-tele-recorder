<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>登录 - NC电话订单管理系统</title>

    <link href="../css/bootstrap.min.css" rel="stylesheet" />
    <!--[if lt IE 9]>
      <script src="//cdn.bootcss.com/html5shiv/3.7.2/html5shiv.min.js"></script>
      <script src="//cdn.bootcss.com/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>

<body>
    <form id="formtop" runat="server" class="form-horizontal" role="form">
        <div class="navbar navbar-default navbar-inverse" role="navigation">
            <div class="navbar-header">
                <!-- .navbar-toggle样式用于toggle收缩的内容，即nav-collapse collapse样式所在元素 -->
                <button class="navbar-toggle" type="button" data-toggle="collapse" data-target=".navbar-responsive-collapse">
                    <span class="sr-only">Toggle Navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <!-- 确保无论是宽屏还是窄屏，navbar-brand都显示 -->
                <a href="##" class="navbar-brand">NC电话订单记录系统</a>
            </div>
            <!-- 屏幕宽度小于768px时，div.navbar-responsive-collapse容器里的内容都会隐藏，显示icon-bar图标，当点击icon-bar图标时，再展开。屏幕大于768px时，默认显示。 -->
            <div class="collapse navbar-collapse navbar-responsive-collapse">
                <ul class="nav navbar-nav">
                    <li><a href="../index.aspx">系统首页</a></li>
                    <li><a href="../order.aspx">增加订单</a></li>
                    <li><a href="../count.aspx">统计页面</a></li>
                    <li><a href="../query.aspx">条件查询</a></li>
                    <li id="admin" runat="server" visible="false" class="dropdown">
                        <a href="##" data-toggle="dropdown" class="dropdown-toggle">管理员菜单<span class="caret"></span></a>
                        <ul class="dropdown-menu">
                            <li><a href="../manage/accounts.aspx">人员管理</a></li>
                            <li><a href="../manage/blocks.aspx">区域管理</a></li>
                            <li><a href="../manage/questiontypes.aspx">问题分类</a></li>
                            <li><a href="../manage/count.aspx">订单统计</a></li>
                        </ul>
                    </li>
                    <li id="logins" runat="server" visible="true"><a href="../login.aspx">登录</a></li>
                    <li id="showuser" runat="server" visible="false">
                        <a href="##" data-toggle="dropdown" class="dropdown-toggle">
                            <asp:Label ID="showuserlabel" runat="server" Text=""></asp:Label><span class="caret"></span></a>
                        <ul class="dropdown-menu">
                            <li><a href="../member/index.aspx">个人情况</a></li>
                            <li><a href="../member/data.aspx">资料维护</a></li>
                            <li><a href="../login.aspx?logout=0">退出系统</a></li>
                        </ul>
                    </li>
                </ul>
            </div>
        </div>
        <div class="container">
            <div runat="server" id="success" visible="false" class="alert alert-success alert-dismissable" role="alert">
                <button class="close" type="button" data-dismiss="alert">&times;</button>
                登录成功，3秒后返回首页。
            </div>
            <div runat="server" id="updbfail" visible="false" class="alert alert-danger alert-dismissable" role="alert">
                <button class="close" type="button" data-dismiss="alert">&times;</button>
                更新数据库失败，请联系管理员。
            </div>
            <div runat="server" id="mysqlex" visible="false" class="alert alert-danger alert-dismissable" role="alert">
                <button class="close" type="button" data-dismiss="alert">&times;</button>
                数据库操作故障，请联系平台管理员。
            </div>
            <div runat="server" id="cookiesfail" visible="false" class="alert alert-danger alert-dismissable" role="alert">
                <button class="close" type="button" data-dismiss="alert">&times;</button>
                写入Cookies失败，请检查您的浏览器设置。
            </div>
            <div runat="server" id="loginfail" visible="false" class="alert alert-danger alert-dismissable" role="alert">
                <button class="close" type="button" data-dismiss="alert">&times;</button>
                登录失败，请检查您的输入是否正确。
            </div>
            <div runat="server" id="unlogined" visible="false" class="alert alert-danger alert-dismissable" role="alert">
                <button class="close" type="button" data-dismiss="alert">&times;</button>
                您还没登录，请登录后再尝试本操作。
            </div>
            <div runat="server" id="stringisnull" visible="false" class="alert alert-danger alert-dismissable" role="alert">
                <button class="close" type="button" data-dismiss="alert">&times;</button>
                用户名或密码为空，请检查您的输入。
            </div>
            <div runat="server" id="FormGroup">
                <div class="form-group" runat="server" id="UserGroup">
                    <label for="inputUserName" class="col-md-2 control-label">一卡通卡号</label>
                    <div class="col-md-10">
                        <input runat="server" type="text" class="form-control" id="inputUserName" placeholder="UserName" />
                    </div>
                </div>
                <div class="form-group" runat="server" id="PswGroup">
                    <label for="inputPassword" class="col-md-2 control-label">密码</label>
                    <div class="col-md-10">
                        <input runat="server" type="password" class="form-control" id="inputPassword" placeholder="Password" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-offset-4 col-md-4">
                        <asp:Button ID="btnLogin" runat="server" Text="登录" OnClick="btnLogin_Click" CssClass="btn btn-primary btn-block" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-offset-4 col-md-4">
                        <asp:Button ID="btnReset" runat="server" Text="重置" OnClick="btnReset_Click" CssClass="btn btn-default btn-block" />
                    </div>
                </div>
            </div>

        </div>
    </form>
    <script src="../js/jquery.js"></script>
    <script src="../js/bootstrap.min.js"></script>
</body>
</html>

