<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="ActiveYardEmployees.aspx.cs" Inherits="astorWork.aspx.Yard.ActiveYardEmployees" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
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
            <telerik:RadGrid ID="rgActiveEmployees" Width="100%" runat="server" PagerStyle-Mode="Advanced"
                AllowFilteringByColumn="false" OnNeedDataSource="rgActiveEmployees_NeedDataSource">
                <MasterTableView Width="100%" DataKeyNames="EmployeeKey" AllowSorting="true" AllowFilteringByColumn="false">
                    <Columns>
                
                        <telerik:GridBoundColumn DataField="EmployeeID" HeaderText="Emp ID" HeaderStyle-Width="9%"
                            ItemStyle-Width="9%" FilterControlWidth="40" />
                        <telerik:GridTemplateColumn HeaderText="Employee Name" DataField="EmployeeName" HeaderStyle-Width="30%"
                            ItemStyle-Width="25%" SortExpression="EmployeeName">
                            <ItemTemplate>
                                <asp:Label ID="lblEmployeeName" runat="server" Text='<%# Eval("EmployeeName") %>' />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="CompanyName" HeaderText="Company Name" HeaderStyle-Width="15%"
                            ItemStyle-Width="15%" AllowFiltering="false" />
                        <telerik:GridBoundColumn DataField="DepartmentName" HeaderText="Department Name" HeaderStyle-Width="15%"
                            ItemStyle-Width="15%" AllowFiltering="false" />
                        <telerik:GridBoundColumn DataField="TradeName" HeaderText="Trade Name" HeaderStyle-Width="15%"
                            ItemStyle-Width="15%" AllowFiltering="false" />

                        <telerik:GridTemplateColumn HeaderText="Time-In" HeaderStyle-Width="20%" ItemStyle-Width="20%"
                            AllowFiltering="false" DataField="TimeIn" SortExpression="TimeIn">
                            <ItemTemplate>
                                <asp:Label ID="lblClockDate" runat="server" Visible="false" Text='<%# Eval("ClockDate") %>' />
                                <asp:Label ID="lblTimeIn" runat="server" Text='<%# Eval("TimeIn") %>' />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="CSName" HeaderText="CS" HeaderStyle-Width="15%"
                            ItemStyle-Width="15%" AllowFiltering="false" Visible="false" />
                        <telerik:GridTemplateColumn HeaderText="Time-In" HeaderStyle-Width="20%" ItemStyle-Width="20%"
                            AllowFiltering="false" DataField="CSTimeIn" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblCSTimeIn" runat="server" Text='<%# Eval("CSTimeIn") %>' />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>

                    </Columns>
                </MasterTableView>

            </telerik:RadGrid>
        </div>
    </asp:Panel>
</asp:Content>
