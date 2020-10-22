<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="ViewZoneMaster.aspx.cs" Inherits="astorWork.aspx.Configuration.ViewZoneMaster" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ MasterType VirtualPath="~/astorWork.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <style>
        .RadButton.rbButton {
            min-width: 25px !important;
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

            function RequestStart() {
                var loadingPanel = document.getElementById("<%= rlpnl.ClientID %>");
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
    <telerik:RadAjaxLoadingPanel ID="rlpnl" runat="server" />
    <telerik:RadAjaxManager ID="ramZone" runat="server" DefaultLoadingPanelID="rlpnl">
        <AjaxSettings>

            <telerik:AjaxSetting AjaxControlID="pnlZone">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlZone" LoadingPanelID="rlpnl" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" />
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnlZone" runat="server">
        <asp:HiddenField runat="server" ID="hdnOperation" />
        <telerik:RadButton runat="server" ID="rbtnRefreshGrid" OnClick="rbtnRefreshGrid_Click" Style="display: none;" />
        <telerik:RadWindow ID="RadWin_ZonePreview" runat="server" Modal="true" VisibleOnPageLoad="false" Title="Zone Preview"
            Behaviors="Close,Reload" VisibleStatusbar="true" Width="614" Height="400">
            <ContentTemplate>
                <telerik:RadBinaryImage Width="100%" Height="98%" runat="server" ID="rimgMapZoneView" />
            </ContentTemplate>
        </telerik:RadWindow>

        <div class="layout">
            <br />
                  <table width="25%">
                <tr>
                    <td class="tdLabel" style="width: 30%;">
                        <asp:Label ID="lblYard" runat="server" Text="Yard" Width="100"></asp:Label></td>
                    <td class="tdField" style="width: 70%;">
                        <telerik:RadDropDownList ID="rddlYard" runat="server" Width="90%" AutoPostBack="true" OnSelectedIndexChanged="rddlYard_SelectedIndexChanged"></telerik:RadDropDownList>

                        <asp:RequiredFieldValidator ID="rvYard" runat="server" ControlToValidate="rddlYard" Text="*"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
            <telerik:RadGrid ID="rgZone" runat="server" Width="60%" OnNeedDataSource="rgZone_NeedDataSource"
                PagerStyle-Mode="Advanced" AllowFilteringByColumn="false">
                <MasterTableView Width="100%" DataKeyNames="ZoneID" AllowFilteringByColumn="false">
                    <Columns>
                        <telerik:GridBoundColumn DataField="ZoneName" HeaderText="Zone Name" AllowSorting="true"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="ZoneDescription" HeaderText="Zone Description"></telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderStyle-HorizontalAlign="Center"
                            HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <telerik:RadButton ID="rbAddZone" runat="server" Text="Add"
                                    CommandName="RowInsert" ButtonType="LinkButton" CausesValidation="false" SkinID="rbnAdd" OnClick="rbAddZone_Click"
                                    >
                                    <Icon PrimaryIconLeft="5" PrimaryIconTop="5" />
                                </telerik:RadButton>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <telerik:RadButton ID="rbtnEdit" runat="server" Text="Edit" CommandName="RowEdit" SkinID="rbnEdit"
                                    CausesValidation="false" NavigateUrl='<%#"AddZoneMaster.aspx?YardID="+ Eval("YardID").ToString()+"&ZoneID="+ Eval("ZoneID").ToString()+"&Edit=1&ID=0" %>' ButtonType="LinkButton">
                                    <Icon PrimaryIconLeft="5" PrimaryIconTop="5" />

                                </telerik:RadButton>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
    </asp:Panel>
</asp:Content>
