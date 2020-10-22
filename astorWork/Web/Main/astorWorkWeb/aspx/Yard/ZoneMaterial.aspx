<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" 
    CodeBehind="ZoneMaterial.aspx.cs" Inherits="astorWork.aspx.Yard.ZoneMaterial" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style type="text/css">
        .RadComboBox_Outlook .rcbDisabled td.rcbInputCellLeft {
            background-image: none !important;
            background-color: InactiveBorder;
        }


        /* Respect margins and paddings */ fieldset {
            padding: 5px;
            margin: 5px;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow)
                oWindow = window.radWindow;
            else if (window.frameElement.radWindow)
                oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function CloseWindow() {
            var oWnd = GetRadWindow();
            oWnd.close();
        }

        function CloseChildWindow() {
         <%--   var PopupWindow = $find("<%= RadWin_EmpDetail.ClientID %>");
            PopupWindow.close();--%>
        }

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

        //This method is fix for he IE 10 issue: "Unable to get property 'toLowerCase' of undefined or null reference" when opening radwindow from codebehind
        function OnClientBeforeShow() {
            document.documentElement.focus();
        }

    </script>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function openRadWindow(sender, args) {
                var hdnZonID = document.getElementById("<%=hdnZonID.ClientID %>");
                var hdnCSID = document.getElementById("<%=hdnCSID.ClientID %>");
                var hdnReportType = document.getElementById("<%=hdnReportType.ClientID %>");
                var hdnZoneName = document.getElementById("<%=hdnZoneName.ClientID %>");
                var hdnType = document.getElementById("<%=hdnType.ClientID %>");
                var hdnSource = document.getElementById("<%=hdnSource.ClientID %>");
                var hdnCompanyIDs = document.getElementById("<%=hdnCompanyID.ClientID %>");
                var hdnDepartmenIDs = document.getElementById("<%=hdnDepartmentID.ClientID %>");
                var hdnTradeIDs = document.getElementById("<%=hdnTradeID.ClientID %>");
                var hdnHasCS = document.getElementById("<%=hdnHasCS.ClientID %>");
                if (hdnType.value == '') {
                    if (hdnSource.value == '')
                        parent.window.openRadWindow(sender, args, hdnZonID.value, hdnReportType.value, hdnZoneName.value, hdnHasCS.value, hdnCompanyIDs.value, hdnDepartmenIDs.value, hdnTradeIDs.value, hdnSource.value, hdnCSID.value);
                    else
                        parent.window.openRadWindow(sender, args, hdnZonID.value, hdnReportType.value, hdnZoneName.value, hdnHasCS.value, hdnCompanyIDs.value, hdnDepartmenIDs.value, hdnTradeIDs.value, hdnSource.value, hdnCSID.value);
                }
                else
                    parent.window.openRadWindowForZone(sender, args, hdnZonID.value, hdnReportType.value, hdnZoneName.value, hdnHasCS.value, hdnCompanyIDs.value, hdnDepartmenIDs.value, hdnTradeIDs.value, hdnSource.value, hdnCSID.value);
            }


        </script>
    </telerik:RadCodeBlock>
    <asp:UpdatePanel ID="upnlOnboardWorkers" runat="server">
        <ContentTemplate>
            <div style="margin-left: 5px; margin-top: 5px; margin-right: 5px;">
                <telerik:RadGrid ID="rgMaterial" Width="100%" runat="server" PagerStyle-Mode="Advanced"
                    AllowFilteringByColumn="false" OnNeedDataSource="rgMaterial_NeedDataSource">
                    <MasterTableView Width="100%" AllowSorting="true" AllowFilteringByColumn="false">
                        <Columns>
                            <telerik:GridBoundColumn DataField="MaterialNo" HeaderText="Material No." HeaderStyle-Width="9%"
                                ItemStyle-Width="9%" FilterControlWidth="40" />
                            <telerik:GridBoundColumn DataField="Status" HeaderText="Status" HeaderStyle-Width="9%"
                                ItemStyle-Width="9%" FilterControlWidth="40" />
                            <telerik:GridBoundColumn DataField="MarkingNo" HeaderText="Marking No." HeaderStyle-Width="9%"
                                ItemStyle-Width="9%" FilterControlWidth="40" />
                            <telerik:GridBoundColumn DataField="RFIDTagID" HeaderText="RFID Tag" HeaderStyle-Width="9%"
                                ItemStyle-Width="9%" FilterControlWidth="40" />
                        </Columns>
                    </MasterTableView>
                    <FilterMenu EnableImageSprites="False">
                    </FilterMenu>
                    <ClientSettings>

                        <ClientEvents OnFilterMenuShowing="filterMenuShowing" />
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" ScrollHeight="300px" SaveScrollPosition="true"></Scrolling>
                    </ClientSettings>
                    <FilterMenu OnClientShown="MenuShowing" />
                </telerik:RadGrid>
                <br />
                <asp:HiddenField runat="server" ID="hdnZonID" />
                <asp:HiddenField runat="server" ID="hdnCSID" />
                <asp:HiddenField runat="server" ID="hdnReportType" />
                <asp:HiddenField runat="server" ID="hdnZoneName" />
                <asp:HiddenField runat="server" ID="hdnType" />
                <asp:HiddenField runat="server" ID="hdnSource" />
                <asp:HiddenField runat="server" ID="hdnHasCS" />
                <asp:HiddenField runat="server" ID="hdnCompanyID" />
                <asp:HiddenField runat="server" ID="hdnDepartmentID" />
                <asp:HiddenField runat="server" ID="hdnTradeID" />
                <center>
                    <table width="20%">
                        <tr>
                            <td>
                                <telerik:RadButton runat="server" Text="Close" OnClientClicking="CloseWindow" ID="rbCancel" />
                            </td>
                        </tr>
                    </table>
                </center>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
