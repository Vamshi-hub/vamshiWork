<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="ViewUsers.aspx.cs" Inherits="astorWork.aspx.Configuration.ViewUsers" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
       .RadButton.rbButton{
           min-width:25px !important;
       }
   </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">

        var column = null;
        function MenuShowing(sender, args) {
            if (column == null) return;
            var menu = sender; var items = menu.get_items();
            if (column.get_dataType() == "System.String") {
                var i = 0;
                while (i < items.get_count()) {
                    if (!(items.getItem(i).get_value() in { 'NoFilter': '', 'Contains': '', 'EqualTo': '', 'StartsWith': '' })) {
                        var item = items.getItem(i);
                        if (item != null)
                            item.set_visible(false);
                    }
                    else {
                        var item = items.getItem(i);
                        if (item != null)
                            item.set_visible(true);
                    } i++;
                }
            }
            column = null;
        }

        function filterMenuShowing(sender, eventArgs) {
            column = eventArgs.get_column();
        }

        function CancelKeyPress(sender, eventArgs) {
            eventArgs.get_domEvent().keyCode = null;
            eventArgs.get_domEvent().stopPropagation();
            eventArgs.get_domEvent().preventDefault();
            eventArgs.get_domEvent().cancelBubble = true;
        }
    </script>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function openRadWindow(sender, args) {

                if (sender.get_commandArgument() == '')
                    $find("<%= RadWin_AddUserPage.ClientID %>").setUrl("AddUser.aspx");
                else
                    $find("<%= RadWin_AddUserPage.ClientID %>").setUrl("AddUser.aspx?UserId=" + sender.get_commandArgument());
                $find("<%= RadWin_AddUserPage.ClientID %>").show();
            }

            function refreshGrid(Operation) {
                var hdnOperation = document.getElementById("<%= hdnOperation.ClientID %>");
                hdnOperation.value = Operation;
                $get("<%= rbtnRefreshGrid.ClientID %>").click();
            }
            function RequestStart() {
                var loadingPanel = document.getElementById("<%= ralpViewUsers.ClientID %>");
                var pageHeight = document.documentElement.scrollHeight;
                var viewportHeight = document.documentElement.clientHeight;
                if (pageHeight > viewportHeight) {
                    loadingPanel.style.height = pageHeight + "px";
                }

                var pageWidth = document.documentElement.scrollWidth;
                var viewportWidth = document.documentElement.clientWidth;

                if (pageWidth > viewportWidth) {
                    loadingPanel.style.width = pageWidth + "px";
                }
                // the following Javascript code takes care of centering the RadAjaxLoadingPanel
                // background image, taking into consideration the scroll offset of the page content

                if ($telerik.isSafari) {
                    var scrollTopOffset = document.body.scrollTop;
                    var scrollLeftOffset = document.body.scrollLeft;
                }
                else {
                    var scrollTopOffset = document.documentElement.scrollTop;
                    var scrollLeftOffset = document.documentElement.scrollLeft;
                }
                var loadingImageWidth = 55;
                var loadingImageHeight = 55;
                loadingPanel.style.backgroundPosition = (parseInt(scrollLeftOffset) + parseInt(viewportWidth / 2) - parseInt(loadingImageWidth / 2)) + "px " + (parseInt(scrollTopOffset) + parseInt(viewportHeight / 2) - parseInt(loadingImageHeight / 2)) + "px";
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadAjaxManager ID="ramViewUsers" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="pnlViewUsers">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlViewUsers" LoadingPanelID="ralpViewUsers" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" />
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralpViewUsers" runat="server"></telerik:RadAjaxLoadingPanel>
    <asp:Panel ID="pnlViewUsers" runat="server">
        <asp:HiddenField runat="server" ID="hdnOperation" />
        <telerik:RadButton runat="server" ID="rbtnRefreshGrid" OnClick="rbtnRefreshGrid_Click" Style="display: none;" />
        <telerik:RadWindow runat="server" ID="RadWin_AddUserPage" Width="450" Height="290" Behaviors="Close,Reload">
        </telerik:RadWindow>
        <div class="layout">
            <br />
            <telerik:RadGrid ID="rgUsers" runat="server" Width="55%" OnNeedDataSource="rgUsers_NeedDataSource"
                PagerStyle-Mode="Advanced" AllowFilteringByColumn="true">
                <MasterTableView Width="100%" DataKeyNames="UserID">
                    <Columns>
                        <telerik:GridBoundColumn DataField="UserID" HeaderText="User ID" />
                        <telerik:GridBoundColumn DataField="UserName" HeaderText="User Name" />
                        
                      
                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderStyle-Width="12%" HeaderStyle-HorizontalAlign="Center"
                            ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <telerik:RadButton ID="rbAddNewUser" runat="server" Text="Add" OnClientClicked="openRadWindow"
                                    AutoPostBack="false" ButtonType="LinkButton" SkinID="rbnAdd" Style="padding-right: 8px;">
                                    <Icon  PrimaryIconLeft="5" PrimaryIconTop="5" />
                                </telerik:RadButton>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <telerik:RadButton ID="rbnEdit" runat="server" Text="Edit" CommandArgument='<%# Eval("UserID") %>' SkinID="rbnEdit"
                                    ButtonType="LinkButton" OnClientClicked="openRadWindow" AutoPostBack="false">
                                   <Icon PrimaryIconLeft="5" PrimaryIconTop="5" />

                                </telerik:RadButton>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
                <FilterMenu EnableImageSprites="False">
                </FilterMenu>
                <ClientSettings>
                    <Scrolling AllowScroll="false" />
                    <ClientEvents OnFilterMenuShowing="filterMenuShowing" />
                </ClientSettings>
                <FilterMenu OnClientShown="MenuShowing" />
            </telerik:RadGrid>
        </div>
    </asp:Panel>
</asp:Content>
