<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Banner.aspx.cs" Inherits="astorWork.aspx.Main.Banner" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title></title>
     <style type="text/css">
      #globalRightLinks {
            position: absolute;
            padding: 0px;
            margin: 0px;
            top: 0px;
            right: 10px;
            z-index: 100;
            text-decoration: none;
            color: Black;
        }

        .image {
            margin-bottom: 3px;
        }

        .rbText {
            padding-left: 10px !important;
        }

        .rbDecorated {
            padding-left: 20px !important;
            padding-right: 7px !important;
        }
    </style>
     <script type="text/javascript">

        var sessionTimeout = '<%# Session.Timeout * 60 * 1000 %>';
        var timeout = sessionTimeout;
        var timer = setInterval(function () { timeout -= 1000; if (timeout == 60000) { ShowSessionExpireAlert(); } else if (timeout == 0) { clearInterval(timer); ShowSessionExpiredMessage(); } }, 1000);

        function ResetSessionTimeOut() {
            timeout = sessionTimeout;
        }

        function ShowSessionExpireAlert() {
            parent.ContentFrame.ShowSessionExpireAlert();
        }

        function ShowSessionExpiredMessage() {
            parent.ContentFrame.ShowSessionExpiredMessage();
        }

        function TabItemChanged(parentModTab) {
            parent.TreeViewFrame.moduleArray = moduleArray;
            parent.TreeViewFrame.pageArray = pageArray;
            parent.TreeViewFrame.GenerateTreePanel(parentModTab.id);
            parent.ContentFrame.CloseAncmntNotification();
            parent.document.getElementById('Bottom').cols = '20%,*';
        }

       

        function OpenFeedBackRadWindow() {
            parent.ContentFrame.OpenFeedBackRadWindow();
        }

        function OpenUserManualRadWindow() {
            var UserManualURL = '/AstorTimeCompleteUserManual.pdf';
            parent.ContentFrame.ShowUserManual(UserManualURL);
        }
       function navigation()
       {
          top.window.location.href = '../../Logout.aspx';
       }

    </script>
</head>
<body class="bannerPage">
    <form id="form1" runat="server">
                <telerik:RadScriptManager runat="server" ID="rsmBanner" />
  <%--<ext:ResourceManager ID="ResourceManager1" runat="server" SkinID="parentModule" />--%>
  
        <table id="globalRightLinks">
            <tr>
                <td>
                       <telerik:RadButton ID="rbHelp" runat="server" ToolTip="First Time Setup User Manual" Visible="false" Width="21px" OnClientClicked="OpenUserManualRadWindow">
                        <Icon PrimaryIconUrl="~/images/HelpDocument.png" PrimaryIconLeft="7" />
                    </telerik:RadButton>
                </td>
                 <td>
                      <telerik:RadButton ID="rbFeedback" runat="server" ToolTip="Write Feedback" Width="21px" Visible="false" OnClientClicked="OpenFeedBackRadWindow">
                        <Icon PrimaryIconUrl="~/images/Feedback.png" PrimaryIconLeft="7" />
                    </telerik:RadButton>
                </td>
                <td>
                 
                     <telerik:RadButton ID="rbUserProfile" runat="server" ToolTip="User Details" Width="55px" AutoPostBack="false"  ButtonType="LinkButton" >
                        <Icon PrimaryIconUrl="~/images/User.png" PrimaryIconLeft="5" />
                    </telerik:RadButton>
                </td>
                <td>
                     <telerik:RadButton ID="rbLogout" runat="server"  AutoPostBack="false" Width="62px" OnClientClicked="navigation" ButtonType="LinkButton"  ToolTip="Log out" Text="Log out">
                        <Icon PrimaryIconUrl="~/images/Logout.png" PrimaryIconLeft="5"  />
                    </telerik:RadButton>
                </td>
            </tr> 
        </table>

     <%--    <ext:TabStrip runat="server" ID="tsParentModules" ActiveTabIndex="-1">
        <Items>
        </Items>
        <Listeners>
            <TabChange Handler="TabItemChanged(this.activeTab)" />
        </Listeners>
    </ext:TabStrip>--%>
  
 
    </form>
</body>
</html>
