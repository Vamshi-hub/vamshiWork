<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="ViewValves.aspx.cs" Inherits="astorWork.aspx.Configuration.ViewValves" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ MasterType VirtualPath="~/astorWork.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .RadButton.rbButton {
            min-width: 25px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            //Valve Coordinates
            function GetCoOrdinates(ZoneID,ZoneName,ValveID) {
                var PopupWindow = $find("<%= RadWin_ZoneImage.ClientID %>");
                PopupWindow.setUrl("GetValveCoordinates.aspx?ZoneID=" + ZoneID + "&ZoneName=" + ZoneName + "&ValveID=" + ValveID);
                PopupWindow.show();
            }
            function CloseCordinatesWindow(val) {
                $find("<%= RadWin_ZoneImage.ClientID %>").close();
                var AddZoneWin = $find("<%=RadWindowManager1.ClientID%>").getWindowByName("RadWin_AddEditZone");
                var contentWin = AddZoneWin.get_contentFrame().contentWindow;
                contentWin.populateCoordinates(val)
            }

            //End Valve Coordinates
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
                var PopupWindow = $find("<%= RadWin_AddEditZone.ClientID %>");
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
                var e = $find("<%=rddlZone.ClientID%>");
                var strZoneID = e.get_selectedItem().get_value();

                var commandArgs = sender.get_commandArgument().toString();
                if (commandArgs == "")
                    $find("<%= RadWin_AddEditZone.ClientID %>").setUrl("AddValveMaster.aspx?ZoneID=" + strZoneID)
                else {
                    var ValveID = commandArgs.split(",")[0];
                    $find("<%= RadWin_AddEditZone.ClientID %>").setUrl("AddValveMaster.aspx?ZoneID=" + strZoneID + "&ValveID=" + ValveID)
                }
                $find("<%= RadWin_AddEditZone.ClientID %>").show();
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
        <telerik:RadWindowManager ID="RadWindowManager1" ShowContentDuringLoad="false" VisibleStatusbar="false"
            ReloadOnShow="true" runat="server" EnableShadow="true">
            <Windows>
                <telerik:RadWindow ID="RadWin_AddEditZone" runat="server" Modal="true" VisibleOnPageLoad="false" Title="Add/Edit Zone"
                    Behaviors="Close,Reload" VisibleStatusbar="true" Width="750" Height="330">
                </telerik:RadWindow>
                <telerik:RadWindow ID="RadWin_ZoneImage" runat="server" Title="Zone LayOut" Modal="true" Style="z-index: 1000000;"
                    Behaviors="Close, Reload" VisibleStatusbar="true" VisibleOnPageLoad="false" Height="600px" Width="1000px">
                </telerik:RadWindow>
            </Windows>
        </telerik:RadWindowManager>
        <div class="layout">
            <br />
            <table class="table">
                <tr>
                    <td class="tdLabel">Select Zone
                    </td>
                    <td class="tdField">
                        <telerik:RadDropDownList ValidationGroup="valve" CausesValidation="true" AutoPostBack="true"
                            OnSelectedIndexChanged="rddlZone_SelectedIndexChanged" ID="rddlZone" runat="server">
                        </telerik:RadDropDownList>

                        <asp:RequiredFieldValidator ValidationGroup="valve" InitialValue="--Select--" ForeColor="Red"
                            Display="Dynamic" ID="rfvZone" ControlToValidate="rddlZone" runat="server" ErrorMessage="Select Zone">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
            <br />
            <telerik:RadGrid ID="rgValve" runat="server" Width="60%" OnNeedDataSource="rgValve_NeedDataSource" Visible="false">
                <MasterTableView Width="100%" DataKeyNames="ZoneID" AllowFilteringByColumn="false">
                    <Columns>
                        <telerik:GridTemplateColumn UniqueName="Image" AllowFiltering="false" HeaderText="Valve Image">
                            <ItemTemplate>
                                <telerik:RadBinaryImage runat="server" ID="rbImgValve" DataValue='<%#Eval("ValveImage") %>' Width="30px" ImageStorageLocation="Session" Height="30px" ResizeMode="Fill" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="ValveName" HeaderText="Valve Name"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="ValveDescription" HeaderText="Valve Description"></telerik:GridBoundColumn>

                        <telerik:GridTemplateColumn UniqueName="ValveStatus" HeaderText="Valve Status">
                            <ItemTemplate>
                                <%#(astorWork.Enums.ValveStatus)Enum.Parse(typeof(astorWork.Enums.ValveStatus), Eval("Status").ToString()) %>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="ValveType" HeaderText="Valve Type">
                            <ItemTemplate>
                                <%#(astorWork.Enums.ValveTypes)Enum.Parse(typeof(astorWork.Enums.ValveTypes), Eval("ValveTypeID").ToString()) %>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>

                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderStyle-HorizontalAlign="Center"
                            HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <telerik:RadButton ID="rbAddValve" runat="server" Text="Add" SkinID="rbnAdd"
                                    CommandName="RowInsert" ButtonType="LinkButton" CausesValidation="false"
                                    OnClientClicked="openRadWindow" AutoPostBack="false">
                                    <Icon PrimaryIconLeft="5" PrimaryIconTop="5" />
                                </telerik:RadButton>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <telerik:RadButton ID="rbtnEdit" Width="60px" runat="server" Text="Edit" CommandName="RowEdit" SkinID="rbnEdit"
                                    CausesValidation="false" OnClientClicked="openRadWindow" AutoPostBack="false" CommandArgument='<%# Eval("ValveID").ToString()+","+Eval("ZoneID").ToString() %>'
                                    ButtonType="LinkButton">
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
