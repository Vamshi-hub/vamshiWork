<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentPage.aspx.cs" EnableSessionState="ReadOnly" Inherits="astorWork.aspx.Main.ContentPage" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <style type="text/css">
        .AnnouncementListItemClass {
            margin-top: 5px;
            padding-left: 3px;
            font-size: 12px;
            margin-left: 15px;
            list-style-type: disc;
            text-align: left;
            color: #0F143F;
        }

        .RadTabStrip_Metro .rtsLevel .rtsTxt {
            text-transform: none !important;
        }

        html .RadNavigation .rnvMore,
        html .RadNavigation .rnvRootLink {
            padding: 7% 43%;
        }


        .RadMenu .rmGroup .rmText {
            padding: 2px 2px 2px 2px !important;
        }

        .RadMenu .rmVertical .rmText {
            padding: 2px 2px 2px 2px !important;
        }

        .RadMenu_Metro .rmRoundedCorners ul.rmGroup, .RadMenu_Metro .rmRoundedCorners ul.rmGroup, .RadMenu_Metro .rmRoundedCorners .rmItem .rmGroup, .RadMenu_Metro.RadMenu_Context.rmRoundedCorners ul.rmGroup {
            padding: 6px !important;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <telerik:RadAjaxLoadingPanel ID="AjaxLoadingPanel1" runat="server" Skin="MetroTouch">

        </telerik:RadAjaxLoadingPanel>
        <ext:ResourceManager ID="ResourceManager1" runat="server" SkinID="pageTab" />

        <asp:UpdatePanel runat="server" ID="upnlAnnouncement" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdnHideNotification" Value="true" />
                <telerik:RadNotification ID="rntfnAnnouncement" Visible="true" runat="server" Skin="MetroTouch" Width="315" Height="185" Animation="Slide" EnableRoundedCorners="true"
                    EnableShadow="true" VisibleOnPageLoad="true" Title="Announcements" AutoCloseDelay="0" OffsetX="-15" OffsetY="-15" TitleIcon="None" ContentScrolling="Auto"
                    OnClientShowing="OnClientShowing">
                    <ContentTemplate>
                        <div id="divUnSeenAnnouncements" runat="server" style="display: none;">
                            <telerik:RadListView ID="rlvNewAnnouncement" runat="server">
                                <ItemTemplate>
                                    <fieldset style="padding: 5px; margin-bottom: 10px; border: 1px solid #25A0DA;">
                                        <legend><b>Release Date: <%#Convert.ToDateTime(Eval("ReleaseDate")).ToString("dd MMM yyyy")%></b>
                                        </legend>
                                        <table style="font-family: Segoe UI,Arial,Helvetica,sans-serif;" border="0">
                                            <tr>
                                                <td style="margin-top: 5px; text-align: justify;">
                                                    <%# Convert.ToBoolean(Eval("IsPreAnnouncementMessage")) ? Eval("PreAnnouncementMessage") : Eval("PostAnnouncementMessage") %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:HyperLink ID="hlnkDocument" runat="server" Text='<%# Eval("LinkLabel")%>' NavigateUrl='<%# Eval("DocumentURL")%>'
                                                        Visible='<%# !String.IsNullOrEmpty(Eval("DocumentURL").ToString()) %>' Target="_blank" />
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </ItemTemplate>
                            </telerik:RadListView>
                        </div>
                        <div id="divAllAnnouncements" style="display: none;">
                            <telerik:RadListView ID="rlvAllAnnouncement" runat="server">
                                <ItemTemplate>
                                    <fieldset style="padding: 5px; margin-bottom: 10px; border: 1px solid #25A0DA;">
                                        <legend><b>Release Date: <%#Convert.ToDateTime(Eval("ReleaseDate")).ToString("dd MMM yyyy")%></b>
                                        </legend>
                                        <table style="font-family: Segoe UI,Arial,Helvetica,sans-serif;" border="0">
                                            <tr>
                                                <td style="margin-top: 5px; text-align: justify;">
                                                    <%# Convert.ToBoolean(Eval("IsPreAnnouncementMessage")) ? Eval("PreAnnouncementMessage") : Eval("PostAnnouncementMessage") %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:HyperLink ID="hlnkDocument" runat="server" Text='<%# Eval("LinkLabel")%>' NavigateUrl='<%# Eval("DocumentURL")%>'
                                                        Visible='<%# !String.IsNullOrEmpty(Eval("DocumentURL").ToString()) %>' Target="_blank" />
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </ItemTemplate>
                            </telerik:RadListView>
                        </div>
                    </ContentTemplate>
                </telerik:RadNotification>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="divWelcomeMsg" runat="server" style="position: fixed; width: 100%; top: 45%; text-align: center; font-size: 15px; color: #293952; font-family: Times New Roman; font-weight: bold;">
            <table width="100%" border="0" cellspacing="2" cellpadding="2">
                <tr>
                    <td align="center">
                        <p>
                          astorWork System
                        </p>
                        <p>
                            for
                        </p>
                        <p>
                            <asp:Label runat="server" ID="lblTenantName" Text="Astoria" />
                        </p>
                    </td>
                </tr>
            </table>
        </div>
        <div>
            <asp:HiddenField ID="hdnSessionExpire" runat="server" />
            <telerik:RadWindow ID="RadWin_SessionTimeout" runat="server" Modal="true" Width="300px" Height="80px"
                Behaviors="Close,Reload" VisibleStatusbar="false" Title="Session Expiry Alert" AutoSize="true">
                <ContentTemplate>
                    <asp:Panel ID="pnlSessionTimeout" runat="server" Width="300px" Height="80px">
                        <br />
                        <table>
                            <tr>
                                <td colspan="3">
                                    <asp:Label ID="lblMessage" runat="server" Style="font-family: Segoe UI Light; font-size: 10pt;" Font-Bold="true" Text="Your session is going to expire in 1 minute. Would you like to extend your session?"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <input type="button" name="Yes" style="width: 40px" value="Yes" onclick="ResetSession();" />
                                </td>
                                <td>
                                    <br />
                                </td>
                                <td align="left">
                                    <input type="button" style="width: 40px" name="Cancel" value="No" onclick="CloseSessionExpireAlert(); return false;" />
                                </td>
                            </tr>
                        </table>

                    </asp:Panel>

                </ContentTemplate>
            </telerik:RadWindow>
            <telerik:RadWindow ID="RadWin_SessionExpired" runat="server" Modal="true" Width="220px" Height="200px"
                Behaviors="Close,Reload" VisibleStatusbar="false" Title="Session Expired" AutoSize="true">
                <ContentTemplate>
                    <asp:Panel ID="pnlSessionExpired" runat="server" Width="220px" class="table">
                        <div class="layout">
                            <table>

                                <tr>
                                    <td align="center">
                                        <asp:Label ID="lblExpMessage1" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Large"
                                            Text="Your Session Expired !"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <br />
                                        <asp:Button ID="btnLogin" runat="server" Text="Login again" OnClientClick="RedirectToLogin();return false;" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="CloseSessionExpiredMessage();return false;" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <br />
                    </asp:Panel>
                </ContentTemplate>
            </telerik:RadWindow>
            <telerik:RadWindow runat="server" ID="RadWin_EditUserProfile" Modal="true" ShowContentDuringLoad="false"
                VisibleStatusbar="false" Behaviors="Close,Reload">
            </telerik:RadWindow>
            <telerik:RadWindowManager Modal="true" ID="rwmFeedBack" onAutoSize="true" Behaviors="Close"
                VisibleStatusbar="false" runat="server">
                <Windows>
                    <telerik:RadWindow ID="RadWin_FeedBack" runat="server" Height="180px" Width="386px"
                        Modal="true" BackColor="#DADADA" VisibleStatusbar="false" Title="Comments/Feedback">
                        <ContentTemplate>
                            <table>
                                <tr>
                                    <td align="center" style="padding: 4px;">
                                        <asp:TextBox ID="tbFeedBack" runat="server" ClientIDMode="Static" ValidationGroup="Feedback"
                                            TextMode="MultiLine" Width="350px" Rows="5" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btnSubmit" runat="server" ValidationGroup="Feedback" Text="Submit"
                                            Width="50px" OnClientClick="SubmitFeedBackAndClose();return false;" />
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </telerik:RadWindow>
                </Windows>
            </telerik:RadWindowManager>

            <div style="position: absolute; top: 1px; z-index: 1000000">
                <img src="../../images/Collapse.png" onclick="CollapseExpandTreePanel('Collapse');" id="imgCollapse" alt="Collapse" title="Collapse" style="cursor: pointer; cursor: hand;" />
                <img src="../../images/Expand.png" title="Expand" onclick="CollapseExpandTreePanel('Expand');" id="imgExpand" alt="Expand" style="display: none; cursor: pointer; cursor: hand;" />
            </div>

            <div style="width: 100%; padding-top: 0px;">
                <div id="divTabStrip" runat="server" style="float: left; width: 96%;">
                   
                    <telerik:RadTabStrip runat="server" ID="RadTabStrip1" ScrollChildren="true" MultiPageID="rmp" Skin="Metro" ShowBaseLine="false" Height="100%">
                    </telerik:RadTabStrip>
                       
                </div>
                <div style="float: right; padding-top: 0px; width: 4%;">
                    <telerik:RadNavigation ID="rnDockMenu" runat="server" Height="100%" CollapseAnimation-Type="None" Skin="MetroTouch">
                        <ExpandAnimation Type="None" />
                        <Nodes>
                            <telerik:NavigationNode>
                                <NodeTemplate>
                                    <telerik:RadButton ID="rbFeedback" runat="server" AutoPostBack="false" Visible="false" ToolTip="Write Feedback" BorderStyle="None" Text="Feedback" OnClientClicked="OpenFeedBackRadWindow">
                                        <Icon PrimaryIconUrl="~/Images/Feedback.png" PrimaryIconLeft="4" />
                                    </telerik:RadButton>
                                </NodeTemplate>
                            </telerik:NavigationNode>
                            <telerik:NavigationNode ID="rnUserProfile">
                                <NodeTemplate>
                                    <telerik:RadButton ID="rbUserProfile" runat="server" AutoPostBack="false" BorderStyle="None" ToolTip="User Details" ButtonType="LinkButton" OnClientClicked="OpenUserRadWindow">
                                        <Icon PrimaryIconUrl="~/Images/User.png" PrimaryIconLeft="6" />
                                    </telerik:RadButton>
                                </NodeTemplate>
                            </telerik:NavigationNode>
                            <telerik:NavigationNode>
                                <NodeTemplate>
                                    <telerik:RadButton ID="rbLogout" runat="server" AutoPostBack="false" BorderStyle="None" NavigateUrl="~/Logout.aspx" Target="_top" ButtonType="LinkButton" ToolTip="Log out" Text="Log out">
                                        <Icon PrimaryIconUrl="~/Images/Logout.png" PrimaryIconLeft="6" />
                                    </telerik:RadButton>
                                </NodeTemplate>
                            </telerik:NavigationNode>
                        </Nodes>
                    </telerik:RadNavigation>
                </div>
              
            </div>
            <telerik:RadMultiPage runat="server" ID="rmp" SelectedIndex="0">
            </telerik:RadMultiPage>
            <div style="padding-left: 150px;">
                <ext:Viewport ID="Viewport1" runat="server" AutoHeight="true" AutoWidth="true" Layout="FitLayout" Margins="0,0,0,10" Visible="false">
                    <Items>
                        <ext:TabPanel ID="tpTabs" runat="server" Collapsible="false" Region="Center" EnableTabScroll="true">
                            <Plugins>
                                <ext:TabScrollerMenu PageSize="5" MaxText="100" />
                                <ext:TabCloseMenu />
                            </Plugins>
                        </ext:TabPanel>
                    </Items>
                </ext:Viewport>
            </div>


        </div>
        <div id="divFooterMsg" runat="server" style="width: 100%; position: fixed; bottom: 20px;"
            align="center">
            <table width="100%" border="0">
                <tr>
                    <td align="center">
                        <asp:Label ID="lblFoterMessage" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <telerik:RadContextMenu runat="server" ID="RadCtxtMenu" Skin="MetroTouch"
            EnableRoundedCorners="true" EnableShadows="true" ShowToggleHandle="true" Width="100%"
            OnClientItemClicked="CloseAllTabs">
            <Items>
                <telerik:RadMenuItem Text="Close Other Tabs" Value="0" />
                <telerik:RadMenuItem Text="Close All Tabs" Value="1" />
            </Items>
        </telerik:RadContextMenu>
        <asp:HiddenField runat="server" ID="hdnYardViewPageDetails" />
    </form>
</body>
<script type="text/javascript">


    var count = 0;
    var myTabs = new Array();
    var maxTabs = 5;
    var addTab = function (id, url) {
        try {
            document.getElementById("divWelcomeMsg").style.display = "none";
            document.getElementById("divFooterMsg").style.display = "none";
            var tabPanel = Ext.getCmp('tpTabs');

            /* Checking if yardview page exists. Set default page and if already exists, making it active when clicked on same page in tree */
            var YardViewTab = tabPanel.getComponent('YardView');
            var tempid = id.replace(' :: ', '').replace(' ', '');
            if (YardViewTab && tempid == 'YardView') {
                tabPanel.setActiveTab(YardViewTab);
                return;
            }
            /* End */

            var tab = tabPanel.getComponent(id);
            if (!tab) {
                if (count < maxTabs) {
                    tab = tabPanel.add({
                        id: id,
                        //title: id + " <img style=\"margin-left: 2px;margin-bottom: -2px;cursor:hand;\" src=\"../../images/help.png\" width=\"13\" height=\"13\" border=\"0\" title=\"Help\" onClick=\"javascript:ShowUserManual('" + url + "')\" /> "
                        //    + " <img style=\"margin-left: 2px; margin-bottom:0px; cursor:hand;border-color: #9CBAEF;border-width:1px;\" src=\"../../images/RefreshTab.bmp\" width=\"9\" height=\"9\" border=\"0\" title=\"Refresh\" onClick=\"javascript:RefreshTab('" + id + "')\" />",
                        closable: true,
                        height: '500',
                        autoWidth: true,
                        autoLoad: {
                            showMask: true,
                            url: url,
                            mode: "iframe",
                            maskMsg: "Loading " + id + "..."
                        }
                    });
                    myTabs.push(id);
                    count = count + 1;
                    tab.on('close', function () {
                        count = count - 1;
                        ShowWelcomeMsg();
                        var idx = myTabs.indexOf(tab.id);
                        if (idx != -1)
                            myTabs.splice(idx, 1);
                    });
                }
                else {
                    var item1 = myTabs[0];
                    tabPanel.remove(item1);
                    myTabs.shift();
                    count = count - 1;
                    addTab(id, url);
                    var tab = tabPanel.getComponent(id);
                    tabPanel.setActiveTab(tab);
                }
            }
            tabPanel.setActiveTab(tab);
        }
        catch (ex) {
        }
    }

    function RefreshTab(id) {
        var tabPanel = Ext.getCmp('tpTabs');
        var tab = tabPanel.getComponent(id);
        if (tab != null)
            tab.reload();
    }

    function ShowWelcomeMsg() {
        var tabPanel = Ext.getCmp('tpTabs');
        if (tabPanel.items.items.length == 1) {
            document.getElementById("divWelcomeMsg").style.display = '';
            document.getElementById("divFooterMsg").style.display = '';
        }
    }

    function ShowUserManual(url) {
        var UserManualURL;
        var pageID = url.split('/');
        pageID = pageID[pageID.length - 1];
        if (pageID.indexOf("aspx") != -1)
            pageID = pageID.replace("aspx", "pdf");
        if (BlobURL != '')
            UserManualURL = encodeURIComponent(BlobURL.replace("{0}", pageID));
        OpenUserManualRadWindow(UserManualURL);
    }

    function CloseAncmntNotification() {
        var notification = Ext.getCmp('ntfyAnnouncement');
        if (notification != undefined)
            Ext.getCmp('ntfyAnnouncement').getEl().hide();
    }


    var interval = 1;
    function Collapse(iteration) {
        var radtabpanelbar = parent.TreeViewFrame.document.getElementById("rpbModuleItems");
        if (iteration >= 0) {
            var size = iteration + '%,*';
            parent.document.getElementById('Bottom').cols = size;
            iteration = iteration - 1;
            setTimeout(function () { Collapse(iteration) }, interval);
        }
        else {
            radtabpanelbar.style.display = 'none';
        }
    }

    function Expand(iteration) {
        if (iteration <= 20) {
            var size = iteration + '%,*';
            parent.document.getElementById('Bottom').cols = size;
            iteration = iteration + 1;
            setTimeout(function () { Expand(iteration) }, interval);
        }
    }

    function CollapseExpandTreePanel(CollapseExpand) {
        var radtabpanelbar = parent.TreeViewFrame.document.getElementById("rpbModuleItems");
        var imgExpand = document.getElementById('imgExpand');
        var imgCollapse = document.getElementById('imgCollapse');
        var size;
        if (CollapseExpand == 'Collapse') {
            size = parent.document.getElementById('Bottom').cols.split(',')[0].replace("%", "");
            Collapse(0);
            parent.document.getElementById('TreeViewFrame').noResize = true;
            imgCollapse.style.display = 'none';
            imgExpand.style.display = 'block';
        }
        else if (CollapseExpand == 'Expand') {
            Expand(20);
            parent.document.getElementById('TreeViewFrame').noResize = false;
            imgExpand.style.display = 'none';
            imgCollapse.style.display = 'block';
            radtabpanelbar.style.display = 'block';
        }
    }


</script>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">

        window.onload = function InitialpageLoad() {
            var YardView = document.getElementById("<%= hdnYardViewPageDetails.ClientID %>").value;
            if (YardView != "") {
                var PageName = YardView.split(',')[0];
                var PageUrl = YardView.split(',')[1];
                AddNewTabs(PageName, PageUrl)
            }

            AddNewTabs('Live View (Resources)', '../Yard/LiveView.aspx');
            //AddNewTabs('Dashboard', '../Reports/JobAllocation/Dashboard.aspx');
        };
        var $ = $telerik.$;
        function OnClientShowing(sender, eventArgs) {
            var hdnHideNotification = document.getElementById("<%= hdnHideNotification.ClientID %>");
            if (hdnHideNotification.value == 'true')
                eventArgs.set_cancel(true);
        }

        function ShowAnnouncements() {
            var rntfnAnnouncement = $find("<%= rntfnAnnouncement.ClientID %>");
            var hdnHideNotification = document.getElementById("<%= hdnHideNotification.ClientID %>");
            var divUnSeenAnnouncements = document.getElementById("<%= divUnSeenAnnouncements.ClientID %>");
            var divAllAnnouncements = document.getElementById("divAllAnnouncements");
            divUnSeenAnnouncements.style.display = 'none';
            divAllAnnouncements.style.display = 'block';
            hdnHideNotification.value = "false";
            rntfnAnnouncement.show();
        }
        function CloseAllTabs(sender, args) {
            var RemoveType = args.get_item();
            itemValue = RemoveType.get_value();
            var tabstrip = $find("<%= RadTabStrip1.ClientID %>");
            tabstrip.trackChanges();
            var multiPage = $find("<%= rmp.ClientID %>");
            multiPage.trackChanges();
            var items = tabstrip.get_allTabs()
            items.forEach
            (function (item) {
                var tabValue = item.get_value();
                var PageViewVal = item.get_pageViewID();
                var PageView = multiPage.findPageViewByID(PageViewVal);
                if (itemValue == "1") {
                    tabstrip.get_tabs().remove(item);
                    multiPage.get_pageViews().remove(PageView);
                    document.getElementById("divWelcomeMsg").style.display = '';
                    document.getElementById("divFooterMsg").style.display = '';
                }
                else {
                    if (sender._targetElement.title == "") {
                        if (sender._targetElement.innerText.trim() != item._element.textContent.trim()) {
                            tabstrip.get_tabs().remove(item);
                            multiPage.get_pageViews().remove(PageView);
                        }
                        else
                            item.set_selected(true);
                    }
                    else {
                        if (sender._targetElement.parentElement.innerText.trim() != item._element.textContent.trim()) {
                            tabstrip.get_tabs().remove(item);
                            multiPage.get_pageViews().remove(PageView);
                        }
                        else
                            item.set_selected(true);

                    }
                }
                tabstrip.commitChanges();
                multiPage.commitChanges();
            }
            )
            tabstrip.commitChanges();
            multiPage.commitChanges();
        }

        function RefreshTab(url) {

            var tabstrip = $find("<%= RadTabStrip1.ClientID %>");
            var tab = tabstrip.findTabByValue(url);
            var pageviewID = tab.get_pageViewID();

            var multiPage = $find("<%= rmp.ClientID %>");
            var pageview = multiPage.findPageViewByID(pageviewID);
            if (pageview) {
                var contentElement = pageviewID;
                var loadingPanel = $find("<%= AjaxLoadingPanel1.ClientID %>");
                loadingPanel.show(contentElement);
                pageview.set_contentUrl(url);
                pageview.set_selected(true);
            }
        }

        function RemoveTab(url) {

            var tabstrip = $find("<%= RadTabStrip1.ClientID %>");
            tabstrip.trackChanges();
            var multiPage = $find("<%= rmp.ClientID %>");
            multiPage.trackChanges();

            var tab = tabstrip.findTabByValue(url);
            var pageviewID = tab.get_pageViewID();
            var pageview = multiPage.findPageViewByID(pageviewID); //multiPage.findPageViewByID(url); => this also will work fine
            var tabToSelect = tab.get_nextTab();
            if (!tabToSelect)
                tabToSelect = tab.get_previousTab();
            tabstrip.get_tabs().remove(tab);
            multiPage.get_pageViews().remove(pageview);
            if (tabToSelect)
                tabToSelect.set_selected(true);
            else {
                document.getElementById("divWelcomeMsg").style.display = '';
                document.getElementById("divFooterMsg").style.display = '';
            }
            if (tabstrip._scroller != null) {
                tabstrip.get_selectedTab().scrollIntoView();
            }
            tabstrip.commitChanges();
            multiPage.commitChanges();
        }

        function AddNewTabs(displayname, url) {
            $find("<%= RadWin_EditUserProfile.ClientID %>").close();
            document.getElementById("divWelcomeMsg").style.display = "none";
            document.getElementById("divFooterMsg").style.display = "none";
            var tabstrip = $find("<%= RadTabStrip1.ClientID %>");
            var roottab = new Telerik.Web.UI.RadTab();
            var tab = tabstrip.findTabByValue(url);
            if (tab == null) {
                var pageView = new Telerik.Web.UI.RadPageView();
                pageView.set_id(url);
                var contentElement = pageView.get_id();
                pageView.set_contentUrl(url);
                pageView.set_selected(true);
                var multiPage = $find("<%= rmp.ClientID %>");
                multiPage.trackChanges();
                multiPage.get_pageViews().add(pageView);
                multiPage.commitChanges();

                roottab.set_text(displayname + " <img style=\"margin-left: 2px;margin-bottom: 0px;cursor:hand;\" src=\"../../images/HelpTab.png\" width=\"12\" height=\"12\" border=\"0\" title=\"Help Document\" onClick=\"javascript:ShowUserManual('" + url + "')\" /> "
                            + " <img style=\"margin-left: 2px; margin-bottom:0px; cursor:hand;border-color: #9CBAEF;border-width:1px;\" src=\"../../images/ReloadTab.png\" width=\"12\" height=\"12\" border=\"0\" title=\"Refresh\" onClick=\"javascript:RefreshTab('" + url + "')\" />"
                            + " <img style=\"margin-left: 2px; margin-bottom:0px; cursor:hand;border-color: #9CBAEF;border-width:1px;\" src=\"../../images/RemoveTab.png\" width=\"12\" height=\"12\" border=\"0\" title=\"Close\" onClick=\"javascript:RemoveTab('" + url + "')\" />");
                roottab.set_value(url);
                tabstrip.trackChanges();
                tabstrip.get_tabs().add(roottab);
                roottab.set_selected(true);
                var loadingPanel = $find("<%= AjaxLoadingPanel1.ClientID %>");
                loadingPanel.show(contentElement);
                var pageHeight = document.documentElement.clientHeight;
                var Pheight = document.getElementById("<%= RadTabStrip1.ClientID %>").clientHeight;
             pageView.get_element().style.height = (pageHeight - Pheight) + "px";
             roottab.set_pageViewID(pageView.get_id());
             tabstrip.commitChanges();
            
         }
         else
             tab.set_selected(true);

         if (tabstrip._scroller != null) {
             tabstrip.get_selectedTab().scrollIntoView();
         }
         var RadCtxtMenu = $find("<%= RadCtxtMenu.ClientID %>")
           RadCtxtMenu.trackChanges();
           RadCtxtMenu.addTargetElement(roottab._element);
           RadCtxtMenu.commitChanges();
       }

       function OpenUserRadWindow() {
           var UserWindow = $find("<%= RadWin_EditUserProfile.ClientID %>");
            UserWindow.set_width(620);
            UserWindow.set_height(250);
            UserWindow.setUrl("../Admin/EditUserProfile.aspx");
            UserWindow.show();
        }

        function OpenUserManualRadWindow(UserManualURL) {
            var UMWindow = $find("<%= RadWin_EditUserProfile.ClientID %>");
        UMWindow.set_width(700);
        UMWindow.set_height(600);
        UMWindow.setUrl("UserManual.aspx?UserManualURL=" + UserManualURL);
        UMWindow.show();
    }

    function OpenFeedBackRadWindow() {
        var UMWindow = $find("<%= RadWin_EditUserProfile.ClientID %>");
            if (UMWindow != null)
                UMWindow.close();
            var FBWindow = $find("<%= RadWin_FeedBack.ClientID %>");
            FBWindow.value = '';
            FBWindow.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close)
            FBWindow.show();
        }

        function SubmitFeedBackAndClose() {
            var tbFeedback = document.getElementById('tbFeedBack');
            var msg = tbFeedback.value.trim();
            if (msg == '') {
                Ext.MessageBox.show({
                    title: 'Error',
                    msg: 'Please Enter Feedback.',
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.ERROR
                });
                //Ext.Msg.alert('Error', 'Please Enter Feedback.');
                return;
            }
            PageMethods.SaveFeedBack(msg);
            tbFeedback.value = '';
            Ext.MessageBox.show({
                title: 'Message',
                msg: 'Thank you for taking time to provide us with your feedback!',
                buttons: Ext.MessageBox.OK,
                icon: Ext.MessageBox.INFO
            });
            //Ext.Msg.alert('Message', 'Thank you for taking time to provide us with your feedback!');
            GetFBRadWindow().close();
        }

        function GetFBRadWindow() {
            //using ContextTemplate instead of Navigate URL for RadWindow so we need to get RadWindow by name
            var oWnd = GetRadWindowManager().getWindowByName("RadWin_FeedBack");
            return oWnd;
        }

        function ShowSessionExpireAlert() {
            $find('<%= RadWin_SessionTimeout.ClientID %>').show();
        }

        function ShowSessionExpiredMessage() {
            $find('<%= RadWin_SessionTimeout.ClientID %>').close();
                $find('<%= RadWin_SessionExpired.ClientID %>').show();
            }

            function CloseSessionExpireAlert() {
                $find('<%= RadWin_SessionTimeout.ClientID %>').close();
            }

            function CloseSessionExpiredMessage() {
                $find('<%= RadWin_SessionExpired.ClientID %>').close();
            }

            function ResetSession() {
                PageMethods.ResetSession();
                top.Top.ResetSessionTimeOut();
                CloseSessionExpireAlert();
            }

            function RedirectToLogin() {
                top.window.location.href = '../../Login.aspx';
            }
            function CloseLoadingPnl() {
                var tabstrip = $find("<%= RadTabStrip1.ClientID %>");
                var currentLoadingPanel = $find("<%= AjaxLoadingPanel1.ClientID %>");
                if (currentLoadingPanel != null)
                    currentLoadingPanel.hide("");
                currentUpdatedControl = null;
                currentLoadingPanel = null; 
                //isTabAdd = true;
            }

            function OpenCompleteUserManualRadWindow() {

                var UserManualURL = '/AstorTimeCompleteUserManual.pdf';
                ShowUserManual(UserManualURL);

            }


    </script>
</telerik:RadCodeBlock>
</html>
