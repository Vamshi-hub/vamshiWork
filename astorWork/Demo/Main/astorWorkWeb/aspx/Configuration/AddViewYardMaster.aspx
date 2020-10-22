<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="AddViewYardMaster.aspx.cs" Inherits="astorWork.aspx.Configuration.AddViewYardMaster" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ MasterType VirtualPath="~/astorWork.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <style type="text/css">
          .RadButton_Metro.rbVerticalButton .rbDecorated.rbPrimary
          {
              padding-left:23px !important;
          }
          </style>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
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
            function CloseChildWindow() {
                var PopupWindow = $find("<%= RadWin_AddEditYard.ClientID %>");
                PopupWindow.close();
            }

            function OnClientBeforeShow() {
                document.documentElement.focus();
            }
            function refreshGrid(Operation) {
                document.getElementById("<%= hdnOperation.ClientID %>").value = Operation;
                $get("<%= rbtnRefreshGrid.ClientID %>").click();
            }


            function Confirmation(sender, args) {
                var res = confirm('Are you sure want to delete ?');
                if (!res) {
                    //Cancel the postback
                    args.set_cancel(true);
                }
            }
            function OnClientBeforeShow() {
                document.documentElement.focus();
            }
            function validateString(oSrc, arguments) {
                //            args.IsValid = (args.Value.length >= 8);
                if (arguments.Value.toLowerCase() == "unknown" || arguments.Value == "UNKNOWN") {
                    arguments.IsValid = false;
                }
                else {
                    arguments.IsValid = true;
                }
            }

            function openRadWindow(sender, args) {

                var commandArgs = sender.get_commandArgument().toString();
                $find("<%= RadWin_AddEditYard.ClientID %>").setUrl("AddYardMaster.aspx?YardID=" + commandArgs)

            $find("<%= RadWin_AddEditYard.ClientID %>").show();
        }

        function RequestStart() {
            var loadingPanel = document.getElementById("<%= RadAjaxLoadingPanel1.ClientID %>");
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

    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
        <AjaxSettings>

            <telerik:AjaxSetting AjaxControlID="pnlYard">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlYard" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" />
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnlYard" runat="server">
        <telerik:RadWindow ID="RadWin_AddEditYard" runat="server" Modal="true" VisibleOnPageLoad="false"
            Behaviors="Close,Reload" VisibleStatusbar="true" Width="750" Height="480">
        </telerik:RadWindow>
        <telerik:RadWindow ID="RadWin_YardPreview" runat="server" Modal="true" VisibleOnPageLoad="false" Title="Yard Preview"
            Behaviors="Close,Reload" VisibleStatusbar="true" Width="614" Height="399">
            <ContentTemplate>
                <telerik:RadBinaryImage runat="server" ID="rimgMapYardView" Style="width:596px !important;height:355px !important" ResizeMode="Fit" />
            </ContentTemplate>
        </telerik:RadWindow>
        <asp:HiddenField runat="server" ID="hdnOperation" />
        <telerik:RadButton runat="server" ID="rbtnRefreshGrid" OnClick="rbtnRefreshGrid_Click" Style="display: none;" />

        <div class="layout">
            <telerik:RadGrid ID="rgYard" runat="server" Width="60%" OnNeedDataSource="rgYard_NeedDataSource"
                PagerStyle-Mode="Advanced" AllowFilteringByColumn="false">
                <MasterTableView Width="100%" DataKeyNames="YardID" AllowFilteringByColumn="false">
                    <Columns>
                        <telerik:GridBoundColumn DataField="YardName" HeaderText="Yard Name" HeaderStyle-Width="10%"/>
                        <telerik:GridBoundColumn DataField="IsDefault" HeaderText="Is Default" AllowSorting="false"
                            HeaderStyle-Width="5%" />
                        <telerik:GridBoundColumn DataField="UTCOffset" AllowFiltering="false" HeaderText="UTC Offset(min)" AllowSorting="false"
                            HeaderStyle-Width="5%" />
                      <%--  <telerik:GridBoundColumn DataField="EnableYardTimeCapture" HeaderText="Capture YardTime" AllowSorting="false"
                            HeaderStyle-Width="5%" />--%>
                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderStyle-HorizontalAlign="Center"
                            HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                               <telerik:RadButton ID="rbtnPreview" runat="server" Text="Preview" OnClick="rbtnPreview_Click" CommandName="RowPreview" CausesValidation="false"
                                    CommandArgument='<%#Eval("YardID") %>' AutoPostBack="true" SkinID="rbnPreview"></telerik:RadButton>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderStyle-HorizontalAlign="Center"
                            HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <telerik:RadButton ID="rbAddYard" runat="server" Text="Add" Style="padding-right: 8px; width:80px"
                                    CommandName="RowInsert" ButtonType="LinkButton" CausesValidation="false" OnClientClicked="openRadWindow" AutoPostBack="false" SkinID="rbnAdd">
                                    <Icon PrimaryIconCssClass="rbAdd" PrimaryIconLeft="5" PrimaryIconTop="5" />
                                </telerik:RadButton>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <telerik:RadButton ID="rbtnEdit" Width="42px" runat="server" Text="Edit" CommandName="RowEdit"
                                    CausesValidation="false" OnClientClicked="openRadWindow" AutoPostBack="false" CommandArgument='<%# Eval("YardID") %>' ButtonType="LinkButton">
                                </telerik:RadButton>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>

                    </Columns>
                </MasterTableView>
                <ClientSettings>
                    <Scrolling AllowScroll="false" />
                    <ClientEvents OnFilterMenuShowing="filterMenuShowing" />
                </ClientSettings>
                <FilterMenu OnClientShown="MenuShowing" />
            </telerik:RadGrid>
        </div>
        <br />
    </asp:Panel>

</asp:Content>
